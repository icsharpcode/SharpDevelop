// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 1591

using System;

namespace Debugger.Wrappers.MetaData
{
	public struct FieldProps
	{
		public uint Token;
		public string Name;
		public uint ClassToken;
		public uint Flags;
		
		public bool IsStatic {
			get {
				return (Flags & (uint)ClassFieldAttribute.fdStatic) != 0;
			}
		}
		
		public bool IsPublic {
			get {
				return (Flags & (uint)ClassFieldAttribute.fdPublic) != 0;
			}
		}
		
		public bool IsLiteral {
			get {
				return (Flags & (uint)ClassFieldAttribute.fdLiteral) != 0;
			}
		}
	}
}

#pragma warning restore 1591
