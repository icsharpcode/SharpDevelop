// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.Profiler.Controller.Data.Linq;
using NUnit.Framework;

namespace Profiler.Tests.Controller.Data
{
	[TestFixture]
	[Ignore("Disabled because SQLite does not work in .NET 4.0 Beta 2")]
	public class LinqTests
	{
		const int k = 1000;
		ProfilingDataSQLiteProvider provider;
		
		const string databaseFileName = "test.sdps";
		
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			if (File.Exists(databaseFileName))
				File.Delete(databaseFileName);
			NameMapping method0 = new NameMapping(0, "r0", "m0", new List<string>());
			NameMapping method1 = new NameMapping(1, "r1", "m1", new List<string>());
			NameMapping method2 = new NameMapping(2, "r2", "m2", new List<string>());
			using (var writer = new ProfilingDataSQLiteWriter(databaseFileName)) {
				writer.ProcessorFrequency = 2000; // MHz
				writer.WriteMappings(new[] { method0, method1, method2 } );
				CallTreeNodeStub dataSet;
				dataSet = new CallTreeNodeStub {
					NameMappingValue = method0,
					AddChildren = {
						new CallTreeNodeStub {
							NameMappingValue = method1,
							RawCallCountValue = 10,
							CpuCyclesSpentValue = 500 * k
						}
					}
				};
				writer.WriteDataSet(new DataSetStub { IsFirst = true, RootNode = dataSet });
				dataSet = new CallTreeNodeStub {
					NameMappingValue = method0,
					IsActiveAtStartValue = true,
					AddChildren = {
						new CallTreeNodeStub {
							NameMappingValue = method1,
							RawCallCountValue = 0,
							IsActiveAtStartValue = true,
							CpuCyclesSpentValue = 200 * k
						},
						new CallTreeNodeStub {
							NameMappingValue = method2,
							RawCallCountValue = 1,
							CpuCyclesSpentValue = 300 * k
						}
					}
				};
				writer.WriteDataSet(new DataSetStub { IsFirst = false, RootNode = dataSet });
				dataSet = new CallTreeNodeStub {
					NameMappingValue = method0,
					IsActiveAtStartValue = true,
					AddChildren = {
						new CallTreeNodeStub {
							NameMappingValue = method2,
							RawCallCountValue = 0,
							IsActiveAtStartValue = true,
							CpuCyclesSpentValue = 50 * k,
							AddChildren = {
								new CallTreeNodeStub {
									NameMappingValue = method1,
									RawCallCountValue = 5,
									CpuCyclesSpentValue = 1 * k
								}
							}
						}
					}
				};
				writer.WriteDataSet(new DataSetStub { IsFirst = false, RootNode = dataSet });
				writer.Close();
			}
			provider = ProfilingDataSQLiteProvider.UpgradeFromOldVersion(databaseFileName);
		}
		
		[TestFixtureTearDown]
		public void FixtureCleanUp()
		{
			provider.Dispose();
			File.Delete(databaseFileName);
		}
		
		[Test]
		public void TestDataSets()
		{
			Assert.AreEqual(3, provider.DataSets.Count);
			
			Assert.IsTrue(provider.DataSets[0].IsFirst);
			Assert.IsFalse(provider.DataSets[1].IsFirst);
			Assert.IsFalse(provider.DataSets[2].IsFirst);
		}
		
		[Test]
		public void TestMergedTree()
		{
			CallTreeNode root = provider.GetRoot(0, 1);
			Assert.IsTrue(root.HasChildren);
			CallTreeNode[] children = root.Children.ToArray();
			Assert.AreEqual(2, children.Length);
			Assert.IsFalse(children[0].HasChildren);
			Assert.AreEqual("m1", children[0].Name);
			Assert.AreEqual(10, children[0].CallCount);
			Assert.AreEqual(700 * k, children[0].CpuCyclesSpent);
			
			Assert.IsFalse(children[1].HasChildren);
			Assert.AreEqual("m2", children[1].Name);
			Assert.AreEqual(1, children[1].CallCount);
			Assert.AreEqual(300 * k, children[1].CpuCyclesSpent);
		}
		
