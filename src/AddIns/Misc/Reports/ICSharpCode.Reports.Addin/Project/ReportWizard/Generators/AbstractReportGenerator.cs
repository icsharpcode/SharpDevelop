// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.SharpDevelop;

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
		private StringWriter stringWriter;
		
		private ReportItemCollection reportItemCollection;
		private AvailableFieldsCollection availableFieldsCollection;
		private ParameterCollection parameterCollection;
		
		
		private ColumnCollection groupColumnCollection;
		
		protected AbstractReportGenerator(ReportModel reportModel,Properties properties)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}

			if (properties == null) {
				throw new ArgumentNullException("customizer");
			}
			this.ReportModel = reportModel;
			this.Properties = properties;
			ReportStructure = (ReportStructure)properties.Get("Generator");
			
			this.AvailableFieldsCollection.Clear();
			this.ReportItemCollection.Clear();
			this.GroupColumnCollection.Clear();
			this.ParameterCollection.Clear();
		}
		
		
		protected void UpdateGenerator ()
		{
			this.AvailableFieldsCollection.AddRange(ReportStructure.AvailableFieldsCollection);
			this.ReportItemCollection.AddRange(ReportStructure.ReportItemCollection);
			
			if (ReportModel.ReportSettings.GroupColumnsCollection.Count > 0) {
				this.GroupColumnCollection.AddRange(ReportModel.ReportSettings.GroupColumnsCollection);
			}
			if (ReportStructure.SqlQueryParameters.Count > 0) {
				ReportModel.ReportSettings.ParameterCollection.AddRange(ReportStructure.SqlQueryParameters);
			}
		}
		
		protected void UpdateModel ()
		{
			var settings = this.ReportModel.ReportSettings;
			settings.AvailableFieldsCollection.Clear();
			settings.AvailableFieldsCollection.AddRange(this.availableFieldsCollection);
			
			settings.GroupColumnsCollection.Clear();
			settings.GroupColumnsCollection.AddRange(this.groupColumnCollection);
		}
		
		
		private void AdjustSectionToDefault () {
			
			ReportSettings settings = ReportModel.ReportSettings;
			foreach (ICSharpCode.Reports.Core.BaseSection s in ReportModel.SectionCollection) {
				s.Size = new Size(settings.PageSize.Width - settings.LeftMargin - settings.RightMargin,
			                        GlobalValues.DefaultSectionHeight);
				Console.WriteLine("Adjust Section To DefaultSize : {0}",s.Size);
			}
		}
		
		
		protected void  WriteToXml ()
		{
			
			LoggingService.Debug("AbstractReportGenerator - Generate Xml from RepotModel");
			
			ReportDesignerWriter rpd = new ReportDesignerWriter();
			StringWriterWithEncoding writer = new StringWriterWithEncoding(System.Text.Encoding.UTF8);
			XmlTextWriter xml =XmlHelper.CreatePropperWriter(writer);
			XmlHelper.CreatePropperDocument(xml);
			
			rpd.Save(this.ReportModel.ReportSettings,xml);
			xml.WriteEndElement();
			xml.WriteStartElement("SectionCollection");
			
			foreach (ICSharpCode.Reports.Core.BaseSection s in this.ReportModel.SectionCollection) {
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
		
		protected ReportStructure ReportStructure {get;private set;}
		
		public ReportModel ReportModel {get;private set;}
	
		
		public Properties Properties {get; private set;}
			
		
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
		
		public ColumnCollection GroupColumnCollection {
			get{
				if (this.groupColumnCollection == null) {
					this.groupColumnCollection = new ColumnCollection();
				}
				return this.groupColumnCollection;
			}
			set { this.groupColumnCollection = value;}
		}
		
		public ParameterCollection ParameterCollection {
			get {
				if (this.parameterCollection == null) {
					this.parameterCollection = new ParameterCollection();
				}
				return this.parameterCollection;
			}
		}
	}
}
