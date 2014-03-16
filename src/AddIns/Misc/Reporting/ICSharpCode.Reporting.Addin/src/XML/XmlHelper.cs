/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.03.2014
 * Time: 18:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Xml;

namespace ICSharpCode.Reporting.Addin.XML
{
	static class XmlHelper
	{
		public static XmlTextWriter CreatePropperWriter (StringWriter writer)
		{
			var xml = new XmlTextWriter(writer);
			xml.Formatting = Formatting.Indented;
			xml.Indentation = 4;
			return xml;
		}
		
		
		public static void CreatePropperDocument (XmlTextWriter writer)
		{
			writer.WriteStartDocument();
			writer.WriteStartElement("ReportModel");
			writer.WriteStartElement("ReportSettings");
		}
	}
}
