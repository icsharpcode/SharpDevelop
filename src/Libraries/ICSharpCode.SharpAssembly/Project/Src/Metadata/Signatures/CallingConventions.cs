// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

namespace ICSharpCode.SharpAssembly.Metadata
{
	public enum CallingConvention : uint
	{
		Default      = 0x00,
		
		Cdecl        = 0x01,
		Stdcall      = 0x02,
		Thiscall     = 0x03,
		Fastcall     = 0x04,
		
		VarArg       = 0x05,
		Field        = 0x06,
		LocalSig     = 0x07,
		Property     = 0x08,
		UnMngd       = 0x09,
		
		HasThis      = 0x20,
		ExplicitThis = 0x40
	}
}
