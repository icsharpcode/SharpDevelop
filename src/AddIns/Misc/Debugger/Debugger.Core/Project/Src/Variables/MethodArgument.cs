// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class MethodArgument: Variable
	{
		int index;
		
		public int Index {
			get {
				return index;
			}
		}
		
		public MethodArgument(string name, int index, Value @value)
			:base (name, @value)
		{
			this.index = index;
		}
	}
}
