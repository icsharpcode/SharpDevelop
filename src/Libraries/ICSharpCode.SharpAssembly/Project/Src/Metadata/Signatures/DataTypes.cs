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
	public enum DataType
	{
		End            = 0x00,
		Void           = 0x01,
		Boolean        = 0x02,
		Char           = 0x03,
		SByte          = 0x04,
		Byte           = 0x05,
		Int16          = 0x06,
		UInt16         = 0x07,
		Int32          = 0x08,
		UInt32         = 0x09,
		Int64          = 0x0A,
		UInt64         = 0x0B,
		Single         = 0x0C,
		Double         = 0x0D,
		
		String         = 0x0E,
		Ptr            = 0x0F,
		ByRef          = 0x10,
		ValueType      = 0x11,
		Class          = 0x12,
		Array          = 0x14,
		
		TypeReference  = 0x16,
		IntPtr         = 0x18,
		UIntPtr        = 0x19,
		FnPtr          = 0x1B,
		Object         = 0x1C,
		SZArray        = 0x1D,
		
		CModReq        = 0x1F,
		CModOpt        = 0x20,
		Internal       = 0x21,
		
		Modifier       = 0x40,
		Sentinel       = 0x41,
		Pinned         = 0x45
		
	}
}
