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
				
				var converter = new DataContainerConverter(base.Graphics,ReportModel.DetailSection,
				                                           CurrentLocation,
				                                           collectionSource);
				
				detail = CreateContainerForSection();

				var position = Point.Empty;
				do {
					
					collectionSource.Fill(Container.Items);
					var r = converter.Convert(Container as ExportContainer,position);
					if (ExeedPage(r)) {
						detail.ExportedItems.AddRange(r);
						CurrentPage.ExportedItems.Insert(2,detail);
						Pages.Add(CurrentPage);
//						CurrentLocation = DetailStart;
						position = Point.Empty;
						CurrentPage = CreateNewPage();
						WriteStandardSections();
						CurrentLocation = DetailStart;
						detail = CreateContainerForSection();
					} else {
						detail.ExportedItems.AddRange(r);
						position = new Point(Container.Location.Y,position.Y + Container.Size.Height);
					}
					
					
				}
				while (collectionSource.MoveNext());
				if (Pages.Count == 0) {
					CurrentPage.ExportedItems.Insert(2,detail);
				} else {
					CurrentPage.ExportedItems.Insert(1,detail);
				}
				
				var a = base.Pages;
			} else {
				detail = base.CreateSection(Container,CurrentLocation);
			}
		}

		IExportContainer CreateContainerForSection( )
		{
			var detail = (ExportContainer)Container.CreateExportColumn();
			detail.Location = CurrentLocation;
			return detail;
		}
		
		
		bool ExeedPage(System.Collections.Generic.List<IExportColumn> r)
		{
			var rect = new Rectangle(r[0].Location,r[0].Size);
			if (rect.Contains(new Point(100,500))) {
				Console.WriteLine("PageBreak");
				return true;
			}
			Console.WriteLine("contains {0} - {1}",rect,DetailEnds);
			return false;
		}
		
		
		
		internal IReportContainer Container { get; private set; }
		
		public IEnumerable List {get; private set;}
		
		public Type ElementType {get;private set;}
	}
}
