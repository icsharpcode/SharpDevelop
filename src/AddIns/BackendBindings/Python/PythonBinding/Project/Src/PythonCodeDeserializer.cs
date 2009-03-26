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
				if (constantExpression != null) {
					args.Add(constantExpression.Value);
				} else if (memberExpression != null) {
					args.Add(Deserialize(memberExpression));
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
			}
			return Deserialize(memberExpression);
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
		/// </summary>
		object Deserialize(CallExpression callExpression)
		{
			MemberExpression memberExpression = callExpression.Target as MemberExpression;
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
			return null;
		}
		
		Type GetType(PythonControlFieldExpression field)
		{
			return componentCreator.GetType(PythonControlFieldExpression.GetPrefix(field.FullMemberName));
		}
	}
}
