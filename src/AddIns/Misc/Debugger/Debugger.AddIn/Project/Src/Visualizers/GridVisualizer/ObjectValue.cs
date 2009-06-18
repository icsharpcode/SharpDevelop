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
using Debugger.Expressions;
using Debugger.MetaData;

namespace Debugger.AddIn.Visualizers.GridVisualizer
{
	/// <summary>
	/// Object in the debugee, with its properties loaded.
	/// </summary>
	public class ObjectValue
	{
		private Dictionary<string, ObjectProperty> properties = new Dictionary<string, ObjectProperty>();

		/// <summary>
		/// Returns property with given name.
		/// </summary>
		public ObjectProperty this[string key]
		{
			get	{ return properties.GetValue(key); }
			set	{ properties[key] = value; }
		}
		
		public static ObjectValue Create(Expression expr, DebugType type, BindingFlags bindingFlags)
		{
			ObjectValue result = new ObjectValue();
			foreach(Expression memberExpr in expr.AppendObjectMembers(type, bindingFlags))
			{
				ObjectProperty property = new ObjectProperty();
				
				Value propertyValue = memberExpr.Evaluate(WindowsDebugger.CurrentProcess).GetPermanentReference();
				property.Name = memberExpr.CodeTail;
				property.Expression = memberExpr;
				property.IsAtomic = propertyValue.Type.IsPrimitive;
				property.IsNull = propertyValue.IsNull;
				property.Value = /*property.IsNull ? "" :*/ propertyValue.AsString;
				
				result.properties.Add(property.Name, property);
			}
			return result;
		}
	}
}
