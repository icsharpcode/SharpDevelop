// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Collections;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
#endregion

namespace WorkflowDesigner
{
	/// <summary>
	/// Description of EventBindingService.
	/// </summary>
	public class EventBindingService : IEventBindingService, IServiceProvider
	{
		IServiceProvider provider;
		
		public EventBindingService(IServiceProvider provider)
		{
			this.provider = provider;
		}

		public object GetService(Type serviceType)
		{
			return provider.GetService(serviceType);
		}
		
		internal IWorkflowDesignerGeneratorService GeneratorService{
			get{
				return (IWorkflowDesignerGeneratorService)GetService(typeof(IWorkflowDesignerGeneratorService));
			}
		}
		
		public string CreateUniqueMethodName(IComponent component, EventDescriptor e)
		{
			LoggingService.Debug("CreateUniqueMethodName(" + component + ", " + e + ")");
			return String.Format("{0}{1}", Char.ToUpper(component.Site.Name[0]) + component.Site.Name.Substring(1), e.DisplayName);
		}
		
		public ICollection GetCompatibleMethods(EventDescriptor e)
		{
			
			LoggingService.Debug("GetCompatibleMethods(" + e + ")");

			IWorkflowDesignerGeneratorService generatorService = GeneratorService;
			if (generatorService != null)
				return GeneratorService.GetCompatibleMethods(e);
			
			return new string[]{};
		}
		
		public EventDescriptor GetEvent(PropertyDescriptor property)
		{
			EventPropertyDescriptor epd = property as EventPropertyDescriptor;
			if (epd == null)
				return null;
			
			return epd.eventDescriptor;
		}
		
		public PropertyDescriptorCollection GetEventProperties(EventDescriptorCollection events)
		{
			ArrayList props = new ArrayList ();
			
			foreach (EventDescriptor e in events)
				props.Add (GetEventProperty (e));
			
			return new PropertyDescriptorCollection ((PropertyDescriptor[]) props.ToArray (typeof (PropertyDescriptor)));
		}
		
		public PropertyDescriptor GetEventProperty(EventDescriptor e)
		{
			return new EventPropertyDescriptor(this,e);
		}

		public bool ShowCode()
		{
			IWorkflowDesignerGeneratorService generatorService = GeneratorService;
			if (generatorService != null)
				return GeneratorService.ShowCode();
			
			return false;
		}
		
		public bool ShowCode(int lineNumber)
		{
			IWorkflowDesignerGeneratorService generatorService = GeneratorService;
			if (generatorService != null)
				return GeneratorService.ShowCode(lineNumber);

			return false;
			
		}
		
		public bool ShowCode(IComponent component, EventDescriptor e)
		{
			IWorkflowDesignerGeneratorService generatorService = GeneratorService;
			if (generatorService != null)
				return GeneratorService.ShowCode(component, e);
			
			return false;
		}
		
		public bool ShowCode(IComponent component, EventDescriptor e, string methodName)
		{
			IWorkflowDesignerGeneratorService generatorService = GeneratorService;
			if (generatorService != null)
				return GeneratorService.ShowCode(component, e, methodName);
			
			return false;
		}

		public void UseMethod(IComponent component, EventDescriptor e, string methodName)
		{
			LoggingService.Debug("UseMethod()");

			IWorkflowDesignerGeneratorService generatorService = GeneratorService;
			if (generatorService != null)
				GeneratorService.UseMethod(component, e, methodName);

		}
	}
	
	public class EventPropertyDescriptor : PropertyDescriptor
	{
		internal EventDescriptor eventDescriptor;
		private IServiceProvider provider;
		
		public EventPropertyDescriptor(IServiceProvider provider,  EventDescriptor eventDescriptor) : base(eventDescriptor)
		{
			this.eventDescriptor = eventDescriptor;
			this.provider = provider;
		}
		
		public override Type ComponentType {
			get {
				return eventDescriptor.ComponentType;
			}
		}
		
		public override bool IsReadOnly {
			get {
				return false;
			}
		}
		
		public override Type PropertyType {
			get {
				return eventDescriptor.EventType;
			}
		}
		
		public override bool CanResetValue(object component)
		{
			return false;
		}
		
		public override object GetValue(object component)
		{
			Activity activity = component as Activity;
			if (component == null)
				throw new ArgumentException("component must be derived from Activity");
			
			string value = string.Empty;
			
			// Find method name associated with the EventDescriptor.
			Hashtable events = activity.GetValue(WorkflowMarkupSerializer.EventsProperty) as Hashtable;
			
			if (events != null) {
				if (events.ContainsKey(this.eventDescriptor.Name))
					value = events[this.eventDescriptor.Name] as string;
			}
			
			return value;
		}
		
		public override void ResetValue(object component)
		{
			SetValue(component, null);
		}
		
		public override void SetValue(object component, object value)
		{
			// Validate the parameters.
			Activity activity = component as Activity;
			if (component == null)
				throw new ArgumentException("component must be derived from Activity");
			
			// Get the event list form the dependency object.
			Hashtable events = activity.GetValue(WorkflowMarkupSerializer.EventsProperty) as Hashtable;

			if (events == null) {
				events = new Hashtable();
				activity.SetValue(WorkflowMarkupSerializer.EventsProperty, events);
			}

			string oldValue = events[this.eventDescriptor.Name] as string;
			
			// Value not changed need go no further.
			if (oldValue != null) {
				if (oldValue.CompareTo(value) == 0)
					return;				
			}
			
			IComponentChangeService componentChangedService = provider.GetService(typeof(IComponentChangeService)) as  IComponentChangeService;
			componentChangedService.OnComponentChanging(component, this.eventDescriptor);

			// Update to new value.
			events[this.eventDescriptor.Name] = value;
			
			componentChangedService.OnComponentChanged(component, this.eventDescriptor, oldValue, value);
		}

		
		public override bool ShouldSerializeValue(object component)
		{
			if (GetValue (component) == null) return false;
			return true;
		}
	}
}
