// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;
using System.Reflection;

namespace Debugger.MetaData
{
	public partial class DebugType
	{
		private bool primitiveTypeCached;
		private System.Type primitiveType;
		
		/// <summary> Returns simple managed type coresponding to the primitive type. </summary>
		[Tests.Ignore]
		public System.Type PrimitiveType {
			get {
				if (!primitiveTypeCached) {
					primitiveTypeCached = true;
					primitiveType = GetPrimitiveType();
				}
				return primitiveType;
			}
		}
		
		/// <summary> Returns simple managed type coresponding to the primitive type. </summary>
		private System.Type GetPrimitiveType()
		{
			if (corElementType == CorElementType.VALUETYPE) {
				CorElementType corType;
				try {
					corType = TypeNameToCorElementType(this.FullName);
				} catch (DebuggerException) {
					return null;
				}
				return CorElementTypeToManagedType(corType);
			} else {
				return CorElementTypeToManagedType(corElementType);
			}
		}
		
		internal static Type CorElementTypeToManagedType(CorElementType corElementType)
		{
			switch(corElementType) {
					case CorElementType.BOOLEAN: return typeof(System.Boolean);
					case CorElementType.CHAR:    return typeof(System.Char);
					case CorElementType.I1:      return typeof(System.SByte);
					case CorElementType.U1:      return typeof(System.Byte);
					case CorElementType.I2:      return typeof(System.Int16);
					case CorElementType.U2:      return typeof(System.UInt16);
					case CorElementType.I4:      return typeof(System.Int32);
					case CorElementType.U4:      return typeof(System.UInt32);
					case CorElementType.I8:      return typeof(System.Int64);
					case CorElementType.U8:      return typeof(System.UInt64);
					case CorElementType.R4:      return typeof(System.Single);
					case CorElementType.R8:      return typeof(System.Double);
					case CorElementType.I:       return typeof(System.IntPtr);
					case CorElementType.U:       return typeof(System.UIntPtr);
					case CorElementType.STRING:  return typeof(System.String);
					default: return null;
			}
		}
		
		internal static CorElementType TypeNameToCorElementType(string fullname)
		{
			switch (fullname) {
					case "System.Boolean": return CorElementType.BOOLEAN;
					case "System.Char":    return CorElementType.CHAR;
					case "System.SByte":   return CorElementType.I1;
					case "System.Byte":    return CorElementType.U1;
					case "System.Int16":   return CorElementType.I2;
					case "System.UInt16":  return CorElementType.U2;
					case "System.Int32":   return CorElementType.I4;
					case "System.UInt32":  return CorElementType.U4;
					case "System.Int64":   return CorElementType.I8;
					case "System.UInt64":  return CorElementType.U8;
					case "System.Single":  return CorElementType.R4;
					case "System.Double":  return CorElementType.R8;
					case "System.IntPtr":  return CorElementType.I;
					case "System.UIntPtr": return CorElementType.U;
					case "System.String":  return CorElementType.STRING;
					default: throw new DebuggerException("Not a primitive type");
			}
		}
		
		/*
		 * Find the super class manually - unused since we have ICorDebugType.GetBase() in .NET 2.0
		 * 
		protected static ICorDebugClass GetSuperClass(Process process, ICorDebugClass currClass)
		{
			Module currModule = process.GetModule(currClass.Module);
			uint superToken = currModule.MetaData.GetTypeDefProps(currClass.Token).SuperClassToken;
			
			// It has no base class
			if ((superToken & 0x00FFFFFF) == 0x00000000) return null;
			
			// TypeDef - Localy defined
			if ((superToken & 0xFF000000) == 0x02000000) {
				return currModule.CorModule.GetClassFromToken(superToken);
			}
			
			// TypeSpec - generic class whith 'which'
			if ((superToken & 0xFF000000) == 0x1B000000) {
				// Walkaround - fake 'object' type
				string fullTypeName = "System.Object";
				
				foreach (Module superModule in process.Modules) {
					try	{
						uint token = superModule.MetaData.FindTypeDefByName(fullTypeName, 0).Token;
						return superModule.CorModule.GetClassFromToken(token);
					} catch {
						continue;
					}
				}
			}
			
			// TypeRef - Referencing to external assembly
			if ((superToken & 0xFF000000) == 0x01000000) {
				string fullTypeName = currModule.MetaData.GetTypeRefProps(superToken).Name;
				
				foreach (Module superModule in process.Modules) {
					// TODO: Does not work for nested
					// TODO: preservesig
					try	{
						uint token = superModule.MetaData.FindTypeDefByName(fullTypeName, 0).Token;
						return superModule.CorModule.GetClassFromToken(token);
					} catch {
						continue;
					}
				}
			}
			
			// TODO: Can also be TypeSpec = 0x1b000000
			
			throw new DebuggerException("Superclass not found");
		}
		 */
	}
}
