/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 20:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace Tools.Diagrams.Drawables
{
	public interface IDrawable
	{
		void DrawToGraphics (Graphics graphics);
	}
}
