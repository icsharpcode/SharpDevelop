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
			BuildDetail();
			base.AddPage(CurrentPage);
		}
		
		
		void BuildDetail()
		{
			
			Container = ReportModel.DetailSection;
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
					collectionSource.Fill(Container.Items);
					var convertedItems = converter.CreateConvertedList(ReportModel.DetailSection,detail,position);
					if (PageFull(convertedItems)) {
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
						position = new Point(Container.Location.Y,position.Y + Container.Size.Height);
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

		Point ResetPosition () {
			return Point.Empty;
		}
		void MeasureAndArrangeContainer(ContainerConverter converter,IExportContainer detail)
		{
			converter.Measure(detail);
			converter.ArrangeContainer(detail);
		}

		
		IExportContainer CreateContainerForSection(Point location )
		{
			var detail = (ExportContainer)Container.CreateExportColumn();
			detail.Location = location;
			return detail;
		}
		
		
		bool PageFull(System.Collections.Generic.List<IExportColumn> columns)
		{
			var rect = new Rectangle(columns[0].Location,columns[0].Size);
			if (rect.Contains(new Point(100,500))) {
				return true;
			}
			return false;
		}
		
		
		void InsertDetailAtPosition(IExportContainer container)
		{
			if (Pages.Count == 0) {
				CurrentPage.ExportedItems.Insert(2, container);
			} else {
				CurrentPage.ExportedItems.Insert(1, container);
			}
		}
		
		internal IReportContainer Container { get; private set; }
		
		public IEnumerable List {get; private set;}
		
		public Type ElementType {get;private set;}
	}
}
