// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;

using ICSharpCode.Scripting;
using IronPython.Compiler;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Creates objects from python code.
	/// </summary>
	public class PythonCodeDeserializer
	{
		IComponentCreator componentCreator;

		public PythonCodeDeserializer(IComponentCreator componentCreator)
		{
			this.componentCreator = componentCreator;
		}
		
		/// <summary>
		/// Gets the arguments passed to the call expression.
		/// </summary>
		public List<object> GetArguments(CallExpression expression)
		{
			List<object> args = new List<object>();
			foreach (Arg a in expression.Args) {
				args.Add(Deserialize(a.Expression));
			}
			return args;
		}		
		
		/// <summary>
		/// Creates or gets the object specified in the python AST.
		/// </summary>
		/// <returns>
		/// Null if the node cannot be deserialized.
		/// </returns>
		public object Deserialize(Node node)
		{
			if (node == null) {
				throw new ArgumentNullException("node");
			}
	
			if (node is CallExpression) {
				return Deserialize((CallExpression)node);
			} else if (node is BinaryExpression) {
				return Deserialize((BinaryExpression)node);
			} else if (node is MemberExpression) {
				return Deserialize((MemberExpression)node);
			} else if (node is UnaryExpression) {
				return Deserialize((UnaryExpression)node);
			} else if (node is ConstantExpression) {
				return Deserialize((ConstantExpression)node);
			} else if (node is NameExpression) {
				return Deserialize((NameExpression)node);
			}
			return null;
		}
				
		/// <summary>
		/// Deserializes expressions of the form:
		/// 
		/// System.Windows.Form.AnchorStyles.Top | System.Windows.Form.AnchorStyles.Bottom
		/// </summary>
		public object Deserialize(BinaryExpression binaryExpression)
		{
			object lhs = Deserialize(binaryExpression.Left);
			object rhs = Deserialize(binaryExpression.Right);
			
			int value = Convert.ToInt32(lhs) | Convert.ToInt32(rhs);
			return Enum.ToObject(lhs.GetType(), value);			
		}
		
		/// <summary>
		/// Deserializes expressions of the form:
		/// 
		/// 1) System.Drawing.Color.FromArgb(0, 192, 0)
		/// 2) System.Array[String](["a", "b"])
		/// </summary>
		object Deserialize(CallExpression callExpression)
		{
			MemberExpression memberExpression = callExpression.Target as MemberExpression;
			IndexExpression indexExpression = callExpression.Target as IndexExpression;
			if (memberExpression != null) {
				return DeserializeMethodCallExpression(callExpression, memberExpression);
			} else if (indexExpression != null) {
				return DeserializeCreateArrayExpression(callExpression, indexExpression);
			}
			return null;
		}
		
		/// <summary>
		/// Deserializes expressions of the form:
		/// 
		/// 1) System.Windows.Forms.Cursors.AppStarting
		/// </summary>
		object Deserialize(MemberExpression memberExpression)
		{
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(memberExpression);			
			Type type = GetType(field);
			if (type != null) {
				if (type.IsEnum) {
					return Enum.Parse(type, field.MemberName);
				} else {
					BindingFlags propertyBindingFlags = BindingFlags.Public | BindingFlags.GetField | BindingFlags.Static | BindingFlags.Instance;
					PropertyInfo propertyInfo = type.GetProperty(field.MemberName, propertyBindingFlags);
					if (propertyInfo != null) {
						return propertyInfo.GetValue(type, null);
					}
				}
			}
			return componentCreator.GetInstance(PythonControlFieldExpression.GetVariableName(field.MemberName));
		}
		
		/// <summary>
		/// Deserializes expressions of the form:
		/// 
		/// 1) self
		/// </summary>
		object Deserialize(NameExpression nameExpression)
		{
			string name = nameExpression.Name;
			if ("self" == name.ToLowerInvariant()) {
				return componentCreator.RootComponent;
			} else {
				bool result;
				if (Boolean.TryParse(name, out result)) {
					return result;
				}
			}
			return componentCreator.GetInstance(name);
		}
		
		Type GetType(PythonControlFieldExpression field)
		{
			string typeName = PythonControlFieldExpression.GetPrefix(field.FullMemberName);
			return componentCreator.GetType(typeName);
		}
		
		/// <summary>
		/// Deserializes a call expression where the target is an array expression.
		/// 
		/// System.Array[String](["a", "b"])
		/// </summary>
		object DeserializeCreateArrayExpression(CallExpression callExpression, IndexExpression target)
		{
			ListExpression list = callExpression.Args[0].Expression as ListExpression;
			Type arrayType = GetType(target.Index as MemberExpression);
			Array array = Array.CreateInstance(arrayType, list.Items.Count);
			for (int i = 0; i < list.Items.Count; ++i) {
				Expression listItemExpression = list.Items[i];
				ConstantExpression constantExpression = listItemExpression as ConstantExpression;
				MemberExpression memberExpression = listItemExpression as MemberExpression;
				NameExpression nameExpression = listItemExpression as NameExpression;
				CallExpression listItemCallExpression = listItemExpression as CallExpression;
				if (constantExpression != null) {
					array.SetValue(constantExpression.Value, i);
				} else if (memberExpression != null) {
					string name = PythonControlFieldExpression.GetVariableName(memberExpression.Name);
					array.SetValue(componentCreator.GetComponent(name), i);
				} else if (nameExpression != null) {
					array.SetValue(componentCreator.GetInstance(nameExpression.Name), i);
				} else if (listItemCallExpression != null) {
					Type arrayInstanceType = GetType(listItemCallExpression.Target as MemberExpression);
					object instance = componentCreator.CreateInstance(arrayInstanceType, GetArguments(listItemCallExpression), null, false);
					array.SetValue(instance, i);
				}
			}
			return array;
		}
		
		Type GetType(MemberExpression memberExpression)
		{
			string typeName = PythonControlFieldExpression.GetMemberName(memberExpression);
			return componentCreator.GetType(typeName);
		}
		
		/// <summary>
		/// Deserializes an expression of the form:
		/// 
		/// System.Drawing.Color.FromArgb(0, 192, 0)
		/// </summary>
		object DeserializeMethodCallExpression(CallExpression callExpression, MemberExpression memberExpression)
		{
			PythonControlFieldExpression field = PythonControlFieldExpression.Create(memberExpression);			
			Type type = GetType(field);
			if (type != null) {
				foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
					if (method.Name == field.MemberName) {
						if (method.GetParameters().Length == callExpression.Args.Count) {
							return method.Invoke(null, GetArguments(callExpression).ToArray());
						}
					}
				}
			} else {
				// Maybe it is a call to a constructor?
				type = componentCreator.GetType(field.FullMemberName);
				if (type != null) {
					return componentCreator.CreateInstance(type, GetArguments(callExpression), null, false);
				}
			}
			return null;
		}
		
		object Deserialize(UnaryExpression expression)
		{
			object rhs = Deserialize(expression.Expression);
			switch (expression.Op) {
				case PythonOperator.Negate:
					return Negate(rhs);
			}
			return rhs;
		}
		
		object Negate(object value)
		{
			if (value is int) {
				return -1 * (int)value;
			} else if (value is double) {
				return -1 * (double)value;
			}
			return value;
		}
		
		object Deserialize(ConstantExpression expression)
		{
			return expression.Value;
		}
	}
}
