// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 2023 $</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	/// <summary>
	/// Binding flags specify which members should be returned.
	/// <para> Use 'or' operation to combine flags. </para>
	/// </summary>
	[Flags]
	public enum BindingFlags {
		/// Return instance (ie non-static members) members
		Instance = 1,
		/// Return static members
		Static = 2,
		/// Return public members
		Public = 4,
		/// Return members which are not public
		NonPublic = 8,
		/// Return all members
		All = Instance | Static | Public | NonPublic
	};
}
