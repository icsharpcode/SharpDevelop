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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

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
			Console.WriteLine("ReportDefinitionDeserializer");
			this.host = host;
			this.stream = stream;
		}
		
		#endregion
		
		public ReportModel LoadObjectFromFileDefinition()
		{
			Console.WriteLine("LoadObjectFromFileDefinition()");
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
			Console.WriteLine("LoadObjectFromXmlDocumen)");
			//ReportSettings
			OpenedFile file =(OpenedFile) host.GetService(typeof(OpenedFile));
			BaseItemLoader baseItemLoader = new BaseItemLoader();
			XmlNodeList n =  elem.FirstChild.ChildNodes;
			XmlElement rse = (XmlElement) n[0];
			ReportModel model = ReportModel.Create();
			
			// manipulate reportSettings if Filename differs
			this.reportSettings = baseItemLoader.Load(rse) as ReportSettings;
			if (this.reportSettings.FileName.CompareTo(file.FileName) != 0) {
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
					MessageService.ShowException(e);
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
						string d = ICSharpCode.Reports.Core.Globals.FileUtility.GetRelativePath(
							Path.GetDirectoryName(fileName),
							Path.GetDirectoryName(baseImageItem.ImageFileName));

						baseImageItem.RelativeFileName = d + Path.DirectorySeparatorChar + Path.GetFileName(baseImageItem.ImageFileName);
						Console.WriteLine("Rel Filename {0}",baseImageItem.RelativeFileName);
					}
				}
			}
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
