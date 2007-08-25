// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2667$</version>
// </file>

using System;
using System.ComponentModel;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;
using System.Windows.Controls;

namespace ICSharpCode.WpfDesign.AddIn
{
	sealed class EventHandlerService : IEventHandlerService
	{
		WpfViewContent viewContent;
		
		public EventHandlerService(WpfViewContent viewContent)
		{
			if (viewContent == null)
				throw new ArgumentNullException("viewContent");
			this.viewContent = viewContent;
		}
		
		IProjectContent GetProjectContent()
		{
			IProject p = ProjectService.OpenSolution.FindProjectContainingFile(viewContent.PrimaryFileName);
			if (p != null)
				return ParserService.GetProjectContent(p) ?? ParserService.DefaultProjectContent;
			else
				return ParserService.DefaultProjectContent;
		}
		
		public void CreateEventHandler(DesignItem item, DesignItemProperty eventProperty)
		{
			/*Designer.Xaml.XamlDesignContext xamlContext = item.Context as Designer.Xaml.XamlDesignContext;
			if (xamlContext != null) {
				string className = xamlContext.ClassName;
				if (!string.IsNullOrEmpty(className)) {
					IClass c = GetProjectContent().GetClass(className, 0);
					if (c != null && !string.IsNullOrEmpty(c.CompilationUnit.FileName)) {
						
					}
				}
			}
			
			return;
			*/
			
			string handlerName = (string)eventProperty.ValueOnInstance;
			if (string.IsNullOrEmpty(handlerName)) {
				if (string.IsNullOrEmpty(item.Name)) {
					// cannot create event for unnamed controls
					if (viewContent.PropertyEditor.NameTextBox.Focus()) {
						IErrorService errorService = item.Context.Services.GetService<IErrorService>();
						if (errorService != null) {
							Label errorLabel = new Label();
							errorLabel.Content = "You need to give the " + item.ComponentType.Name + " a name.";
							errorService.ShowErrorTooltip(viewContent.PropertyEditor.NameTextBox, errorLabel);
						}
					}
					return;
				}
				
				handlerName = item.Name + eventProperty.Name;
				eventProperty.SetValue(handlerName);
			}
			
			//viewContent.PrimaryFileName + ".cs"
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
