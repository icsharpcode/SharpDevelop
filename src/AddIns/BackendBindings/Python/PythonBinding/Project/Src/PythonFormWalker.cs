// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
		
		public PythonFormWalker(IComponentCreator componentCreator)
		{
			this.componentCreator = componentCreator;
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
			if (node.Body != null) {
				node.Body.Walk(this);
			}
			return false;
		}
				
		public override bool Walk(FunctionDefinition node)
		{
			if (IsInitializeComponentMethod(node)) {			
				form = (Form)componentCreator.CreateComponent(typeof(Form), "MainForm");
				node.Body.Walk(this);
			}
			return false;
		}
		
		public override bool Walk(AssignmentStatement node)
		{			
			if (node.Left.Count > 0) {
				MemberExpression memberExpression = node.Left[0] as MemberExpression;
				if (memberExpression != null) {
					fieldExpression = PythonControlFieldExpression.Create(memberExpression);
					
					walkingAssignment = true;
					node.Right.Walk(this);
					walkingAssignment = false;
				}	
			}
			return false;
		}
		
		public override bool Walk(ConstantExpression node)
		{
			Control control = GetCurrentControl();
			SetPropertyValue(control, fieldExpression.MemberName, node.Value);
			return false;
		}
		
		Control GetCurrentControl()
		{
			string variableName = PythonControlFieldExpression.GetVariableNameFromSelfReference(fieldExpression.FullMemberName);
			if (variableName.Length > 0) {
				return GetControl(variableName);
			}
			return form;
		}
		
		public override bool Walk(CallExpression node)
		{			
			MemberExpression memberExpression = node.Target as MemberExpression;
			if (memberExpression != null) {
				string name = PythonControlFieldExpression.GetMemberName(memberExpression);
				if (walkingAssignment) {		
					Type type = GetType(name);					
					List<object> args = GetArguments(node);
					object instance = componentCreator.CreateInstance(type, args, fieldExpression.MemberName, false);
					if (!SetPropertyValue(form, fieldExpression.MemberName, instance)) {
						AddComponent(fieldExpression.MemberName, instance);
					}
				} else if (name == "self.Controls.Add") {
					string controlName = PythonControlFieldExpression.GetControlNameBeingAdded(node);
					form.Controls.Add(GetControl(controlName));
				}
			}
			return false;
		}
		
		/// <summary>
		/// Gets the arguments passed to the call expression.
		/// </summary>
		static List<object> GetArguments(CallExpression expression)
		{
			List<object> args = new List<object>();
			foreach (Arg a in expression.Args) {
				ConstantExpression constantExpression = a.Expression as ConstantExpression;
				if (constantExpression != null) {
					args.Add(constantExpression.Value);
				}
			}
			return args;
		}
		
		static bool IsInitializeComponentMethod(FunctionDefinition node)
		{
			string name = node.Name.ToString().ToLowerInvariant();
			return name == "initializecomponent" || name == "initializecomponents";
		}
		
		/// <summary>
		/// Sets the value of a form's property.
		/// </summary>
		bool SetPropertyValue(Control control, string name, object @value)
		{
			PropertyInfo propertyInfo = control.GetType().GetProperty(name);
			if (propertyInfo != null) {
				propertyInfo.SetValue(control, @value, null);
				return true;
			}
			return false;
		}
				
		Type GetType(string typeName)
		{
			Type type = componentCreator.GetType(typeName);
			if (type == null) {	
				throw new PythonFormWalkerException(String.Format("Could not find type '{0}'.", typeName));
			}
			return type;
		}
		
		/// <summary>
		/// Looks for the control with the specified name in the objects that have been
		/// created whilst processing the InitializeComponent method.
		/// </summary>
		Control GetControl(string name)
		{
			object o = null;
			if (createdObjects.TryGetValue(name, out o)) {
				return o as Control;
			}
			return null;
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
		
		static string GetFirstArgumentAsString(CallExpression node)
		{
			List<object> args = GetArguments(node);
			if (args.Count > 0) {
				return args[0] as String;
			}
			return null;
		}
	}
}
