// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Chronos.Extensions.Windows;
using Chronos.Presentation.Core.ViewModel;
using Chronos.Presentation.Core.VirtualDesktops;
using Chronos.Presentation.Core.Windows;
using Chronos.Presentation.Windows.Controls;
using NLog;
using nRoute.Components.Composition;
using nRoute.Services;

namespace Chronos.Presentation.Windows
{
    [MapService(typeof(IVirtualDesktopManager),
        InitializationMode=InitializationMode.OnDemand,
        Lifetime=InstanceLifetime.Singleton)]
    public sealed class VirtualDesktopManager 
        : DispatcherObject, IVirtualDesktopManager
    {
        #region · Logger ·

        private static Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion

        #region · Attached Properties ·

        /// <summary>
        /// Identifies the IsDesktopCanvas dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDesktopProperty =
            DependencyProperty.RegisterAttached("IsDesktop", typeof(bool), typeof(VirtualDesktopManager),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnIsDesktop)));

        /// <summary>
        /// Identifies the IsModalContainer dependency property.
        /// </summary>
        public static readonly DependencyProperty IsIsModalContainerProperty =
            DependencyProperty.RegisterAttached("IsModalContainer", typeof(bool), typeof(VirtualDesktopManager),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnIsModalContainer)));

        #endregion

        #region · Attached Property Get/Set Methods ·

        /// <summary>
        /// Gets the value of the IsDesktopCanvas attached property
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool GetIsDesktop(DependencyObject d)
        {
            return (bool)d.GetValue(IsDesktopProperty);
        }

        /// <summary>
        /// Sets the value of the IsDesktopCanvas attached property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        public static void SetIsDesktop(DependencyObject d, bool value)
        {
            d.SetValue(IsDesktopProperty, value);
        }

        /// <summary>
        /// Gets the value of the IsModalContainer attached property
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool GetIsModalContainer(DependencyObject d)
        {
            return (bool)d.GetValue(IsIsModalContainerProperty);
        }

        /// <summary>
        /// Sets the value of the IsModalContainer attached property
        /// </summary>
        /// <param name="d"></param>
        /// <param name="value"></param>
        public static void SetIsModalContainer(DependencyObject d, bool value)
        {
            d.SetValue(IsIsModalContainerProperty, value);
        }

        #endregion

        #region · Attached Properties Callbacks ·

        private static void OnIsDesktop(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                IVirtualDesktopManager service;

                if (ServiceLocator.TryGetService<IVirtualDesktopManager>(out service))
                {
                    service.RegisterDesktop(d);
                }
            }
        }

        private static void OnIsModalContainer(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d != null)
            {
                IVirtualDesktopManager service;

                if (ServiceLocator.TryGetService<IVirtualDesktopManager>(out service))
                {
                    service.RegisterModalContainer(d);
                }
            }
        }

        #endregion

        #region · Fields ·

        private List<IVirtualDesktop>   virtualDesktops;
        private IVirtualDesktop         activeDesktop;

        #endregion

        #region · Properties ·

        public ReadOnlyObservableCollection<INavigationViewModel> ActiveDesktopWindows
        {
            get { return this.ActiveDesktop.ActiveWindows; }
        }

        public bool HasDesktopActive
        {
            get { return (this.ActiveDesktop != null); }
        }

        #endregion

        #region · Private Properties ·

        private List<IVirtualDesktop> Desktops
        {
            get
            {
                if (this.virtualDesktops == null)
                {
                    this.virtualDesktops = new List<IVirtualDesktop>();
                }

                return this.virtualDesktops;
            }
        }

        private IVirtualDesktop ActiveDesktop
        {
            get { return this.activeDesktop; }
            set
            {
                if (this.activeDesktop != value)
                {
                    this.activeDesktop = value;
                }
            }
        }

        #endregion

        #region · Constructors ·

        public VirtualDesktopManager()
        {
        }

        #endregion

        #region · Virtual Desktop Actions ·

        /// <summary>
        /// Activates the default desktop
        /// </summary>
        public void ActivateDefaultDesktop()
        {
            Logger.Debug("Activar el escritorio virtual por defecto");

            Debug.Assert(this.Desktops.Count > 0, "There are no desktop registered");

            this.ActiveDesktop = this.Desktops.First();
            this.ActiveDesktop.Activate();
        }

        /// <summary>
        /// Switches the active desktop
        /// </summary>
        public void SwitchDesktop()
        {
            Logger.Debug("Cambiar el escritorio virtual activo");

            Debug.Assert(this.Desktops.Count > 0, "There are no desktop registered");

            if (!this.HasDesktopActive)
            {
                this.ActivateDefaultDesktop();
            }
            else
            {
                IVirtualDesktop currentDesktop = this.ActiveDesktop;

                this.Desktops
                    .ToList()
                    .ForEach(vd => vd.Deactivate());

                this.ActiveDesktop = null;

                IVirtualDesktop desktop = this.Desktops.Where(vd => vd.Id != currentDesktop.Id).FirstOrDefault();

                if (desktop != null)
                {
                    this.ActiveDesktop = desktop;
                    this.ActiveDesktop.Activate();
                }
                else
                {
                    this.ActivateDefaultDesktop();
                }
            }
        }

        /// <summary>
        /// Shows the desktop
        /// </summary>
        public void ShowDesktop()
        {
            Logger.Debug("Mostrar el escritorio virtual activo");

            this.VerifyActiveDesktop();

            this.ActiveDesktop.ShowDesktop();
        }

        /// <summary>
        /// Saves the active desktop to disk
        /// </summary>
        public void SaveCurrentDesktop()
        {
            Logger.Debug("Guardar los cambios del escritorio virtual activo");

            this.VerifyActiveDesktop();

            this.ActiveDesktop.Save();
        }

        public void SaveAllDesktops()
        {
            Logger.Debug("Guardar los cambios de todos los escritorios virtuales");

            this.Desktops
                .ToList()
                .ForEach(vd => vd.Save());
        }

        #endregion

        #region · Window Methods ·

        /// <summary>
        /// Shows the given window instance
        /// </summary>
        /// <param name="element">A <see cref="WindowElement"/> instance</param>
        public void Show(IWindow window)
        {
            Logger.Debug<IWindow>("Mostrar una nueva ventana ({0})", window);

            this.VerifyActiveDesktop();

            this.ActiveDesktop.Show(window);
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <returns></returns>
        public DialogResult ShowDialog(IModalVindow window)
        {
            Logger.Debug<IModalVindow>("Mostrar una nueva ventana modal ({0})", window);

            DialogResult dialogResult = DialogResult.None;

            this.Invoke
            (
                delegate
                {
                    try
                    {
                        dialogResult = window.ShowDialog();
                    }
                    finally
                    {
                        if (window != null)
                        {
                            window.Close();
                            window = null;
                        }
                    }
                }
            );

            return dialogResult;
        }

        /// <summary>
        /// Closes the current dialog
        /// </summary>
        public void CloseDialog()
        {
            Logger.Debug("Cierre de la ventana modal activa");

            this.VerifyActiveDesktop();

            this.Invoke(
                delegate
                {
                    WindowElement window = WindowElement.ModalContainerPanel.Children.OfType<WindowElement>().FirstOrDefault();

                    if (window != null)
                    {
                        window.Hide();
                    }
                });
        }

        /// <summary>
        /// Restores the window with the given Id
        /// </summary>
        /// <param name="id"></param>
        public void Restore(Guid id)
        {
            this.VerifyActiveDesktop();

            this.ActiveDesktop.Restore(id);
        }

        #endregion

        #region · Shortcut Methods ·

        /// <summary>
        /// Creates a new shortcut with the given title and target
        /// </summary>
        /// <param name="title"></param>
        /// <param name="target"></param>
        public void CreateShortcut<T>(string title, string target) 
            where T : IShortcutViewModel, new()
        {
            Logger.Debug("Crear un nuevo acceso directo en el escritorio virtual activo ({0} - {1})", title, target);

            this.VerifyActiveDesktop();

            this.ActiveDesktop.CreateShortcut<T>(title, target);
        }

        /// <summary>
        /// Creates a new shortcut with the given title and target
        /// </summary>
        /// <param name="title"></param>
        /// <param name="target"></param>
        public void CreateShortcut<T>(string title, string target, Point position) 
            where T : IShortcutViewModel, new()
        {
            Logger.Debug("Crear un nuevo acceso directo en el escritorio virtual activo ({0} - {1})", title, target);

            this.VerifyActiveDesktop();

            this.ActiveDesktop.CreateShortcut<T>(title, target, position);
        }

        #endregion

        #region · IDesktopElement Methods ·

        public void Show<T>()
            where T : IDesktopElement, new()
        {
            Logger.Debug("Mostrar un nuevo elemento en el escritorio virtual activo ({0})", typeof(T));

            this.VerifyActiveDesktop();

            this.ActiveDesktop.Show<T>();            
        }

        public void Show<T>(Point position)
            where T : IDesktopElement, new()
        {
            Logger.Debug("Mostrar un nuevo elemento en el escritorio virtual activo ({0})", typeof(T));

            this.VerifyActiveDesktop();

            this.ActiveDesktop.Show<T>(position);
        }

        public void Show(IDesktopElement instance)
        {
            Logger.Debug("Mostrar un nuevo elemento en el escritorio virtual activo ({0})", instance.GetType());

            this.VerifyActiveDesktop();

            this.ActiveDesktop.Show(instance);
        }

        public void Show(IDesktopElement instance, Point position)
        {
            Logger.Debug("Mostrar un nuevo elemento en el escritorio virtual activo ({0})", instance.GetType());

            this.VerifyActiveDesktop();

            this.ActiveDesktop.Show(instance, position);
        }

        #endregion

        #region · Common Methods ·

        /// <summary>
        /// Closes the given element with the given Id
        /// </summary>
        /// <param name="id">The id.</param>
        public void Close(Guid id)
        {
            Logger.Debug("Cerrar un elemento en el escritorio virtual activo ({0})", id);

            this.VerifyActiveDesktop();

            this.ActiveDesktop.Close(id);
        }

        /// <summary>
        /// Closes all
        /// </summary>
        public void CloseAll()
        {
            Logger.Debug("Cerrar todos los elementos en el escritorio virtual activo");

            this.VerifyActiveDesktop();

            this.Desktops
                .ToList()
                .ForEach(vd => vd.CloseAll());

            this.DeactivateAll();
        }

        #endregion

        #region · Desktop registration Methods ·

        /// <summary>
        /// Attach the given dependency object as a Desktop to the Window manager
        /// </summary>
        /// <param name="desktop"></param>
        public void RegisterDesktop(DependencyObject d)
        {
            Logger.Debug("Registro de escritorio virtual ({0})", d);

            Debug.Assert(d != null, "do");
            Debug.Assert(d is Desktop, "do is not an instance of Desktop");

            Desktop desktop = d as Desktop;

            if (desktop != null)
            {
                if (this.Desktops.Where(vd => vd.Id == desktop.Id).Count() == 0)
                {
                    this.Desktops.Add(new VirtualDesktop(desktop));
                }
            }
        }

        /// <summary>
        /// Attach the given dependency object as a Modal Container to the Window manager
        /// </summary>
        /// <param name="desktop"></param>
        public void RegisterModalContainer(DependencyObject d)
        {
            Logger.Debug("Registro de contenedor para ventanas modales ({0})", d);

            Panel container = d as Panel;

            if (container != null)
            {
                WindowElement.ModalContainerPanel = container;
            }
        }

        #endregion

        #region · Private Methods ·

        private void VerifyActiveDesktop()
        {
            if (!this.HasDesktopActive)
            {
                this.ActivateDefaultDesktop();
            }
        }

        private void DeactivateAll()
        {
            this.Desktops
                .AsParallel()
                .ForAll(vd => vd.Deactivate());
        }

        #endregion
    }
}
