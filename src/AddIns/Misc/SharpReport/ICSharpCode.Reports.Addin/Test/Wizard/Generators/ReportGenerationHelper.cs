/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 03.11.2008
 * Zeit: 19:36
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Addin.ReportWizard;
using System.Xml;
using System.Xml.Linq;

namespace ICSharpCode.Reports.Addin.Test.Wizard
{
	/// <summary>
	/// Description of ReportGenerationHelper.
	/// </summary>
	public class ReportGenerationHelper
	{
		public static ReportModel FormSheetModel()
		{
			ReportStructure rs = new ReportStructure();
			rs.ReportName = "PlainReport";
			rs.GraphicsUnit = System.Drawing.GraphicsUnit.Millimeter;
			rs.ReportType = GlobalEnums.ReportType.FormSheet;
			rs.DataModel = GlobalEnums.PushPullModel.FormSheet;
			ReportModel m = ReportModel.Create();
			
			return rs.CreateAndFillReportModel ();
		}
		
		
		public static XDocument XmlDocumentToXDocument (XmlDocument xmlDoc)
		{
			XDocument xDoc = new XDocument();
			
			using (XmlWriter w = xDoc.CreateWriter()){
				xmlDoc.Save (w);
			}
			return xDoc;
		}
		
		
		public static ReportModel CreateModel (string reportName)
		{
			
			ReportStructure structure = CreateReportStructure(reportName);
			
			AvailableFieldsCollection abstractColumns = new AvailableFieldsCollection();
			AbstractColumn a1 = new AbstractColumn("Field1",typeof(System.String));
			structure.AvailableFieldsCollection.Add(a1);
			
			ICSharpCode.Reports.Core.BaseDataItem bri = new ICSharpCode.Reports.Core.BaseDataItem();
			bri.Name ="Field1";
			structure.ReportItemCollection.Add(bri);
			
			ReportModel m = structure.CreateAndFillReportModel();
			ICSharpCode.Core.Properties customizer = new ICSharpCode.Core.Properties();
			
			customizer.Set("Generator", structure);
			customizer.Set("ReportLayout",GlobalEnums.ReportLayout.ListLayout);
			IReportGenerator generator = new GeneratePushDataReport(m,customizer);
			generator.GenerateReport();
			
			ReportLoader rl = new ReportLoader();
			object root = rl.Load(generator.XmlReport.DocumentElement);
			ReportModel model = root as ReportModel;
			if (model != null) {
				model.ReportSettings.FileName = GlobalValues.PlainFileName;
				FilePathConverter.AdjustReportName(model);
			} else {
				throw new InvalidReportModelException();
			}
			return model;
		}
		
		
		private static ReportStructure CreateReportStructure (string reportName)
		{
			ReportStructure structure = new ReportStructure();
			structure.ReportName = reportName;
			structure.DataModel = GlobalEnums.PushPullModel.PushData;
			structure.ReportType = GlobalEnums.ReportType.DataReport;
			return structure;
		}
	}
}
