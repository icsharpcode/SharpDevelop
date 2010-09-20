/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.06.2010
 * Time: 19:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Data;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage.IntegrationTests
{
	[TestFixture]
	public class ConcatFieldsFixture
	{
		
		private IExpressionEvaluatorFacade evaluator;
		private DataTable testTable;
		private SinglePage singlePage;
		private IDataManager dataManager;

		
		[Test]
		public void CheckTable ()
		{
			Assert.AreEqual(4,this.testTable.Rows.Count);
		}
		
		[Test]
		
		public void Return_FieldName_If_DataNavigator_Invalid()
		{
			const string expression = "=Fields!Name";
			this.singlePage.IDataNavigator.Reset();
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("Name"));
		}
		
		
		[Test]
		public void Can_Compile_Misspelled_Simple_FieldReference()
		{
			const string expression = "=fields!Name";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("Name"));
		}
		
		
		[Test]
		public void Evaluate_If_DataNavigator_Is_Invalid()
		{
			const string expression = "=Fields!Name";
			this.singlePage.IDataNavigator.MoveNext();
			DataRow row = testTable.Rows[0];
			string testVal = row["Name"].ToString();
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(testVal));          
		}
		
		
		
		[Test]
		[Ignore]
		public void Unkown_ColumnName ()
		{
			const string expression = "=Fields!Unknown";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("Unknown"));
		}
		
		
		[Test]
		[IgnoreAttribute]
		public void ExtractFieldbyName ()
		{
			const string expression = "=Fields!Name";
			Assert.That(this.evaluator.Evaluate(expression),
			            Is.EqualTo(this.testTable.Rows.Count.ToString()));
		}
		
		[TestFixtureSetUp]
		public void Init()
		{
			this.evaluator = new ExpressionEvaluatorFacade();
			this.singlePage = TestHelper.CreateSinglePage();
			this.evaluator.SinglePage = this.singlePage;

			AggregateFuctionHelper ah = new AggregateFuctionHelper();
			this.testTable = ah.AggregateTable;
			
			
			this.dataManager = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.testTable, new ReportSettings());
			this.singlePage.IDataNavigator = this.dataManager.GetNavigator;
		}
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}
