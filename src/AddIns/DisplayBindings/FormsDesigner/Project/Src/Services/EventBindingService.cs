// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.ComponentModel;

namespace ICSharpCode.FormsDesigner.Services
{
	public class EventBindingService : System.ComponentModel.Design.EventBindingService
	{
		readonly IFormsDesigner formsDesigner;
		
		public EventBindingService(IFormsDesigner formsDesigner, IServiceProvider provider) : base(provider)
		{
			if (formsDesigner == null)
				throw new ArgumentNullException("formDesigner");
			this.formsDesigner = formsDesigner;
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
//			if (formDesigner.SharpDevelopDesignerOptions.GenerateVisualStudioStyleEventHandlers) {
//				return "{0}_{1}";
//			}
//			return "{0}{1}";
			return formsDesigner.DesignerOptions.EventHandlerNameFormat;
		}

		// sohuld look around in form class for compatible methodes
		protected override ICollection GetCompatibleMethods(EventDescriptor e)
		{
			return this.formsDesigner.Generator.GetCompatibleMethods(new EventDescriptorProxy(e));
		}
		
		protected override bool ShowCode()
		{
			if (formsDesigner != null) {
				formsDesigner.ShowSourceCode();
				return true;
			}
			return false;
		}

		protected override bool ShowCode(int lineNumber)
		{
			if (formsDesigner != null) {
				formsDesigner.ShowSourceCode(lineNumber);
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
			if (formsDesigner != null && edesc != null) {
				formsDesigner.ShowSourceCode(component, edesc, methodName);
				return true;
			}
			return false;
		}
	}
	
	public class EventDescriptorProxy : MarshalByRefObject
	{
		public string Name { get; private set; }
		public bool IsMulticast { get; private set; }
		public Type ComponentType { get; private set; }
		public Type EventType { get; private set; }
		public AttributeCollection Attributes { get; private set; }
		public string Category { get; private set; }
		public string Description { get; private set; }
		public bool IsBrowsable { get; private set; }
		public bool DesignTimeOnly { get; private set; }
		public string DisplayName { get; private set; }
		
		public EventDescriptorProxy(EventDescriptor ed)
		{
			Name = ed.Name;
			IsMulticast = ed.IsMulticast;
			ComponentType = ed.ComponentType;
			EventType = ed.EventType;
			Attributes = ed.Attributes;
			Category = ed.Category;
			Description = ed.Description;
			IsBrowsable = ed.IsBrowsable;
			Name = ed.Name;
			DesignTimeOnly = ed.DesignTimeOnly;
			DisplayName  = ed.DisplayName;
		}
	}
}
