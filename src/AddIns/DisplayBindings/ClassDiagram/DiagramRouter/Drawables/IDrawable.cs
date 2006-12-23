// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Itai Bar-Haim" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace Tools.Diagrams.Drawables
{
	public interface IDrawable
	{
		void DrawToGraphics (Graphics graphics);
	}
}
