// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
