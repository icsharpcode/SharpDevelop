// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

using ICSharpCode.Reports.Core;

namespace ReportSamples
{
	/// <summary>
	/// Description of MissingConnectionString.
	/// </summary>
	public class ProviderIndependent:BaseSample
	{
		/*
		Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\SharpReport_TestReports\Nordwind.mdb;Persist Security Info=False
D:\SharpReport_TestReports\TestReports
		 
		 */
		string conOleDbString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\SharpReport_TestReports\TestReports\Nordwind.mdb;Persist Security Info=False";
//		string conSqlConString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Northwind";
		ReportParameters parameters;
		
		public ProviderIndependent():base(){
			base.Run();
			parameters =  ReportEngine.LoadParameters(base.ReportName);
		
			ConnectionObject con = ConnectionObject.CreateInstance(this.conOleDbString,
			                                                       System.Data.Common.DbProviderFactories.GetFactory("System.Data.OleDb") );
			
			parameters.ConnectionObject = con;
		}
	
		public ReportParameters Parameters {
			get { return parameters; }
		}
		
	}
}

