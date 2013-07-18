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
			CurrentPage = CreateNewPage ();
			WriteStandardSections();
			CurrentLocation = DetailStart;
			BuildDetail();
			base.AddPage(CurrentPage);
		}
		
		
		void aaBuildDetail()
		{
			
			CurrentSection = ReportModel.DetailSection;
			var collectionSource = new CollectionSource(List,ElementType,ReportModel.ReportSettings);
			IExportContainer detail = null;
			if(collectionSource.Count > 0) {
				collectionSource.Bind();
				CurrentLocation = DetailStart;
				
				var position = ResetPosition();
				var converter = new ContainerConverter(base.Graphics, CurrentLocation);
//				var converter = new ContainerConverter(base.Graphics, position);
				detail = CreateContainerForSection(DetailStart);

				
				do {
					collectionSource.Fill(CurrentSection.Items);
					var convertedItems = converter.CreateConvertedList(ReportModel.DetailSection,detail,position);
					if (old_PageFull(convertedItems)) {
						detail.ExportedItems.AddRange(convertedItems);
						CurrentPage.ExportedItems.Insert(2,detail);
						Pages.Add(CurrentPage);
						MeasureAndArrangeContainer(converter,detail);

						position = ResetPosition();
						CurrentPage = CreateNewPage();
						WriteStandardSections();
						CurrentLocation = DetailStart;
						detail = CreateContainerForSection(DetailStart);
						
					} else {
						detail.ExportedItems.AddRange(convertedItems);
						MeasureAndArrangeContainer(converter,detail);
						position = new Point(CurrentSection.Location.Y,position.Y + CurrentSection.Size.Height);
					}
				}
				
				while (collectionSource.MoveNext());
				InsertDetailAtPosition(detail);
				base.BuildReportFooter();
				
			} else {
				detail = CreateContainerForSection(DetailStart);
				InsertDetailAtPosition(detail);
				base.BuildReportFooter();
			}
		}
		
		
		void aa_1_BuildDetail()
		{
			
			CurrentSection = ReportModel.DetailSection;
			var collectionSource = new CollectionSource(List,ElementType,ReportModel.ReportSettings);
			IExportContainer detail = null;
			if(collectionSource.Count > 0) {
				collectionSource.Bind();

				var position = DetailStart;
				var converter = new ContainerConverter(base.Graphics, CurrentLocation);
				detail = CreateDetail(DetailStart);

				do {
					
					var row = CreateContainerIfNotExist(CurrentSection,detail, position);
					collectionSource.Fill(CurrentSection.Items);
					
//var convertedItems	 =  converter.CreateConvertedList(ReportModel.DetailSection,row);
					var convertedItems	 =  converter.CreateConvertedList(ReportModel.DetailSection,row,position);

					MeasureAndArrangeContainer(converter,row);
					row.ExportedItems.AddRange(convertedItems);
					/*
					var rr = new Rectangle(row.Location,row.DesiredSize);
					
					if (rr.Bottom >DetailEnds.Y) {
						Console.WriteLine("new pagebreak {0} - {1}",rr.ToString(),DetailEnds.Y);
						InsertDetailAtPosition(detail);
						Pages.Add(CurrentPage);
						CurrentPage = CreateNewPage();
						WriteStandardSections();
						position = ResetPosition();
						detail = CreateDetail(DetailStart);
						CurrentLocation = DetailStart;
						
						row = CreateContainerIfNotExist(CurrentSection,detail,position);
//						var recreate =  converter.CreateConvertedList(ReportModel.DetailSection,row,position);
						var recreate =  converter.CreateConvertedList(ReportModel.DetailSection,row);
						MeasureAndArrangeContainer(converter,row);
						row.ExportedItems.AddRange(recreate);
					}
					*/
				
					if (old_PageFull(convertedItems)) {
						InsertDetailAtPosition(detail);
						Pages.Add(CurrentPage);
						CurrentPage = CreateNewPage();
						WriteStandardSections();
						position = ResetPosition();
						detail = CreateDetail(DetailStart);
						CurrentLocation = DetailStart;
						
						row = CreateContainerIfNotExist(CurrentSection,detail,position);
						var recreate =  converter.CreateConvertedList(ReportModel.DetailSection,row,position);
//						var recreate =  converter.CreateConvertedList(ReportModel.DetailSection,row);
						MeasureAndArrangeContainer(converter,row);
						row.ExportedItems.AddRange(recreate);
					}
					
					detail.ExportedItems.Add(row);
					position = new Point(CurrentSection.Location.Y,position.Y + CurrentSection.Size.Height);
				}
				
				while (collectionSource.MoveNext());
				InsertDetailAtPosition(detail);
//				base.BuildReportFooter();
				
			} else {
				detail = CreateContainerForSection(DetailStart);
				InsertDetailAtPosition(detail);
				base.BuildReportFooter();
			}
		}

	
		void BuildDetail()
		{
			var converter = new ContainerConverter(base.Graphics, CurrentLocation);
			var position = DetailStart;
			CurrentSection = ReportModel.DetailSection;
			var collectionSource = new CollectionSource(List,ElementType,ReportModel.ReportSettings);
			IExportContainer detail = null;
			if(collectionSource.Count > 0) {
				collectionSource.Bind();
				
				
				detail = CreateDetail(DetailStart);

				do {
					
					var row = CreateContainerIfNotExist(CurrentSection,detail, position);
					collectionSource.Fill(CurrentSection.Items);
					
					var convertedItems	 =  converter.CreateConvertedList(ReportModel.DetailSection,row);

					MeasureAndArrangeContainer(converter,row);
					row.ExportedItems.AddRange(convertedItems);
					
					if (PageFull(row)) {
						InsertDetailAtPosition(detail);
						Pages.Add(CurrentPage);
						CurrentPage = CreateNewPage();
						WriteStandardSections();
						position = ResetPosition();
						detail = CreateDetail(DetailStart);
						CurrentLocation = DetailStart;
						
						row = CreateContainerIfNotExist(CurrentSection,detail,position);
						var recreate =  converter.CreateConvertedList(ReportModel.DetailSection,row);
						MeasureAndArrangeContainer(converter,row);
						row.ExportedItems.AddRange(recreate);
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
		
		
		IExportContainer CreateContainerIfNotExist(IReportContainer container, IExportContainer parent, Point position)
		{
			var isContainer = container.Items[0] is IReportContainer;
			if (!isContainer) {
				var row = CreateContainerForSection(position);
				row.Name = "Row";
				row.Parent = parent;
				row.Location = new Point(50, position.Y);
				row.Size = new Size(400, 40);
				row.BackColor = Color.Green;
				return row;
			}
			return CreateContainerForSection(container.Items[0].Location);
		}

		
		IExportContainer CreateDetail(Point startLocation)
		{
			var detail = CreateContainerForSection(startLocation);
			detail.Parent = CurrentPage;
			return detail;
		}

		
		
		Point ResetPosition () {
			return DetailStart;
		}
		
		
		void MeasureAndArrangeContainer(IContainerConverter converter,IExportContainer container)
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
		
		
		void InsertDetailAtPosition(IExportContainer container)
		{
			if (Pages.Count == 0) {
				CurrentPage.ExportedItems.Insert(2, container);
			} else {
				CurrentPage.ExportedItems.Insert(1, container);
			}
		}
		
		
//		internal IReportContainer Container { get; private set; }
		internal IReportContainer CurrentSection { get; private set; }
		
		public IEnumerable List {get; private set;}
		
		public Type ElementType {get;private set;}
	}
}
