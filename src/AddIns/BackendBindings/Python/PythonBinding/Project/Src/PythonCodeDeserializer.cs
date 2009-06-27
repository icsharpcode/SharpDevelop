// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
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
				ConstantExpression constantExpression = a.Expression as ConstantExpression;
				MemberExpression memberExpression = a.Expression as MemberExpression;
				CallExpression callExpression = a.Expression as CallExpression;
				NameExpression nameExpression = a.Expression as NameExpression;
				if (constantExpression != null) {
					args.Add(constantExpression.Value);
				} else if (memberExpression != null) {
					args.Add(Deserialize(memberExpression));
				} else if (callExpression != null) {
					args.Add(Deserialize(callExpression));
				} else if (nameExpression != null) {
					args.Add(Deserialize(nameExpression));
				}
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
	
			MemberExpression memberExpression = node as MemberExpression;
			CallExpression callExpression = node as CallExpression;
			BinaryExpression binaryExpression = node as BinaryExpression;
			if (callExpression != null) {
				return Deserialize(callExpression);
			} else if (binaryExpression != null) {
				return Deserialize(binaryExpression);
			} else if (memberExpression != null) {
				return Deserialize(memberExpression);
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
			if ("self" == nameExpression.Name.ToString().ToLowerInvariant()) {
				return componentCreator.RootComponent;
			}
			return null;
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
			MemberExpression arrayTypeMemberExpression = target.Index as MemberExpression;
			Type arrayType = componentCreator.GetType(PythonControlFieldExpression.GetMemberName(arrayTypeMemberExpression));
			Array array = Array.CreateInstance(arrayType, list.Items.Length);
			for (int i = 0; i < list.Items.Length; ++i) {
				Expression listItemExpression = list.Items[i];
				ConstantExpression constantExpression = listItemExpression as ConstantExpression;
				MemberExpression memberExpression = listItemExpression as MemberExpression;
				NameExpression nameExpression = listItemExpression as NameExpression;
				if (constantExpression != null) {
					array.SetValue(constantExpression.Value, i);
				} else if (memberExpression != null) {
					string name = PythonControlFieldExpression.GetVariableName(memberExpression.Name.ToString());
					array.SetValue(componentCreator.GetComponent(name), i);
				} else if (nameExpression != null) {
					array.SetValue(componentCreator.GetInstance(nameExpression.Name.ToString()), i);
				}
			}
			return array;			
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
						if (method.GetParameters().Length == callExpression.Args.Length) {
							return method.Invoke(null, GetArguments(callExpression).ToArray());
						}
					}
				}
			}
			return null;
		}
	}
}
