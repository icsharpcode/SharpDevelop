/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 11.02.2014
 * Time: 20:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.Reporting.Factories;
using ICSharpCode.Reporting.Items;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.Reporting.Addin.Commands;
using ICSharpCode.Reporting.Addin.Factory;

namespace ICSharpCode.Reporting.Addin.DesignerBinding {
	
	
	public class ReportDesignerBinding:IDisplayBinding {
		
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return true;
		}
		
		
		public bool CanCreateContentForFile(FileName fileName)
		{
			return Path.GetExtension(fileName).Equals(".srd", StringComparison.OrdinalIgnoreCase);
		}
		
		
		public double AutoDetectFileContent(FileName fileName, System.IO.Stream fileContent, string detectedMimeType)
		{
			throw new System.NotImplementedException();
		}
		
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			bool hasContent = false;
			using (var sr = new StreamReader(file.OpenRead()))
			{
				string line;
				while (!hasContent && (line = sr.ReadLine()) != null)
					if (line.Length != 0)
						hasContent = true;
			}
			if (!hasContent) {

				var reportModel = ReportModelFactory.Create();
	
				var reportFactory = new CreateFormSheetFromModel();
				var xml = reportFactory.ToXml(reportModel);
				
				var doc = new XmlDocument();
				doc.LoadXml(xml.ToString());
				var ar = XmlToArray(doc);
				file.SetData(ar);
				file.MakeDirty();
			}
			
			var viewCmd = new CreateDesignerCommand(file);
			viewCmd.Run();
			LoggingService.Info("return DesignerView");
			return viewCmd.DesignerView;
		}

		static byte[] XmlToArray(XmlDocument doc)
		{
			using (var stream = new MemoryStream()) {
				doc.Save(stream);
				return stream.ToArray();
			}
		}
	}
}