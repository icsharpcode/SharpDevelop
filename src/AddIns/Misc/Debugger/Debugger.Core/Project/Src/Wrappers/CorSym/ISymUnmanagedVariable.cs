// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorSym
{
	using System;
	using System.Runtime.InteropServices;
	
	public partial class ISymUnmanagedVariable
	{
		public string Name {
			get {
				return Util.GetString(GetName);
			}
		}
	}
}
