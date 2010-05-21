/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 14.05.2010
 * Time: 20:04
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Core.Project.Interfaces;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.Exporter
{
	[TestFixture]
	public class DataPageBuilderFixture:ConcernOf<IReportCreator_2>
	{
		
		IReportModel reportModel;
		
		
		[Test]
		public void Can_Create_DataReportCreator()
		{
			
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			IDataManager dataManager = ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			
			ILayouter layouter = new Layouter();
			IReportCreator_2 sut = DataReportCreator.CreateInstance(reportModel,dataManager,layouter);
			Assert.IsNotNull(sut);
		}
		
		
		[Test]
		public void Empty_ReportModel_Should_Return_Empty_List()
		{
			Sut.BuildExportList();
			Assert.AreEqual(1,Sut.Pages.Count);
		}
		
		
		
		
	
		
		public override void Setup()
		{
			reportModel = ReportModel.Create();
			ILayouter layouter = new Layouter();
			
			ReportSettings rs = new ReportSettings();
			rs.ConnectionString = "goodConnection";
			
			IDataAccessStrategy da = new MockDataAccessStrategy (rs);
			
			IDataManager dataManager = ICSharpCode.Reports.Core.DataManager.CreateInstance(rs,da);
			
			Sut =  DataReportCreator.CreateInstance(reportModel,dataManager,layouter);
		}
		
		[TestFixtureSetUp]
		public void Init()
		{
			// TODO: Add Init code.
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
	
	
	public class MockDataAccessStrategy:IDataAccessStrategy
	{
		ReportSettings reportSettings;
		
		public MockDataAccessStrategy(ReportSettings reportSettings)
		{
			this.reportSettings = reportSettings;
		}
		
		
		
		public bool OpenConnection ()
		{
			
			if (String.IsNullOrEmpty(reportSettings.ConnectionString)) {
				throw new ArgumentNullException("ConnectionString");
			}
			
			if (reportSettings.ConnectionString == "bad") {
				throw new ArgumentException();
			}
			return true;
		}
		
		public System.Data.DataSet ReadData()
		{
			ContributorsList contributorsList = new ContributorsList();
			DataSet ds = new DataSet();
			ds.Tables.Add(contributorsList.ContributorTable);
			return ds;
		}
		
	}
}
