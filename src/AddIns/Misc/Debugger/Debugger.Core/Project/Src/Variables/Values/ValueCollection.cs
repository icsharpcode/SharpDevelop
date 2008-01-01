// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

namespace Debugger
{
	/// <summary>
	/// An enumerable collection of values
	/// </summary>
	public class ValueCollection: List<Value>
	{
		public static ValueCollection Empty = new ValueCollection(new Value[0]);
		
		public ValueCollection(IEnumerable<Value> values):base(values)
		{
			
		}
	}
}
