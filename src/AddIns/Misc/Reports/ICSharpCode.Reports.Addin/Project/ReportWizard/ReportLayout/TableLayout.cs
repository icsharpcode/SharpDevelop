// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of TableLayout.
	/// </summary>

	public class TableLayout: AbstractLayout
	{
		ReportItemCollection reportItems;
		
		
		public TableLayout(ReportModel reportModel,ReportItemCollection reportItemCollection):base(reportModel)
		{
			this.reportItems = reportItemCollection;
		}
		
	
		
		public override void CreatePageHeader()
		{
			base.CreatePageHeader();
			base.ReportModel.PageHeader.Size = new Size(base.ReportModel.PageHeader.Size.Width,10);
			base.ReportModel.PageHeader.BackColor = Color.LightGray;
		}
		
		
		public override void CreateDataSection(ICSharpCode.Reports.Core.BaseSection section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			System.Drawing.Printing.Margins margin = GlobalValues.ControlMargins;
			
			ICSharpCode.Reports.Core.BaseTableItem table = new ICSharpCode.Reports.Core.BaseTableItem();
			table.Name = "Table1";
			
			AdjustContainer(base.ReportModel.DetailSection,table);
			
			
			ICSharpCode.Reports.Core.BaseRowItem headerRow = CreateRowWithTextColumns(table,
			                                                                   this.reportItems);
			
			Point insertLocation =  new Point (margin.Left,headerRow.Location.Y + headerRow.Size.Height + margin.Bottom + margin.Top);
			
			
			if (base.ReportModel.ReportSettings.GroupColumnsCollection.Count > 0) {                 
				var groupHeader = base.CreateGroupHeader(insertLocation);
				table.Items.Add(groupHeader);
				insertLocation = new Point(margin.Left,insertLocation.Y + groupHeader.Size.Height + margin.Bottom + margin.Top);
			}
			
			
			ICSharpCode.Reports.Core.BaseRowItem detailRow = new ICSharpCode.Reports.Core.BaseRowItem();
			AdjustContainer (table,detailRow);
			
			
			detailRow.Location = insertLocation;

			int defX = detailRow.Size.Width / this.reportItems.Count;
			
			int startX = detailRow.Location.X + margin.Left;
			
			foreach (ICSharpCode.Reports.Core.BaseReportItem ir in this.reportItems)
			{
				Point np = new Point(startX,margin.Top);
				startX += defX;
				ir.Location = np;
				ir.Parent = detailRow;
				detailRow.Items.Add(ir);
			}
			
			insertLocation = new Point(margin.Left,insertLocation.Y + detailRow.Size.Height + margin.Bottom + margin.Top);
			
		
			table.Items.Add (headerRow);
			table.Items.Add (detailRow);
			
			table.Size = CalculateContainerSize(table,margin);

			section.Size = new Size (section.Size.Width,table.Size.Height + margin.Top + margin.Bottom);
			base.ReportModel.DetailSection.Items.Add(table);
		}
		
		
		private Size CalculateContainerSize(ISimpleContainer container,System.Drawing.Printing.Margins margin)
		{
			int h = margin.Top;
			
			foreach (ICSharpCode.Reports.Core.BaseReportItem item  in container.Items)
			{
				h = h + item.Size.Height + margin.Bottom;
			}
			h 	= h + 3*margin.Bottom;
			return new Size (container.Size.Width,h);
		}
	}
}
