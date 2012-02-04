// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace ICSharpCode.Reports.Core.Exporter
{
	public class LineDecorator : GraphicStyleDecorator, ILineDecorator
	{
		public LineDecorator(BaseShape shape) : base(shape)
		{
		}

		public Point From { get; set; }
		public Point To { get; set; }
	}
}
