// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="David Srbecký" email="dsrbecky@post.cz"/>
//     <version>$Revision$</version>
// </file>

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
