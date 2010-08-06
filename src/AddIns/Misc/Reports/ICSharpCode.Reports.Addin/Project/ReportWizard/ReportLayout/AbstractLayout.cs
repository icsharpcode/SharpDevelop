/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 30.05.2008
 * Zeit: 16:33
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	
	/// <summary>
	/// Description of AbstractLayout.
	/// </summary>
	public class AbstractLayout
	{

		ISimpleContainer parentItem;
		
		public AbstractLayout(ReportModel reportModel)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			this.ReportModel = reportModel;
		}
		
		
		public virtual void CreateReportHeader ()
		{
			string header = this.ReportModel.ReportSettings.ReportName;
			var textItem = WizardHelper.CreateTextItem(header);
			textItem.Name = header;
			this.FixLayout(this.ReportModel.ReportHeader,textItem,GlobalEnums.ItemsLayout.Center);
			this.ReportModel.ReportHeader.Items.Add(textItem);
		}
		
		public virtual void SetParent (ISimpleContainer parentItem)
		{
			this.parentItem = parentItem;
		}
		
		public virtual void CreatePageHeader ()
		{
		}
		
		public virtual void CreateDataSection (ICSharpCode.Reports.Core.BaseSection section)
		{
		}
		
		
		public virtual void CreatePageFooter ()
		{
			var textItem = WizardHelper.CreateTextItem("=Globals!PageNumber");
			textItem.Name = "PageNumber1";
			this.FixLayout(this.ReportModel.PageFooter,textItem,GlobalEnums.ItemsLayout.Right);
			this.ReportModel.PageFooter.Items.Add(textItem);
		}
		
		
		private void FixLayout (ICSharpCode.Reports.Core.BaseSection section,
		                        BaseReportItem item,GlobalEnums.ItemsLayout layout)
		{
			item.Size = new Size (GlobalValues.PreferedSize.Width,GlobalValues.PreferedSize.Height);
			Point p =Point.Empty;
			
			switch (layout) {
				case GlobalEnums.ItemsLayout.Left:
					p = new Point (this.ReportModel.ReportSettings.LeftMargin,
					               GlobalValues.ControlMargins.Top);
					break;
				case GlobalEnums.ItemsLayout.Center:
					p = new Point((section.Size.Width / 2) - (item.Size.Width /2),
					              GlobalValues.ControlMargins.Top);
					break;
				case GlobalEnums.ItemsLayout.Right:
					p = new Point(section.Size.Width  - item.Size.Width - GlobalValues.ControlMargins.Right,
					              GlobalValues.ControlMargins.Top);
					break;
				default:
					break;
			}
			item.Location = p;
		}
		
		
		protected static void AdjustContainer (ICSharpCode.Reports.Core.BaseReportItem parent,
		                                       ICSharpCode.Reports.Core.BaseReportItem item)
		{
			item.Size = new Size (parent.Size.Width - GlobalValues.ControlMargins.Left - GlobalValues.ControlMargins.Right,
			                      parent.Size.Height - GlobalValues.ControlMargins.Top - GlobalValues.ControlMargins.Bottom);
			
			item.Location = new Point(GlobalValues.ControlMargins.Left,
			                          GlobalValues.ControlMargins.Top);
			item.Parent = parent;
		}
		
		
		#region HeaderColumns
		
		protected static ICSharpCode.Reports.Core.BaseRowItem CreateRowWithTextColumns(BaseReportItem parent,ReportItemCollection items)
		{
			ReportItemCollection colDetail = AbstractLayout.HeaderColumnsFromReportItems(items);
			
			ICSharpCode.Reports.Core.BaseRowItem row = new ICSharpCode.Reports.Core.BaseRowItem();
			AdjustContainer(parent,row);
			
			int defY = parent.Location.Y + GlobalValues.ControlMargins.Top;
			int defX = row.Size.Width / colDetail.Count;
			int startX = parent.Location.X + GlobalValues.ControlMargins.Left;
			foreach (ICSharpCode.Reports.Core.BaseTextItem ir in colDetail) {
				Point np = new Point(startX,defY);
				startX += defX;
				ir.Location = np;
				ir.Parent = row;
				row.Items.Add(ir);
			}
			return row;
		}
		
		
		private static ReportItemCollection HeaderColumnsFromReportItems(ReportItemCollection reportItemCollection)
		{
			if (reportItemCollection == null) {
				throw new ArgumentNullException ("reportItemCollection");
			}
			
			ReportItemCollection itemCol = new ReportItemCollection();
			ICSharpCode.Reports.Core.BaseTextItem rItem;
			int i = 1;
			foreach (IDataRenderer dataRenderer in reportItemCollection) {
				rItem = WizardHelper.CreateTextItem(dataRenderer.ColumnName);
				rItem.Name = rItem.Text + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
				itemCol.Add(rItem);
				i ++;
			}
			return itemCol;
		}
		
		#endregion
		
		protected static void AddItemsToSection (ICSharpCode.Reports.Core.BaseSection section,ReportItemCollection items)
		{
			int defY = section.Location.Y + GlobalValues.ControlMargins.Top;
			int defX = section.Size.Width / items.Count;
			int startX = section.Location.X + GlobalValues.ControlMargins.Left;
			
			foreach (var ir in items) {
				Point np = new Point(startX,defY);
				startX += defX;
				ir.Location = np;
				section.Items.Add(ir);
			}
		}
		
		
		protected  void AddItemsToContainer (ICSharpCode.Reports.Core.BaseSection section,ReportItemCollection items)
		{
			section.Items.Add(this.parentItem as BaseReportItem);
			ICSharpCode.Reports.Core.BaseReportItem bri = this.parentItem as ICSharpCode.Reports.Core.BaseReportItem;
			
			int defY = bri.Location.Y + GlobalValues.ControlMargins.Top;
			int defX = bri.Size.Width / items.Count;
			int startX = bri.Location.X + GlobalValues.ControlMargins.Left;
			
			foreach (var ir in items) {
				Point np = new Point(startX,defY);
				startX += defX;
				ir.Location = np;
				this.parentItem.Items.Add(ir);
			}
		}
		
		
		protected ReportModel ReportModel {get; private set;}
	
		
		protected ISimpleContainer ParentItem {
			get { return parentItem; }
		}
		
	}
}
