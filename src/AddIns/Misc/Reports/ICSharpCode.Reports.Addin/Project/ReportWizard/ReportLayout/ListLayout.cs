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
			base.SetParent(row);
		}
		
		
		#region overrides
		
		
		public override void CreatePageHeader()
		{
			base.CreatePageHeader();
			ICSharpCode.Reports.Core.BaseRowItem row = CreateRowWithTextColumns(base.ReportModel.PageHeader,
			                                                                    this.reportItems);
			base.ReportModel.PageHeader.Items.Add(row);
		}
		
		
		public override void CreateDataSection (ICSharpCode.Reports.Core.BaseSection section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
		
			if (base.ReportModel.ReportSettings.GroupColumnsCollection.Count > 0) {
				var groupheader = base.CreateGroupHeader(new Point (5,10));
				
				base.ReportModel.DetailSection.Items.Add(groupheader);
				
				ParentItem.Location = new Point(ParentItem.Location.X,50);
				ParentItem.Size = new Size(ParentItem.Size.Width,40);
				section.Size = new Size(section.Size.Width,100);
			}
			
			if (base.ParentItem != null) {
				base.AddItemsToContainer(base.ReportModel.DetailSection,this.reportItems);
				ParentItem.Size = new Size(ParentItem.Size.Width,40);
			}
			else{
				AddItemsToSection (base.ReportModel.DetailSection,this.reportItems);
			}
			
		}
		
		#endregion
	}
}
