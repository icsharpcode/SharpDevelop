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
		readonly FormsDesignerViewContent formDesigner;
		
		public EventBindingService(FormsDesignerViewContent formDesigner, IServiceProvider provider) : base(provider)
		{
			if (formDesigner == null)
				throw new ArgumentNullException("formDesigner");
			this.formDesigner = formDesigner;
		}

		protected override string CreateUniqueMethodName(IComponent component, EventDescriptor e)
		{
			return String.Format("{0}{1}", Char.ToUpper(component.Site.Name[0]) + component.Site.Name.Substring(1), e.DisplayName);
		}

		// sohuld look around in form class for compatiable methodes
		protected override ICollection GetCompatibleMethods(EventDescriptor e)
		{
			return this.formDesigner.GetCompatibleMethods(e);
		}
		
		protected override bool ShowCode()
		{
			if (formDesigner != null) {
				formDesigner.ShowSourceCode();
				return true;
			}
			return false;
		}

		protected override bool ShowCode(int lineNumber)
		{
			if (formDesigner != null) {
				formDesigner.ShowSourceCode(lineNumber);
				return true;
			}
			return false;
		}

		protected override bool ShowCode(IComponent component, EventDescriptor edesc, string methodName)
		{
			if (formDesigner != null) {
				formDesigner.ShowSourceCode(component, edesc, methodName);
				return true;
			}
			return false;
		}

	}
}
