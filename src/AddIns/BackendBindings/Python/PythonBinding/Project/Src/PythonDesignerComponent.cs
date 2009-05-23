// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents an IComponent in the designer.
	/// </summary>
	public class PythonDesignerComponent
	{		
		IComponent component;
		static readonly Attribute[] notDesignOnlyFilter = new Attribute[] { DesignOnlyAttribute.No };
		static readonly DesignerSerializationVisibility[] notHiddenDesignerVisibility = new DesignerSerializationVisibility[] { DesignerSerializationVisibility.Content, DesignerSerializationVisibility.Visible };
		static readonly DesignerSerializationVisibility[] contentDesignerVisibility = new DesignerSerializationVisibility[] { DesignerSerializationVisibility.Content };
		IEventBindingService eventBindingService;
		PythonDesignerComponent parent;
		
		protected static readonly string[] suspendLayoutMethods = new string[] {"SuspendLayout()"};
		protected static readonly string[] resumeLayoutMethods = new string[] {"ResumeLayout(False)", "PerformLayout()"};
		
		/// <summary>
		/// Used so the EventBindingService.GetEventProperty method can be called to get the property descriptor
		/// for an event.
		/// </summary>
		class PythonEventBindingService : EventBindingService
		{
			public PythonEventBindingService()
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

		public PythonDesignerComponent(IComponent component)
			: this(null, component)
		{
		}
		
		public PythonDesignerComponent(PythonDesignerComponent parent, IComponent component)
			: this(parent, component, new PythonEventBindingService())
		{
		}
		
		PythonDesignerComponent(PythonDesignerComponent parent, IComponent component, IEventBindingService eventBindingService)
		{
			this.parent = parent;
			this.component = component;
			this.eventBindingService = eventBindingService;
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
		/// Returns true if the component has the DesignTimeVisible attribute set to false.
		/// </summary>
		public static bool IsHiddenFromDesigner(IComponent component)
		{
			foreach (DesignTimeVisibleAttribute attribute in component.GetType().GetCustomAttributes(typeof(DesignTimeVisibleAttribute), true)) {
				return !attribute.Visible;
			}
			return false;
		}
		
		/// <summary>
		/// A component is non-visual if it is not a control and is not hidden from the designer.
		/// </summary>
		public static bool IsNonVisualComponent(IComponent component)
		{
			Control control = component as Control;
			return (control == null) && !IsHiddenFromDesigner(component);
		}
		
		/// <summary>
		/// Returns true if this component is non-visual.
		/// </summary>
		public bool IsNonVisual {
			get { return IsNonVisualComponent(component); }
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
		/// Gets the component type.
		/// </summary>
		public Type GetComponentType()
		{
			return component.GetType();
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
		/// Appends code that creates an instance of the component.
		/// </summary>
		public virtual void AppendCreateInstance(PythonCodeBuilder codeBuilder)
		{			
			AppendComponentCreation(codeBuilder, component);
		}
		
		public void AppendCreateInstance(PythonCodeBuilder codeBuilder, string parameters)
		{
			AppendComponentCreation(codeBuilder, component, parameters);
		}
		
		/// <summary>
		/// Appends the code to create the child components.
		/// </summary>
		public void AppendCreateChildComponents(PythonCodeBuilder codeBuilder)
		{
			AppendCreateChildComponents(codeBuilder, GetChildComponents());
		}
		
		/// <summary>
		/// Appends the component's properties.
		/// </summary>
		public virtual void AppendComponent(PythonCodeBuilder codeBuilder)
		{
			AppendComponentProperties(codeBuilder);
			AppendChildComponentProperties(codeBuilder);
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
		/// Determines whether this designer component is sited.
		/// </summary>
		public bool IsSited {
			get { return IsSitedComponent(component); }
		}
				
		/// <summary>
		/// Gets the child objects that need to be stored in the generated designer code on the specified object.
		/// </summary>
		/// <remarks>
		/// For a MenuStrip the child components include the MenuStrip.Items.
		/// For a Control the child components include the Control.Controls.
		/// </remarks>
		public virtual PythonDesignerComponent[] GetChildComponents()
		{
			List<PythonDesignerComponent> components = new List<PythonDesignerComponent>();
			foreach (PropertyDescriptor property in GetSerializableContentProperties(component)) {
				ICollection collection = property.GetValue(component) as ICollection;
				if (collection != null) {
					foreach (object childObject in collection) {
						IComponent childComponent = childObject as IComponent;
						if (childComponent != null) {
							PythonDesignerComponent designerComponent = PythonDesignerComponentFactory.CreateDesignerComponent(this, childComponent);
							if (designerComponent.IsSited) {
								components.Add(designerComponent);
							}
						}
					}
				}
			}
			return components.ToArray();
		}
		
		/// <summary>
		/// Appends SuspendLayout method call if the component has any sited child components.
		/// </summary>
		public virtual void AppendSuspendLayout(PythonCodeBuilder codeBuilder)
		{
			if (HasSitedChildComponents()) {
				AppendMethodCalls(codeBuilder, suspendLayoutMethods);
			}
		}
		
		/// <summary>
		/// Appends the ResumeLayout and PerformLayout method calls if the component has any sited
		/// child components.
		/// </summary>
		public virtual void AppendResumeLayout(PythonCodeBuilder codeBuilder)
		{
			if (HasSitedChildComponents()) {
				AppendMethodCalls(codeBuilder, resumeLayoutMethods);
			}
		}
		
		public void AppendChildComponentsSuspendLayout(PythonCodeBuilder codeBuilder)
		{
			AppendChildComponentsMethodCalls(codeBuilder, suspendLayoutMethods);
		}
		
		public void AppendChildComponentsResumeLayout(PythonCodeBuilder codeBuilder)
		{
			AppendChildComponentsMethodCalls(codeBuilder, resumeLayoutMethods);
		}
							
		/// <summary>
		/// Appends the code to create the specified object.
		/// </summary>
		public void AppendCreateInstance(PythonCodeBuilder codeBuilder, object obj, int count, object[] parameters)
		{
			if (obj is String) {
				// Do nothing.
			} else {
				codeBuilder.AppendIndented(GetVariableName(obj, count) + " = " + obj.GetType().FullName);
				
				codeBuilder.Append("(");
				for (int i = 0; i < parameters.Length; ++i) {
					if (i > 0) {
						codeBuilder.Append(", ");
					}
					object currentParameter = parameters[i];
					Array array = currentParameter as Array;
					if (array != null) {
						AppendSystemArray(codeBuilder, array.GetValue(0).GetType().FullName, currentParameter as ICollection);
						codeBuilder.DecreaseIndent();
					} else {
						codeBuilder.Append(PythonPropertyValueAssignment.ToString(currentParameter));
					}
				}
				codeBuilder.Append(")");
				codeBuilder.AppendLine();
			}
		}
		
		/// <summary>
		/// Appends the code to create the specified IComponent
		/// </summary>
		public void AppendComponentCreation(PythonCodeBuilder codeBuilder, IComponent component)
		{
			AppendComponentCreation(codeBuilder, component, String.Empty);
		}

		/// <summary>
		/// Appends the code to create the specified IComponent
		/// </summary>
		public void AppendComponentCreation(PythonCodeBuilder codeBuilder, IComponent component, string parameters)
		{
			codeBuilder.AppendIndentedLine("self._" + component.Site.Name + " = " + component.GetType().FullName + "(" + parameters + ")");
		}
		
		/// <summary>
		/// Generates the code for the component's properties.
		/// </summary>
		public void AppendComponentProperties(PythonCodeBuilder codeBuilder)
		{
			AppendComponentProperties(codeBuilder, true, false, true);
		}
		
		/// <summary>
		/// Appends the properties of any component that is contained in a collection property that is
		/// marked as DesignerSerializationVisibility.Content.
		/// </summary>
		public void AppendChildComponentProperties(PythonCodeBuilder codeBuilder)
		{
			foreach (PropertyDescriptor property in PythonDesignerComponent.GetSerializableContentProperties(component)) {
				object propertyCollection = property.GetValue(component);
				ICollection collection = propertyCollection as ICollection;
				if (collection != null) {
					foreach (object childObject in collection) {
						IComponent childComponent = childObject as IComponent;
						if (childComponent != null) {
							PythonDesignerComponent designerComponent = PythonDesignerComponentFactory.CreateDesignerComponent(this, childComponent);
							if (designerComponent.IsSited) {
								designerComponent.AppendComponentProperties(codeBuilder, true, true, true);
							}
						}
					}
				}
			}
		}		
		/// <summary>
		/// Generates python code for an object's properties when the object is not an IComponent.
		/// </summary>
		public void AppendObjectProperties(PythonCodeBuilder codeBuilder, object obj, int count)
		{
			AppendProperties(codeBuilder, PythonDesignerComponent.GetVariableName(obj, count), obj);
		}

		/// <summary>
		/// Appends the comment lines containing the component name before the component has its properties set.
		/// </summary>
		/// 
		public void AppendComment(PythonCodeBuilder codeBuilder)
		{
			codeBuilder.AppendIndentedLine("# ");
			codeBuilder.AppendIndentedLine("# " + component.Site.Name);
			codeBuilder.AppendIndentedLine("# ");
		}

		public bool HasSitedChildComponents()
		{
			return HasSitedComponents(GetChildComponents());
		}
		
		/// <summary>
		/// Appends the method calls for this component.
		/// </summary>
		public void AppendMethodCalls(PythonCodeBuilder codeBuilder, string[] methods)
		{
			foreach (string method in methods) {
				codeBuilder.AppendIndentedLine(GetPropertyOwnerName() + "." + method);
			}
		}

		/// <summary>
		/// Gets the variable name for the specified type.
		/// </summary>
		/// <remarks>
		/// The variable name is simply the type name with the first character in lower case followed by the
		/// count.
		/// </remarks>
		public static string GetVariableName(object obj, int count)
		{
			string typeName = obj.GetType().Name;
			return typeName[0].ToString().ToLowerInvariant() + typeName.Substring(1) + count;
		}
		
		/// <summary>
		/// Appends an array as a parameter and its associated method call. 
		/// </summary>
		/// <remarks>
		/// Looks for the AddRange method first. If that does not exist or is hidden from the designer the
		/// Add method is looked for.
		/// </remarks>
		public static void AppendMethodCallWithArrayParameter(PythonCodeBuilder codeBuilder, string propertyOwnerName, object propertyOwner, PropertyDescriptor propertyDescriptor)
		{			
			IComponent component = propertyOwner as IComponent;
			ICollection collectionProperty = propertyDescriptor.GetValue(propertyOwner) as ICollection;
			if (collectionProperty != null) {
				MethodInfo addRangeMethod = GetAddRangeSerializationMethod(collectionProperty);
				if (addRangeMethod != null) {
					Type arrayElementType = GetArrayParameterType(addRangeMethod);
					AppendSystemArray(codeBuilder, component.Site.Name, propertyDescriptor.Name + "." + addRangeMethod.Name, arrayElementType.FullName, GetSitedComponentsAndNonComponents(collectionProperty));
				} else {
					MethodInfo addMethod = GetAddSerializationMethod(collectionProperty);
					ParameterInfo[] parameters = addMethod.GetParameters();
					foreach (object item in collectionProperty) {
						IComponent collectionComponent = item as IComponent;
						if (PythonDesignerComponent.IsSitedComponent(collectionComponent)) {
							codeBuilder.AppendIndentedLine(propertyOwnerName + "." + propertyDescriptor.Name + "." + addMethod.Name + "(self._" + collectionComponent.Site.Name + ")");
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Appends a property.
		/// </summary>
		public void AppendProperty(PythonCodeBuilder codeBuilder, string propertyOwnerName, object obj, PropertyDescriptor propertyDescriptor)
		{			
			object propertyValue = propertyDescriptor.GetValue(obj);
			if (propertyValue == null) {
				return;
			}
			
			ExtenderProvidedPropertyAttribute extender = GetExtenderAttribute(propertyDescriptor);
			if (extender != null) {
				AppendExtenderProperty(codeBuilder, propertyOwnerName, extender, propertyDescriptor, propertyValue);
			} else if (propertyDescriptor.SerializationVisibility == DesignerSerializationVisibility.Visible) {
				string propertyName = propertyOwnerName + "." + propertyDescriptor.Name;
				Control control = propertyValue as Control;
				if (control != null) {
					codeBuilder.AppendIndentedLine(propertyName + " = " + GetControlReference(control));
				} else {
					codeBuilder.AppendIndentedLine(propertyName + " = " + PythonPropertyValueAssignment.ToString(propertyValue));
				}
			} else { 
				// DesignerSerializationVisibility.Content
				AppendMethodCallWithArrayParameter(codeBuilder, propertyOwnerName, obj, propertyDescriptor);
			}
		}

		/// <summary>
		/// Appends an extender provider property.
		/// </summary>
		public void AppendExtenderProperty(PythonCodeBuilder codeBuilder, string propertyOwnerName, ExtenderProvidedPropertyAttribute extender, PropertyDescriptor propertyDescriptor, object propertyValue)
		{
			IComponent component = extender.Provider as IComponent;
			codeBuilder.AppendIndented("self._" + component.Site.Name);
			codeBuilder.Append(".Set" + propertyDescriptor.Name);
			codeBuilder.Append("(");
			codeBuilder.Append(propertyOwnerName);
			codeBuilder.Append(", ");
			codeBuilder.Append(PythonPropertyValueAssignment.ToString(propertyValue));
			codeBuilder.Append(")");
			codeBuilder.AppendLine();
		}
		
		/// <summary>
		/// Appends the properties of the object to the code builder.
		/// </summary>
		public void AppendProperties(PythonCodeBuilder codeBuilder, string propertyOwnerName, object obj)
		{
			foreach (PropertyDescriptor property in GetSerializableProperties(obj)) {
				AppendProperty(codeBuilder, propertyOwnerName, obj, property);
			}
		}
		
		/// <summary>
		/// Appends the properties of the component.
		/// </summary>
		public void AppendProperties(PythonCodeBuilder codeBuilder)
		{
			AppendProperties(codeBuilder, GetPropertyOwnerName(), component);
		}
		
		/// <summary>
		/// Generates python code for the component.
		/// </summary>
		public void AppendComponentProperties(PythonCodeBuilder codeBuilder, bool addComponentNameToProperty, bool addChildComponentProperties, bool addComment)
		{			
			PythonCodeBuilder propertiesBuilder = new PythonCodeBuilder(codeBuilder.Indent);
			propertiesBuilder.IndentString = codeBuilder.IndentString;
			
			AppendProperties(propertiesBuilder);
			AppendEventHandlers(propertiesBuilder, eventBindingService);

			// Add comment if we have added some properties or event handlers.
			if (addComment && propertiesBuilder.Length > 0) {
				AppendComment(codeBuilder);
			}
			codeBuilder.Append(propertiesBuilder.ToString());
			
			if (addChildComponentProperties) {	
				AppendChildComponentProperties(codeBuilder);
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
		public void AppendEventHandlers(PythonCodeBuilder codeBuilder, IEventBindingService eventBindingService)
		{
			EventDescriptorCollection events = TypeDescriptor.GetEvents(component, notDesignOnlyFilter).Sort();
			if (events.Count > 0) {
				EventDescriptor dummyEventDescriptor = events[0];
			}
			foreach (EventDescriptor eventDescriptor in events) {
				AppendEventHandler(codeBuilder, component, eventDescriptor, eventBindingService);
			}
		}
		
		void AppendEventHandler(PythonCodeBuilder codeBuilder, object component, EventDescriptor eventDescriptor, IEventBindingService eventBindingService)
		{
			PropertyDescriptor propertyDescriptor = eventBindingService.GetEventProperty(eventDescriptor);
			if (propertyDescriptor.ShouldSerializeValue(component)) {
				string methodName = (string)propertyDescriptor.GetValue(component);
				codeBuilder.AppendIndentedLine(GetPropertyOwnerName() + "." + eventDescriptor.Name + " += self." + methodName);
			}
		}

		/// <summary>
		/// Gets the owner of any properties generated (e.g. "self._textBox1").
		public virtual string GetPropertyOwnerName()
		{
			return "self._" + component.Site.Name;			
		}
		
		/// <summary>
		/// Determines whether the component has a constructor that takes a single IContainer parameter.
		/// </summary>
		public bool HasIContainerConstructor() 
		{
			foreach (ConstructorInfo constructor in GetComponentType().GetConstructors()) {
				ParameterInfo[] parameters = constructor.GetParameters();
				if (parameters.Length == 1) {
					ParameterInfo parameter = parameters[0];
					if (parameter.ParameterType.IsAssignableFrom(typeof(IContainer))) {
						return true;
					}
				}
			}
			return false;
		}
		
		protected IComponent Component {
			get { return component; }
		}
		
		static bool HasSitedComponents(PythonDesignerComponent[] components)
		{	
			foreach (PythonDesignerComponent component in components) {
				if (component.IsSited) {
					return true;
				}
			}
			return false;
		}
		
		void AppendCreateChildComponents(PythonCodeBuilder codeBuilder, PythonDesignerComponent[] childComponents)
		{
			foreach (PythonDesignerComponent designerComponent in childComponents) {
				if (designerComponent.IsSited) {
					designerComponent.AppendCreateInstance(codeBuilder);
					designerComponent.AppendCreateChildComponents(codeBuilder);
				}
			}
		}
		
		/// <summary>
		/// Returns the sited components in the collection. If an object in the collection is not
		/// an IComponent then this is added to the collection.
		/// </summary>
		static ICollection GetSitedComponentsAndNonComponents(ICollection components)
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
		
		static void AppendSystemArray(PythonCodeBuilder codeBuilder, string componentName, string methodName, string typeName, ICollection components)
		{
			if (components.Count > 0) {
				codeBuilder.AppendIndented("self._" + componentName + "." + methodName + "(");
				AppendSystemArray(codeBuilder, typeName, components);
				codeBuilder.Append(")");
				codeBuilder.AppendLine();
				codeBuilder.DecreaseIndent();
			}
		}
		
		static void AppendSystemArray(PythonCodeBuilder codeBuilder, string typeName, ICollection components)
		{
			if (components.Count > 0) {
				codeBuilder.Append("System.Array[" + typeName + "](");
				codeBuilder.AppendLine();
				codeBuilder.IncreaseIndent();
				int i = 0;
				foreach (object component in components) {
					if (i == 0) {
						codeBuilder.AppendIndented("[");
					} else {
						codeBuilder.Append(",");
						codeBuilder.AppendLine();
						codeBuilder.AppendIndented(String.Empty);
					}
					if (component is IComponent) {
						codeBuilder.Append("self._" + ((IComponent)component).Site.Name);
					} else if (component is String) {
						codeBuilder.Append(PythonPropertyValueAssignment.ToString(component));
					} else {
						codeBuilder.Append(GetVariableName(component, i + 1));
					}
					++i;
				}
				codeBuilder.Append("])");
			}
		}
	
		void AppendChildComponentsMethodCalls(PythonCodeBuilder codeBuilder, string[] methods)
		{
			foreach (PythonDesignerComponent designerComponent in GetChildComponents()) {
				if (typeof(Control).IsAssignableFrom(designerComponent.GetComponentType())) {
					if (designerComponent.HasSitedChildComponents()) {
						designerComponent.AppendMethodCalls(codeBuilder, methods);
					}
				}
				designerComponent.AppendChildComponentsMethodCalls(codeBuilder, methods);
			}
		}
		
		bool IsRootComponent(IComponent component)
		{
			if (parent == null) {
				return this.component == component;
			}
			return parent.IsRootComponent(component);
		}
		
		string GetControlReference(Control control)
		{
			if (IsRootComponent(control)) {
				return "self";
			}
			return "self._" + control.Name;
		}
		
		static ExtenderProvidedPropertyAttribute GetExtenderAttribute(PropertyDescriptor property)
		{
			foreach (Attribute attribute in property.Attributes) {
				ExtenderProvidedPropertyAttribute extenderAttribute = attribute as ExtenderProvidedPropertyAttribute;
				if (extenderAttribute != null) {
					return extenderAttribute;
				}
			}
			return null;
		}
	}
}
