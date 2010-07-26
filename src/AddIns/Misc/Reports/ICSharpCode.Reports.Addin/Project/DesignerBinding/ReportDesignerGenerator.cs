/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 26.11.2007
 * Zeit: 18:02
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of AbstractDesignerLoader.
	/// </summary>
	
	public class ReportDesignerGenerator:IDesignerGenerator
	{
		private ReportDesignerView viewContent;
		
		public ReportDesignerGenerator()
		{
		}
		
		public CodeDomProvider CodeDomProvider {
			get {
				throw new NotImplementedException();
			}
		}
		
		
		public ReportDesignerView ViewContent {
			get { return this.viewContent; }
		}
		
		
		public void Attach(ReportDesignerView viewContent)
		{
			if (viewContent == null) {
				throw new ArgumentNullException("viewContent");
			}
			this.viewContent = viewContent;
		}
		
		
		public void Detach()
		{
			this.viewContent = null;
		}
		
		
		public IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile)
		{
			designerCodeFile = this.viewContent.PrimaryFile;
			return new [] {designerCodeFile};
		}
		
		
		public void MergeFormChanges(CodeCompileUnit unit){
			
				System.Diagnostics.Trace.WriteLine("Generator:MergeFormChanges");
				StringWriterWithEncoding writer = new StringWriterWithEncoding(System.Text.Encoding.UTF8);
				XmlTextWriter xml = XmlHelper.CreatePropperWriter(writer);
				this.InternalMergeFormChanges(xml);
				viewContent.ReportFileContent = writer.ToString();
		}
		
		
		
		private void InternalMergeFormChanges(XmlTextWriter xml)
		{
			if (xml == null) {
				throw new ArgumentNullException("xml");
			}
			
			ReportDesignerWriter rpd = new ReportDesignerWriter();
			XmlHelper.CreatePropperDocument(xml);
			
			foreach (IComponent component in viewContent.Host.Container.Components) {
				if (!(component is Control)) {
					rpd.Save(component,xml);
				}
			}
			xml.WriteEndElement();
			xml.WriteStartElement("SectionCollection");
			
			// we look only for Sections
			foreach (IComponent component in viewContent.Host.Container.Components) {
				BaseSection b = component as BaseSection;
				if (b != null) {
					rpd.Save(component,xml);
				}
			}
			//SectionCollection
			xml.WriteEndElement();
			//Reportmodel
			xml.WriteEndElement();
			xml.WriteEndDocument();
			xml.Close();
		}
	
		
		public bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{
			throw new NotImplementedException();
		}
	}
}
