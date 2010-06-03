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
		[IgnoreAttribute]
		public void Can_Compile_Simple_FieldReference()
		{
			const string expression = "=Fields!Field1";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("Field1"));
		}
		
		
		[Test]
		[IgnoreAttribute]
		public void Can_Compile_Missspelled_Simple_FieldReference()
		{
			const string expression = "=fields!Field1";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("Field1"));
		}
		
		
		[Test]
		[IgnoreAttribute]
		public void ExtractFieldbyName ()
		{
			const string expression = "=Fields!Name";
//			Console.WriteLine("{0} - {1}",expression,evaluator.Evaluate(expression));
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
			
//			foreach (DataRow r in this.testTable.Rows)
//			{
////				this.intResult = this.intResult + Convert.ToInt16(r["IntValue"]);
////				this.doubleResult = this.doubleResult + Convert.ToDouble(r["Amount"]);
//			}
			
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
