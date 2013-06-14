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
using ICSharpCode.Reporting.DataManager.Listhandling;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.PageBuilder.Converter;

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
			WritePages ();
		}
		
		
		void BuilDetail()
		{
			var collectionSource = new CollectionSource(List,ElementType,ReportModel.ReportSettings);
			collectionSource.Bind();
			CurrentLocation = DetailStart;
			var converter = new DataContainerConverter(base.Graphics,ReportModel.DetailSection,CurrentLocation,collectionSource);
			var detail = converter.Convert();
			CurrentPage.ExportedItems.Insert(2,detail);
		}
		
		
		protected override void WritePages()
		{
			base.WritePages();
			BuilDetail();
			base.AddPage(CurrentPage);
		}
		
		public IEnumerable List {get; private set;}
		
		public Type ElementType {get;private set;}
	}
}
