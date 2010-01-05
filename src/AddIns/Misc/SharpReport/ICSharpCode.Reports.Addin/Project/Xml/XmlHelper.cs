/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 20.05.2008
 * Zeit: 18:24
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Xml;
using System.IO;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of XmlHelper.
	/// </summary>
	internal class XmlHelper
	{
		private XmlHelper()
		{
		}
		
		
		public static XmlTextWriter CreatePropperWriter (StringWriter writer)
		{
			XmlTextWriter xml = new XmlTextWriter(writer);
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
