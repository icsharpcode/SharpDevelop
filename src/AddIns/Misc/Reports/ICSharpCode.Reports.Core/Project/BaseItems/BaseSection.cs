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
using System.Linq;
using System.Xml.Serialization;

using ICSharpCode.Reports.Core.Interfaces;

/// <summary>
/// This Class is the BaseClass for <see cref="ReportSection"></see>
/// </summary>


namespace ICSharpCode.Reports.Core
{
	public class BaseSection : BaseReportItem,ISimpleContainer
	{
		
		private ReportItemCollection items;
		
		public event EventHandler<SectionEventArgs> SectionPrinting;
		public event EventHandler<SectionEventArgs> SectionPrinted;
		
		
		#region Constructors
		
		public BaseSection(): base()
		{
			base.Name = String.Empty;
		}
		
		public BaseSection (string sectionName) :base()
		{
			base.Name = sectionName;
		}
		
		#endregion
		
		#region Rendering
		
		public override void Render(ReportPageEventArgs rpea)
		{
			this.NotifyPrinting();
			base.Render(rpea);
			this.NotifyPrinted();
		}
		
		
		protected override Rectangle DisplayRectangle {
			get { 
					return new Rectangle(Location.X , this.Location.Y ,
					                     this.Size.Width,this.Size.Height);
			}
		}
		
		
		private void NotifyPrinting () 
		{
			if (this.SectionPrinting != null) {
				SectionEventArgs ea = new SectionEventArgs (this);
				SectionPrinting (this,ea);
			} 
		}
		
		
		private void NotifyPrinted () 
		{
			if (this.SectionPrinted != null) {
				SectionEventArgs ea = new SectionEventArgs (this);
				SectionPrinted (this,ea);
			}
		}
		
		#endregion
		
		#region FindItem
		
		private BaseReportItem FindRec (ReportItemCollection items, string name)
		{
			foreach(BaseReportItem item in items)
			{
				ISimpleContainer cont = item as ISimpleContainer;
				if (cont != null) {
					return FindRec(cont.Items,name);
				} else {
					var query = from bt in items where bt.Name == name select bt;
					if (query.Count() >0) {
						return query.FirstOrDefault();
					}
				}
			}
			return null;
		}
		
		
		
		public BaseReportItem FindItem (string itemName)
		{
			foreach (BaseReportItem item in items)
			{
				ISimpleContainer cont = item as ISimpleContainer;
				if (cont != null) {
					return FindRec (cont.Items,itemName);
				} else {
					return FindRec(this.items,itemName);
				}
			}
			return null;
		}
		
		#endregion
		
		#region properties
		
		public  int SectionMargin {get;set;}
	
//		public virtual int SectionOffset {get;set;}
		
		
		public ReportItemCollection Items
		{
			get {
				if (this.items == null) {
					items = new ReportItemCollection();
				}
				return items;
			}
		}
		
	
		public virtual bool PageBreakAfter {get;set;}
	
		#endregion

		
		public Size MeasureOverride (Size availableSize)
		{
			Size resultSize = new Size(0,0);
//			Console.WriteLine("MeasureOverride");
			foreach (var item in Items) {
//				Console.WriteLine("{0} - {1}",item.Location,item.Size);
				resultSize.Width = Math.Max(resultSize.Width, item.Size.Width);
				resultSize.Height = Math.Max(resultSize.Height, item.Size.Height);
			}

//			resultSize.Width = double.IsPositiveInfinity(availableSize.Width) ?
//				resultSize.Width : availableSize.Width;
//
//			resultSize.Height = double.IsPositiveInfinity(availableSize.Height) ?
//				resultSize.Height : availableSize.Height;
			
			resultSize.Width = double.IsPositiveInfinity(availableSize.Width) ?
				resultSize.Width : availableSize.Width;
			var b = double.IsPositiveInfinity(availableSize.Height);
			resultSize.Height = double.IsPositiveInfinity(availableSize.Height) ?
				resultSize.Height : availableSize.Height;
			
			return resultSize;
		}
		
		#region System.IDisposable interface implementation
		
		protected override void Dispose(bool disposing)
		{
			try {
				if (disposing){
					if (this.items != null) {
						this.items.Clear();
						this.items = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		
		#endregion
	}
}
