/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 26.04.2009
 * Zeit: 18:47
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Interfaces;
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

			Application.UseWaitCursor = true;
			Application.DoEvents();
			try {
				IComponent cc = this.host.CreateComponent(typeof(ICSharpCode.Reports.Addin.RootReportModel),"RootReportModel");
				ICSharpCode.Reports.Addin.RootReportModel rootControl = cc as ICSharpCode.Reports.Addin.RootReportModel;
				UpdateStatusbar();
				this.CreateNamedSurface();
				rootControl.Size = this.ReportModel.ReportSettings.PageSize;
				
			} catch (Exception e) {
				MessageService.ShowException(e,"LoadOrCreateReport");
			} finally {
				Application.UseWaitCursor = false;
			}
		}
		
		private void UpdateStatusbar ()
		{
			string message;
				if (this.generator.ViewContent.PrimaryFile.IsDirty) {
					message = String.Format("Create Report <{0}> ...",Path.GetFileName(this.generator.ViewContent.PrimaryFile.FileName));
				} else {
					message = String.Format("Load  Report <{0}> ...",Path.GetFileName(this.generator.ViewContent.PrimaryFile.FileName));
				}
			SharpDevelop.Gui.WorkbenchSingleton.StatusBar.SetMessage(message);
		}
			
		
		private void CreateNamedSurface ()
		{
			ReportDefinitionDeserializer rl = new ReportDefinitionDeserializer(this.host,stream);
			this.ReportModel = rl.LoadObjectFromFileDefinition();
		}
		
		
		public ReportModel ReportModel {get; private set;}

	}
}
