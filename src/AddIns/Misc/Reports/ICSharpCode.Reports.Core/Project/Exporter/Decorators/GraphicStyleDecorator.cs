// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

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
