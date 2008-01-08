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

namespace Debugger
{
	public partial class DebugType
	{
		IList<T> FilterMemberInfo<T>(IList<T> input, string name) where T:MemberInfo
		{
			List<T> filtered = new List<T>();
			foreach(T memberInfo in input) {
				if (memberInfo.Name == name) {
					filtered.Add(memberInfo);
				}
			}
			return filtered.AsReadOnly();
		}
		
		IList<T> FilterMemberInfo<T>(IList<T> input, BindingFlags bindingFlags) where T:MemberInfo
		{
			List<T> filtered = new List<T>();
			foreach(T memberInfo in input) {
				// Filter by access
				if ((bindingFlags & (BindingFlags.Public | BindingFlags.NonPublic)) != 0) {
					if (memberInfo.IsPublic) {
						// If we do not want public members
						if ((bindingFlags & BindingFlags.Public) == 0) continue; // Reject item
					} else {
						if ((bindingFlags & BindingFlags.NonPublic) == 0) continue; // Reject item
					}
				}
				// Filter by static / instance
				if ((bindingFlags & (BindingFlags.Static | BindingFlags.Instance)) != 0) {
					if (memberInfo.IsStatic) {
						if ((bindingFlags & BindingFlags.Static) == 0) continue; // Reject item
					} else {
						if ((bindingFlags & BindingFlags.Instance) == 0) continue; // Reject item
					}
				}
				filtered.Add(memberInfo);
			}
			return filtered.AsReadOnly();
		}
		
		/// <summary>
		/// Returns simple managed type coresponding to the debug type.
		/// Any class yields System.Object
		/// </summary>
		public System.Type ManagedType {
			get {
				switch(this.corElementType) {
					case CorElementType.BOOLEAN: return typeof(System.Boolean);
					case CorElementType.CHAR: return typeof(System.Char);
					case CorElementType.I1: return typeof(System.SByte);
					case CorElementType.U1: return typeof(System.Byte);
					case CorElementType.I2: return typeof(System.Int16);
					case CorElementType.U2: return typeof(System.UInt16);
					case CorElementType.I4: return typeof(System.Int32);
					case CorElementType.U4: return typeof(System.UInt32);
					case CorElementType.I8: return typeof(System.Int64);
					case CorElementType.U8: return typeof(System.UInt64);
					case CorElementType.R4: return typeof(System.Single);
					case CorElementType.R8: return typeof(System.Double);
					case CorElementType.I: return typeof(int);
					case CorElementType.U: return typeof(uint);
					case CorElementType.SZARRAY:
					case CorElementType.ARRAY: return typeof(System.Array);
					case CorElementType.OBJECT: return typeof(System.Object);
					case CorElementType.STRING: return typeof(System.String);
					default: return null;
				}
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
