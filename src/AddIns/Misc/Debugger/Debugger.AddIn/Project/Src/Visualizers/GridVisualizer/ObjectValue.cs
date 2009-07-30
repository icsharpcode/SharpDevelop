// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.SharpDevelop.Services;
using System;
using System.Collections.Generic;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.NRefactory.Ast;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Object in the debugee, with its properties loaded.
	/// </summary>
	public class ObjectValue
	{
		// couldn't be properties loaded lazily? - when listview databinding asks for key, we could ask fetch the value from debugger
		private Dictionary<string, ObjectProperty> properties = new Dictionary<string, ObjectProperty>();

		/// <summary>
		/// Returns property with given name.
		/// </summary>
		public ObjectProperty this[string key]
		{
			get	{ return properties.GetValue(key); }
			set	{ properties[key] = value; }
		}
		
		public static ObjectValue Create(Debugger.Value value, DebugType type, BindingFlags bindingFlags)
		{
			ObjectValue result = new ObjectValue();
			foreach(MemberInfo memberInfo in value.Type.GetMembers(bindingFlags))
			{
				Value memberValue = value.GetMemberValue(memberInfo);
				
				ObjectProperty property = new ObjectProperty();
				property.Name = memberInfo.Name;
				// property.Expression = ?.Age 
				// - cannot use expression, 
				// if the value is returned from an enumerator, it has no meaningful expression
				property.IsAtomic = memberValue.Type.IsPrimitive;
				property.IsNull = memberValue.IsNull;
				//property.Value = memberValue.AsString;
				property.Value = memberValue.IsNull ? "" : memberValue.InvokeToString();
				result.properties.Add(property.Name, property);
			}
			return result;
		}
		
		public static ObjectValue Create(Expression expr, DebugType type, BindingFlags bindingFlags)
		{
			ObjectValue result = new ObjectValue();
			foreach(MemberInfo memberInfo in type.GetMembers(bindingFlags))
			{
				Expression memberExpression = expr.AppendMemberReference(memberInfo);
				Value memberValue = memberExpression.Evaluate(WindowsDebugger.CurrentProcess);
					
				ObjectProperty property = new ObjectProperty();
				property.Name = memberInfo.Name;
				property.Expression = memberExpression;
				property.IsAtomic = memberValue.Type.IsPrimitive;
				property.IsNull = memberValue.IsNull;
				property.Value = memberValue.IsNull ? "" : memberValue.InvokeToString();

				result.properties.Add(property.Name, property);
			}
			return result;
		}
	}
}
