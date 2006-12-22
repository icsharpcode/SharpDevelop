// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2022 $</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class Value
	{
		internal static ICorDebugValue DereferenceUnbox(ICorDebugValue corValue)
		{
			// Method arguments can be passed 'by ref'
			if (corValue.Type == (uint)CorElementType.BYREF) {
				corValue = corValue.CastTo<ICorDebugReferenceValue>().Dereference();
			}
			
			// Pointers may be used in 'unsafe' code - CorElementType.PTR
			// Classes need to be dereferenced
			while (corValue.Is<ICorDebugReferenceValue>()) {
				ICorDebugReferenceValue refValue = corValue.CastTo<ICorDebugReferenceValue>();
				if (refValue.IsNull != 0) {
					return null; // Reference is null
				} else {
					try {
						corValue = refValue.Dereference();
						// TODO: Investigate: Must not acutally be null
						//       eg. Assembly.AssemblyHandle   See SD2-1117
						if (corValue == null) return null; // Dereference() returned null
					} catch {
						return null; // Error during dereferencing
					}
				}
			}
			
			// Unbox value types
			if (corValue.Is<ICorDebugBoxValue>()) {
				corValue = corValue.CastTo<ICorDebugBoxValue>().Object.CastTo<ICorDebugValue>();
			}
			
			return corValue;
		}
		
		/*
		internal VariableCollection GetDebugInfo()
		{
			return GetDebugInfo(this.RawCorValue);
		}
		
		internal static VariableCollection GetDebugInfo(ICorDebugValue corValue)
		{
			List<VariableCollection> items = new List<VariableCollection>();
			
			if (corValue.Is<ICorDebugValue>()) {
				List<VariableCollection> more = new List<VariableCollection>();
				more.Add(new VariableCollection("type", ((CorElementType)corValue.Type).ToString()));
				more.Add(new VariableCollection("size", corValue.Size.ToString()));
				more.Add(new VariableCollection("address", corValue.Address.ToString("X")));
				items.Add(new VariableCollection("ICorDebugValue", "", more, null));
			}
			if (corValue.Is<ICorDebugValue2>())         items.Add(new VariableCollection("ICorDebugValue2", "", null, null));
			if (corValue.Is<ICorDebugGenericValue>()) {
				List<VariableCollection> more = new List<VariableCollection>();
				try {
					byte[] bytes = corValue.CastTo<ICorDebugGenericValue>().RawValue;
					for(int i = 0; i < bytes.Length; i += 8) {
						string val = "";
						for(int j = i; j < bytes.Length && j < i + 8; j++) {
							val += bytes[j].ToString("X2") + " ";
						}
						more.Add(new VariableCollection("data" + i.ToString("X2"), val));
					}
				} catch (ArgumentException) {
					more.Add(new VariableCollection("data", "N/A"));
				}
				items.Add(new VariableCollection("ICorDebugGenericValue", "", more, null));
			}
			if (corValue.Is<ICorDebugReferenceValue>()) {
				List<VariableCollection> more = new List<VariableCollection>();
				ICorDebugReferenceValue refValue = corValue.CastTo<ICorDebugReferenceValue>();
				bool isNull = refValue.IsNull != 0;
				more.Add(new VariableCollection("isNull", isNull.ToString()));
				if (!isNull) {
					more.Add(new VariableCollection("address", refValue.Value.ToString("X")));
					if (refValue.Dereference() != null) {
						VariableCollection deRef = GetDebugInfo(refValue.Dereference());
						more.Add(new VariableCollection("dereferenced", deRef.Value, deRef.SubCollections, deRef.Items));
					} else {
						more.Add(new VariableCollection("dereferenced", "N/A", null, null));
					}
					
				}
				items.Add(new VariableCollection("ICorDebugReferenceValue", "", more, null));
			}
			if (corValue.Is<ICorDebugHeapValue>())      items.Add(new VariableCollection("ICorDebugHeapValue", "", null, null));
			if (corValue.Is<ICorDebugHeapValue2>())     items.Add(new VariableCollection("ICorDebugHeapValue2", "", null, null));
			if (corValue.Is<ICorDebugObjectValue>()) {
				List<VariableCollection> more = new List<VariableCollection>();
				bool isValue = corValue.CastTo<ICorDebugObjectValue>().IsValueClass != 0;
				more.Add(new VariableCollection("isValue", isValue.ToString()));
				items.Add(new VariableCollection("ICorDebugObjectValue", "", more, null));
			}
			if (corValue.Is<ICorDebugObjectValue2>())   items.Add(new VariableCollection("ICorDebugObjectValue2", "", null, null));
			if (corValue.Is<ICorDebugBoxValue>()) {
				List<VariableCollection> more = new List<VariableCollection>();
				VariableCollection unboxed = GetDebugInfo(corValue.CastTo<ICorDebugBoxValue>().Object.CastTo<ICorDebugValue>());
				more.Add(new VariableCollection("unboxed", unboxed.Value, unboxed.SubCollections, unboxed.Items));
				items.Add(new VariableCollection("ICorDebugBoxValue", "", more, null));
			}
			if (corValue.Is<ICorDebugStringValue>())    items.Add(new VariableCollection("ICorDebugStringValue", "", null, null));
			if (corValue.Is<ICorDebugArrayValue>())     items.Add(new VariableCollection("ICorDebugArrayValue", "", null, null));
			if (corValue.Is<ICorDebugHandleValue>())    items.Add(new VariableCollection("ICorDebugHandleValue", "", null, null));
			
			return new VariableCollection("$debugInfo", ((CorElementType)corValue.Type).ToString(), items.ToArray(), null);
		}
		*/
	}
}
