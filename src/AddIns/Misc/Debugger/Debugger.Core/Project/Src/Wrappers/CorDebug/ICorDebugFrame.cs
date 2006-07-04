// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Wrappers.CorDebug
{
	using System;
	using System.Collections.Generic;
	
	
	public partial class ICorDebugFrame
	{
		uint index;
		
		public uint Index {
			get {
				return index;
			}
			set {
				index = value;
			}
		}
	}
}
