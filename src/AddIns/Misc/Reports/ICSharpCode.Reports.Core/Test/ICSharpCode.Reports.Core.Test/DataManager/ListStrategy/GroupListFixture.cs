// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using ICSharpCode.Reports.Core.Test.TestHelpers;
using ICSharpCode.Reports.Expressions.ReportingLanguage;
using NUnit.Framework;

namespace ICSharpCode.Reports.Core.Test.DataManager.ListStrategy
{
	[TestFixture]
	public class GroupListFixture
	{
		
		ContributorCollection contributorCollection;
		
		
		#region standard test's
		
		[Test]
		public void AvaiableFields_Should_Be_Set()
		{
			var dataNavigator = PrepareStringGrouping();
			dataNavigator.MoveNext();
			IDataNavigator child = dataNavigator.GetChildNavigator;
			AvailableFieldsCollection availableFieldsCollection = child.AvailableFields;
			Assert.That(availableFieldsCollection,Is.Not.Null);
			Assert.That(availableFieldsCollection.Count,Is.GreaterThan(0));
		}
		
		#endregion
		
		#region Group by StringValue
		
		[Test]
		public void Has_Children()
		{
			var dataNavigator = PrepareStringGrouping();
			while (dataNavigator.MoveNext()) {
				Assert.That(dataNavigator.HasChildren,Is.True);
			}
		}
		
		
		[Test]
		public void Can_Read_Child_Count ()
		{
			var dataNavigator = PrepareStringGrouping();
			while (dataNavigator.MoveNext())
			{
				if (dataNavigator.HasChildren)
				{
					var childNavigator = dataNavigator.GetChildNavigator;
					Assert.That(childNavigator.Count,Is.GreaterThan(0));
				}
			}
		}
		
		
		[Test]
		public void Can_FillChild()
		{
			var dataNavigator = PrepareStringGrouping();
			ReportItemCollection searchCol = new ReportItemCollection();
			
			searchCol.Add(new BaseDataItem ()
			              {
			              	Name ="Last",
			              	ColumnName ="Last"
			              }
			             );
			searchCol.Add(new BaseDataItem ()
			              {
			              
			              	ColumnName ="GroupItem"
			              }
			             );
			dataNavigator.Reset();
			dataNavigator.MoveNext();

			do
			{
				if (dataNavigator.HasChildren)
				{
					dataNavigator.Fill(searchCol);
					var b = (BaseDataItem)searchCol[1];
					Console.WriteLine("-- <{0}>-",b.DBValue);
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						Assert.That(dataNavigator.HasChildren,Is.True);
						childNavigator.Fill(searchCol);
						var a = (BaseDataItem)searchCol[0];
						Console.WriteLine ("\t{0}",a.DBValue);
				
					}
					while (childNavigator.MoveNext());
				}
			}
			while (dataNavigator.MoveNext());
		}
		
		
		[Test]
		public void Collection_Contains_Subclass ()
		{
			var modifyedCollection = this.ModifyCollection();
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(modifyedCollection,rs);
			IDataNavigator dataNavigator = dm.GetNavigator;
			
			ReportItemCollection searchCol = new ReportItemCollection();
			searchCol.Add(new BaseDataItem ()
			              {
			              	ColumnName ="DummyClass.DummyString"			     
			              }
			             );
			searchCol.Add(new BaseDataItem ()
			              {
			              	Name ="Last",			           
			              	ColumnName ="Last"	
			              }
			             );
			
			searchCol.Add(new BaseDataItem ()
			              {
			             			           
			              	ColumnName ="GroupItem"	
			              }
			             );
			
			dataNavigator.Reset();
			dataNavigator.MoveNext();

			do
			{
				dataNavigator.Fill(searchCol);
				var a1 = (BaseDataItem)searchCol[0];
				var b1 = (BaseDataItem)searchCol[2];	
				Console.WriteLine ("-----{0} - {1}------",a1.DBValue,b1.DBValue);
							
				if (dataNavigator.HasChildren)
				{
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						childNavigator.Fill(searchCol);
						var a = (BaseDataItem)searchCol[0];
						var b = (BaseDataItem)searchCol[1];
						var c = (BaseDataItem)searchCol[2];
						Console.WriteLine ("\t{0} - {1} - {2}",a.DBValue,b.DBValue,c.DBValue);

					}
					while (childNavigator.MoveNext());
				}
			}
			while (dataNavigator.MoveNext());
		}
		
		#endregion
		
		#region FieldReference
		
		[Test]
		public void Check_Field_Reference()
		{
			var modifyedCollection = this.ModifyCollection();
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(modifyedCollection,rs);
			IDataNavigator dataNavigator = dm.GetNavigator;
			
//			IExpressionEvaluatorFacade  evaluator = new ExpressionEvaluatorFacade();
			
			ReportItemCollection searchCol = new ReportItemCollection();
			
			searchCol.Add(new BaseDataItem ()
			              {
			              	ColumnName ="DummyClass.DummyString"			     
			              }
			             );
			searchCol.Add(new BaseDataItem ()
			              {
			              	Name ="Last",			           
			              	ColumnName ="Last"	
			              }
			             );
			
			searchCol.Add(new BaseDataItem ()
			              {
			             			           
			              	ColumnName ="GroupItem"	
			              }
			             );
//			searchCol.Add(new BaseTextItem()
//			              {
//			              	Name ="FieldRef",
//			              	Text ="=Fields!Last"
//			              }
//			             );
			              
			dataNavigator.Reset();
			dataNavigator.MoveNext();

			do
			{
				
				dataNavigator.Fill(searchCol);
				var a1 = (BaseDataItem)searchCol[0];
				var b1 = (BaseDataItem)searchCol[2];	
				Console.WriteLine ("-----{0} - {1}------",a1.DBValue,b1.DBValue);
							
				if (dataNavigator.HasChildren)
				{
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						var dataRow = childNavigator.GetDataRow;
						var item = dataRow.Find("Last");
//						childNavigator.Fill(searchCol);
//						var a = (BaseDataItem)searchCol[0];
//						var b = (BaseDataItem)searchCol[1];
//						var c = (BaseDataItem)searchCol[3];
//						Console.WriteLine ("\t{0} - {1} - {2}",a.DBValue,b.DBValue,c.DBValue);
						                        Console.WriteLine(item.Value);
					}
					while (childNavigator.MoveNext());
				}
			}
			while (dataNavigator.MoveNext());
		}
	
		#endregion
		
		
		#region Group by DataTime
		
		[Test]
		public void DateTimeCan_FillChild()
		{
			var dataNavigator = PrepareDateTimeGrouping();
			
			Console.WriteLine("start datetime");
			while (dataNavigator.MoveNext()) {
				if (dataNavigator.HasChildren) {
					var childNavigator = dataNavigator.GetChildNavigator;
					do
					{
						Assert.That(dataNavigator.HasChildren,Is.True);
						// we know that current is a 'contributor'
						Contributor c = dataNavigator.Current as Contributor;
						string v2 = c.Last + " GroupVal :" +  c.RandomDate;
						Console.WriteLine(v2);
					}
					while (childNavigator.MoveNext());
				}
				
			}
		}
		
		#endregion
		
		private ContributorCollection ModifyCollection ()
		{
			var newcol = this.contributorCollection;
			MyDummyClass dummy;
			int start = 0;
			foreach (var element in newcol) 
			{
				dummy = new MyDummyClass();
				dummy.DummyString = "dummy" + start.ToString();
				dummy.DummyInt = start;
				element.DummyClass = dummy;
				start ++;
			}
			return newcol;
		}
		
		
		private IDataNavigator PrepareDateTimeGrouping ()
		{
			GroupColumn gc = new GroupColumn("RandomDate",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			return dm.GetNavigator;
		}
		
		
		private IDataNavigator PrepareStringGrouping ()
		{
			GroupColumn gc = new GroupColumn("GroupItem",1,ListSortDirection.Ascending);
			ReportSettings rs = new ReportSettings();
			rs.GroupColumnsCollection.Add(gc);
			IDataManager dm = ICSharpCode.Reports.Core.DataManager.CreateInstance(this.contributorCollection,rs);
			return dm.GetNavigator;
		}
		
		
		[TestFixtureSetUp]
		public void Init()
		{
			ContributorsList contributorsList = new ContributorsList();
			this.contributorCollection = contributorsList.ContributorCollection;
		}
		
		
		
		[TestFixtureTearDown]
		public void Dispose()
		{
			// TODO: Add tear down code.
		}
	}
}
