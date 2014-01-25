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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Scripting;
using ICSharpCode.SharpDevelop;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Visits the code's Python AST and creates a Windows Form.
	/// </summary>
	public class PythonComponentWalker : PythonWalker, IComponentWalker
	{
		IComponent component;
		PythonControlFieldExpression fieldExpression;
		IComponentCreator componentCreator;
		bool walkingAssignment;
		string componentName = String.Empty;
		PythonCodeDeserializer deserializer;
		ClassDefinition classDefinition;
		bool walkingInitializeComponentMethod;
		
		public PythonComponentWalker(IComponentCreator componentCreator)
		{
			this.componentCreator = componentCreator;
			deserializer = new PythonCodeDeserializer(componentCreator);
		}
		
		/// <summary>
		/// Creates a control either a UserControl or Form from the python code.
		/// </summary>
		public IComponent CreateComponent(string pythonCode)
		{
			PythonParser parser = new PythonParser();
			PythonAst ast = parser.CreateAst(@"Control.py", new StringTextBuffer(pythonCode));
			ast.Walk(this);
			
			// Did we find the InitializeComponent method?
			if (component == null) {
				throw new PythonComponentWalkerException("Unable to find InitializeComponents method.");
			}
			return component;
		}
		
		/// <summary>
		/// Gets the fully qualified name of the base class.
		/// </summary>
		public static string GetBaseClassName(ClassDefinition classDefinition)
		{
			if (classDefinition.Bases.Count > 0) {
				Expression baseClassExpression = classDefinition.Bases[0];
				NameExpression nameExpression = baseClassExpression as NameExpression;
				MemberExpression memberExpression = baseClassExpression as MemberExpression;
				if (nameExpression != null) {
					return nameExpression.Name;
				}
				return PythonControlFieldExpression.GetMemberName(memberExpression);
			}
			return String.Empty;
		}

		public override bool Walk(ClassDefinition node)
		{
			classDefinition = node;
			componentName = node.Name;
			node.Body.Walk(this);
			return false;
		}
				
		public override bool Walk(FunctionDefinition node)
		{
			if (IsInitializeComponentMethod(node)) {
				Type type = GetComponentType();
				component = componentCreator.CreateComponent(type, componentName);
				IResourceReader reader = componentCreator.GetResourceReader(CultureInfo.InvariantCulture);
				if (reader != null) {
					reader.Dispose();
				}
				walkingInitializeComponentMethod = true;
				node.Body.Walk(this);
				walkingInitializeComponentMethod = false;
			}
			return false;
		}
		
		public override bool Walk(AssignmentStatement node)
		{
			if (!walkingInitializeComponentMethod) {
				return false;
			}
			
			if (node.Left.Count > 0) {
				MemberExpression lhsMemberExpression = node.Left[0] as MemberExpression;
				NameExpression lhsNameExpression = node.Left[0] as NameExpression;
				if (lhsMemberExpression != null) {
					fieldExpression = PythonControlFieldExpression.Create(lhsMemberExpression);
					WalkMemberExpressionAssignmentRhs(node.Right);
				} else if (lhsNameExpression != null) {
					CallExpression callExpression = node.Right as CallExpression;
					if (callExpression != null) {
						object instance = CreateInstance(lhsNameExpression.Name.ToString(), callExpression);
						if (instance == null) {
							ThrowCouldNotFindTypeException(callExpression.Target as MemberExpression);
						}
					}
				}
			}
			return false;
		}
		
		public override bool Walk(ConstantExpression node)
		{
			if (!walkingInitializeComponentMethod) {
				return false;
			}
			
			fieldExpression.SetPropertyValue(componentCreator, node.Value);
			return false;
		}
		
		public override bool Walk(CallExpression node)
		{
			if (!walkingInitializeComponentMethod) {
				return false;
			}
				
			if (walkingAssignment) {
				WalkAssignmentRhs(node);
			} else {
				WalkMethodCall(node);
			}
			return false;
		}
		
		public override bool Walk(NameExpression node)
		{
			if (!walkingInitializeComponentMethod) {
				return false;
			}
			
			fieldExpression.SetPropertyValue(componentCreator, node);
			return false;
		}
		
		/// <summary>
		/// Walks a statement of the form:
		/// 
		/// self.a += self.b
		/// </summary>
		public override bool Walk(AugmentedAssignStatement node)
		{
			if (!walkingInitializeComponentMethod) {
				return false;
			}
			
			MemberExpression eventExpression = node.Left as MemberExpression;
			string eventName = eventExpression.Name.ToString();
			fieldExpression = PythonControlFieldExpression.Create(eventExpression);
			
			MemberExpression eventHandlerExpression = node.Right as MemberExpression;
			string eventHandlerName = eventHandlerExpression.Name.ToString();
			
			IComponent currentComponent = fieldExpression.GetObject(componentCreator) as IComponent;
			
			EventDescriptor eventDescriptor = TypeDescriptor.GetEvents(currentComponent).Find(eventName, false);
			PropertyDescriptor propertyDescriptor = componentCreator.GetEventProperty(eventDescriptor);
			propertyDescriptor.SetValue(currentComponent, eventHandlerName);
			return false;
		}
		
		/// <summary>
		/// Walks the binary expression which is the right hand side of an assignment statement.
		/// </summary>
		void WalkAssignment(BinaryExpression binaryExpression)
		{
			object value = deserializer.Deserialize(binaryExpression);
			fieldExpression.SetPropertyValue(componentCreator, value);
		}

		/// <summary>
		/// Walks the right hand side of an assignment to a member expression.
		/// </summary>
		void WalkMemberExpressionAssignmentRhs(Expression rhs)
		{
			MemberExpression rhsMemberExpression = rhs as MemberExpression;
			if (rhsMemberExpression != null) {
				object propertyValue = GetPropertyValueFromAssignmentRhs(rhsMemberExpression);
				fieldExpression.SetPropertyValue(componentCreator, propertyValue);
			} else {
				walkingAssignment = true;
				BinaryExpression binaryExpression = rhs as BinaryExpression;
				if (binaryExpression != null) {
					WalkAssignment(binaryExpression);
				} else {
					rhs.Walk(this);
				}
				walkingAssignment = false;
			}
		}
		
		static bool IsInitializeComponentMethod(FunctionDefinition node)
		{
			string name = node.Name.ToString().ToLowerInvariant();
			return name == "initializecomponent" || name == "initializecomponents";
		}

		/// <summary>
		/// Adds a component to the list of created objects.
		/// </summary>
		void AddComponent(string name, object obj)
		{
			IComponent component = obj as IComponent;
			if (component != null) {
				string variableName = PythonControlFieldExpression.GetVariableName(name);
				componentCreator.Add(component, variableName);
			}
		}
		
		/// <summary>
		/// Gets the type for the control being walked.
		/// </summary>
		Type GetComponentType()
		{
			string baseClass = GetBaseClassName(classDefinition);
			Type type = componentCreator.GetType(baseClass);
			if (type != null) {
				return type;
			}
			
			if (baseClass.Contains("UserControl")) {
				return typeof(UserControl);
			}
			return typeof(Form);
		}
		
		/// <summary>
		/// Gets the property value from the member expression. The member expression is taken from the
		/// right hand side of an assignment.
		/// </summary>
		object GetPropertyValueFromAssignmentRhs(MemberExpression memberExpression)
		{
			return deserializer.Deserialize(memberExpression);
		}
		
		/// <summary>
		/// Walks the right hand side of an assignment where the assignment expression is a call expression.
		/// Typically the call expression will be a constructor call.
		/// 
		/// Constructor call: System.Windows.Forms.Form()
		/// </summary>
		void WalkAssignmentRhs(CallExpression node)
		{
			MemberExpression memberExpression = node.Target as MemberExpression;
			if (memberExpression != null) {
				string name = fieldExpression.GetInstanceName(componentCreator);
				object instance = CreateInstance(name, node);
				if (instance != null) {
					if (!fieldExpression.SetPropertyValue(componentCreator, instance)) {
						AddComponent(fieldExpression.MemberName, instance);
					}
				} else {
					object obj = deserializer.Deserialize(node);
					if (obj != null) {
						fieldExpression.SetPropertyValue(componentCreator, obj);
					} else if (IsResource(memberExpression)) {
						fieldExpression.SetPropertyValue(componentCreator, GetResource(node));
					} else {
						ThrowCouldNotFindTypeException(memberExpression);
					}
				}
			} else if (node.Target is IndexExpression) {
				WalkArrayAssignmentRhs(node);
			}
		}
		
		/// <summary>
		/// Walks a method call. Typical method calls are:
		/// 
		/// self._menuItem1.Items.AddRange(...)
		/// 
		/// This method will execute the method call.
		/// </summary>
		void WalkMethodCall(CallExpression node)
		{
			// Try to get the object being called. Try the form first then
			// look for other controls.
			object member = PythonControlFieldExpression.GetMember(component, node);
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(node);
			if (member == null) {
				member = field.GetMember(componentCreator);
			}
			
			// Execute the method on the object.
			if (member != null) {
				object[] args = deserializer.GetArguments(node).ToArray();
				InvokeMethod(member, field.MethodName, args);
			}
		}
		
		void InvokeMethod(object obj, string name, object[] args)
		{
			Type type = obj.GetType();
			try {
				type.InvokeMember(name, BindingFlags.InvokeMethod, Type.DefaultBinder, obj, args);
			} catch (MissingMethodException ex) {
				// Look for an explicitly implemented interface.
				MethodInfo method = FindInterfaceMethod(type, name);
				if (method != null) {
					method.Invoke(obj, args);
				} else {
					throw ex;
				}
			}
		}
		
		/// <summary>
		/// Looks for an explicitly implemented interface.
		/// </summary>
		MethodInfo FindInterfaceMethod(Type type, string name)
		{
			string nameMatch = "." + name;
			foreach (MethodInfo method in type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)) {
				if (method.Name.EndsWith(nameMatch)) {
					return method;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Creates a new instance with the specified name.
		/// </summary>
		object CreateInstance(string name, CallExpression node)
		{
			MemberExpression memberExpression = node.Target as MemberExpression;
			if (memberExpression != null) {
				string typeName = PythonControlFieldExpression.GetMemberName(memberExpression);
				Type type = componentCreator.GetType(typeName);
				if (type != null) {
					if (type.IsAssignableFrom(typeof(ComponentResourceManager))) {
						return componentCreator.CreateInstance(type, new object[0], name, false);
					}
					List<object> args = deserializer.GetArguments(node);
					return componentCreator.CreateInstance(type, args, name, false);
				}
			}
			return null;
		}
		
		/// <summary>
		/// Returns true if the expression is of the form:
		/// 
		/// resources.GetObject(...) or
		/// resources.GetString(...)
		/// </summary>
		bool IsResource(MemberExpression memberExpression)
		{
			string fullName = PythonControlFieldExpression.GetMemberName(memberExpression);
			return fullName.StartsWith("resources.", StringComparison.InvariantCultureIgnoreCase);
		}
		
		object GetResource(CallExpression callExpression)
		{
			IResourceReader reader = componentCreator.GetResourceReader(CultureInfo.InvariantCulture);
			if (reader != null) {
				using (ResourceSet resources = new ResourceSet(reader)) {
					List<object> args = deserializer.GetArguments(callExpression);
					return resources.GetObject(args[0] as String);
				}
			}
			return null;
		}
		
		/// <summary>
		/// Walks the right hand side of an assignment when the assignment is an array creation.
		/// </summary>
		void WalkArrayAssignmentRhs(CallExpression callExpression)
		{
			object array = deserializer.Deserialize(callExpression);
			fieldExpression.SetPropertyValue(componentCreator, array);	
		}
		
		void ThrowCouldNotFindTypeException(MemberExpression memberExpression)
		{
			string typeName = PythonControlFieldExpression.GetMemberName(memberExpression);
			throw new PythonComponentWalkerException(String.Format(StringParser.Parse("${res:ICSharpCode.PythonBinding.UnknownTypeName}"), typeName));
		}
	}
}
