/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.02.2014
 * Time: 19:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.Reporting.Addin.DesignableItems;
using ICSharpCode.Reporting.Addin.Globals;
using ICSharpCode.Reporting.Addin.Views;
using ICSharpCode.Reporting.Addin.XML;

namespace ICSharpCode.Reporting.Addin.DesignerBinding
{
	/// <summary>
	/// Description of DesignerGenerator.
	/// </summary>
	class DesignerGenerator:IDesignerGenerator
	{
		DesignerView viewContent;
		
		public DesignerGenerator()
		{
			LoggingService.Info("Create DesignerGenerator");
		}
		
		#region IDesignerGenerator implementation
		
		public void Attach(DesignerView viewContent)
		{
			if (viewContent == null)
				throw new ArgumentNullException("viewContent");
			LoggingService.Info("DesignerGenerator:Attach");
			this.viewContent = viewContent;
		}
		
		
		public void Detach()
		{
			throw new NotImplementedException();
		}
		
		public System.Collections.Generic.IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile)
		{
			throw new NotImplementedException();
		}
		
		public void MergeFormChanges(System.CodeDom.CodeCompileUnit unit)
		{
			var writer = InternalMergeFormChanges();
			viewContent.ReportFileContent = writer.ToString();
		}
		
		
		StringWriter InternalMergeFormChanges()
		{
			var writer = new StringWriterWithEncoding(System.Text.Encoding.UTF8);
			var xml = XmlHelper.CreatePropperWriter(writer);
		
			var reportDesignerWriter = new ReportDesignerWriter();
			XmlHelper.CreatePropperDocument(xml);
			
			foreach (IComponent component in viewContent.Host.Container.Components) {
				if (!(component is Control)) {
					reportDesignerWriter.Save(component,xml);
				}
			}
			xml.WriteEndElement();
			xml.WriteStartElement("SectionCollection");
			
			// we look only for Sections
			foreach (var component in viewContent.Host.Container.Components) {
				var b = component as BaseSection;
				if (b != null) {
					reportDesignerWriter.Save(component,xml);
				}
			}
			//SectionCollection
			xml.WriteEndElement();
			//Reportmodel
			xml.WriteEndElement();
			xml.WriteEndDocument();
			xml.Close();
			return writer;
		}
		
	
		public DesignerView ViewContent {
			get {return viewContent;}
			
		}
		
		public bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{
			throw new NotImplementedException();
		}
		
		public System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get {
				throw new NotImplementedException();
			}
		}
		
		#endregion
	}
}
