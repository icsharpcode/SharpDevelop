using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ICSharpCode.NRefactory.TypeSystem;
using Debugger.Interop.CorDebug;

namespace Debugger
{
	public static class Util
	{
		// Type-safe box because ConditionalWeakTable requires reference type
		class Box<T>
		{
			public T Value; 
			
			public static implicit operator Box<T>(T value)
		    {
		        return new Box<T> { Value = value };
		    }
		}
		
		static ConditionalWeakTable<IUnresolvedMember, Box<bool>> hasStepOverAttribute = new ConditionalWeakTable<IUnresolvedMember, Box<bool>>();
				
		public static bool HasStepOverAttribute(this IMethod method)
		{
			return hasStepOverAttribute.GetValue(method.UnresolvedMember, delegate {
				string[] debuggerAttributes = {
					typeof(System.Diagnostics.DebuggerStepThroughAttribute).FullName,
					typeof(System.Diagnostics.DebuggerNonUserCodeAttribute).FullName,
					typeof(System.Diagnostics.DebuggerHiddenAttribute).FullName
				};
				if (method.Attributes.Any(a => debuggerAttributes.Contains(a.AttributeType.FullName)))
					return true;
				if (method.DeclaringType.GetDefinition().Attributes.Any(a => debuggerAttributes.Contains(a.AttributeType.FullName)))
					return true;
				return false;
			}).Value;
		}
		
		static ConditionalWeakTable<IUnresolvedMember, Box<uint>> backingFieldToken = new ConditionalWeakTable<IUnresolvedMember, Box<uint>>();
		
		/// <summary> Is this method in form 'return this.field;'? </summary>
		public static uint GetBackingFieldToken(this IMethod method)
		{
			if (method.IsVirtual || method.Parameters.Count != 0) return 0;
			return backingFieldToken.GetValue(method.UnresolvedMember, delegate {
				ICorDebugCode corCode;
				try {
					corCode = method.ToCorFunction().GetILCode();
				} catch (System.Runtime.InteropServices.COMException) {
					return 0;
				}
				
				if (corCode == null || corCode.IsIL() == 0 || corCode.GetSize() > 12) {
					return 0;
				}
				
				List<byte> code = new List<byte>(corCode.GetCode());
				
				uint token = 0;
				bool success =
					(Read(code, 0x00) || true) &&                     // nop || nothing
					(Read(code, 0x02, 0x7B) || Read(code, 0x7E)) &&   // ldarg.0; ldfld || ldsfld
					 ReadToken(code, ref token) &&                    //   <field token>
					(Read(code, 0x0A, 0x2B, 0x00, 0x06) || true) &&   // stloc.0; br.s; offset+00; ldloc.0 || nothing
					 Read(code, 0x2A);                                // ret
				
				if (!success) {
					return 0;
				}
				
				return token;
			}).Value;
		}
		
		// Read expected sequence of bytes
		static bool Read(List<byte> code, params byte[] expected)
		{
			if (code.Count < expected.Length)
				return false;
			for(int i = 0; i < expected.Length; i++) {
				if (code[i] != expected[i])
					return false;
			}
			code.RemoveRange(0, expected.Length);
			return true;
		}
		
		// Read field token
		static bool ReadToken(List<byte> code, ref uint token)
		{
			if (code.Count < 4)
				return false;
			if (code[3] != 0x04) // field token
				return false;
			token = ((uint)code[0]) + ((uint)code[1] << 8) + ((uint)code[2] << 16) + ((uint)code[3] << 24);
			code.RemoveRange(0, 4);
			return true;
		}
	}
}
