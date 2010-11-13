// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.WpfDesign.AddIn
{
	abstract class AbstractEventHandlerService : IEventHandlerService
	{
		WpfViewContent viewContent;
		
		protected AbstractEventHandlerService(WpfViewContent viewContent)
		{
			if (viewContent == null)
				throw new ArgumentNullException("viewContent");
			this.viewContent = viewContent;
		}
		
		protected IProjectContent GetProjectContent()
		{
			IProject p = FindProjectContainingFile();
			if (p != null)
				return ParserService.GetProjectContent(p) ?? ParserService.DefaultProjectContent;
			else
				return ParserService.DefaultProjectContent;
		}
		
		IProject FindProjectContainingFile()
		{
			if (ProjectService.OpenSolution != null) {
				return ProjectService.OpenSolution.FindProjectContainingFile(viewContent.PrimaryFileName);
			}
			return null;
		}
		
		protected IClass GetDesignedClass()
		{
			Designer.Xaml.XamlDesignContext xamlContext = viewContent.DesignContext as Designer.Xaml.XamlDesignContext;
			if (xamlContext != null) {
				string className = xamlContext.ClassName;
				if (!string.IsNullOrEmpty(className)) {
					return GetProjectContent().GetClass(className, 0);
				}
			}
			return null;
		}
		
		protected IClass GetDesignedClassCodeBehindPart(IClass c)
		{
			CompoundClass compound = c as CompoundClass;
			if (compound != null) {
				c = null;
				foreach (IClass part in compound.Parts) {
					if (string.IsNullOrEmpty(part.CompilationUnit.FileName))
						continue;
					if (".xaml".Equals(Path.GetExtension(part.CompilationUnit.FileName), StringComparison.OrdinalIgnoreCase))
						continue;
					if (c == null || c.CompilationUnit.FileName.Length > part.CompilationUnit.FileName.Length)
						c = part;
				}
			}
			return c;
		}
		
		protected abstract void CreateEventHandlerInternal(Type eventHandlerType, string handlerName);
		
		public void CreateEventHandler(DesignItemProperty eventProperty)
		{
			var item = eventProperty.DesignItem;
			string handlerName = (string)eventProperty.ValueOnInstance;			

			if (string.IsNullOrEmpty(handlerName)) {
				if (string.IsNullOrEmpty(item.Name)) {
					GenerateName(eventProperty.DesignItem);
				}
				handlerName = item.Name + "_" + eventProperty.Name;
				eventProperty.SetValue(handlerName);
			}
			CreateEventHandlerInternal(eventProperty.ReturnType, handlerName);
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
	}
}
