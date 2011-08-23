// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Interfaces;

/// <summary>
///This class drwas a Circle
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 29.09.2005 11:54:19
/// </remarks>
namespace ICSharpCode.Reports.Core {	
	
	public class BaseCircleItem : BaseGraphicItem, IExportColumnBuilder,ISimpleContainer
	{
		private ReportItemCollection items;
		EllipseShape shape = new EllipseShape();
		
		#region Constructor
		
		public BaseCircleItem():base() 
		{
		}
		
		#endregion
		
		#region IExportColumnBuilder
		
		public IBaseExportColumn CreateExportColumn(){
			IGraphicStyleDecorator style = base.CreateItemStyle(this.shape);
			return new ExportGraphicContainer(style);
		}
	
		
		#endregion
		
		public override void Render(ReportPageEventArgs rpea) 
		{
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			base.Render (rpea);
			Rectangle rect = base.DisplayRectangle;
			
			shape.FillShape(rpea.PrintPageEventArgs.Graphics,
			                new SolidFillPattern(this.BackColor),
			                rect);
			
			shape.DrawShape (rpea.PrintPageEventArgs.Graphics,
			                 base.Baseline(),
			                 rect);
		}
		
			
		public ReportItemCollection Items {
			get {
				if (this.items == null) {
					this.items = new ReportItemCollection();
				}
				return this.items;
			}
		}
	}
}
