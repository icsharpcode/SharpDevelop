// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorSym
{
	using System;
	
	
	public partial class ISymUnmanagedDocument
	{
		public string URL {
			get {
				return Util.GetString(GetURL, 0);
			}
		}
	}
}
