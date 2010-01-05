/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 04.10.2008
 * Zeit: 19:29
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Drawing;
using ICSharpCode.Reports.Core;

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
			
			ICSharpCode.Reports.Core.BaseRowItem r1 = CreateRowWithTextColumns(table,
			                                                                   this.reportItems);
			
			
			ICSharpCode.Reports.Core.BaseRowItem r2 = new ICSharpCode.Reports.Core.BaseRowItem();
			
			AdjustContainer (table,r2);
						
			r2.Location = new Point (margin.Left,
			                         r1.Location.Y + r1.Size.Height + margin.Bottom + margin.Top);

			int defX = r2.Size.Width / this.reportItems.Count;
			
			int startX = r2.Location.X + margin.Left;
			
			foreach (ICSharpCode.Reports.Core.BaseReportItem ir in this.reportItems)
			{
				Point np = new Point(startX,margin.Top);
				startX += defX;
				ir.Location = np;
				ir.Parent = r2;
				r2.Items.Add(ir);
			}
			
			table.Size = new Size (table.Size.Width,
			                       margin.Top + r1.Size.Height + margin.Bottom + r2.Size.Height + margin.Bottom);
			section.Size = new Size (section.Size.Width,table.Size.Height + margin.Top + margin.Bottom);
			table.Items.Add (r1);
			table.Items.Add (r2);
			base.ReportModel.DetailSection.Items.Add(table);
		}
	}
}
