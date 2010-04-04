// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Collections.Generic;
using System.Linq;
using System;

namespace Debugger.AddIn.Visualizers.Graph.SplineRouting
{
	/// <summary>
	/// Description of EdgeStartEnd.
	/// </summary>
	public class EdgeStartEnd
	{
		public IRect From { get; set; }
		public IRect To { get; set; }
		
		public override bool Equals(object obj)
		{
			var other = obj as EdgeStartEnd;
			if (other == null)
				return false;
			return this.From == other.From && this.To == other.To;
		}
		
		public override int GetHashCode()
		{
			return this.From.GetHashCode() ^ this.To.GetHashCode();
		}
	}
}
