// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger.AddIn.Visualizers.Graph.Layout
{
	/// <summary>
	/// Generates sequential ids, usefull for node and edge ids.
	/// </summary>
	public class IdGenerator
	{
		int currentId = 0;
		
		public IdGenerator()
		{
		}
		
		public int GetNextId()
		{
			return currentId++;
		}
	}
}
