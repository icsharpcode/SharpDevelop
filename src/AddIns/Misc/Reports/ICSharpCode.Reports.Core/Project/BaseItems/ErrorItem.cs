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
using System.Drawing;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of ErrorItem.
	/// </summary>
	public class ErrorItem:BaseTextItem,IExportColumnBuilder
	{
		string errMess;
		
		public ErrorItem():base()
		{
		}
		
		private void SetErrorLayout ()
		{
			base.DrawBorder = true;
			base.ForeColor = Color.Red;
			this.errMess = String.Format(System.Globalization.CultureInfo.CurrentCulture,
			                             "Error : <{0}> is missing or obsolete",base.Text);
		}
		
		
		public override void Render(ReportPageEventArgs rpea)
		{	
			if (rpea == null) {
				throw new ArgumentNullException("rpea");
			}
			
			this.SetErrorLayout();
			Border b = new Border(new BaseLine (this.ForeColor,System.Drawing.Drawing2D.DashStyle.Solid,1));
			base.DrawFrame(rpea.PrintPageEventArgs.Graphics,b);
			Print (rpea,this.errMess,base.DisplayRectangle);
		}
		
		#region IExportColumnBuilder  implementation
		
		public override IBaseExportColumn CreateExportColumn(){
			this.SetErrorLayout();
			TextStyleDecorator st = base.CreateItemStyle();
			ExportText item = new ExportText(st);
			item.Text = this.errMess;
			return item;
		}
		
		#endregion
	}
}
