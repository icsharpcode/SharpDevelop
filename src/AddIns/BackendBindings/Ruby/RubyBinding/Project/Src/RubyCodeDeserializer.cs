// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

using ICSharpCode.Scripting;
using IronRuby.Builtins;
using IronRuby.Compiler;
using IronRuby.Compiler.Ast;

namespace ICSharpCode.RubyBinding
{
	public class RubyCodeDeserializer
	{
		IComponentCreator componentCreator;
		
		public RubyCodeDeserializer(IComponentCreator componentCreator)
		{
			this.componentCreator = componentCreator;
		}
		
		public object Deserialize(Expression expression)
		{
			return Deserialize(null, expression);
		}
		
		public object Deserialize(string name, Expression expression)
		{
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			
			if (expression is MethodCall) {
				return Deserialize(name, (MethodCall)expression);
			} else if (expression is Literal) {
				return Deserialize((Literal)expression);
			} else if (expression is StringConstructor) {
				return Deserialize((StringConstructor)expression);
			} else if (expression is InstanceVariable) {
				return Deserialize((InstanceVariable)expression);
			} else if (expression is SelfReference) {
				return Deserialize((SelfReference)expression);
			} else if (expression is LocalVariable) {
				return Deserialize((LocalVariable)expression);
			}
			return null;
		}
		
		public object Deserialize(string name, MethodCall methodCall)
		{
			if ("new".Equals(methodCall.MethodName, StringComparison.InvariantCultureIgnoreCase)) {
				if (IsArrayCreation(methodCall)) {
					return DeserializeCreateArrayExpression(name, methodCall);
				}
				return CreateInstance(name, methodCall);
			} else if ("|".Equals(methodCall.MethodName)) {
				return DeserializeBitwiseOr(methodCall);
			}
			return CreateInstance(name, methodCall);
		}
		
		bool IsArrayCreation(MethodCall methodCall)
		{
			MethodCall targetMethodCall = methodCall.Target as MethodCall;
			if (targetMethodCall != null) {
				return targetMethodCall.MethodName == "[]";
			}
			return false;
		}
		
		public static object Deserialize(StringConstructor stringConstructor)
		{
			if (stringConstructor.Parts.Count > 0) {
				StringLiteral literal = stringConstructor.Parts[0] as StringLiteral;
				MutableString mutableString = literal.GetMutableString();
				return mutableString.ConvertToString();
			}
			return String.Empty;
		}
		
		public object Deserialize(InstanceVariable instance)
		{
			string name = instance.Name.Substring(1);
			object obj = componentCreator.GetComponent(name);
			if (obj != null) {
				return obj;
			}
			return componentCreator.GetInstance(name);
		}
		
		public List<object> GetArguments(MethodCall methodCall)
		{
			return GetArguments(methodCall.Arguments);
		}
		
		/// <summary>
		/// Gets the arguments passed to the call expression.
		/// </summary>
		public List<object> GetArguments(Arguments arguments)
		{
			List<object> args = new List<object>();
			if (arguments.Expressions != null) {
				foreach (Expression expression in arguments.Expressions) {
					args.Add(Deserialize(expression));
				}
			}
			return args;
		}		
		
		public object Deserialize(Literal literal)
		{
			return literal.Value;
		}
		
		object DeserializeBitwiseOr(MethodCall methodCall)
		{
			List<object> items = new List<object>();
			while (methodCall != null) {
				if (methodCall.Arguments != null) {
					items.Add(CreateInstance(null, methodCall.Arguments.Expressions[0] as MethodCall));
				}
				methodCall = methodCall.Target as MethodCall;
			}

			int value = 1;
			foreach (object item in items) {
				value = Convert.ToInt32(item) | value;
			}
			return Enum.ToObject(items[0].GetType(), value);
		}
		
		object CreateInstance(string name, MethodCall methodCall)
		{
			RubyControlFieldExpression field = RubyControlFieldExpression.Create(methodCall);
			Type type = GetType(field);
			if (type != null) {
				if (type.IsEnum) {
					return Enum.Parse(type, methodCall.MethodName);
				}
				
				BindingFlags propertyBindingFlags = BindingFlags.Public | BindingFlags.GetField | BindingFlags.Static | BindingFlags.Instance;
				PropertyInfo propertyInfo = type.GetProperty(methodCall.MethodName, propertyBindingFlags);
				if (propertyInfo != null) {
					return propertyInfo.GetValue(type, null);
				}
				
				if (type.IsAssignableFrom(typeof(ComponentResourceManager))) {
					return componentCreator.CreateInstance(type, new object[0], name, false);
				}
				
				if (methodCall.Arguments != null) {
					return CreateInstance(type, name, methodCall);
				}
			} else {
				return componentCreator.GetInstance(field.MethodName);
			}
			return null;
		}

		object CreateInstance(Type type, string name, MethodCall methodCall)
		{
			List<object> args = GetArguments(methodCall.Arguments);
					
			// Try to execute a method on the object.
			foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
				if (method.Name == methodCall.MethodName) {
					int parameterCount = method.GetParameters().Length;
					if ((methodCall.Arguments.IsEmpty && (parameterCount == 0)) || 
					    (parameterCount == methodCall.Arguments.Expressions.Length)) {
						return method.Invoke(null, args.ToArray());
					}
				}
			}
			return componentCreator.CreateInstance(type, args, name, false);
		}
		
		Type GetType(RubyControlFieldExpression field)
		{
			string typeName = field.FullMemberName;
			if (!String.IsNullOrEmpty(typeName)) {
				return componentCreator.GetType(typeName);
			}
			return null;
		}
		
		/// <summary>
		/// Deserializes a method call where the target is an array expression.
		/// 
		/// System::Array[System::String].new("a", "b")
		/// </summary>
		object DeserializeCreateArrayExpression(string name, MethodCall methodCall)
		{
			MethodCall arrayCreationMethodCall = methodCall.Target as MethodCall;
			ConstantVariable constantVariable = arrayCreationMethodCall.Arguments.Expressions[0] as ConstantVariable;
			string arrayTypeName = RubyControlFieldExpression.GetQualifiedName(constantVariable);
			
			ArrayConstructor arrayConstructor = methodCall.Arguments.Expressions[0] as ArrayConstructor;
			Expression[] arrayItems = arrayConstructor.Arguments.Expressions;
			
			Type arrayType = componentCreator.GetType(arrayTypeName);
			Array array = Array.CreateInstance(arrayType, arrayItems.Length);
			for (int i = 0; i < arrayItems.Length; ++i) {
				Expression arrayItemExpression = arrayItems[i];
				object instance = Deserialize(arrayItemExpression);
				array.SetValue(instance, i);
			}
			return array;
		}
		
		object Deserialize(SelfReference selfRef)
		{
			return componentCreator.RootComponent;
		}
		
		object Deserialize(LocalVariable localVariable)
		{
			return componentCreator.GetInstance(localVariable.Name);
		}
	}
}
