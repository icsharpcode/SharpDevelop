// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
