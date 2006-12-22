// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	/// <summary>
	/// Provides more specific access
	/// </summary>
	public partial class Value
	{
		/// <summary> Returns true if the value is null </summary>
		public bool IsNull {
			get {
				return CorValue == null;
			}
		}
		
		/// <summary> Gets a string representation of the value </summary>
		public string AsString {
			get {
				if (IsNull)      return "<null reference>";
				if (IsArray)     return "{" + this.Type.Name + "}";
				if (IsObject)    return "{" + this.Type.Name + "}";
				if (IsPrimitive) return PrimitiveValue != null ? PrimitiveValue.ToString() : String.Empty;
				throw new DebuggerException("Unknown value type");
			}
		}
	}
}
