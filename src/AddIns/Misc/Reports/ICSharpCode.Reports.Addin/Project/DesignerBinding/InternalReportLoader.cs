/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 26.04.2009
 * Zeit: 18:47
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Reports.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportLoader.
	/// </summary>
	internal class InternalReportLoader
	{
		private IDesignerLoaderHost host;
		private Stream stream;
		private IDesignerGenerator generator;
		private ReportModel reportModel;
		
		public InternalReportLoader(IDesignerLoaderHost host,IDesignerGenerator generator, Stream stream)
		{
			if (host == null) {
				throw new ArgumentNullException("host");
			}
		
			if (generator == null) {
				throw new ArgumentNullException("generator");
			}
			this.host = host;
			this.generator = generator;
			this.stream = stream;
		}
		
		public void LoadOrCreateReport()
		{
			//string baseClassName = String.Empty;

			Application.UseWaitCursor = true;
			Application.DoEvents();
			try {
				IComponent cc = this.host.CreateComponent(typeof(ICSharpCode.Reports.Addin.RootReportModel),"RootReportModel");
				ICSharpCode.Reports.Addin.RootReportModel rootControl = cc as ICSharpCode.Reports.Addin.RootReportModel;
				
				string message;
				if (this.generator.ViewContent.PrimaryFile.IsDirty) {
					message = String.Format("Create Report + {0} ...",Path.GetFileName(this.generator.ViewContent.PrimaryFile.FileName));
				} else {
					message = String.Format("Load  Report + {0} ...",Path.GetFileName(this.generator.ViewContent.PrimaryFile.FileName));
				}
				StatusBarService.SetMessage(message);			
				this.CreateNamedSurface();
				UnitConverter pageWidth = new UnitConverter(iTextSharp.text.PageSize.A4.Width, XGraphicsUnit.Point);
				UnitConverter pageHeight = new UnitConverter(iTextSharp.text.PageSize.A4.Height + (this.reportModel.SectionCollection.Count +1) * GlobalsDesigner.GabBetweenSection
				                                             , XGraphicsUnit.Point);
				rootControl.Size = new System.Drawing.Size((int)pageWidth.Pixel,(int)pageHeight.Pixel);
				
			} catch (Exception e) {
				System.Console.WriteLine(e.Message);
			} finally {
				StatusBarService.SetMessage(String.Empty);
				Application.UseWaitCursor = false;
			}
		}
		
		
		private void CreateNamedSurface ()
		{
			ReportDefinitionDeserializer rl = new ReportDefinitionDeserializer(this.host,stream);
			this.reportModel = rl.LoadObjectFromFileDefinition();
		}
		
		
		public ReportModel ReportModel {
			get { return reportModel; }
		}
		
	}
}
