// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;

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
			base.Container = row;
		}
		
		
		#region overrides
		
		
		public override void CreatePageHeader()
		{
			base.CreatePageHeader();
			ICSharpCode.Reports.Core.BaseRowItem row = CreateRowWithTextColumns(base.ReportModel.PageHeader);
			                                                                   
			AdjustContainer(Container,row);
			base.ReportModel.PageHeader.Items.Add(row);
		}
		
		
		public override void CreateDataSection (ICSharpCode.Reports.Core.BaseSection section)
		{
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			Size detailSize = Size.Empty;
			Size itemSize = Size.Empty;
			Point rowLoction = Point.Empty;
			if (base.ReportModel.ReportSettings.GroupColumnsCollection.Count > 0)
			{
				
				var groupheader = base.CreateGroupHeader(new Point (GlobalValues.ControlMargins.Left,GlobalValues.ControlMargins.Top));
				base.ReportModel.DetailSection.Items.Add(groupheader);
				
				// Detail
				itemSize = CreateDetail();
				detailSize = new Size(Container.Size.Width,itemSize.Height  + GlobalValues.ControlMargins.Top + GlobalValues.ControlMargins.Bottom);

				
				// GroupFooter
				var groupFooter = base.CreateFooter(new Point(GlobalValues.ControlMargins.Left,80));
				base.ReportModel.DetailSection.Items.Add(groupFooter);
				section.Size = new Size(section.Size.Width,125);
				rowLoction = new Point (Container.Location.X,45);
			}
			else
			{
				itemSize = CreateDetail();
				detailSize = new Size(Container.Size.Width,itemSize.Height + GlobalValues.ControlMargins.Top + GlobalValues.ControlMargins.Bottom);
				section.Size = new Size(section.Size.Width,Container.Size.Height + GlobalValues.ControlMargins.Top + GlobalValues.ControlMargins.Bottom);
				rowLoction = new Point(Container.Location.X,GlobalValues.ControlMargins.Top);
			}
			base.ConfigureDetails (rowLoction,detailSize);
			section.Items.Add(Container as BaseReportItem);
		}
		
		
		Size CreateDetail()
		{
			var items = base.CreateItemsCollection();
			Container.Items.AddRange(items);
			return items[0].Size;
		}
		
		#endregion
	}
}
