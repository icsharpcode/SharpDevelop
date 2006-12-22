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
	public struct MethodProps
	{
		public uint Token;
		public string Name;
		public uint ClassToken;
		public uint Flags;
		public uint ImplFlags;
		public uint CodeRVA;
		
		public bool IsStatic {
			get {
				return (Flags & (uint)CorMethodAttr.mdStatic) != 0;
			}
		}
		
		public bool IsPublic {
			get {
				return (Flags & (uint)CorMethodAttr.mdPublic) != 0;
			}
		}
		
		public bool HasSpecialName {
			get {
				return (Flags & (uint)CorMethodAttr.mdSpecialName) != 0;
			}
		}
	}
}

#pragma warning restore 1591
