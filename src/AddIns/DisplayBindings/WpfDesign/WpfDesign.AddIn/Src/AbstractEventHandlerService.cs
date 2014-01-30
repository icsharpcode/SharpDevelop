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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Controls;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WpfDesign;

namespace ICSharpCode.WpfDesign.AddIn
{
	class SharpDevelopEventHandlerService : IEventHandlerService
	{
		WpfViewContent viewContent;
		
		public SharpDevelopEventHandlerService(WpfViewContent viewContent)
		{
			if (viewContent == null)
				throw new ArgumentNullException("viewContent");
			this.viewContent = viewContent;
		}
		
		IProject FindProjectContainingFile()
		{
			return SD.ProjectService.FindProjectContainingFile(viewContent.PrimaryFileName);
		}
		
		protected IType GetDesignedClass(ICompilation compilation)
		{
			var xamlContext = viewContent.DesignContext as Designer.Xaml.XamlDesignContext;
			if (xamlContext != null) {
				string className = xamlContext.ClassName;
				if (!string.IsNullOrEmpty(className)) {
					return compilation.FindType(new FullTypeName(className));
				}
			}
			return null;
		}
		
		public void CreateEventHandler(DesignItemProperty eventProperty)
		{
			string handlerName = (string)eventProperty.ValueOnInstance;

			if (string.IsNullOrEmpty(handlerName)) {
				var item = eventProperty.DesignItem;
				if (string.IsNullOrEmpty(item.Name)) {
					GenerateName(eventProperty.DesignItem);
				}
				handlerName = item.Name + "_" + eventProperty.Name;
				eventProperty.SetValue(handlerName);
			}
			
			IType t = GetDesignedClass(SD.ParserService.GetCompilation(FindProjectContainingFile()));
			if (t != null) {
				IMethod method = t.GetMethods(m => m.Name == handlerName).FirstOrDefault();
				if (method != null) {
					FileService.JumpToFilePosition(method.Region.FileName,
					                               method.Region.BeginLine, method.Region.BeginColumn);
					return;
				}
			}
			
			IProject p = FindProjectContainingFile();
			ITypeDefinition c = t.GetDefinition();
			
			if (p != null && c != null) {
				var e = FindEventDeclaration(c.Compilation, eventProperty.DeclaringType, eventProperty.Name);
				p.LanguageBinding.CodeGenerator.InsertEventHandler(c, handlerName, e, true);
			}
		}
		
		public DesignItemProperty GetDefaultEvent(DesignItem item)
		{
			object[] attributes = item.ComponentType.GetCustomAttributes(typeof(DefaultEventAttribute), true);
			if (attributes.Length == 1) {
				DefaultEventAttribute dae = (DefaultEventAttribute)attributes[0];
				var events = TypeDescriptor.GetEvents(item.Component);
				var eventInfo = events[dae.Name];
				if(eventInfo != null)
				{
					DesignItemProperty property = item.Properties.GetProperty(dae.Name);
					if (property != null && property.IsEvent)
					{
						return property;
					}
				}
			}
			return null;
		}

		void GenerateName(DesignItem item)
		{
			for (int i = 1;; i++) {
				try {
					string name = item.ComponentType.Name + i;
					name = char.ToLower(name[0]) + name.Substring(1);
					item.Name = name;
					break;
				} catch {
				}
			}
		}
		
		IEvent FindEventDeclaration(ICompilation compilation, Type declaringType, string name)
		{
			return compilation.FindType(declaringType).GetEvents(ue => ue.Name == name).FirstOrDefault();
		}
	}
}
