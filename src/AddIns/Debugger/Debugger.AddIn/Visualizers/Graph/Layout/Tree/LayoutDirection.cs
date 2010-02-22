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
	/// Direction of tree layout.
	/// </summary>
	public enum LayoutDirection
	{
		[Display("Left to right")]
		LeftRight,
		[Display("Top to bottom")]
		TopBottom
	}
}
