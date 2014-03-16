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
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.Reporting.Addin.DesignableItems;
using ICSharpCode.Reporting.Addin.Globals;
using ICSharpCode.Reporting.Addin.Views;
using ICSharpCode.Reporting.Addin.XML;

namespace ICSharpCode.Reporting.Addin.DesignerBinding
{
	/// <summary>
	/// Description of DesignerGenerator.
	/// </summary>
	public class DesignerGenerator:IDesignerGenerator
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
		
		public System.Collections.Generic.IEnumerable<ICSharpCode.SharpDevelop.Workbench.OpenedFile> GetSourceFiles(out ICSharpCode.SharpDevelop.Workbench.OpenedFile designerCodeFile)
		{
			throw new NotImplementedException();
		}
		
		public void MergeFormChanges(System.CodeDom.CodeCompileUnit unit)
		{
			System.Diagnostics.Trace.WriteLine("Generator:MergeFormChanges");
			var writer = new StringWriterWithEncoding(System.Text.Encoding.UTF8);
			var xml = XmlHelper.CreatePropperWriter(writer);
			InternalMergeFormChanges(xml);
			Console.WriteLine(writer.ToString());
			viewContent.ReportFileContent = writer.ToString();
		}
		
		public bool InsertComponentEvent(System.ComponentModel.IComponent component, System.ComponentModel.EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{
			throw new NotImplementedException();
		}
		
		public System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get {
				throw new NotImplementedException();
			}
		}
		
		public DesignerView ViewContent {
			get {return viewContent;}
			
		}
		
		#endregion
		
		void InternalMergeFormChanges(XmlTextWriter xml)
		{
			if (xml == null) {
				throw new ArgumentNullException("xml");
			}
			
			var rpd = new ReportDesignerWriter();
			XmlHelper.CreatePropperDocument(xml);
			
			foreach (IComponent component in viewContent.Host.Container.Components) {
				if (!(component is Control)) {
					rpd.Save(component,xml);
				}
			}
			xml.WriteEndElement();
			xml.WriteStartElement("SectionCollection");
			
			// we look only for Sections
			foreach (var component in viewContent.Host.Container.Components) {
				var b = component as BaseSection;
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
	}
}
