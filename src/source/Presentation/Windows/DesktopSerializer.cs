// Copyright (c) Carlos Guzmán Álvarez. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Linq;
using Chronos.Extensions.Windows;
using Chronos.Presentation.Core.Services;
using Chronos.Presentation.Core.Windows;
using Chronos.Presentation.Windows.Controls;
using nRoute.ViewServices;

namespace Chronos.Presentation.Windows
{
    /// <summary>
    /// Desktop serializer for <see cref="E:Chronos.Presentation.Windows.Elements.WidgetElement"/> and 
    /// <see cref="E:Chronos.Presentation.Windows.Elements.ShortcutElement"/> contents.
    /// </summary>
    internal static class DesktopSerializer
    {
        #region · Save/Load Methods ·

        /// <summary>
        /// Loads the contents of the specified filename in the given <see cref="Desktop"/> instance.
        /// </summary>
        /// <param name="desktop">The desktop.</param>
        /// <param name="filename">The filename.</param>
        public static void Load(Desktop desktop, string filename)
        {
            XElement root = LoadFromFile(filename);

            if (root == null)
            {
                return;
            }

            // Widget Deserialization
            IEnumerable<XElement> widgetsXML = root.Elements("WidgetElements").Elements("WidgetElement");

            foreach (XElement widgetXML in widgetsXML)
            {
                Guid id = new Guid(widgetXML.Element("Id").Value);
                WidgetElement item = DeserializeWidget(widgetXML, id, 0, 0);

                desktop.AddElement(item, item.GetPosition());
            }

            // Shortcut Deserialization
            IEnumerable<XElement> shortcutsXML = root.Elements("ShortcutElements").Elements("ShortcutElement");
            int ti = 0;

            foreach (XElement shortcutXML in shortcutsXML)
            {
                Guid id = new Guid(shortcutXML.Element("Id").Value);
                ShortcutElement item = DeserializeShortcut(shortcutXML, id, 0, 0);

                item.TabIndex = ++ti;

                desktop.AddElement(item, item.GetPosition());
            }
        }

        /// <summary>
        /// Saves the contents of the specified <see cref="Desktop"/> instance in the given file.
        /// </summary>
        /// <param name="desktop">The desktop.</param>
        /// <param name="filename">The filename.</param>
        public static void Save(Desktop desktop, string filename)
        {
            IEnumerable<WidgetElement> widgets = desktop.Children.OfType<WidgetElement>();
            IEnumerable<ShortcutElement> shortcuts = desktop.Children.OfType<ShortcutElement>();

            XElement widgetsItemsXML = SerializeWidgets(widgets);
            XElement shortcutsItemsXML = SerializeShortcuts(shortcuts);

            XElement root = new XElement("Root");

            root.Add(widgetsItemsXML);
            root.Add(shortcutsItemsXML);

            SaveFile(filename, root);
        }

        #endregion

        #region · Serialization Methods ·

        private static XElement LoadFromFile(string filename)
        {
            try
            {
                if (File.Exists(filename))
                {
                    return XElement.Load(filename);
                }
            }
            catch (Exception)
            {
                IShowMessageViewService showMessageService = ViewServiceLocator.GetViewService<IShowMessageViewService>();

                showMessageService.ButtonSetup = DialogButton.Ok;
                showMessageService.Caption = "Chronos - Error en la carga del escritorio";
                showMessageService.Text = "No ha sido posible realizar la carga del escritorio.";

                showMessageService.ShowMessage();
            }

            return null;
        }

        private static void SaveFile(string filename, XElement xElement)
        {
            try
            {
                xElement.Save(filename);
            }
            catch (Exception)
            {
                IShowMessageViewService showMessageService = ViewServiceLocator.GetViewService<IShowMessageViewService>();

                showMessageService.ButtonSetup = DialogButton.Ok;
                showMessageService.Caption = "Chronos - Error al guardar el estado del escritorio";
                showMessageService.Text = "No ha sido posible al guardar el estado del escritorio.";

                showMessageService.ShowMessage();
            }
        }

