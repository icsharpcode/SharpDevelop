/*
 * Created by SharpDevelop.
 * User: itai
 * Date: 28/09/2006
 * Time: 19:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ClassDiagram
{
		
	public abstract class SmallIconShape : VectorShape
	{
		public override float ShapeWidth
		{
			get { return 10.0f; }
		}
		
		public override float ShapeHeight
		{
			get { return 10.0f; }
		}
	}
}
