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
	
		public GraphicStyleDecorator(BaseShape shape):base()
		{
			if (shape == null) {
				throw new ArgumentNullException("shape");
			}
			this.Shape = shape;
			Thickness = 1;
			DashStyle = DashStyle.Solid;
		}
		
		public BaseShape Shape {get;set;}
		
		public int Thickness {get;set;}
		
		public DashStyle DashStyle {get;set;}
			
	}
}