		[Test]
		public void TestFunctions()
		{
			CallTreeNode[] functions = provider.GetFunctions(1, 2).OrderBy(f => f.Name).WithQueryLog(Console.Out).ToArray();
			Assert.AreEqual(2, functions.Length);
			
			Assert.AreEqual("m1", functions[0].Name);
			Assert.IsFalse(functions[0].HasChildren);
			Assert.AreEqual(6, functions[0].CallCount);
			Assert.AreEqual(201 * k, functions[0].CpuCyclesSpent);
			
			Assert.AreEqual("m2", functions[1].Name);
			Assert.IsTrue(functions[1].HasChildren);
			Assert.AreEqual(1, functions[1].CallCount);
			Assert.AreEqual(350 * k, functions[1].CpuCyclesSpent);
		}
		
		
		[Test]
		public void TestFunctionsChildren()
		{
			CallTreeNode[] functions = provider.GetFunctions(1, 2).OrderBy(f => f.Name).WithQueryLog(Console.Out).ToArray();
			CallTreeNode[] children = functions[1].Children.ToArray();
			
			Assert.AreEqual(1, children.Length);
			Assert.AreEqual("m1", children[0].Name);
			Assert.IsFalse(children[0].HasChildren);
			Assert.AreEqual(5, children[0].CallCount);
			Assert.AreEqual(1 * k, children[0].CpuCyclesSpent);
		}
		
		[Test]
		public void TestFunctionsQuery()
		{
			var query = provider.GetFunctions(0, 1);
			Assert.AreEqual("AllFunctions(0, 1).Filter(c => (c.NameMapping.Id != 0) && c => Not(GlobImpl(c.NameMapping.Name, \"Thread#*\")))",
			                SQLiteQueryProvider.OptimizeQuery(query.Expression).ToString());
		}
		
		/*
		 We currently do not support querying on GetAllCalls - doing this would require support for merging by path id
		[Test]
		public void TestAllCallsMergedToFunctions()
		{
			var query = provider.GetAllCalls(0, 1).MergeByName();
			Assert.AreEqual("AllFunctions(0, 1).Filter(c => (c.NameMapping.Id != 0))",
			                SQLiteQueryProvider.OptimizeQuery(query.Expression).ToString());
		}
		*/
		
		[Test]
		public void TestSupportedOrderByOnRootChildren()
		{
			CallTreeNode root = provider.GetRoot(0, 0);
			var query = root.Children.OrderBy(f => f.CallCount);
			Assert.AreEqual("AllCalls.Filter(c => (c.ParentID == 0)).MergeByName().Sort(f => f.CallCount)",
			                SQLiteQueryProvider.OptimizeQuery(query.Expression).ToString());
		}
		
		[Test]
		public void TestOrderByNameMappingName()
		{
			CallTreeNode root = provider.GetRoot(0, 0);
			var query = root.Children.OrderBy(f => f.NameMapping.Name);
			Assert.AreEqual("AllCalls.Filter(c => (c.ParentID == 0)).MergeByName().Sort(f => f.NameMapping.Name)",
			                SQLiteQueryProvider.OptimizeQuery(query.Expression).ToString());
		}
		
		[Test]
		public void TestOrderByWithUnsupporedThenBy()
		{
			CallTreeNode root = provider.GetRoot(0, 0);
			var query = root.Children.OrderBy(f => f.CallCount).ThenBy(f => f.Ancestors.Count());
			Assert.AreEqual("AllCalls.Filter(c => (c.ParentID == 0)).MergeByName()" +
			                ".OrderBy(f => f.CallCount).ThenBy(f => f.Ancestors.Count())",
			                SQLiteQueryProvider.OptimizeQuery(query.Expression).ToString());
		}
		
		[Test]
		public void TestOrderBySupportedThenByButUnsupportedConverter()
		{
			CallTreeNode root = provider.GetRoot(0, 0);
			var query = root.Children.OrderBy(f => f.CallCount).ThenBy(f => f.NameMapping.Name, StringComparer.CurrentCultureIgnoreCase);
			Assert.AreEqual("AllCalls.Filter(c => (c.ParentID == 0)).MergeByName()" +
			                ".OrderBy(f => f.CallCount).ThenBy(f => f.NameMapping.Name, value(System.CultureAwareComparer))",
			                SQLiteQueryProvider.OptimizeQuery(query.Expression).ToString());
		}
		
		[Test]
		public void TestOrderByWithMultiLevelUnsupporedThenBy()
		{
			CallTreeNode root = provider.GetRoot(0, 0);
			var query = root.Children.OrderBy(f => f.CallCount).ThenBy(f => f.NameMapping.Name).ThenBy(f => f.Ancestors.Count());
			Assert.AreEqual("AllCalls.Filter(c => (c.ParentID == 0)).MergeByName()" +
			                ".OrderBy(f => f.CallCount).ThenBy(f => f.NameMapping.Name).ThenBy(f => f.Ancestors.Count())",
			                SQLiteQueryProvider.OptimizeQuery(query.Expression).ToString());
		}
	}
}
