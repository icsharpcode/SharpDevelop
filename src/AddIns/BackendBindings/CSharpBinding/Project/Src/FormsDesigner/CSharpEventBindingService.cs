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
using System.Linq;
using System.Reflection;
using System.Windows.Threading;
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Gui.OptionPanels;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using CSharpBinding.Refactoring;

namespace CSharpBinding.FormsDesigner
{
	public class CSharpEventBindingService : System.ComponentModel.Design.EventBindingService
	{
		readonly CSharpDesignerLoader loader;
		readonly ICSharpDesignerLoaderContext context;
		
		public CSharpEventBindingService(ICSharpDesignerLoaderContext context, IServiceProvider provider, CSharpDesignerLoader loader) : base(provider)
		{
			this.loader = loader;
			if (context == null)
				throw new ArgumentNullException("context");
			if (loader == null)
				throw new ArgumentNullException("loader");
			this.context = context;
			this.loader = loader;
		}

		protected override string CreateUniqueMethodName(IComponent component, EventDescriptor e)
		{
			string componentName = GetComponentName(component);
			return GetEventHandlerName(componentName, e.DisplayName);
		}
		
		string GetComponentName(IComponent component)
		{
			string siteName = component.Site.Name;
			if (GeneralOptionsPanel.GenerateVisualStudioStyleEventHandlers)
				return siteName;
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

		protected override ICollection GetCompatibleMethods(EventDescriptor e)
		{
			ITypeDefinition definition = loader.GetPrimaryTypeDefinition();
			ArrayList compatibleMethods = new ArrayList();
			MethodInfo methodInfo = e.EventType.GetMethod("Invoke");
			var methodInfoParameters = methodInfo.GetParameters();
			
			foreach (IMethod method in definition.Methods) {
				if (method.Parameters.Count == methodInfoParameters.Length) {
					bool found = true;
					for (int i = 0; i < methodInfoParameters.Length; ++i) {
						ParameterInfo pInfo = methodInfoParameters[i];
						IParameter p = method.Parameters[i];
						if (p.Type.ReflectionName != pInfo.ParameterType.ToString()) {
							found = false;
							break;
						}
					}
					if (found) {
						compatibleMethods.Add(method.Name);
					}
				}
			}
			
			return compatibleMethods;
		}
		
		protected override bool ShowCode()
		{
			if (context != null) {
				context.ShowSourceCode();
				return true;
			}
			return false;
		}

		protected override bool ShowCode(int lineNumber)
		{
			if (context != null) {
				context.ShowSourceCode(lineNumber);
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
			if (context != null && edesc != null) {
				// TODO : does not properly update events list in properties pad!
				IEvent evt = FindEvent(edesc);
				if (evt == null) return false;
				context.ShowSourceCode();
				Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)delegate { InsertEventHandlerInternal(methodName, evt); });
				return true;
			}
			return false;
		}
		
		void InsertEventHandlerInternal(string methodName, IEvent evt)
		{
			CSharpCodeGenerator generator = new CSharpCodeGenerator();
			var primary = loader.GetPrimaryTypeDefinition();
			var evtHandler = primary.GetMethods(m => m.Name == methodName, GetMemberOptions.IgnoreInheritedMembers).FirstOrDefault();
			if (evtHandler == null) {
				generator.InsertEventHandler(primary, methodName, evt, true);
			}
			else {
				CSharpBinding.Parser.CSharpFullParseInformation parseInfo;
				var node = evtHandler.GetDeclaration(out parseInfo) as MethodDeclaration;
				if (node != null && !node.Body.IsNull) {
					var location = node.Body.FirstChild.StartLocation;
					var firstStatement = node.Body.Children.OfType<Statement>().FirstOrDefault();
					if (firstStatement != null)
						location = firstStatement.StartLocation;
					// TODO : does not jump correctly...
					SD.FileService.JumpToFilePosition(new FileName(evtHandler.Region.FileName), location.Line, location.Column);
				}
			}
		}
		
		IEvent FindEvent(EventDescriptor edesc)
		{
			var compilation = context.GetCompilation();
			var type = compilation.FindType(edesc.ComponentType);
			return type.GetEvents(evt => evt.Name == edesc.Name).FirstOrDefault();
		}
	}
}
