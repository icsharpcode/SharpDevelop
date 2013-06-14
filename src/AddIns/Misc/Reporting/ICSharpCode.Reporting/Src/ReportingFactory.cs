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
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Reporting.Factories;
using ICSharpCode.Reporting.Globals;
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
		public ReportingFactory()
		{
		}
		
		
		public IReportCreator ReportCreator (Stream stream,Type listType,IEnumerable list)
		{
			ReportModel = LoadReportModel (stream);
			IReportCreator builder = null;
			builder = new DataPageBuilder(ReportModel,listType,list );
			return builder;
		}
		
		
		internal IReportCreator ReportCreator (ReportModel reportModel) {
			if (reportModel == null)
				throw new ArgumentNullException("reportModel");
			IReportCreator builder = null;
			ReportModel = reportModel;
			builder = ReportCreatorFactory.ExporterFactory(reportModel);
			return builder;
		}
		
		
		internal IReportCreator ReportCreator (Stream stream)
		{
			ReportModel = LoadReportModel (stream);
			IReportCreator builder = null;
			builder = ReportCreatorFactory.ExporterFactory(ReportModel);
			return builder;
		}
		
		
		internal ReportModel LoadReportModel (Stream stream)
		{
			var doc = new XmlDocument();
			doc.Load(stream);
			var rm = LoadModel(doc);
			return rm;
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
