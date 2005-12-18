// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace Debugger
{
	struct MethodProps
	{
		public uint Token;
		public string Name;
		public uint ClassToken;
		public uint Flags;
		public uint ImplFlags;
		public uint CodeRVA;
		public SignatureStream Signature;
		
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
