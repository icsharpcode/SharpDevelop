// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Data;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Reports.Core.BaseClasses;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage.IntegrationTests
{
	[TestFixture]

//	[SetCulture("en-US")]
	public class AggregateFunctionsFromTableFixture
	{
		
		private IExpressionEvaluatorFacade evaluator;
		private DataTable testTable;
		private SinglePage singlePage;
		private IDataManager dataManager;
		private int intResult;
		private double doubleResult;
		
		
		#region CheckDataSource
		
		[Test]
		public void CheckTable ()
		{
			Assert.AreEqual(4,this.testTable.Rows.Count);
		}
		
		[Test]
		public void CheckDataManager()
		{
			Assert.IsNotNull(this.dataManager);
			IDataNavigator n = this.singlePage.IDataNavigator;
			Assert.AreEqual(this.testTable.Rows.Count,n.Count);
		}
		
		#endregion
		
		#region Count()
		
		[Test]
		public void Can_Count_With_NamedField()
		{
			
			const string expression = "=count(IntValue)";
			Assert.That(this.evaluator.Evaluate(expression),
			            Is.EqualTo(this.testTable.Rows.Count.ToString()));
		}
		
		
		[Test]
		public void Can_Count_From_Table()
		{
			const string expression = "=count()";
			Assert.That(this.evaluator.Evaluate(expression),
			            Is.EqualTo(this.testTable.Rows.Count.ToString()));
		}
		
		
		[Test]
        public void Count_Only_If_Matches()
        {
           
           const string expression = "=count(intvalue,current > 2)";
           Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo("2"));           
        }
		
		
		#endregion
		
		
		#region sum()
		
		[Test]
		public void Can_Sum_Integer ()
		{
			const string expression = "=sum(IntValue)";
			Assert.That(this.evaluator.Evaluate(expression),
			            Is.EqualTo(this.intResult.ToString()));
		}
		
		
		[Test]
		public void Can_Sum_Double ()
		{
			const string expression = "=sum(amount)";
			Assert.That(this.evaluator.Evaluate(expression),
			            Is.EqualTo(this.doubleResult.ToString()));
		}
		
		#endregion
		
		#region Max-min
		
		[Test]
		public void Can_Look_For_MaxValue()
		{
			const string expression = "=max(amount)";
			//var s = this.evaluator.Evaluate(expression);
			Assert.That(this.evaluator.Evaluate(expression),
			            Is.EqualTo("400,5"));
		}
		
		
		[Test]
		public void Can_Look_For_MinValue()
		{
			const string expression = "=Min(intvalue)";
			Assert.That(this.evaluator.Evaluate(expression),
			            Is.EqualTo("1"));
		}
		
		
		[Test]
		public void UnknownField_ErrorMessage ()
		{
			const string expression = "=max(Unknown)";
			string s  = this.evaluator.Evaluate(expression);
			Assert.That(s.Contains("not found"));
		}
		
		#endregion

		
		[TestFixtureSetUp]
		public void Init()
		{
			this.singlePage = TestHelper.CreateSinglePage();
			this.evaluator = new ExpressionEvaluatorFacade(this.singlePage);

			AggregateFuctionHelper ah = new AggregateFuctionHelper();
			this.testTable = ah.AggregateTable;
			
			foreach (DataRow r in this.testTable.Rows)
			{
				this.intResult = this.intResult + Convert.ToInt16(r["IntValue"]);
				this.doubleResult = this.doubleResult + Convert.ToDouble(r["Amount"]);
			}
			
			this.dataManager = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.testTable, new ReportSettings());
			this.singlePage.IDataNavigator = this.dataManager.GetNavigator;
		}
	}
}
