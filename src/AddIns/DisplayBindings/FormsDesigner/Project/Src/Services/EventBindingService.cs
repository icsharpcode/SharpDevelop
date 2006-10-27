// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Services
{
	public class EventBindingService : System.ComponentModel.Design.EventBindingService
	{
		
		public EventBindingService(IServiceProvider provider) : base(provider)
		{
		}

		protected override string CreateUniqueMethodName(IComponent component, EventDescriptor e)
		{
			return String.Format("{0}{1}", Char.ToUpper(component.Site.Name[0]) + component.Site.Name.Substring(1), e.DisplayName);
		}

		// sohuld look around in form class for compatiable methodes
		protected override ICollection GetCompatibleMethods(EventDescriptor e)
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window != null) {
				FormsDesignerViewContent formDesigner = window.ActiveViewContent as FormsDesignerViewContent;
				
				if (formDesigner != null) {
					return formDesigner.GetCompatibleMethods(e);
				}
			}
			return new string[]{};
		}
		
		protected override bool ShowCode()
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) {
				return false;
			}

			FormsDesignerViewContent formDesigner = window.ActiveViewContent as FormsDesignerViewContent;

			if (formDesigner != null) {
				formDesigner.ShowSourceCode();
				return true;
			}
			return false;
		}

		protected override bool ShowCode(int lineNumber)
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null) {
				return false;
			}

			FormsDesignerViewContent formDesigner = window.ActiveViewContent as FormsDesignerViewContent;

			if (formDesigner != null) {
				formDesigner.ShowSourceCode(lineNumber);
				return true;
			}
			return false;
		}

		protected override bool ShowCode(IComponent component, EventDescriptor edesc, string methodName)
		{
			IWorkbenchWindow window = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (window == null || edesc == null || edesc.Name == null || edesc.Name.Length == 0) {
				return false;
			}
			FormsDesignerViewContent formDesigner = window.ActiveViewContent as FormsDesignerViewContent;
			
			if (formDesigner != null) {
				formDesigner.ShowSourceCode(component, edesc, methodName);
				return true;
			}
			return false;
		}

	}
}
