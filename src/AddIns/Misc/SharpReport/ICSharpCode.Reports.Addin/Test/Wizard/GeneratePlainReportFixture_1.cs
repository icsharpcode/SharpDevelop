/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 03.11.2008
 * Zeit: 19:34
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Wizard
{
	[TestFixture]
	public class GeneratePlainReportFixture
	{
		[Test]
		public void GeneratePlainReport_1()
		{
			ReportModel model = ReportModel.Create();
			Properties customizer = new Properties();
			
			customizer.Set("ReportLayout",GlobalEnums.ReportLayout.ListLayout);
			IReportGenerator generator = new GeneratePlainReport(model,customizer);
			generator.GenerateReport();
			
			XDocument doc1 = XDocument.Load(new XmlNodeReader (generator.XmlReport));
			
			XDocument doc2 = new XDocument();
			
			using (XmlWriter w = doc2.CreateWriter()){
				generator.XmlReport.Save (w);
			}
			XDocument doc3 = ReportGenerationHelper.XmlDocumentToXDocument(generator.XmlReport);
			Assert.IsNotNull (doc1);
			Assert.IsNotNull (doc2);
			Assert.IsNotNull (doc2);
			
			var sq = from si in doc1.Descendants() select si;
			Console.WriteLine ("xxxxx");
			foreach (XElement a in sq)
			{
				Console.WriteLine (a.Name);
		}
			
		}
		
		[Test]
		public void CheckReportSettings ()
		{
			XmlDocument xmlDocument = this.CreateXmlFromModel();
			XDocument doc2 = new XDocument();
			
			using (XmlWriter w = doc2.CreateWriter()){
				xmlDocument.Save (w);
			}
			Console.WriteLine ("<GeneratePlainReportFixture>----ReportSettings-----");
			var sq1 = from si in doc2.Descendants("ReportSettings").Descendants() select si;
			foreach (XElement a in sq1)
			{
				Console.WriteLine (a.Name);
			}
		}
		
		[Test]
		public void CheckSections ()
		{
			Console.WriteLine("GeneratePlainReport.CheckSection");
			XmlDocument xmlDocument = this.CreateXmlFromModel();
			XDocument doc3 = ReportGenerationHelper.XmlDocumentToXDocument(xmlDocument);
			Console.WriteLine ("----Sections-----");
//			var sq1 = from si in doc3.Descendants("SectionCollection").Descendants() select si;
			// get all Basesections
			var sq1 = from si in doc3.Descendants("SectionCollection").Descendants("BaseSection") select si;
			foreach (XElement a in sq1)
			{
				Console.WriteLine (a.Name);
			}
			Assert.AreEqual (5,sq1.Count(),"Should be 5 times a 'BaseSection'");
		}
		
		private XmlDocument CreateXmlFromModel ()
		{
			ReportModel model = ReportModel.Create();
			Properties customizer = new Properties();
			
			customizer.Set("ReportLayout",GlobalEnums.ReportLayout.ListLayout);
			IReportGenerator generator = new GeneratePlainReport(model,customizer);
			generator.GenerateReport();
			return generator.XmlReport;
		}
	}
}
