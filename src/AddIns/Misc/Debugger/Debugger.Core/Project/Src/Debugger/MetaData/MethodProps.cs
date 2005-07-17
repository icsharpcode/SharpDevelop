using System;
using System.Collections.Generic;
using System.Text;

namespace DebuggerLibrary
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

		public bool HasSpecialName {
			get {
				return (Flags & (uint)CorMethodAttr.mdSpecialName) != 0;
			}
		}
	}
}
