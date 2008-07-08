// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public partial class Value
	{
		internal static ICorDebugValue DereferenceUnbox(ICorDebugValue corValue)
		{
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
	}
}
