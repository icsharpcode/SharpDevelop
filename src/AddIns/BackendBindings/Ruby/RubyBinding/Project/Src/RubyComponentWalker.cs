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
using IronRuby.Compiler;
using IronRuby.Compiler.Ast;
using IronRuby.Hosting;
using IronRuby.Runtime;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Visits the code's Ruby AST and creates a Windows Form.
	/// </summary>
	public class RubyComponentWalker : Walker, IComponentWalker
	{
		IComponent component;
		IComponentCreator componentCreator;
		string componentName = String.Empty;
		RubyCodeDeserializer deserializer;
		ClassDefinition classDefinition;
		
		public RubyComponentWalker(IComponentCreator componentCreator)
		{
			this.componentCreator = componentCreator;
			deserializer = new RubyCodeDeserializer(componentCreator);
		}
		
		/// <summary>
		/// Creates a control either a UserControl or Form from the Ruby code.
		/// </summary>
		public IComponent CreateComponent(string rubyCode)
		{			
			RubyParser parser = new RubyParser();
			SourceUnitTree ast = parser.CreateAst(@"Control.rb",  new StringTextBuffer(rubyCode));
			Walk(ast);
			
			// Did we find the InitializeComponent method?
			if (!FoundInitializeComponentMethod) {
				throw new RubyComponentWalkerException("Unable to find InitializeComponents method.");
			}
			return component;
		}
		
		/// <summary>
		/// Gets the fully qualified name of the base class.
		/// </summary>
		public static string GetBaseClassName(ClassDefinition classDefinition)
		{
			ConstantVariable variable = classDefinition.SuperClass as ConstantVariable;
			if (variable != null) {
				return RubyControlFieldExpression.GetQualifiedName(variable);
			}
			return String.Empty;
		}
		
		protected override void Walk(ClassDefinition node)
		{
			classDefinition = node;
			componentName = node.QualifiedName.Name;
			base.Walk(node);
		}

		protected override void Walk(MethodDefinition node)
		{
			if (IsInitializeComponentMethod(node)) {
				Type type = GetComponentType();
				component = componentCreator.CreateComponent(type, componentName);
				
				IResourceReader reader = componentCreator.GetResourceReader(CultureInfo.InvariantCulture);
				if (reader != null) {
					reader.Dispose();
				}

				WalkMethodStatements(node.Body.Statements);
			}
		}
		
		void WalkMethodStatements(Statements statements)
		{
			foreach (Expression expression in statements) {
				WalkExpression(expression);
			}
		}
		
		void WalkExpression(Expression node)
		{
			if (node is SimpleAssignmentExpression) {
				WalkSimpleAssignment((SimpleAssignmentExpression)node);
			} else if (node is MethodCall) {
				WalkMethodCall((MethodCall)node);
			}
		}
		
		void WalkSimpleAssignment(SimpleAssignmentExpression node)
		{
			AttributeAccess attributeAccess = node.Left as AttributeAccess;
			InstanceVariable instance = node.Left as InstanceVariable;
			LocalVariable localVariable = node.Left as LocalVariable;
			if (attributeAccess != null) {
				RubyControlFieldExpression field = RubyControlFieldExpression.Create(attributeAccess);
				object propertyValue = deserializer.Deserialize(node.Right);
				if (propertyValue != null) {
					field.SetPropertyValue(componentCreator, propertyValue);
				} else if (IsResource(node.Right)) {
					field.SetPropertyValue(componentCreator, GetResource(node.Right as MethodCall));
				} else {
					ThrowCouldNotFindTypeException(node.Right);
				}
			} else if (instance != null) {
				string instanceName = RubyControlFieldExpression.GetVariableName(instance.Name);
				object propertyValue = deserializer.Deserialize(instanceName, node.Right);
				AddComponent(instanceName, propertyValue);
			} else if (localVariable != null) {
				object propertyValue = deserializer.Deserialize(localVariable.Name, node.Right);
				if (propertyValue == null) {
					ThrowCouldNotFindTypeException(node.Right);
				}
			}
		}
		
		/// <summary>
		/// Walks a method call. Typical method calls are:
		/// 
		/// @menuItem1.Items.AddRange(...)
		/// 
		/// This method will execute the method call.
		/// </summary>
		void WalkMethodCall(MethodCall node)
		{
			// Try to get the object being called. Try the form first then
			// look for other controls.
			object member = RubyControlFieldExpression.GetMember(component, node);
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(node);
			if (member == null) {
				member = field.GetMember(componentCreator);
			}
			
			// Execute the method on the object.
			if (member != null) {
				if (node.Arguments == null) {
					RegisterEventHandler(node, member);
				} else {
					object[] args = deserializer.GetArguments(node).ToArray();
					InvokeMethod(member, field.MethodName, args);
				}
			}
		}
		
		void RegisterEventHandler(MethodCall node, object component)
		{
			string eventHandlerName = GetEventHandlerName(node);
			RegisterEventHandler(component, node.MethodName, eventHandlerName);
		}
		
		void RegisterEventHandler(object component, string eventName, string eventHandlerName)
		{
			EventDescriptor eventDescriptor = TypeDescriptor.GetEvents(component).Find(eventName, false);
			PropertyDescriptor propertyDescriptor = componentCreator.GetEventProperty(eventDescriptor);
			propertyDescriptor.SetValue(component, eventHandlerName);
		}
		
		string GetEventHandlerName(MethodCall eventHandlerMethodBlock)
		{
			BlockDefinition blockDef = eventHandlerMethodBlock.Block as BlockDefinition;
			MethodCall eventHandlerMethodCall = blockDef.Body.First as MethodCall;
			return eventHandlerMethodCall.MethodName;
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
		
		static bool IsInitializeComponentMethod(MethodDefinition node)
		{
			string name = node.Name.ToLowerInvariant();
			return name == "initializecomponent" || name == "initializecomponents";
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
		
		bool FoundInitializeComponentMethod {
			get { return component != null; }
		}
		
		/// <summary>
		/// Adds a component to the list of created objects.
		/// </summary>
		void AddComponent(string name, object obj)
		{
			IComponent component = obj as IComponent;
			if (component != null) {
				componentCreator.Add(component, name);
			}
		}
		
		bool IsResource(Expression expression)
		{
			MethodCall methodCall = expression as MethodCall;
			if (methodCall != null) {
				string memberName = RubyControlFieldExpression.GetMemberName(methodCall);
				return memberName.StartsWith("resources.", StringComparison.InvariantCultureIgnoreCase);
			}
			return false;
		}
		
		object GetResource(MethodCall methodCall)
		{
			IResourceReader reader = componentCreator.GetResourceReader(CultureInfo.InvariantCulture);
			if (reader != null) {
				using (ResourceSet resources = new ResourceSet(reader)) {
					List<object> args = deserializer.GetArguments(methodCall);
					return resources.GetObject(args[0] as String);
				}
			}
			return null;
		}
		
		void ThrowCouldNotFindTypeException(Expression expression)
		{
			string typeName = String.Empty;
			MethodCall methodCall = expression as MethodCall;
			if (methodCall != null) {
				typeName = RubyControlFieldExpression.GetMemberName(methodCall);
			}
			string message = String.Format(StringParser.Parse("${res:ICSharpCode.PythonBinding.UnknownTypeName}"), typeName);
			throw new RubyComponentWalkerException(message);
		}
	}
}
