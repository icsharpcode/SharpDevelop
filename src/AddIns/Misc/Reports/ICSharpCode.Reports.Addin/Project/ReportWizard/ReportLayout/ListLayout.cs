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
		
		ReportItemCollection reportItems;
		
		
		public ListLayout(ReportModel reportModel,ReportItemCollection reportItemCollection):base(reportModel)
		{
			this.reportItems = reportItemCollection;
			ICSharpCode.Reports.Core.BaseRowItem row = new ICSharpCode.Reports.Core.BaseRowItem();
			AdjustContainer(base.ReportModel.DetailSection,row);
			base.ParentItem = row;
		}
		
		
		#region overrides
		
		
		public override void CreatePageHeader()
		{
			base.CreatePageHeader();
			ICSharpCode.Reports.Core.BaseRowItem row = CreateRowWithTextColumns(base.ReportModel.PageHeader,
			                                                                    this.reportItems);
			AdjustContainer(ParentItem,row);
			base.ReportModel.PageHeader.Items.Add(row);
		}
		
		
		public override void CreateDataSection (ICSharpCode.Reports.Core.BaseSection section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			
			if (base.ReportModel.ReportSettings.GroupColumnsCollection.Count > 0)
			{
				
				var groupheader = base.CreateGroupHeader(new Point (GlobalValues.ControlMargins.Left,GlobalValues.ControlMargins.Top));
				
				base.ReportModel.DetailSection.Items.Add(groupheader);
				
				ParentItem.Location = new Point(ParentItem.Location.X,50);
				ParentItem.Size = new Size(ParentItem.Size.Width,40);
				section.Size = new Size(section.Size.Width,90);
			}
			
			var items = base.AddItemsToContainer(this.reportItems);
			ParentItem.Items.AddRange(items);
			ParentItem.Size = new Size(ParentItem.Size.Width,items[0].Size.Height + GlobalValues.ControlMargins.Top + GlobalValues.ControlMargins.Bottom);
			section.Items.Add(ParentItem as BaseReportItem);
		}
		
		#endregion
	}
}
