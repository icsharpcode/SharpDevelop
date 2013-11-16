/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.06.2013
 * Time: 20:27
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Exporter.Visitors;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Interfaces.Export;
using ICSharpCode.Reporting.PageBuilder.Converter;
using ICSharpCode.Reporting.PageBuilder.ExportColumns;

namespace ICSharpCode.Reporting.PageBuilder
{
	/// <summary>
	/// Description of DataPageBuilder.
	/// </summary>
	public class DataPageBuilder:BasePageBuilder
	{
		
		public DataPageBuilder(IReportModel reportModel,IEnumerable list):base(reportModel)
		{
			List = list;
		}
		
		
		public override void BuildExportList()
		{
			CreateDataSource();
			base.BuildExportList();
			BuildDetail();
			BuildReportFooter();
			AddPage(CurrentPage);
			UpdatePageInfo();
			RunExpressions(ReportModel.ReportSettings,DataSource);
			
			var formatVisitor = new FormatVisitor();
			formatVisitor.Run(Pages);

		}
		
		
		void BuildDetail()
		{
			var exportRows = new List<IExportContainer>();
			var converter = new ContainerConverter(base.Graphics, CurrentLocation);

			var position = DetailStart;
			
			CurrentSection = ReportModel.DetailSection;
			
			if(DataSourceContainsData()) {
				CurrentLocation = DetailStart;
				
				do {
					var row = CreateContainerForSection(CurrentPage,position);

					DataSource.Fill(CurrentSection.Items);
					
					var convertedItems = converter.CreateConvertedList(ReportModel.DetailSection.Items);
					
					converter.SetParent(row,convertedItems);
					
					MeasureAndArrangeContainer(row);

					if (PageFull(row)) {
						InsertExportRows(exportRows);
						MeasureAndArrangeContainer(row);
						exportRows.Clear();
//						ExpressionVisitor.Visit(CurrentPage);
						CurrentPage.PageInfo.PageNumber = Pages.Count + 1;
						Pages.Add(CurrentPage);

						position = ResetPosition();
						CurrentPage = CreateNewPage();
						WriteStandardSections();
						CurrentLocation = DetailStart;
						
						position = DetailStart;
						row.Location = position;
					}

					row.ExportedItems.AddRange(convertedItems);
//					ExpressionVisitor.Visit(row as ExportContainer);
					exportRows.Add(row);
					position = new Point(CurrentSection.Location.X,position.Y + row.DesiredSize.Height + 1);
				}
				
				while (DataSource.MoveNext());
				InsertExportRows(exportRows);
			}
		}

		
		void CreateDataSource()
		{
			DataSource = new CollectionDataSource(List, ReportModel.ReportSettings);
			if (DataSourceContainsData()) {
				DataSource.Bind();
			}
		}
		
		
		bool DataSourceContainsData () {
			if (DataSource.Count > 0) {
				return true;
			}
			return false;
		}
		

		Point ResetPosition () {
			return new Point(DetailStart.X,1);
		}
		
		
		void MeasureAndArrangeContainer(IExportContainer container)
		{
			container.DesiredSize = MeasureElement(container);
			ArrangeContainer(container);
		}

		
		ExportContainer CreateContainerForSection(ExportPage parent,Point location )
		{
			var detail = (ExportContainer)CurrentSection.CreateExportColumn();
			detail.Location = location;
			detail.Parent = parent;
			return detail;
		}
		
		
		void InsertExportRows(List<IExportContainer> list)
		{
			if (Pages.Count == 0) {
				CurrentPage.ExportedItems.InsertRange(2, list);
			} else {
				CurrentPage.ExportedItems.InsertRange(1, list);
			}
		}
		
		
		internal CollectionDataSource DataSource {get; private set;}
		
		internal IEnumerable List {get; private set;}
		
		protected IReportContainer CurrentSection { get; private set; }
		
	}
}
