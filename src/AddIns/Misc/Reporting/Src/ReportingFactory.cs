/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.03.2013
 * Time: 17:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Xml;

using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Items;
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
		
		
		public IReportCreator CreatePageBuilder (Stream stream, IList list)
		{
			return null;
		}
			
		
		internal ReportModel LoadReport (Stream stream)
		{
			Console.WriteLine("ReportEngine:LoadReportModel_2");
			var doc = new XmlDocument();
			doc.Load(stream);
			var rm = LoadModel(doc);
			return rm;
		}
		
		static ReportModel LoadModel(XmlDocument doc)
		{
			Console.WriteLine("ReportEngine:LoadModel");
			var loader = new ModelLoader();
			object root = loader.Load(doc.DocumentElement);

			var model = root as ReportModel;
			if (model == null) {
//				throw new IllegalFileFormatException("ReportModel");
			}
			return model;
		}
	}
}
