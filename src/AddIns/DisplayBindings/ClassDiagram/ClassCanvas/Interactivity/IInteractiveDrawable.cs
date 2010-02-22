// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Itai Bar-Haim"/>
//     <version>$Revision$</version>
// </file>

using System;

using Tools.Diagrams;
using Tools.Diagrams.Drawables;

namespace ClassDiagram
{
	public interface IInteractiveDrawable : IDrawable, IHitTestable, IMouseInteractable
	{
	}

	public interface IInteractiveRectangle : IDrawableRectangle, IHitTestable, IMouseInteractable
	{
	}
}
