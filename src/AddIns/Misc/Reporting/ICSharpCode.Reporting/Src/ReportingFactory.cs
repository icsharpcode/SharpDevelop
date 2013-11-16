/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.03.2013
 * Time: 17:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.IO;
using System.Xml;

using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.PageBuilder;
using ICSharpCode.Reporting.Xml;

namespace ICSharpCode.Reporting
{
	/// <summary>
	/// Description of Reporting.
	/// </summary>
	public class ReportingFactory
	{
		
		public IReportCreator ReportCreator (Stream stream,IEnumerable list)
		{
			ReportModel = LoadReportModel (stream);
			IReportCreator builder = null;
			builder = new DataPageBuilder(ReportModel,list );
			return builder;
		}
		
		[Obsolete("Use public IReportCreator ReportCreator (Stream stream,IEnumerable list")]
		public IReportCreator ReportCreator (Stream stream,Type listType,IEnumerable list)
		{
			ReportModel = LoadReportModel (stream);
			IReportCreator builder = null;
			builder = new DataPageBuilder(ReportModel,list );
			return builder;
		}
		
		
		public IReportCreator ReportCreator (Stream stream)
		{
			ReportModel = LoadReportModel (stream);
			IReportCreator builder = null;
//			builder = ReportCreatorFactory.ExporterFactory(ReportModel);
			builder = new FormPageBuilder(ReportModel);
			return builder;
		}
		

		internal ReportModel LoadReportModel (Stream stream)
		{
			var doc = new XmlDocument();
			doc.Load(stream);
			ReportModel = LoadModel(doc);
			return ReportModel;
		}
		
		static ReportModel LoadModel(XmlDocument doc)
		{
			var loader = new ModelLoader();
			object root = loader.Load(doc.DocumentElement);

			var model = root as ReportModel;
			if (model == null) {
//				throw new IllegalFileFormatException("ReportModel");
			}
			return model;
		}
		
		public ReportModel ReportModel {get;private set;}
	}
}
