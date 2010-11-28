// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
//		private bool pageBreakAfter;
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
					//return InnerFind(itemName);
					return FindRec(this.items,itemName);
				}
			}
			return null;
		}
		
		#endregion
		
		#region properties
		
		public  int SectionMargin {get;set;}
	
		
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
