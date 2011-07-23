// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Data;
using ICSharpCode.Reports.Core.BaseClasses;
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
		public void Return_FieldName_If_DataNavigator_Invalid()
		{
			const string expression = "=Fields!Name";
			this.singlePage.IDataNavigator.Reset();
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("Name"));
		}
		
		
		
		[Test]
		public void Evaluate_If_DataNavigator_Is_Invalid()
		{
			const string expression = "=Fields!Name";
			this.singlePage.IDataNavigator.Reset();
			this.singlePage.IDataNavigator.MoveNext();
			DataRow row = testTable.Rows[0];
			string testVal = row["Name"].ToString();
			var retVal = this.evaluator.Evaluate(expression);
			Assert.That(retVal, Is.EqualTo(testVal));
		}

		
		[Test]
		public void Unkown_ColumnName ()
		{
			const string expression = "=Fields!Unknown";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("Unknown"));
		}
		
		
		[Test]
		public void ConcatStringAndFuction ()
		{
			 const string expression = "='testvalue :' + count()";
			 Assert.That(this.evaluator.Evaluate(expression),Is.StringStarting("test").And.StringEnding(""));	  
		}
		
		
		[Test]
		public void Extract_Field_ByName ()
		{
			const string expression = "=Fields!Name";
			this.singlePage.IDataNavigator.Reset();
			this.singlePage.IDataNavigator.MoveNext();
			DataRow r = testTable.Rows[0];
			string result = r["Name"].ToString();
			Assert.That(this.evaluator.Evaluate(expression),Is.EqualTo(result));            	            
		}
		
		
		[TestFixtureSetUp]
		public void Init()
		{
		
			this.singlePage = TestHelper.CreateSinglePage();
			this.evaluator = new ExpressionEvaluatorFacade(this.singlePage);
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
