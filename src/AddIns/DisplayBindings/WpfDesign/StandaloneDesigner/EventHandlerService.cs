// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2667$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.WpfDesign;

namespace StandaloneDesigner
{
	sealed class EventHandlerService : IEventHandlerService
	{
		Window1 mainWindow;
		
		public EventHandlerService(Window1 mainWindow)
		{
			this.mainWindow = mainWindow;
		}
		
		public void CreateEventHandler(DesignItem item, DesignItemProperty eventProperty)
		{
			string handlerName = (string)eventProperty.ValueOnInstance;
			if (string.IsNullOrEmpty(handlerName)) {
				if (string.IsNullOrEmpty(item.Name)) {
					// cannot create event for unnamed controls
					if (mainWindow.propertyEditor.NameTextBox.Focus()) {
						IErrorService errorService = item.Context.Services.GetService<IErrorService>();
						if (errorService != null) {
							Label errorLabel = new Label();
							errorLabel.Content = "You need to give the " + item.ComponentType.Name + " a name.";
							errorService.ShowErrorTooltip(mainWindow.propertyEditor.NameTextBox, errorLabel);
						}
					}
					return;
				}
				
				handlerName = item.Name + eventProperty.Name;
				eventProperty.SetValue(handlerName);
			}
			
			MessageBox.Show("show " + handlerName);
		}
		
		public DesignItemProperty GetDefaultEvent(DesignItem item)
		{
			object[] attributes = item.ComponentType.GetCustomAttributes(typeof(DefaultEventAttribute), true);
			if (attributes.Length == 1) {
				DefaultEventAttribute dae = (DefaultEventAttribute)attributes[0];
				DesignItemProperty property = item.Properties.GetProperty(dae.Name);
				if (property != null && property.IsEvent) {
					return property;
				}
			}
			return null;
		}
	}
}