        private static XElement SerializeWidgets(IEnumerable<WidgetElement> widgets)
        {
            XElement serializedItems = new XElement
                ("WidgetElements",
                    from item in widgets
                    select new XElement
                    ("WidgetElement",
                                new XElement("Type", item.GetType().AssemblyQualifiedName),
                                new XElement("Left", Canvas.GetLeft(item)),
                                new XElement("Top", Canvas.GetTop(item)),
                                new XElement("Width", item.Width),
                                new XElement("Height", item.RealHeight),
                                new XElement("Id", item.Id),
                                new XElement("zIndex", Canvas.GetZIndex(item)),
                                new XElement("WidgetState", item.WidgetState),
                                new XElement("ShowMinimizeButton", item.ShowMinimizeButton)
                    )
                );

            return serializedItems;
        }

        private static WidgetElement DeserializeWidget(XElement itemXML, Guid id, double offsetX, double offsetY)
        {
            WidgetElement item = (WidgetElement)Activator.CreateInstance(Type.GetType(itemXML.Element("Type").Value));

            item.ShowMinimizeButton = Boolean.Parse(itemXML.Element("ShowMinimizeButton").Value);
            item.StartupLocation    = StartupPosition.Manual;
            
            item.Move(Double.Parse(itemXML.Element("Left").Value, CultureInfo.InvariantCulture) + offsetX,
                      Double.Parse(itemXML.Element("Top").Value, CultureInfo.InvariantCulture) + offsetY);
            item.SetZIndex(Int32.Parse(itemXML.Element("zIndex").Value));

            WindowState widgetState = (WindowState)Enum.Parse(typeof(WindowState), itemXML.Element("WidgetState").Value as String);

            item.Width      = Double.Parse(itemXML.Element("Width").Value, CultureInfo.InvariantCulture);
            item.Height     = Double.Parse(itemXML.Element("Height").Value, CultureInfo.InvariantCulture);

            if (widgetState == WindowState.Minimized)
            {
                item.WidgetState = widgetState;
            }

            return item;
        }

        private static XElement SerializeShortcuts(IEnumerable<ShortcutElement> shortcuts)
        {
            XElement serializedItems = new XElement("ShortcutElements",
                                       from item in shortcuts
                                       let contentXaml = XamlWriter.Save(((ShortcutElement)item).DataContext)
                                       select new XElement("ShortcutElement",
                                                  new XElement("Left", Canvas.GetLeft(item)),
                                                  new XElement("Top", Canvas.GetTop(item)),
                                                  new XElement("Width", item.Width),
                                                  new XElement("Height", item.Height),
                                                  new XElement("Id", item.Id),
                                                  new XElement("zIndex", Canvas.GetZIndex(item)),
                                                  new XElement("DataContext", contentXaml)
                                              ));

            return serializedItems;
        }

        private static ShortcutElement DeserializeShortcut(XElement itemXML, Guid id, double offsetX, double offsetY)
        {
            ShortcutElement item = new ShortcutElement
            {
                DataContext = XamlReader.Load(XmlReader.Create(new StringReader(itemXML.Element("DataContext").Value))),
                Width       = Double.Parse(itemXML.Element("Width").Value, CultureInfo.InvariantCulture),
                Height      = Double.Parse(itemXML.Element("Height").Value, CultureInfo.InvariantCulture)
            };

            item.StartupLocation = StartupPosition.Manual;
            item.Move(Double.Parse(itemXML.Element("Left").Value, CultureInfo.InvariantCulture) + offsetX,
                      Double.Parse(itemXML.Element("Top").Value, CultureInfo.InvariantCulture) + offsetY);
            item.SetZIndex(Int32.Parse(itemXML.Element("zIndex").Value));

            return item;
        }

        #endregion
    }
}