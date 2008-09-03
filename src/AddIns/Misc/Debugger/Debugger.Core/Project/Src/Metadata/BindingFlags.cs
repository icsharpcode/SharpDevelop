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

namespace Debugger.MetaData
{
	/// <summary>
	/// Binding flags specify which members should be returned.
	/// <para> Use 'or' operation to combine flags. </para>
	/// </summary>
	[Flags]
	public enum BindingFlags: uint {
		/// Return all members
		All = 0xFFFFFFFF,
		AllInThisType = 0xFFFF,
		
		AccessMask = 0x0F,
		Public     = 0x01,
		NonPublic  = 0x02,
		
		InstanceStaticMask = 0xF0,
		Instance           = 0x10,
		Static             = 0x20,
		
		TypeMask    = 0x0F00,
		Field       = 0x0100,
		Property    = 0x0200,
		Method      = 0x0400,
		GetProperty = 0x0800,
		
		IncludeSuperType = 0x10000
	};
}
