// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Itai Bar-Haim"/>
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
