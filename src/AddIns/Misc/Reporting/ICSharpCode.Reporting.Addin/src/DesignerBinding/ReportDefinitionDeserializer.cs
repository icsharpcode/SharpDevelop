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
using System.ComponentModel.Design;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Reporting.Items;
using ICSharpCode.Reporting.Xml;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.Reporting.Addin.XML;

namespace ICSharpCode.Reporting.Addin.DesignerBinding
{
	class ReportDefinitionDeserializer : ReportDefinitionParser
	{
		IDesignerHost host;
		ICSharpCode.Reporting.Addin.DesignableItems.ReportSettings reportSettings;
		Stream stream;
		
		#region Constructor
		
		public ReportDefinitionDeserializer(IDesignerHost host,Stream stream)
		{
			if (host == null) {
				throw new ArgumentNullException("host");
			}
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			Console.WriteLine("ReportDefinitionDeserializer");
			this.host = host;
			this.stream = stream;
		}
		
		#endregion
		
		public ReportModel LoadObjectFromFileDefinition()
		{
			Console.WriteLine("LoadObjectFromFileDefinition()");
			var xmlDocument = new XmlDocument();
			xmlDocument.Load(this.stream);
			if (xmlDocument.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
			{
				XmlDeclaration xmlDeclaration = (XmlDeclaration)xmlDocument.FirstChild;
				xmlDeclaration.Encoding = "utf-8";
			}
			return LoadObjectFromXmlDocument(xmlDocument.DocumentElement);
		}
		
		
		private ReportModel LoadObjectFromXmlDocument(XmlElement elem)
		{
			Console.WriteLine("LoadObjectFromXmlDocumen)");
			//ReportSettings
			var file =(OpenedFile) host.GetService(typeof(OpenedFile));
			
			XmlNodeList nodes =  elem.FirstChild.ChildNodes;
			var rse = (XmlElement) nodes[0];
//			var reportModel = new ReportModel();
			// manipulate reportSettings if Filename differs
			var modelLoader = new ModelLoader();
			this.reportSettings = modelLoader.Load(rse) as ICSharpCode.Reporting.Addin.DesignableItems.ReportSettings;
			
			if (string.Compare(this.reportSettings.FileName, file.FileName, StringComparison.CurrentCulture) != 0) {
				System.Diagnostics.Trace.WriteLine("LoadObjectFromXmlDocument - filename changed" );
				this.reportSettings.FileName = file.FileName;
			}
			var reportModel = new ReportModel();
//			reportModel.ReportSettings = this.reportSettings;
			
			host.Container.Add(this.reportSettings);
			
			//Move to SectionCollection
			XmlNodeList sectionList =  elem.LastChild.ChildNodes;
			
			foreach (XmlNode sectionNode in sectionList) {
				try {
					object o = this.Load(sectionNode as XmlElement,null);
					BaseSection section = o as BaseSection;
//					ConvertAbsolut2RelativePath(section.Controls,this.reportSettings.FileName);
//					ConvertAbsolut2RelativePath(section,this.reportSettings.FileName);
//					host.Container.Add(section);
				} catch (Exception e) {
					MessageService.ShowException(e);
				}
			}
			return reportModel;
		}
		
		
		
		
		private static void ConvertAbsolut2RelativePath (System.Windows.Forms.Control.ControlCollection controls, string fileName)
		{
			/*
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
						string d = ICSharpCode.Reports.Core.Globals.FileUtility.GetRelativePath(
							Path.GetDirectoryName(fileName),
							Path.GetDirectoryName(baseImageItem.ImageFileName));

						baseImageItem.RelativeFileName = d + Path.DirectorySeparatorChar + Path.GetFileName(baseImageItem.ImageFileName);
						Console.WriteLine("Rel Filename {0}",baseImageItem.RelativeFileName);
					}
				}
			}
			*/
		}
		
		
		protected override Type GetTypeByName(string ns, string name)
		{
			Type t = typeof(BaseSection).Assembly.GetType(typeof(BaseSection).Namespace + "." + name);
			return t;
		}
		
		/*
		#region Properties
		
		public string ReportName {
			get { return this.reportSettings.ReportName; }
		}
		
		#endregion
		*/
	}
}
