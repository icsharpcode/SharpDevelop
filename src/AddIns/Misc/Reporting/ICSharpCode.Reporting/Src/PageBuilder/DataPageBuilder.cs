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
using ICSharpCode.Reporting.Items;
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
			var dv = new DebugVisitor();
			dv.Run(Pages);
		}
		
		
		void BuildDetail()
		{
			CurrentSection = ReportModel.DetailSection;
			if(DataSourceContainsData()) {
				CurrentLocation = DetailStart;
				var converter = new ContainerConverter(DetailStart);
				if (IsGrouped()) {
					BuildGroupedDetails(converter,DetailStart);
				} else {
					BuildSortedDetails(converter,DetailStart);
				}
			}
		}


		void BuildGroupedDetails (IContainerConverter converter,Point startPosition) {
			var pagePosition = startPosition;
			var sectionPosition = pagePosition;
			
			foreach (IGrouping<object, object> grouping in DataSource.GroupedList) {
				
				var groupHeader = (BaseRowItem)CurrentSection.Items.Where(p => p.GetType() == typeof(GroupHeader)).FirstOrDefault();
				var sectionContainer = CreateContainerForSection(CurrentPage, pagePosition);
				
				DataSource.Fill(groupHeader.Items,grouping.FirstOrDefault());
				
				var headerRow = converter.ConvertToExportContainer(groupHeader);
				headerRow.Location = new Point(headerRow.Location.X,groupHeader.Location.Y);
				
				var headerItems = converter.CreateConvertedList(groupHeader.Items);
				converter.SetParent(sectionContainer, headerItems);
				
				headerRow.ExportedItems.AddRange(headerItems);
				
				sectionContainer.ExportedItems.Add(headerRow);
				
				pagePosition = new Point(CurrentSection.Location.X, pagePosition.Y + sectionContainer.DesiredSize.Height + 1);
				
				// Set Position Child Elements
				
				sectionPosition = new Point(pagePosition.X,headerRow.Location.Y + headerRow.Size.Height + 3);
				
				//Childs
				foreach (var child in grouping) {
					var dataItems = CurrentSection.Items.Where(p => p.GetType() == typeof(BaseDataItem)).ToList();
					List<IExportColumn> convertedItems = FillAndConvert(sectionContainer, child, dataItems, converter);
					
					AdjustLocationInSection(sectionPosition,  convertedItems);
					
					sectionContainer.ExportedItems.AddRange(convertedItems);
					MeasureAndArrangeContainer(sectionContainer);
					
					if (PageFull(sectionContainer)) {
						PerformPageBreak();
						InsertContainer(sectionContainer);
						pagePosition = DetailStart;
						sectionContainer.Location = DetailStart;
					}
					
					sectionPosition = new Point(CurrentSection.Location.X, sectionPosition.Y + convertedItems[0].DisplayRectangle.Size.Height + 5);
					sectionContainer.Size = new Size(sectionContainer.Size.Width,sectionContainer.Size.Height + convertedItems[0].Size.Height);
				}
				MeasureAndArrangeContainer(sectionContainer);
				InsertContainer(sectionContainer);
				pagePosition = new Point(pagePosition.X,sectionContainer.DisplayRectangle.Bottom + 1);
			}
		}

		
		void BuildSortedDetails(IContainerConverter converter,Point startPosition){
			
			var exportRows = new List<IExportContainer>();
			var pagePosition = startPosition;
			foreach (var element in DataSource.SortedList) {
				var sectionContainer = CreateContainerForSection(CurrentPage, pagePosition);
				var convertedItems = FillAndConvert(sectionContainer,element,ReportModel.DetailSection.Items,converter);
			
				sectionContainer.ExportedItems.AddRange(convertedItems);
				MeasureAndArrangeContainer(sectionContainer);
				
				if (PageFull(sectionContainer)) {
					InsertExportRows(exportRows);
					exportRows.Clear();
					PerformPageBreak();
					pagePosition = DetailStart;
					sectionContainer.Location = pagePosition;
				}

//				sectionContainer.ExportedItems.AddRange(convertedItems);
//				MeasureAndArrangeContainer(sectionContainer);
				
				exportRows.Add(sectionContainer);
				pagePosition = new Point(CurrentSection.Location.X, pagePosition.Y + sectionContainer.DesiredSize.Height + 1);
			}

			InsertExportRows(exportRows);
		}
		
		
		void PerformPageBreak(){
			CurrentPage.PageInfo.PageNumber = Pages.Count + 1;
			Pages.Add(CurrentPage);
			CurrentPage = CreateNewPage();
			WriteStandardSections();
			CurrentLocation = DetailStart;
		}

		
		bool IsGrouped(){
			return DataSource.OrderGroup == OrderGroup.Grouped;
		}

		
		List<IExportColumn> FillAndConvert(ExportContainer parent, object current, List<IPrintableObject> dataItems, IContainerConverter converter)
		{
			DataSource.Fill(dataItems, current);
			var convertedItems = converter.CreateConvertedList(dataItems.ToList());
			converter.SetParent(parent, convertedItems);
			return convertedItems;
		}
		
		
		void CreateDataSource(){
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
		
		
		static void AdjustLocationInSection(Point sectionPosition,List<IExportColumn> convertedItems)
		{
			foreach (var element in convertedItems) {
				element.Location = new Point(element.Location.X, sectionPosition.Y);
			}
		}
		
		
		void MeasureAndArrangeContainer(IExportContainer container){
			container.DesiredSize = MeasureElement(container);
			ArrangeContainer(container);
		}

		
		ExportContainer CreateContainerForSection(ExportPage parent,Point location ){
			var detail = (ExportContainer)CurrentSection.CreateExportColumn();
			detail.Location = location;
			detail.Parent = parent;
			return detail;
		}
	
		
		void InsertContainer(ExportContainer sectionContainer)
		{
			if (Pages.Count == 0) {
				CurrentPage.ExportedItems.Insert(2, sectionContainer);
			} else {
				CurrentPage.ExportedItems.Insert(1, sectionContainer);
			}
		}
		
		
		void InsertExportRows(List<IExportContainer> list){
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
