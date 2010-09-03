// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
