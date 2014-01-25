// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
		 MemoryStream Generated {get;}
		 ReportStructure ReportStructure {get;}
	}
	
	
	public abstract class AbstractReportGenerator : IReportGenerator
	{
		private StringWriter stringWriter;
		private ReportItemCollection reportItemCollection;
		private AvailableFieldsCollection availableFieldsCollection;
		private ParameterCollection parameterCollection;
		private ColumnCollection groupColumnCollection;
		
		protected AbstractReportGenerator(ReportModel reportModel,ReportStructure reportStructure)
		{
			if (reportModel == null) {
				throw new ArgumentNullException("reportModel");
			}
			if (reportStructure == null) {
				throw new ArgumentNullException("reportStructure");
			}
			this.ReportModel = reportModel;
			ReportStructure = reportStructure;
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
				ReportModel.ReportSettings.SqlParameters.AddRange(ReportStructure.SqlQueryParameters);
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
		
		public ReportStructure ReportStructure {get;private set;}
		
		public ReportModel ReportModel {get;private set;}
	

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
