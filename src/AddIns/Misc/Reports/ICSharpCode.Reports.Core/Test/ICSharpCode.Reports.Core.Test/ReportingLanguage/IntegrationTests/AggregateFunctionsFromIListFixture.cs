/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 24.07.2009
 * Zeit: 20:08
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Data;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.ReportingLanguage.IntegrationTests
{
	[TestFixture]
	[SetCulture("de-DE")]
	public class AggregateFunctionsFromIListFixture
	{
		private IExpressionEvaluatorFacade evaluator;
		private SinglePage singlePage;
		private IDataManager dataManager;
		private AggregateCollection aggregateCollection ;
		private int intResult;
		private double doubleResult;
		
		#region CheckDataSource
		
		[Test]
		public void CheckTable ()
		{
			Assert.AreEqual(4,this.aggregateCollection.Count);
		}
		
		[Test]
		public void CheckDataManager()
		{
			Assert.IsNotNull(this.dataManager);
			IDataNavigator n = this.singlePage.IDataNavigator;
			Assert.AreEqual(this.aggregateCollection.Count,n.Count);
		}
		
		#endregion
		
		#region Count()
		
		[Test]
		public void Can_Count_With_NamedField()
		{
			
			const string expression = "=count(IntValue)";
			Assert.That(this.evaluator.Evaluate(expression),
			            Is.EqualTo(this.aggregateCollection.Count.ToString()));
		}
		
		[Test]
		public void Can_Count_From_IList()
		{
			const string expression = "=count()";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(this.aggregateCollection.Count.ToString()));
		}
		
		
		[Test]
        public void Count_Only_If_Matches()
        {
           
           const string expression = "=count(intvalue,current > 2)";
           Assert.That(this.evaluator.Evaluate(expression),
                       Is.EqualTo("2"));
        }
        
        [Test]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void Throw_On_Unknown_Field ()
		{
			const string expression = "=count(unknown)";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(this.aggregateCollection.Count));
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
		[ExpectedException(typeof(FieldNotFoundException))]
		public void Throw_On_UnknownField ()
		{
			const string expression = "=max(Unknown)";
			Assert.That(this.evaluator.Evaluate(expression), Is.EqualTo(this.aggregateCollection.Count.ToString()));
		}
		
		#endregion

		
		
		[TestFixtureSetUp]
		public void Init()
		{
			this.evaluator = new ExpressionEvaluatorFacade();
			this.singlePage = TestHelper.CreateSinglePage();
			this.evaluator.SinglePage = this.singlePage;
			

			
			AggregateFuctionHelper ah = new AggregateFuctionHelper();
			this.aggregateCollection = ah.AggregateCollection;
			
			foreach (Aggregate r in this.aggregateCollection)
			{
				this.intResult = this.intResult + r.IntValue;
				this.doubleResult = this.doubleResult + r.Amount;
			}
			
			this.dataManager = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.aggregateCollection, new ReportSettings());
			this.singlePage.IDataNavigator = this.dataManager.GetNavigator;
		}
	}
}
