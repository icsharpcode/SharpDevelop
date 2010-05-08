using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Reports.Addin
{
	internal class ReportDefinitionDeserializer : ReportDefinitionParser
	{
		private IDesignerHost host;
		private ReportSettings reportSettings;
		private Stream stream;
		
		#region Constructor
		
		public ReportDefinitionDeserializer(IDesignerHost host,Stream stream)
		{
			if (host == null) {
				throw new ArgumentNullException("host");
			}
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			this.host = host;
			this.stream = stream;
		}
		
		#endregion
		
		public ReportModel LoadObjectFromFileDefinition()
		{
			
			XmlDocument doc = new XmlDocument();
			doc.Load(this.stream);
			if (doc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
			{
				XmlDeclaration xmlDeclaration = (XmlDeclaration)doc.FirstChild;
				xmlDeclaration.Encoding = "utf-8";
			}
			return LoadObjectFromXmlDocument(doc.DocumentElement);
		}
		
		
		private ReportModel LoadObjectFromXmlDocument(XmlElement elem)
		{
			//ReportSettings
			OpenedFile file =(OpenedFile) host.GetService(typeof(OpenedFile));
			BaseItemLoader baseItemLoader = new BaseItemLoader();
			XmlNodeList n =  elem.FirstChild.ChildNodes;
			XmlElement rse = (XmlElement) n[0];
			ReportModel model = ReportModel.Create();
			
			// manipulate reportSettings if Filename differs
			this.reportSettings = baseItemLoader.Load(rse) as ReportSettings;
			if (this.reportSettings.FileName != file.FileName) {
				System.Diagnostics.Trace.WriteLine("LoadObjectFromXmlDocument - filename changed" );
				this.reportSettings.FileName = file.FileName;
			}
			
			model.ReportSettings = this.reportSettings;
			
			host.Container.Add(this.reportSettings);
			
			//Move to SectionCollection
			XmlNodeList sectionList =  elem.LastChild.ChildNodes;
			
			foreach (XmlNode sectionNode in sectionList) {
				try {
					object o = this.Load(sectionNode as XmlElement,null);
					BaseSection section = o as BaseSection;
					ConvertAbsolut2RelativePath(section.Controls,this.reportSettings.FileName);
//					ConvertAbsolut2RelativePath(section,this.reportSettings.FileName);
					host.Container.Add(section);
				} catch (Exception e) {
					MessageService.ShowError(e);
				}
			}
			return model;
		}
		
		
		
		
		private static void ConvertAbsolut2RelativePath (System.Windows.Forms.Control.ControlCollection controls, string fileName)
		{
			foreach (Control control in controls) {
				
				if (control.Controls.Count > 0) {
					ConvertAbsolut2RelativePath(control.Controls,fileName);
				}
				
				BaseImageItem baseImageItem = control as BaseImageItem;
				if (baseImageItem != null) {
					baseImageItem.ReportFileName = fileName;
					
					if (Path.IsPathRooted(baseImageItem.ImageFileName)) {
						Console.WriteLine("Absolut2RelativePath");
						Console.WriteLine("Image Filename {0}",fileName);
						Console.WriteLine("Image Filename {0}",baseImageItem.ImageFileName);
						string d = ICSharpCode.Reports.Core.FileUtility.GetRelativePath(
							Path.GetDirectoryName(fileName),
							Path.GetDirectoryName(baseImageItem.ImageFileName));

						baseImageItem.RelativeFileName = d + Path.DirectorySeparatorChar + Path.GetFileName(baseImageItem.ImageFileName);
						Console.WriteLine("Rel Filename {0}",baseImageItem.RelativeFileName);
					}
				}
			}
			Console.WriteLine("------------------");
		}
		
		
		protected override Type GetTypeByName(string ns, string name)
		{
			Type t = typeof(BaseSection).Assembly.GetType(typeof(BaseSection).Namespace + "." + name);
			return t;
		}
		
		
		#region Properties
		
		public string ReportName {
			get { return this.reportSettings.ReportName; }
		}
		
		#endregion
		
	}
}
