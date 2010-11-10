// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using ICSharpCode.Reports.Core;

/// <summary>
/// This class build a TableLayout
/// </summary>

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	
	public class ListLayout : AbstractLayout
	{
		

		public ListLayout(ReportModel reportModel,ReportItemCollection reportItemCollection):base(reportModel)
		{
			base.ReportItems = reportItemCollection;
			ICSharpCode.Reports.Core.BaseRowItem row = new ICSharpCode.Reports.Core.BaseRowItem();
			AdjustContainer(base.ReportModel.DetailSection,row);
			base.ParentItem = row;
		}
		
		
		#region overrides
		
		
		public override void CreatePageHeader()
		{
			base.CreatePageHeader();
			ICSharpCode.Reports.Core.BaseRowItem row = CreateRowWithTextColumns(base.ReportModel.PageHeader);
			                                                                   
			AdjustContainer(ParentItem,row);
			base.ReportModel.PageHeader.Items.Add(row);
		}
		
		
		public override void CreateDataSection (ICSharpCode.Reports.Core.BaseSection section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			Size detailSize = Size.Empty;
			Size itemSize = Size.Empty;
			
			if (base.ReportModel.ReportSettings.GroupColumnsCollection.Count > 0)
			{
				
				var groupheader = base.CreateGroupHeader(new Point (GlobalValues.ControlMargins.Left,GlobalValues.ControlMargins.Top));
				base.ReportModel.DetailSection.Items.Add(groupheader);
				
				// Detail
				itemSize = CreateDetail();
				detailSize = new Size(ParentItem.Size.Width,itemSize.Height  + GlobalValues.ControlMargins.Top + GlobalValues.ControlMargins.Bottom);

				
				// GroupFooter
				var groupFooter = base.CreateFooter(new Point(GlobalValues.ControlMargins.Left,80));
				base.ReportModel.DetailSection.Items.Add(groupFooter);
				section.Size = new Size(section.Size.Width,125);
			}
			else
			{
				itemSize = CreateDetail();
				detailSize = new Size(ParentItem.Size.Width,itemSize.Height + GlobalValues.ControlMargins.Top + GlobalValues.ControlMargins.Bottom);
				section.Size = new Size(section.Size.Width,ParentItem.Size.Height + GlobalValues.ControlMargins.Top + GlobalValues.ControlMargins.Bottom);
			}
			base.ConfigureDetails ( new Point(ParentItem.Location.X,45),detailSize);
			section.Items.Add(ParentItem as BaseReportItem);
		}
		
		
		Size CreateDetail()
		{
			var items = base.AddItemsToContainer();
			ParentItem.Items.AddRange(items);
			return items[0].Size;
		}
		
		#endregion
	}
}
