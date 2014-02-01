// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
