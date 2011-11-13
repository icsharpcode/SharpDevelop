// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Serialization;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Core {
	/// <summary>
	/// Baseclass for all Graphical Items	
	/// </summary>
	/// <remarks>
	/// 	created by - Forstmeier Peter
	/// 	created on - 31.08.2005 13:24:59
	/// </remarks>
	public class BaseGraphicItem : BaseReportItem {
		
		public BaseGraphicItem():base()
		{
			this.Thickness = 1;
			DashStyle = DashStyle.Solid;
		}
		
		
		protected IGraphicStyleDecorator CreateItemStyle (BaseShape shape)
		{
			IGraphicStyleDecorator style = new GraphicStyleDecorator(shape);
			
			style.Size = this.Size;
			style.Location = this.Location;
			style.BackColor = this.BackColor;
			style.ForeColor = this.ForeColor;
			style.FrameColor = this.FrameColor;
			
			style.Thickness = this.Thickness;
			style.DashStyle = this.DashStyle;
			return style;
		}
		
		protected static SizeF MeasureReportItem (IReportItem item) {
			if (item == null) {
				throw new ArgumentNullException("item");
			}
			return new SizeF (item.Size.Width,item.Size.Height);
		}
		
		protected BaseLine Baseline()
		{
			if (this.BackColor == GlobalValues.DefaultBackColor) {
				return new BaseLine (this.ForeColor,this.DashStyle,this.Thickness);
			} else {
				return new BaseLine (this.BackColor,this.DashStyle,this.Thickness);
			}
		}
		
		
		#region Overrides
		
		public override string ToString()
		{
			return "BaseGraphicItem";
		}
		
		#endregion
		
		
		#region property's
		/// <summary>
		/// Line Thickness of graphical Element
		/// </summary>
		
		public virtual int Thickness {get;set;}
		
		public virtual DashStyle DashStyle {get;set;}	
		
		#endregion
	}
}
