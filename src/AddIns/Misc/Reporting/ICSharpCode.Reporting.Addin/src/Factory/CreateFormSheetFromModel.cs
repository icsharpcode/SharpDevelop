/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.04.2014
 * Time: 19:55
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.IO;
using ICSharpCode.Reporting.Interfaces;
using ICSharpCode.Reporting.Addin.DesignableItems;
using ICSharpCode.Reporting.Addin.Globals;
using ICSharpCode.Reporting.Addin.XML;

namespace ICSharpCode.Reporting.Addin.Factory
{
	/// <summary>
	/// Description of CreateFormSheetReport.
	/// </summary>
	static class CreateFormSheetFromModel
	{
		
		public static StringWriter ToXml(IReportModel reportModel) {
			int locY = reportModel.ReportSettings.TopMargin;
			
				foreach (var section in reportModel.SectionCollection)
				{
					section.Location = new Point(reportModel.ReportSettings.LeftMargin,locY);
					section.Size = new Size(reportModel.ReportSettings.PageSize.Width - reportModel.ReportSettings.LeftMargin - reportModel.ReportSettings.RightMargin,
						70);
					locY = locY + section.Size.Height + DesignerGlobals.GabBetweenSection;
				}
	
				
				var xml = ToXmlInternal(reportModel);
			return xml;
		}
		
		static StringWriter ToXmlInternal(IReportModel model)
		{
			var writer = new StringWriterWithEncoding(System.Text.Encoding.UTF8);
			var xml = XmlHelper.CreatePropperWriter(writer);
		
			var reportDesignerWriter = new ReportDesignerWriter();
			XmlHelper.CreatePropperDocument(xml);
			
			
			reportDesignerWriter.Save(model.ReportSettings,xml);
			
			xml.WriteEndElement();
			xml.WriteStartElement("SectionCollection");
			
			// we look only for Sections
			foreach (var section in model.SectionCollection) {
					reportDesignerWriter.Save(section,xml);
			}
			
			//SectionCollection
			xml.WriteEndElement();
			//Reportmodel
			xml.WriteEndElement();
			xml.WriteEndDocument();
			xml.Close();
			return writer;
		}
		
	}
}
