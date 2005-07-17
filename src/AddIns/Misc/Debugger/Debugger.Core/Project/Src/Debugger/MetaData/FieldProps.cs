using System;
using System.Collections.Generic;
using System.Text;

namespace DebuggerLibrary
{
	struct FieldProps
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

		public bool IsLiteral {
			get {
				return (Flags & (uint)ClassFieldAttribute.fdLiteral) != 0;
			}
		}
	}
}
