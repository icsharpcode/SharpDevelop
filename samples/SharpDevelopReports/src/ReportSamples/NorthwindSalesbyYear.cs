/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier
 * Datum: 04.02.2007
 * Zeit: 15:26
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

using ICSharpCode.Reports.Core;
namespace ReportSamples
{
	/// <summary>
	/// Description of NorthwindSalesbyYear.
	/// </summary>
	public class NorthwindSalesbyYear:BaseSample
	{
//		string conString = @"Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Northwind";
		static string start = "01.01.1997";
		static string end = "31.03.1997";
		ReportParameters parameters;
		
		public NorthwindSalesbyYear()
		{
			base.Run();
			
			parameters =  ReportEngine.LoadParameters(base.ReportName);
//			this.parameters.ConnectionObject = ConnectionObject.CreateInstance(conString);
//			base.CreateReportModel();
			parameters.SqlParameters.Add(new SqlParameter("@Beginning_Date",DbType.DateTime,start));
			parameters.SqlParameters.Add(new SqlParameter("@Ending_Date",DbType.DateTime,end));
			
			
			parameters.SortColumnCollection.Add(new SortColumn("ShippedDate",
			                                                   System.ComponentModel.ListSortDirection.Ascending));
		
//			MessageBox.Show("Not implemented","NorthwindSalesbyYear");
//			base.Engine.PreviewStandardReport(base.ReportName,parameters);
}
		
		
		
		public static void OnRenderSalesByYear (object sender,	SectionRenderEventArgs e){
			
			switch (e.CurrentSection) {
				case GlobalEnums.ReportSection.ReportHeader:
					break;

				case GlobalEnums.ReportSection.ReportPageHeader:
//					BaseDataItem ss = (BaseDataItem)e.Section.Items.Find("ParamFrom");
//					string startval = String.Format("[{0}]",start);
//					ss.DBValue = startval;
//					BaseDataItem ee = (BaseDataItem)e.Section.Items.Find("ParamTo");
//					string endval = String.Format("[{0}]",end);
//					ee.DBValue = endval;
					break;
					
				case GlobalEnums.ReportSection.ReportDetail:
//					BaseRowItem rowItem = e.Section.Items[0] as BaseRowItem;
//					BaseDataItem ss1 = (BaseDataItem)rowItem.Items.Find("reportDbTextItem1");
//					System.Console.WriteLine("{0}",ss1.FormatString);
					break;
					
				case GlobalEnums.ReportSection.ReportPageFooter:
					break;
					
				case GlobalEnums.ReportSection.ReportFooter:
//					System.Console.WriteLine("\tReportFooter");
					break;
					
				default:
					break;
			}
		}
		public ReportParameters Parameters {
			get { return parameters; }
		}
		
		
	}
}

