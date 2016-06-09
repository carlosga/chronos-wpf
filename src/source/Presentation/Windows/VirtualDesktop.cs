// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Chronos.Extensions.Windows;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.Core.Windows;
using Chronos.Presentation.Windows.Controls;
using NLog;
using nRoute.Components.Messaging;

namespace Chronos.Presentation.Windows
{
    /// <summary>
    /// Virtual desktop class
    /// </summary>
    public sealed class VirtualDesktop
        : DispatcherObject, IVirtualDesktop
    {
        #region · Logger ·

        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · Fields ·

        private Desktop _desktop;
        private ObservableCollection<INavigationViewModel> _activeWindows;
        private ReadOnlyObservableCollection<INavigationViewModel> _activeWindowsWrapper;
        private bool _hasBeenActivated;

        #endregion

        #region · Properties ·

        public Guid Id
        {
            get
            {
                if (_desktop != null)
                {
                    return _desktop.Id;
                }

                return Guid.Empty;
            }
        }

        /// <summary>
        /// Gets the list of active Windows
        /// </summary>
        public ReadOnlyObservableCollection<INavigationViewModel> ActiveWindows
        {
            get
            {
                if (_activeWindowsWrapper == null)
                {
                    _activeWindowsWrapper = new ReadOnlyObservableCollection<INavigationViewModel>(this.Windows);
                }

                return _activeWindowsWrapper;
            }
        }

        #endregion

        #region · Private Properties ·

        private ObservableCollection<INavigationViewModel> Windows
        {
            get
            {
                if (_activeWindows == null)
                {
                    _activeWindows = new ObservableCollection<INavigationViewModel>();
                }

                return _activeWindows;
            }
        }

        #endregion

        #region · Constructors ·

        /// <summary>
        /// Initializes a new instance of <see cref="VirtualDesktop"/> class
        /// </summary>
        internal VirtualDesktop(Desktop desktop)
        {
            _desktop = desktop;
        }

        #endregion

        #region · Desktop Actions ·

        /// <summary>
        /// Activates the desktop instance
        /// </summary>
        public void Activate()
        {
            this.InvokeAsynchronouslyInBackground(
                () =>
                {
                    if (!_hasBeenActivated)
                    {
                        this.Load();
                        _hasBeenActivated = true;

                        _desktop.Activate();
                    }

                    _desktop.Visibility = Visibility.Visible;

                    Channel<ActiveDesktopChangedInfo>.Public.OnNext(new ActiveDesktopChangedInfo(this), true);
                });
        }

        /// <summary>
        /// Deactivates the desktop instance
        /// </summary>
        public void Deactivate()
        {
            this.Invoke(
                () =>
                {
                    _desktop.Visibility = Visibility.Hidden;
                });
        }

        /// <summary>
        /// Shows the desktop
        /// </summary>
        public void ShowDesktop()
        {
            this.InvokeAsynchronouslyInBackground
            (
                () =>
                {
                    _desktop.Children
                        .OfType<WindowElement>()
                        .ToList()
                        .ForEach(window => window.WindowState = WindowState.Minimized);
                }
            );
        }

        /// <summary>
        /// Saves desktop contents to disk
        /// </summary>
        public void Save()
        {
            this.Invoke(
                () =>
                {
                    DesktopSerializer.Save(_desktop, this.GetXamlFilename());
                });
        }

        #endregion

        #region · Window Methods ·

        /// <summary>
        /// Shows the given window instance
        /// </summary>
        public void Show(IWindow window)
        {
            this.InvokeAsynchronouslyInBackground
            (
                () =>
                {
                    WindowElement element = window as WindowElement;

                    _desktop.AddElement(element);

                    element.Show();

                    this.InvokeAsynchronouslyInBackground
                    (
                        () =>
                        {
                            if (element.DataContext is INavigationViewModel)
                            {
                                INavigationViewModel windowViewModel = element.DataContext as INavigationViewModel;

                                ((IClosableViewModel)windowViewModel).Title = element.Title;

                                this.Windows.Add(windowViewModel);
                            }
                        }
                    );
                }
            );
        }

        /// <summary>
        /// Restores the window with the given Id
        /// </summary>
        /// <param name="id"></param>
        public void Restore(Guid id)
        {
            this.InvokeAsynchronouslyInBackground
            (
                () =>
                {
                    IWindow window = _desktop.Children.OfType<IWindow>().Where(wc => wc.Id == id).SingleOrDefault();

                    if (window != null)
                    {
                        window.WindowState = WindowState.Normal;
                        window.Activate();
                    }
                }
            );
        }

        #endregion

        #region · Shortcut Methods ·

        /// <summary>
        /// Creates a new shortcut with the given title and target
        /// </summary>
        /// <param name="title"></param>
        /// <param name="target"></param>
        public void CreateShortcut<T>(string title, string target) where T : IShortcutViewModel, new()
        {
            this.CreateShortcut<T>(title, target, new Point(0, 0));
        }

        /// <summary>
        /// Creates a new shortcut with the given title and target
        /// </summary>
        /// <param name="title"></param>
        /// <param name="target"></param>
        public void CreateShortcut<T>(string title, string target, Point point) where T : IShortcutViewModel, new()
        {
            T shortcut = new T
            {
                Title = title,
                Target = target
            };

            ShortcutElement element = new ShortcutElement
            {
                DataContext = shortcut
            };

            this.Show(element, point);
        }

        #endregion

        #region · IDesktopElement Methods ·

        public void Show<T>() where T : IDesktopElement, new()
        {
            this.Show(new T());
        }

        public void Show<T>(Point position) where T : IDesktopElement, new()
        {
            this.Show(new T(), position);
        }

        public void Show(IDesktopElement element)
        {
            this.InvokeAsynchronously(
                () =>
                {
                    DesktopElement desktopElement = element as DesktopElement;

                    _desktop.AddElement(desktopElement);
                });
        }

        public void Show(IDesktopElement element, Point position)
        {
            this.InvokeAsynchronously(
                () =>
                {
                    DesktopElement desktopElement = element as DesktopElement;

                    desktopElement.StartupLocation = StartupPosition.Manual;

                    _desktop.AddElement(desktopElement, position);
                });
        }

        #endregion

        #region · Common Methods ·

        /// <summary>
        /// Closes all
        /// </summary>
        public void CloseAll()
        {
            this.Invoke(
                () =>
                {
                    _desktop.Children
                        .OfType<DesktopElement>()
                        .ToList()
                        .ForEach(e => this.Close(e.Id));

                    _hasBeenActivated = false;
                });
        }

        /// <summary>
        /// Closes the element with the given Id
        /// </summary>
        /// <param name="id">The id.</param>
        public void Close(Guid id)
        {
            DesktopElement instance = _desktop.Children.OfType<DesktopElement>().Where(x => x.Id == id).FirstOrDefault();

            if (instance != null)
            {
                if (instance is WindowElement)
                {
                    this.Windows.Remove((instance as WindowElement).DataContext as INavigationViewModel);
                }

                instance.Close();
            }
        }

        #endregion

        #region · Private Methods ·

        /// <summary>
        /// Loads desktop contents from disk
        /// </summary>
        private void Load()
        {
            this.Invoke(
                () =>
                {
                    DesktopSerializer.Load(_desktop, this.GetXamlFilename());
                });
        }

        private string GetXamlFilename()
        {
            return String.Format("{0}.xaml", _desktop.Name);
        }

        private void RemoveElement(DesktopElement element)
        {
            this.InvokeAsynchronouslyInBackground(
                () =>
                {
                    _desktop.RemoveElement(element);
                });
        }

        #endregion
    }
}
