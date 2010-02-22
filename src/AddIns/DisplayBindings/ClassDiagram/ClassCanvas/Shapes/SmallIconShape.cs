// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Itai Bar-Haim"/>
//     <version>$Revision$</version>
// </file>

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
