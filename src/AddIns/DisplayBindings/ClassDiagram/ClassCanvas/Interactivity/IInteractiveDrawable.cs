/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:03
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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
