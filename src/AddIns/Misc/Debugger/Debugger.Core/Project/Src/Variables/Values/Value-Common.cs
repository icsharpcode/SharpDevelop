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
		
//		public bool MayHaveSubVariables {
//			get {
//				#if DEBUG
//					if (IsNull)      return true;
//					if (IsArray)     return true;
//					if (IsObject)    return true;
//					if (IsPrimitive) return true;
//				#else
//					if (IsNull)      return false;
//					if (IsArray)     return true;
//					if (IsObject)    return true;
//					if (IsPrimitive) return false;
//				#endif
//				throw new DebuggerException("Unknown value type");
//			}
//		}
//		
//		public VariableCollection SubVariables {
//			get {
//				VariableCollection subVars = null;
//				if (IsNull)      subVars = new VariableCollection(new Variable[] {});
//				if (IsArray)     subVars = new VariableCollection(GetArrayElements());
//				if (IsObject)    subVars = this.ObjectSubVariables;
//				if (IsPrimitive) subVars = new VariableCollection(new Variable[] {});
//				if (subVars == null) throw new DebuggerException("Unknown value type");
//				#if DEBUG
//					return new VariableCollection(subVars.Name,
//					                              subVars.Value,
//					                              Util.MergeLists(this.GetDebugInfo(), subVars.SubCollections).ToArray(),
//					                              subVars.Items);
//				#else
//					return subVars;
//				#endif
//			}
//		}
	}
}
