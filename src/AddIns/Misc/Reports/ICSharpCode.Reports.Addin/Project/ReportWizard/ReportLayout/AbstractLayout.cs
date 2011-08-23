// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	
	/// <summary>
	/// Description of AbstractLayout.
	/// </summary>
	public class AbstractLayout
	{

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
		
		
		protected static void AdjustContainer (ISimpleContainer parent,
		                                       ICSharpCode.Reports.Core.BaseReportItem item)
		{
			item.Size = new Size (parent.Size.Width - GlobalValues.ControlMargins.Left - GlobalValues.ControlMargins.Right,
			                      parent.Size.Height - GlobalValues.ControlMargins.Top - GlobalValues.ControlMargins.Bottom);
			
			item.Location = new Point(GlobalValues.ControlMargins.Left,
			                          GlobalValues.ControlMargins.Top);
			item.Parent = parent as BaseReportItem;
		}
		
		
		#region HeaderColumns
		
		protected ICSharpCode.Reports.Core.BaseRowItem CreateRowWithTextColumns(ISimpleContainer parent)
		{
			ReportItemCollection colDetail = AbstractLayout.HeaderColumnsFromReportItems(ReportItems);
			
			ICSharpCode.Reports.Core.BaseRowItem row = new ICSharpCode.Reports.Core.BaseRowItem();
			AdjustContainer(parent,row);

			int defY =  GlobalValues.ControlMargins.Top;
			int ctrlWidth = CalculateControlWidth(row,colDetail);
			int startX = parent.Location.X + GlobalValues.ControlMargins.Left;
			
			foreach (ICSharpCode.Reports.Core.BaseTextItem ir in colDetail) {
				Point np = new Point(startX,defY);
				startX += ctrlWidth;
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
			foreach (IDataItem dataRenderer in reportItemCollection) {
//			foreach (IDataRenderer dataRenderer in reportItemCollection) {
				rItem = WizardHelper.CreateTextItem(dataRenderer.ColumnName);
				rItem.Name = rItem.Text + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
				itemCol.Add(rItem);
				i ++;
			}
			return itemCol;
		}
		
		#endregion
		
		
		protected ReportItemCollection AddItemsToContainer ()
		{
			int locationX = GlobalValues.ControlMargins.Left;
			
			var minCtrlWidth = CalculateControlWidth(Container,ReportItems);
			
			var col = new ReportItemCollection();
			
			foreach (var ir in ReportItems) {
				ir.Location = new Point(locationX,GlobalValues.ControlMargins.Top);
				col.Add(ir);
				locationX += minCtrlWidth;
			}
			return col;
		}
		
		
		protected static int CalculateControlWidth(ISimpleContainer row, ReportItemCollection colDetail)
		{
			return row.Size.Width / colDetail.Count;
		}
		
		
		#region Grouping
		
		protected ICSharpCode.Reports.Core.GroupHeader CreateGroupHeader(Point headerLocation)
		{
			ICSharpCode.Reports.Core.BaseDataItem dataItem = new ICSharpCode.Reports.Core.BaseDataItem();
			
			dataItem.ColumnName = ReportModel.ReportSettings.GroupColumnsCollection[0].ColumnName;
			
			dataItem.DataType = ReportModel.ReportSettings.GroupColumnsCollection[0].DataTypeName;
			dataItem.Location = new Point (GlobalValues.ControlMargins.Left,GlobalValues.ControlMargins.Top);
			dataItem.Size = new Size (150,20);
			dataItem.Text = ReportModel.ReportSettings.GroupColumnsCollection[0].ColumnName;
			
			ICSharpCode.Reports.Core.GroupHeader groupHeader = new ICSharpCode.Reports.Core.GroupHeader();
			groupHeader.Location = headerLocation;
			groupHeader.Size = new Size (300,dataItem.Size.Height + GlobalValues.ControlMargins.Top + GlobalValues.ControlMargins.Bottom);
			groupHeader.Items.Add(dataItem);
			return groupHeader;
		}
		
		
		protected ICSharpCode.Reports.Core.GroupFooter CreateFooter (Point footerLocation)
		{
			ICSharpCode.Reports.Core.GroupFooter groupFooter = new ICSharpCode.Reports.Core.GroupFooter();
			groupFooter.Location = footerLocation;
			groupFooter.Size = new Size (300,30);
			return groupFooter;
		}
		
		
		protected void ConfigureDetails (Point detailLocation,Size detailSize)
		{
			Container.Location = detailLocation;
			Container.Size = detailSize;
		}
			
		#endregion
		
		protected ReportModel ReportModel {get; private set;}
		
		
		protected ISimpleContainer Container {get;set;}
		
		protected ReportItemCollection ReportItems {get;set;}
		
	}
}
