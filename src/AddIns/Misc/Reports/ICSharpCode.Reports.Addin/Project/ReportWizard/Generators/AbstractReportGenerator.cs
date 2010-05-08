// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.Reports.Core;
using ICSharpCode.Core;
/// <summary>
/// Abstract Class for all ReportGenerators
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 07.09.2005 14:21:07
/// </remarks>
/// 
namespace ICSharpCode.Reports.Addin.ReportWizard
{
	public interface IReportGenerator{
		 void GenerateReport ();
		 XmlDocument XmlReport {get;}
		 MemoryStream Generated {
		 	get;
		 }
		 Properties Properties{
		 	get;
		 }
	}
	
	public abstract class AbstractReportGenerator : IReportGenerator
	{
		private ReportModel reportModel;
		private StringWriter stringWriter;
		private Properties properties;
		private ReportItemCollection reportItemCollection;
		private AvailableFieldsCollection availableFieldsCollection;
		private ParameterCollection parameterCollection;
		
		protected AbstractReportGenerator(ReportModel reportModel,Properties customizer)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}

			if (customizer == null) {
				throw new ArgumentNullException("customizer");
			}
			this.properties = customizer;
			this.reportModel = reportModel;
		}
		
		
		private void AdjustSectionToDefault () {
		
			ReportSettings settings = reportModel.ReportSettings;
			foreach (ICSharpCode.Reports.Core.BaseSection s in reportModel.SectionCollection) {
				s.Size = new Size(settings.PageSize.Width - settings.LeftMargin - settings.RightMargin,
			                        GlobalValues.DefaultSectionHeight);
			}
		}
		
		
		protected void  WriteToXml ()
		{
			ReportDesignerWriter rpd = new ReportDesignerWriter();
			StringWriterWithEncoding writer = new StringWriterWithEncoding(System.Text.Encoding.UTF8);
			XmlTextWriter xml =XmlHelper.CreatePropperWriter(writer);
			XmlHelper.CreatePropperDocument(xml);
			
			rpd.Save(this.reportModel.ReportSettings,xml);
			xml.WriteEndElement();
			xml.WriteStartElement("SectionCollection");
			
			foreach (ICSharpCode.Reports.Core.BaseSection s in this.reportModel.SectionCollection) {
				rpd.Save(s,xml);
			}

			xml.WriteEndElement();
			xml.WriteEndElement();
			xml.WriteEndDocument();
			stringWriter = writer;
			xml.Flush();
			stringWriter = writer;
		}
		
		#region ReportGenerator.IReportGenerator interface implementation
		
		public virtual void GenerateReport()
		{
			this.AdjustSectionToDefault();
		}
	
		
		public XmlDocument XmlReport
		{
			get {
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
				doc.LoadXml(this.stringWriter.ToString());
				return doc;
			}
		}
		
		
		public MemoryStream Generated
		{
			get {
				System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
				doc.LoadXml(this.stringWriter.ToString());
				using (System.IO.MemoryStream stream = new System.IO.MemoryStream()) {
					doc.Save(stream);
					return stream;
				}
			}
		}
		
		
		#endregion
		
		
		
		public ReportModel ReportModel {
			get { return reportModel; }
		}
		
		public Properties Properties {
			get { return properties; }
		}
		
		
		protected ReportItemCollection ReportItemCollection {
			get { if (this.reportItemCollection == null) {
					this.reportItemCollection = new ReportItemCollection();
				}
				return this.reportItemCollection;
			}
		}
		
		
		
		public AvailableFieldsCollection AvailableFieldsCollection {
			get {
				if (this.availableFieldsCollection == null) {
					this.availableFieldsCollection = new AvailableFieldsCollection();
				}
				return this.availableFieldsCollection;
			}
		}
		
		
		public ParameterCollection SqlQueryParameters {
			get {
				if (this.parameterCollection == null) {
					this.parameterCollection = new ParameterCollection();
				}
				return this.parameterCollection;
			}
		}
	}
}



