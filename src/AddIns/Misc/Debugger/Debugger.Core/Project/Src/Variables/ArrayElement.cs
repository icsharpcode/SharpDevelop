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
	public class ArrayElement: Variable
	{
		uint[] indicies;
		
		public uint[] Indicies {
			get { return indicies; }
		}
		
		public ArrayElement(string name, uint[] indicies, Value @value)
			:base (name, @value)
		{
			this.indicies = indicies;
		}
	}
}
