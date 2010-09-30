// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of GraphicStyleDecorator.
	/// </summary>
	public interface IGraphicStyleDecorator :IBaseStyleDecorator
	{
		BaseShape Shape {get;set;}
		int Thickness { get;set;}
		DashStyle DashStyle {get;set;}
	}
		
	public class GraphicStyleDecorator:BaseStyleDecorator,IGraphicStyleDecorator
	{
		private BaseShape shape;
		private int thickness = 1;
		private DashStyle dashStyle = DashStyle.Solid;
	
		public GraphicStyleDecorator(BaseShape shape):base()
		{
			if (shape == null) {
				throw new ArgumentNullException("shape");
			}
			this.shape = shape;
		}
		
		
		public BaseShape Shape {
			get { return shape; }
			set { shape = value; }
		}
		
		public int Thickness {
			get { return thickness; }
			set { thickness = value; }
		}
		
		public DashStyle DashStyle {
			get { return dashStyle; }
			set { dashStyle = value; }
		}
	}
}
