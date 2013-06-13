// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Addin.Test.Wizard
{
	/// <summary>
	/// Description of ReportGenerationHelper.
	/// </summary>
	/// 

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
		
		
		public static ReportModel CreateModel (string reportName,bool createGrouping)
		{
			
			ReportStructure reportStructure = CreateReportStructure(reportName);
			
			AvailableFieldsCollection abstractColumns = new AvailableFieldsCollection();
			AbstractColumn a1 = new AbstractColumn("Field1",typeof(System.String));
			reportStructure.AvailableFieldsCollection.Add(a1);
			
			ICSharpCode.Reports.Core.BaseDataItem bri = new ICSharpCode.Reports.Core.BaseDataItem();
			bri.Name ="Field1";
			reportStructure.ReportItemCollection.Add(bri);
			
			if (createGrouping) {
				reportStructure.Grouping = "group";
			}
			
			ReportModel m = reportStructure.CreateAndFillReportModel();
			
			IReportGenerator generator = new GeneratePushDataReport(m,reportStructure);
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
