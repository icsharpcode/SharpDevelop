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
			
			if (base.ParentItem != null) {
				base.AddItemsToContainer(base.ReportModel.DetailSection,this.reportItems);
			}
			else{
				AddItemsToSection (base.ReportModel.DetailSection,this.reportItems);
			}
		}
		
		#endregion
	}
}
