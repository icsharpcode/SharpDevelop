// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>
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
			
			Point p = new Point (5,5);
			if (base.ReportModel.ReportSettings.GroupColumnsCollection.Count > 0) {
				p = InsertGroupHeader();
				AdjustParent(section);
			}
			
			if (base.ParentItem != null) {
				base.AddItemsToContainer(base.ReportModel.DetailSection,this.reportItems,p);
			}
			else{
				AddItemsToSection (base.ReportModel.DetailSection,this.reportItems);
			}
			
		}
		
		
		void AdjustParent(ICSharpCode.Reports.Core.BaseSection section)
		{
			base.ParentItem.Size = new Size(base.ParentItem.Size.Width,base.ParentItem.Size.Height + 30);
			section.Size = new Size(section.Size.Width,section.Size.Height + 30);
		}
		
		
		private Point InsertGroupHeader()
		{
			ICSharpCode.Reports.Core.BaseDataItem dataItem = new ICSharpCode.Reports.Core.BaseDataItem();
			dataItem.ColumnName = base.ReportModel.ReportSettings.GroupColumnsCollection[0].ColumnName;
			dataItem.DataType = base.ReportModel.ReportSettings.GroupColumnsCollection[0].DataTypeName;
			dataItem.Location = new Point (10,5);
			dataItem.Size = new Size (150,20);
			dataItem.Text = base.ReportModel.ReportSettings.GroupColumnsCollection[0].ColumnName;
			
			ICSharpCode.Reports.Core.BaseGroupedRow groupHeader = new ICSharpCode.Reports.Core.BaseGroupedRow();
			groupHeader.Location = new Point(10,10);
			groupHeader.Size = new Size (300,30);
			groupHeader.Items.Add(dataItem);
			base.ReportModel.DetailSection.Items.Add(groupHeader);
			return new Point (5,45);
		}
		
		#endregion
	}
}
