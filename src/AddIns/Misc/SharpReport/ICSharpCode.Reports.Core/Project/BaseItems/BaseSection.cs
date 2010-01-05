// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Xml.Serialization;

/// <summary>
/// This Class is the BaseClass for <see cref="ReportSection"></see>
/// </summary>


namespace ICSharpCode.Reports.Core
{
	public class BaseSection : BaseReportItem {
		
		private int sectionMargin;
		private bool pageBreakAfter;
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
		
		#region properties
		
		
		public  int SectionMargin 
		{
			get {
				return this.sectionMargin;
			}
			set {
				this.sectionMargin = value;
			}
		}
	
		
		public override System.Drawing.Point Location
		{
			get { return base.Location; }
			set { base.Location = value; }
		}
		
	
		public ReportItemCollection Items
		{
			get {
				if (this.items == null) {
					items = new ReportItemCollection();
				}
				return items;
			}
		}
		
	
		public virtual bool PageBreakAfter 
		{
			get {
				return pageBreakAfter;
			}
			set {
				pageBreakAfter = value;
			}
		}
		
		
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

