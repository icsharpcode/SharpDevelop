// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.ComponentModel;
using ICSharpCode.FormsDesigner.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Services
{
	/*
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
			string componentName = GetComponentName(component);
			return GetEventHandlerName(componentName, e.DisplayName);
		}
		
		string GetComponentName(IComponent component)
		{
			string siteName = component.Site.Name;
			return Char.ToUpper(siteName[0]) + siteName.Substring(1);
		}
		
		string GetEventHandlerName(string componentName, string eventName)
		{
			string eventHandlerNameFormat = GetEventHandlerNameFormat();
			return String.Format(eventHandlerNameFormat, componentName, eventName);
		}
		
		string GetEventHandlerNameFormat()
		{
			if (GeneralOptionsPanel.GenerateVisualStudioStyleEventHandlers) {
				return "{0}_{1}";
			}
			return "{0}{1}";
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
			// There were reports of an ArgumentNullException caused by edesc==null.
			// Looking at the .NET code calling this method, this can happen when there are two calls to ShowCode() before the Application.Idle
			// event gets raised. In that case, ShowCode() already was called for the second set of arguments, and we can safely ignore
			// the call with edesc==null.
			if (formDesigner != null && edesc != null) {
				formDesigner.ShowSourceCode(component, edesc, methodName);
				return true;
			}
			return false;
		}

	}
	*/
}
