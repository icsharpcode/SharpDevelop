// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents a form or user control in the designer. Used to generate
	/// Python code after the form has been changed in the designer.
	/// </summary>
	public class PythonControl
	{
		StringBuilder codeBuilder;
		string indentString = String.Empty;
		int indent;
		IEventBindingService eventBindingService;
		static readonly Attribute[] notDesignOnlyFilter = new Attribute[] { DesignOnlyAttribute.No };
		static readonly DesignerSerializationVisibility[] notHiddenDesignerVisibility = new DesignerSerializationVisibility[] { DesignerSerializationVisibility.Content, DesignerSerializationVisibility.Visible };
		static readonly DesignerSerializationVisibility[] contentDesignerVisibility = new DesignerSerializationVisibility[] { DesignerSerializationVisibility.Content };
		
		/// <summary>
		/// Used so the EventBindingService.GetEventProperty method can be called to get the property descriptor
		/// for an event.
		/// </summary>
		class PythonFormEventBindingService : EventBindingService
		{
			public PythonFormEventBindingService()
				: base(new ServiceContainer())
			{
			}
			
			protected override string CreateUniqueMethodName(IComponent component, EventDescriptor e)
			{
				return String.Empty;
			}
			
			protected override ICollection GetCompatibleMethods(EventDescriptor e)
			{
				return new ArrayList();
			}
			
			protected override bool ShowCode()
			{
				return false;
			}
			
			protected override bool ShowCode(int lineNumber)
			{
				return false;
			}
			
			protected override bool ShowCode(IComponent component, EventDescriptor e, string methodName)
			{
				return false;
			}
		}
		
		public PythonControl() 
			: this("\t")
		{
		}
		
		public PythonControl(string indentString) 
			: this(indentString, new PythonFormEventBindingService())
		{
		}
		
		PythonControl(string indentString, IEventBindingService eventBindingService)
		{
			this.indentString = indentString;
			this.eventBindingService = eventBindingService;
		}

		/// <summary>
		/// Generates python code for the InitializeComponent method based on the controls added to the form.
		/// </summary>
		public string GenerateInitializeComponentMethod(Control control)
		{
			codeBuilder = new StringBuilder();
			
			AppendIndentedLine("def InitializeComponent(self):");
			IncreaseIndent();
			
			GenerateInitializeComponentMethodBodyInternal(control);
			
			return codeBuilder.ToString();
		}
		
		/// <summary>
		/// Generates the InitializeComponent method body.
		/// </summary>
		public string GenerateInitializeComponentMethodBody(Control control, int initialIndent)
		{
			codeBuilder = new StringBuilder();
			
			indent = initialIndent;
			GenerateInitializeComponentMethodBodyInternal(control);
			
			return codeBuilder.ToString();
		}
		
		/// <summary>
		/// Gets a list of properties that should be serialized for the specified form.
		/// </summary>
		public static PropertyDescriptorCollection GetSerializableProperties(object obj)
		{
			return GetSerializableProperties(obj, notHiddenDesignerVisibility);
		}
		
		/// <summary>
		/// Gets a list of properties that should have their content serialized for the specified form.
		/// </summary>
		public static PropertyDescriptorCollection GetSerializableContentProperties(object obj)
		{
			return GetSerializableProperties(obj, contentDesignerVisibility);
		}
	
		/// <summary>
		/// Gets the serializable properties with the specified designer serialization visibility.
		/// </summary>
		public static PropertyDescriptorCollection GetSerializableProperties(object obj, DesignerSerializationVisibility[] visibility)
		{
			List<DesignerSerializationVisibility> requiredVisibility = new List<DesignerSerializationVisibility>(visibility);
			List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(obj, notDesignOnlyFilter).Sort()) {
				if (requiredVisibility.Contains(property.SerializationVisibility)) {
					if (property.ShouldSerializeValue(obj)) {
						properties.Add(property);
					}
				} 
			}
			return new PropertyDescriptorCollection(properties.ToArray());
		}

		/// <summary>
		/// Determines whether the object is an IComponent and has a non-null ISite.
		/// </summary>
		public static bool IsSitedComponent(object obj)
		{
			IComponent component = obj as IComponent;
			if (component != null) {
				return component.Site != null;
			}
			return false;
		}
		
		/// <summary>
		/// Gets the AddRange method on the object that is not hidden from the designer. 
		/// </summary>
		public static MethodInfo GetAddRangeSerializationMethod(object obj)
		{
			foreach (MethodInfo methodInfo in obj.GetType().GetMethods()) {
				if (methodInfo.Name == "AddRange") {
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length == 1) {
						if (parameters[0].ParameterType.IsArray) {
							if (!IsHiddenFromDesignerSerializer(methodInfo)) {
								return methodInfo;
							}
						}
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the Add serialization method that is not hidden from the designer.
		/// </summary>
		public static MethodInfo GetAddSerializationMethod(object obj)
		{
			foreach (MethodInfo methodInfo in obj.GetType().GetMethods()) {
				if (methodInfo.Name == "Add") {
					return methodInfo;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Gets the type used in the array for the first parameter to the method.
		/// </summary>
		public static Type GetArrayParameterType(MethodInfo methodInfo)
		{
			if (methodInfo != null) {
				ParameterInfo[] parameters = methodInfo.GetParameters();
				if (parameters.Length > 0) {
					Type arrayType = parameters[0].ParameterType;
					return arrayType.GetElementType();
				}
			}
			return null;
		}
		
		/// <summary>
		/// Checks whether the method is marked with the DesignerSerializationVisibility.Hidden attribute.
		/// </summary>
		public static bool IsHiddenFromDesignerSerializer(MethodInfo methodInfo)
		{
			foreach (DesignerSerializationVisibilityAttribute attribute in methodInfo.GetCustomAttributes(typeof(DesignerSerializationVisibilityAttribute), true)) {
				if (attribute.Visibility == DesignerSerializationVisibility.Hidden) {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Gets the child components that are sited on the specified object.
		/// </summary>
		/// <remarks>
		/// For a MenuStrip the child components include the MenuStrip.Items.
		/// For a Control the child components include the Control.Controls.
		/// </remarks>
		public static IComponent[] GetChildComponents(object obj)
		{
			List<IComponent> childComponents = new List<IComponent>();			
			foreach (PropertyDescriptor property in GetSerializableContentProperties(obj)) {
				ICollection collection = property.GetValue(obj) as ICollection;
				if (collection != null) {
					foreach (object component in collection) {
						if (IsSitedComponent(component)) {
							childComponents.Add(component as IComponent);
						}
					}
				}
			}			
			return childComponents.ToArray();
		}
				
		void GenerateInitializeComponentMethodBodyInternal(Control control)
		{
			AppendChildControlCreation(control);
			AppendChildControlSuspendLayout(control.Controls);
			AppendIndentedLine("self.SuspendLayout()");
			AppendRootControl(control);			
			AppendChildControlResumeLayout(control.Controls);
			AppendIndentedLine("self.ResumeLayout(False)");
			AppendIndentedLine("self.PerformLayout()");
		}
		
		/// <summary>
		/// Generates python code for the control's InitializeComponent method.
		/// </summary>
		void AppendRootControl(Control rootControl)
		{
			// Add the controls on the form.
			foreach (Control control in rootControl.Controls) {
				AppendComponent(control);
			}
			
			// Add root control.
			AppendComponent(rootControl, false, false);
		}

		void AppendComponent(IComponent component)
		{
			AppendComponent(component, true, true);
		}
		
		/// <summary>
		/// Generates python code for the component.
		/// </summary>
		void AppendComponent(IComponent component, bool addComponentNameToProperty, bool addChildComponentProperties)
		{
			AppendComment(component.Site.Name);

			string propertyOwnerName = GetPropertyOwnerName(component, addComponentNameToProperty);
			AppendProperties(propertyOwnerName, component);
			AppendEventHandlers(propertyOwnerName, component);

			if (addChildComponentProperties) {			
				AppendChildComponentProperties(component);
			}
		}
		
		/// <summary>
		/// Appends a property to the InitializeComponents method.
		/// </summary>
		void AppendProperty(string propertyOwnerName, object obj, PropertyDescriptor propertyDescriptor)
		{			
			object propertyValue = propertyDescriptor.GetValue(obj);
			if (propertyValue == null) {
				return;
			}
			
			if (propertyDescriptor.SerializationVisibility == DesignerSerializationVisibility.Visible) {
				string propertyName = GetPropertyName(propertyOwnerName, propertyDescriptor.Name);
				Control control = propertyValue as Control;
				if (control != null) {
					AppendIndentedLine(propertyName + " = self._" + control.Name);
				} else {
					AppendIndentedLine(propertyName + " = " + PythonPropertyValueAssignment.ToString(propertyValue));
				}
			} else { 
				// DesignerSerializationVisibility.Content
				AppendMethodCallWithArrayParameter(propertyOwnerName, obj, propertyDescriptor);
			}
		}
		
		static string GetPropertyName(string propertyOwnerName, string propertyName)
		{
			if (String.IsNullOrEmpty(propertyOwnerName)) {
				return "self." + propertyName;
			}
			return "self._" + propertyOwnerName + "." + propertyName;
		}
		
		/// <summary>
		/// Appends the comment lines before the control has its properties set.
		/// </summary>
		void AppendComment(string controlName)
		{
			AppendIndentedLine("# ");
			AppendIndentedLine("# " + controlName);
			AppendIndentedLine("# ");
		}
		
		/// <summary>
		/// Increases the indent of any append lines.
		/// </summary>
		void IncreaseIndent()
		{
			++indent;
		}
		
		void DecreaseIndent()
		{
			--indent;
		}

		void Append(string text)
		{
			codeBuilder.Append(text);
		}

		void AppendLine()
		{
			Append("\r\n");
		}
		
		void AppendIndentedLine(string text)
		{
			AppendIndented(text + "\r\n");
		}
		
		void AppendIndented(string text)
		{
			for (int i = 0; i < indent; ++i) {
				codeBuilder.Append(indentString);
			}			
			codeBuilder.Append(text);
		}
		
		void AppendChildControlCreation(Control parentControl)
		{
			AppendChildComponentCreation(GetChildComponents(parentControl));
		}

		void AppendChildComponentCreation(ICollection components)
		{
			foreach (object obj in components) {
				IComponent component = obj as IComponent;
				if (IsSitedComponent(component)) {	
					AppendComponentCreation(component);
					AppendChildComponentCreation(GetChildComponents(component));
				}
			}
		}
				
		void AppendComponentCreation(IComponent component)
		{	
			AppendComponentCreation(component.Site.Name, component);
		}

		void AppendComponentCreation(string name, object obj)
		{	
			AppendIndentedLine("self._" + name + " = " + obj.GetType().FullName + "()");
		}
		
		void AppendChildControlSuspendLayout(Control.ControlCollection controls)
		{
			AppendChildControlLayoutMethodCalls(controls, new string[] {"SuspendLayout()"});
		}
		
		void AppendChildControlResumeLayout(Control.ControlCollection controls)
		{
			AppendChildControlLayoutMethodCalls(controls, new string[] {"ResumeLayout(False)", "PerformLayout()"});
		}
	
		void AppendChildControlLayoutMethodCalls(Control.ControlCollection controls, string[] methods)
		{
			foreach (Control control in controls) {
				if (HasSitedChildComponents(control)) {
					foreach (string method in methods) {
						AppendIndentedLine("self._" + control.Name + "." + method);
					}
					AppendChildControlLayoutMethodCalls(control.Controls, methods);
				}
			}
		}		
		
		/// <summary>
		/// Generates code that wires an event to an event handler.
		/// </summary>
		/// <remarks>
		/// Note that the EventDescriptorCollection.Sort method does not work if the
		/// enumerator is called first. Sorting will only occur if an item is retrieved after calling
		/// Sort or CopyTo is called. The PropertyDescriptorCollection class does not behave
		/// in the same way.</remarks>
		void AppendEventHandlers(string propertyOwnerName, object component)
		{
			EventDescriptorCollection events = TypeDescriptor.GetEvents(component, notDesignOnlyFilter).Sort();
			if (events.Count > 0) {
				EventDescriptor dummyEventDescriptor = events[0];
			}
			foreach (EventDescriptor eventDescriptor in events) {
				AppendEventHandler(propertyOwnerName, component, eventDescriptor);
			}
		}
		
		void AppendEventHandler(string propertyOwnerName, object component, EventDescriptor eventDescriptor)
		{
			PropertyDescriptor propertyDescriptor = eventBindingService.GetEventProperty(eventDescriptor);
			if (propertyDescriptor.ShouldSerializeValue(component)) {
				string methodName = (string)propertyDescriptor.GetValue(component);
				AppendIndentedLine(GetPropertyName(propertyOwnerName, eventDescriptor.Name) + " += self." + methodName);
			}
		}
		
		bool HasSitedChildComponents(Control control)
		{
			return HasSitedComponents(GetChildComponents(control));
		}
		
		bool HasSitedComponents(ICollection items)
		{	
			foreach (object item in items) {
				if (IsSitedComponent(item)) {
					return true;
				}
			}
			return false;
		}
		
		void AppendSystemArray(string componentName, string methodName, string typeName, ICollection components)
		{
			if (components.Count > 0) {
				AppendIndentedLine("self._" + componentName + "." + methodName + "(System.Array[" + typeName + "](");
				IncreaseIndent();
				int i = 0;
				foreach (object component in components) {
					if (i == 0) {
						AppendIndented("[");
					} else {
						Append(",");
						AppendLine();
						AppendIndented(String.Empty);
					}
					if (component is IComponent) {
						Append("self._" + ((IComponent)component).Site.Name);
					} else {
						Append(PythonPropertyValueAssignment.ToString(component));
					}
					++i;
				}
				Append("]))");
				AppendLine();
				DecreaseIndent();
			}
		}		
		
		void AppendProperties(string propertyOwnerName, object obj)
		{
			foreach (PropertyDescriptor property in GetSerializableProperties(obj)) {
				AppendProperty(propertyOwnerName, obj, property);
			}			
		}

		string GetPropertyOwnerName(IComponent component, bool addComponentNameToProperty)
		{
			if (addComponentNameToProperty) {
				return component.Site.Name;
			}
			return String.Empty;
		}
		
		/// <summary>
		/// Appends the properties of any component that is contained in a collection property that is
		/// marked as DesignerSerializationVisibility.Content.
		/// </summary>
		void AppendChildComponentProperties(object component)
		{
			foreach (PropertyDescriptor property in PythonControl.GetSerializableContentProperties(component)) {
				object propertyCollection = property.GetValue(component);
				ICollection collection = propertyCollection as ICollection;
				if (collection != null) {
					foreach (object childComponent in collection) {
						if (IsSitedComponent(childComponent)) {
							AppendComponent(childComponent as IComponent , true, true);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Returns the sited components in the collection. If an object in the collection is not
		/// an IComponent then this is added to the collection.
		/// </summary>
		ICollection GetSitedComponentsAndNonComponents(ICollection components)
		{
			List<object> sitedComponents = new List<object>();
			foreach (object obj in components) {
				IComponent component = obj as IComponent;
				if (component == null || IsSitedComponent(component)) {
					sitedComponents.Add(obj);
				}
			}
			return sitedComponents.ToArray();
		}
	
		/// <summary>
		/// Appends an array as a parameter and its associated method call. 
		/// </summary>
		/// <remarks>
		/// Looks for the AddRange method first. If that does not exist or is hidden from the designer the
		/// Add method is looked for.
		/// </remarks>
		void AppendMethodCallWithArrayParameter(string propertyOwnerName, object propertyOwner, PropertyDescriptor propertyDescriptor)
		{			
			IComponent component = propertyOwner as IComponent;
			ICollection collectionProperty = propertyDescriptor.GetValue(propertyOwner) as ICollection;
			if (collectionProperty != null) {
				MethodInfo addRangeMethod = GetAddRangeSerializationMethod(collectionProperty);
				if (addRangeMethod != null) {
					Type arrayElementType = GetArrayParameterType(addRangeMethod);
					AppendSystemArray(component.Site.Name, propertyDescriptor.Name + "." + addRangeMethod.Name, arrayElementType.FullName, GetSitedComponentsAndNonComponents(collectionProperty));
				} else {
					MethodInfo addMethod = GetAddSerializationMethod(collectionProperty);
					ParameterInfo[] parameters = addMethod.GetParameters();
					foreach (object item in collectionProperty) {
						IComponent collectionComponent = item as IComponent;
						if (IsSitedComponent(collectionComponent)) {
							AppendIndentedLine(GetPropertyName(propertyOwnerName, propertyDescriptor.Name) + "." + addMethod.Name + "(self._" + collectionComponent.Site.Name + ")");
						}
					}
				}
			}
		}
	}
}
