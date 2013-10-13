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
using ICSharpCode.Reporting.Expressions;
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
		public DataPageBuilder(IReportModel reportModel, Type elementType,IEnumerable list):base(reportModel)
		{
			List = list;
			ElementType = elementType;
		}
		
		
		public override void BuildExportList()
		{
			base.BuildExportList();
			BuildDetail();
			base.BuildReportFooter();
			base.AddPage(CurrentPage);
			UpdatePageInfo();
			RunExpressions(ReportModel.ReportSettings);
		}
		
		void BuildDetail()
		{
			var rows = new List<IExportContainer>();
			var converter = new ContainerConverter(base.Graphics, CurrentLocation);

			var position = DetailStart;
			var collectionSource = new CollectionSource(List,ElementType,ReportModel.ReportSettings);
			CurrentSection = ReportModel.DetailSection;
			
			if(collectionSource.Count > 0) {
				collectionSource.Bind();
				CurrentLocation = DetailStart;
				
				do {
					var row = CreateContainerForSection(position);
					row.Parent = CurrentPage;
					collectionSource.Fill(CurrentSection.Items);
					var convertedItems = converter.CreateConvertedList(ReportModel.DetailSection.Items);
					converter.SetParent(row,convertedItems);
					MeasureAndArrangeContainer(row);

					if (PageFull(row)) {
						InsertRange(rows);
						MeasureAndArrangeContainer(row);
						rows.Clear();
						Pages.Add(CurrentPage);
						MeasureAndArrangeContainer(row);

						position = ResetPosition();
						CurrentPage = CreateNewPage();
						WriteStandardSections();
						CurrentLocation = DetailStart;
						
						position = DetailStart;
						row.Location = position;
					}

					row.ExportedItems.AddRange(convertedItems);
					rows.Add(row);
					position = new Point(CurrentSection.Location.X,position.Y + row.DesiredSize.Height + 1);
				}
				
				while (collectionSource.MoveNext());
				InsertRange(rows);
			}
		}
		
		/*
		void BuildDetail_2()
		{
			var converter = new ContainerConverter(base.Graphics, CurrentLocation);
//			var position = ResetPosition();
			var position = DetailStart;
			var collectionSource = new CollectionSource(List,ElementType,ReportModel.ReportSettings);
			CurrentSection = ReportModel.DetailSection;
			
			IExportContainer detail = null;
			
			CurrentSection = ReportModel.DetailSection;

			if(collectionSource.Count > 0) {
				collectionSource.Bind();
				CurrentLocation = DetailStart;
				
//				detail = CreateContainerForSection(DetailStart);
//				detail.DesiredSize = new Size(detail.Size.Width,DetailEnds.Y - DetailStart.Y);

//				detail.Parent = CurrentPage;
				detail = CreateDetailSection(DetailStart);
				detail.Parent = CurrentPage;
				InsertDetailAtPosition(detail);
				do {
//					detail = CreateContainerForSection(position);
					var row = CreateContainerForSection(position);
					row.Parent = detail;
					collectionSource.Fill(CurrentSection.Items);
					var convertedItems = converter.CreateConvertedList(ReportModel.DetailSection.Items,position);
					converter.SetParent(row,convertedItems);
					if (PageFull(convertedItems)) {
//						detail.ExportedItems.AddRange(convertedItems);
//						CurrentPage.ExportedItems.Insert(2,detail);
						row.ExportedItems.AddRange(convertedItems);
						MeasureAndArrangeContainer(row);
						detail.ExportedItems.Add(row);
						Pages.Add(CurrentPage);
//						MeasureAndArrangeContainer(detail);

						position = ResetPosition();
						CurrentPage = CreateNewPage();
						WriteStandardSections();
						CurrentLocation = DetailStart;
//						detail = CreateContainerForSection(DetailStart);
						
					} else {
//						detail.ExportedItems.AddRange(convertedItems);
						row.ExportedItems.AddRange(convertedItems);
						MeasureAndArrangeContainer(row);
						detail.ExportedItems.Add(row);
//						InsertDetailAtPosition(detail);
//						position = new Point(CurrentSection.Location.Y,position.Y + CurrentSection.Size.Height + 5);
						position = new Point(CurrentSection.Location.X,position.Y + row.DesiredSize.Height + 5);
					}
				}
				
				while (collectionSource.MoveNext());
				InsertDetailAtPosition(detail);
				
			} else {
				detail = CreateContainerForSection(DetailStart);
				InsertDetailAtPosition(detail);
			}
		}
		*/
		
		/*
		void BuildDetail_1()
		{
			var converter = new ContainerConverter(base.Graphics, CurrentLocation);
			var position = ResetPosition();
			var collectionSource = new CollectionSource(List,ElementType,ReportModel.ReportSettings);
			CurrentSection = ReportModel.DetailSection;
			
			IExportContainer detail = null;
			
			CurrentSection = ReportModel.DetailSection;

			if(collectionSource.Count > 0) {
				collectionSource.Bind();
				CurrentLocation = DetailStart;
				
				detail = CreateContainerForSection(DetailStart);
//				detail.DesiredSize = new Size(detail.Size.Width,DetailEnds.Y - DetailStart.Y);

				detail.Parent = CurrentPage;
				
				do {
					collectionSource.Fill(CurrentSection.Items);
					var convertedItems = converter.CreateConvertedList(ReportModel.DetailSection.Items,position);
					converter.SetParent(detail,convertedItems);
					if (PageFull(convertedItems)) {
						detail.ExportedItems.AddRange(convertedItems);
						CurrentPage.ExportedItems.Insert(2,detail);
						Pages.Add(CurrentPage);
						MeasureAndArrangeContainer(detail);

						position = ResetPosition();
						CurrentPage = CreateNewPage();
						WriteStandardSections();
						CurrentLocation = DetailStart;
						detail = CreateContainerForSection(DetailStart);
						
					} else {
						detail.ExportedItems.AddRange(convertedItems);
						MeasureAndArrangeContainer(detail);
//						position = new Point(CurrentSection.Location.Y,position.Y + CurrentSection.Size.Height + 5);
						position = new Point(CurrentSection.Location.X,position.Y + detail.DesiredSize.Height + 5);
					}
				}
				
				while (collectionSource.MoveNext());
				InsertDetailAtPosition(detail);
				
			} else {
				detail = CreateContainerForSection(DetailStart);
				InsertDetailAtPosition(detail);
			}
		}
		*/
		
		
		/*
		void row_BuildDetail()
		{
			var converter = new ContainerConverter(base.Graphics, CurrentLocation);
			var position = ResetPosition();
			var collectionSource = new CollectionSource(List,ElementType,ReportModel.ReportSettings);
			CurrentSection = ReportModel.DetailSection;
			
			IExportContainer detail = null;
			if(collectionSource.Count > 0) {
				collectionSource.Bind();
				
				detail = CreateDetail(DetailStart);

				do {
					collectionSource.Fill(CurrentSection.Items);
					
					var row = CreateAndArrangeContainer(converter,position,detail);
					Console.WriteLine("position {0}",position);
					if (row_PageFull(row)) {
						InsertDetailAtPosition(detail);
						Pages.Add(CurrentPage);
						CurrentPage = CreateNewPage();
						WriteStandardSections();
						position = ResetPosition();
						detail = CreateDetail(DetailStart);
						CurrentLocation = DetailStart;
						row = CreateAndArrangeContainer(converter,position,detail);
					}
					
					detail.ExportedItems.Add(row);
					position = new Point(CurrentSection.Location.Y,position.Y + CurrentSection.Size.Height);
				}
				while (collectionSource.MoveNext());
				
				InsertDetailAtPosition(detail);
			} else {
				detail = CreateContainerForSection(DetailStart);
				InsertDetailAtPosition(detail);
				base.BuildReportFooter();
			}
		}
		*/
	
	
		/*
		IExportContainer CreateContainerIfNotExist(IReportContainer container, IExportContainer parent, Point position)
		{
			var isContainer = container.Items[0] is IReportContainer;
			if (!isContainer) {
				var row = CreateContainerForSection(position);
				row.Name = "Row";
				row.Parent = parent;
				row.Location = new Point(50, position.Y);
				row.Size = new Size(400, container.Items[0].Size.Height + 4);
				row.BackColor = Color.Green;
				return row;
			}
			return CreateContainerForSection(container.Items[0].Location);
		}

		
		IExportContainer CreateDetailSection(Point startLocation)
		{
			var detail = CreateContainerForSection(startLocation);
			detail.Parent = CurrentPage;
			detail.DesiredSize = new Size(detail.Size.Width,DetailEnds.Y - DetailStart.Y);
			return detail;
		}
*/
		
		
		Point ResetPosition () {
			return new Point(DetailStart.X,1);
		}
		
		
		void MeasureAndArrangeContainer(IExportContainer container)
		{
			container.DesiredSize = MeasureElement(container);
			ArrangeContainer(container);
		}

		
		IExportContainer CreateContainerForSection(Point location )
		{
			var detail = (ExportContainer)CurrentSection.CreateExportColumn();
			detail.Location = location;
			return detail;
		}
		
		
		void InsertRange(List<IExportContainer> list)
		{
			if (Pages.Count == 0) {
				CurrentPage.ExportedItems.InsertRange(2, list);
			} else {
				CurrentPage.ExportedItems.InsertRange(1, list);
			}
		}
		
		
		void InsertDetailAtPosition(IExportContainer container)
		{
			if (Pages.Count == 0) {
				CurrentPage.ExportedItems.Insert(2, container);
			} else {
				CurrentPage.ExportedItems.Insert(1, container);
			}
		}
		
		
		internal IReportContainer CurrentSection { get; private set; }
		
		public IEnumerable List {get; private set;}
		
		public Type ElementType {get;private set;}
	}
}
