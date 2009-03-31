// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Visits the code's Python AST and creates a Windows Form.
	/// </summary>
	public class PythonFormWalker : PythonWalker
	{
		Form form;
		PythonControlFieldExpression fieldExpression;
		IComponentCreator componentCreator;
		bool walkingAssignment;
		Dictionary<string, object> createdObjects = new Dictionary<string, object>();
		string formName = String.Empty;
		PythonCodeDeserializer deserializer;
		
		public PythonFormWalker(IComponentCreator componentCreator)
		{
			this.componentCreator = componentCreator;
			deserializer = new PythonCodeDeserializer(componentCreator);
		}		
		
		/// <summary>
		/// Creates a form from python code.
		/// </summary>
		public Form CreateForm(string pythonCode)
		{
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"Form.py", pythonCode);
			ast.Walk(this);
		
			// Did we find the InitializeComponent method?
			if (form == null) {
				throw new PythonFormWalkerException("Unable to find InitializeComponents method.");
			}
			
			return form;
		}		

		public override bool Walk(ClassDefinition node)
		{
			formName = node.Name.ToString();
			if (node.Body != null) {
				node.Body.Walk(this);
			}
			return false;
		}
				
		public override bool Walk(FunctionDefinition node)
		{
			if (IsInitializeComponentMethod(node)) {			
				form = (Form)componentCreator.CreateComponent(typeof(Form), formName);
				node.Body.Walk(this);
			}
			return false;
		}
		
		public override bool Walk(AssignmentStatement node)
		{			
			if (node.Left.Count > 0) {
				MemberExpression lhsMemberExpression = node.Left[0] as MemberExpression;
				if (lhsMemberExpression != null) {
					fieldExpression = PythonControlFieldExpression.Create(lhsMemberExpression);
					
					MemberExpression rhsMemberExpression = node.Right as MemberExpression;
					if (rhsMemberExpression != null) {
						object propertyValue = GetPropertyValueFromAssignmentRhs(rhsMemberExpression);
						SetPropertyValue(fieldExpression.MemberName, propertyValue);
					} else {
						walkingAssignment = true;
						BinaryExpression binaryExpression = node.Right as BinaryExpression;
						if (binaryExpression != null) {
							WalkAssignment(binaryExpression);
						} else {
							node.Right.Walk(this);
						}
						walkingAssignment = false;
					}
				}	
			}
			return false;
		}
		
		public override bool Walk(ConstantExpression node)
		{
			SetPropertyValue(fieldExpression.MemberName, node.Value);
			return false;
		}
		
		public override bool Walk(CallExpression node)
		{			
			MemberExpression memberExpression = node.Target as MemberExpression;
			if (memberExpression != null) {
				string name = PythonControlFieldExpression.GetMemberName(memberExpression);
				if (walkingAssignment) {
					Type type = componentCreator.GetType(name);
					if (type != null) {
						List<object> args = deserializer.GetArguments(node);
						object instance = componentCreator.CreateInstance(type, args, null, false);
						if (!SetPropertyValue(fieldExpression.MemberName, instance)) {
							AddComponent(fieldExpression.MemberName, instance);
						}
					} else {
						object obj = deserializer.Deserialize(node);
						if (obj != null) {
							SetPropertyValue(form, fieldExpression.MemberName, obj);
						} else {
							throw new PythonFormWalkerException(String.Format("Could not find type '{0}'.", name));
						}
					}
				} else {
					// Add child controls if expression is of the form self.controlName.Controls.Add.
					string parentControlName = PythonControlFieldExpression.GetParentControlNameAddingChildControls(name);
					if (parentControlName != null) {
						AddChildControl(parentControlName, node);
					}
				}
			}
			return false;
		}
		
		public override bool Walk(NameExpression node)
		{
			SetPropertyValue(fieldExpression.MemberName, node.Name.ToString());
			return false;
		}
		
		/// <summary>
		/// Walks a statement of the form:
		/// 
		/// self.a += self.b
		/// </summary>
		public override bool Walk(AugmentedAssignStatement node)
		{
			MemberExpression eventExpression = node.Left as MemberExpression;
			string eventName = eventExpression.Name.ToString();
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(eventExpression);
			
			MemberExpression eventHandlerExpression = node.Right as MemberExpression;
			string eventHandlerName = eventHandlerExpression.Name.ToString();
			
			Control control = form;
			if (field.VariableName.Length > 0) {
				control = GetControl(field.VariableName);
			}
			
			EventDescriptor eventDescriptor = TypeDescriptor.GetEvents(control).Find(eventName, false);
			PropertyDescriptor propertyDescriptor = componentCreator.GetEventProperty(eventDescriptor);
			propertyDescriptor.SetValue(control, eventHandlerName);
			return false;
		}
		
		/// <summary>
		/// Walks the binary expression which is the right hand side of an assignment statement.
		/// </summary>
		void WalkAssignment(BinaryExpression binaryExpression)
		{
			object value = deserializer.Deserialize(binaryExpression);
			SetPropertyValue(fieldExpression.MemberName, value);
		}
		
		static bool IsInitializeComponentMethod(FunctionDefinition node)
		{
			string name = node.Name.ToString().ToLowerInvariant();
			return name == "initializecomponent" || name == "initializecomponents";
		}

		/// <summary>
		/// Sets the value of a property on the current control.
		/// </summary>
		bool SetPropertyValue(string name, object propertyValue)
		{
			Control control = GetCurrentControl();
			return SetPropertyValue(control, name, propertyValue);
		}
		
		/// <summary>
		/// Sets the value of a property on the control.
		/// </summary>
		bool SetPropertyValue(Control control, string name, object propertyValue)
		{
			PropertyDescriptor property = TypeDescriptor.GetProperties(control).Find(name, true);
			if (property != null) {
				propertyValue = ConvertPropertyValue(property, propertyValue);
				property.SetValue(control, propertyValue);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Converts the value to the property's type if required.
		/// </summary>
		static object ConvertPropertyValue(PropertyDescriptor propertyDescriptor, object propertyValue)
		{
			if (!propertyDescriptor.PropertyType.IsAssignableFrom(propertyValue.GetType())) {
				return propertyDescriptor.Converter.ConvertFrom(propertyValue);
			}
			return propertyValue;
		}
				
		/// <summary>
		/// Looks for the control with the specified name in the objects that have been
		/// created whilst processing the InitializeComponent method.
		/// </summary>
		Control GetControl(string name)
		{
			object o = null;
			createdObjects.TryGetValue(name, out o);
			return o as Control;
		}

		/// <summary>
		/// Adds a component to the list of created objects.
		/// </summary>
		void AddComponent(string name, object component)
		{
			string variableName = PythonControlFieldExpression.GetVariableName(name);
			componentCreator.Add(component as IComponent, variableName);
			createdObjects.Add(variableName, component);
		}
				
		/// <summary>
		/// Gets the current control being walked.
		/// </summary>
		Control GetCurrentControl()
		{
			if (fieldExpression.VariableName.Length > 0) {
				return GetControl(fieldExpression.VariableName);
			}
			return form;
		}
		
		void AddChildControl(string parentControlName, CallExpression node)
		{
			string childControlName = PythonControlFieldExpression.GetControlNameBeingAdded(node);
			Control parentControl = form;
			if (parentControlName.Length > 0) {
				parentControl = GetControl(parentControlName);
			}
			parentControl.Controls.Add(GetControl(childControlName));
		}
		
		/// <summary>
		/// Gets the property value from the member expression. The member expression is taken from the
		/// right hand side of an assignment.
		/// </summary>
		object GetPropertyValueFromAssignmentRhs(MemberExpression memberExpression)
		{
			object propertyValue = deserializer.Deserialize(memberExpression);
			if (propertyValue == null) {
				PythonControlFieldExpression field = PythonControlFieldExpression.Create(memberExpression);
				if (field.MemberName.Length > 0) {
					propertyValue = GetControl(PythonControlFieldExpression.GetVariableName(field.MemberName));
				} else {
					propertyValue = field.FullMemberName;
				}
			}
			return propertyValue;
		}
	}
}
