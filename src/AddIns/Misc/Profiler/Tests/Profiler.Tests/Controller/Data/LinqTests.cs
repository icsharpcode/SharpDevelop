// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>


using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.Profiler.Controller.Data.Linq;
using NUnit.Framework;

namespace Profiler.Tests.Controller.Data
{
	[TestFixture]
	public class LinqTests
	{
		const int k = 1000;
		ProfilingDataSQLiteProvider provider;
		
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			if (File.Exists("temp.sdps"))
				File.Delete("temp.sdps");
			NameMapping method0 = new NameMapping(0, "r0", "m0", new List<string>());
			NameMapping method1 = new NameMapping(1, "r1", "m1", new List<string>());
			NameMapping method2 = new NameMapping(2, "r2", "m2", new List<string>());
			using (var writer = new ProfilingDataSQLiteWriter("temp.sdps", false, null)) {
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
				writer.WriteDataSet(new DataSetStub { CpuUsage = 0.3, IsFirst = true, RootNode = dataSet });
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
				writer.WriteDataSet(new DataSetStub { CpuUsage = 0.4, IsFirst = false, RootNode = dataSet });
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
				writer.WriteDataSet(new DataSetStub { CpuUsage = 0.1, IsFirst = false, RootNode = dataSet });
				writer.Close();
			}
			provider = new ProfilingDataSQLiteProvider("temp.sdps");
		}
		
		[TestFixtureTearDown]
		public void FixtureCleanUp()
		{
			provider.Dispose();
			File.Delete("temp.sdps");
		}
		
		[Test]
		public void TestDataSets()
		{
			Assert.AreEqual(3, provider.DataSets.Count);
			Assert.AreEqual(0.3, provider.DataSets[0].CpuUsage);
			Assert.AreEqual(0.4, provider.DataSets[1].CpuUsage);
			Assert.AreEqual(0.1, provider.DataSets[2].CpuUsage);
			
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
			CallTreeNode[] functions = provider.GetFunctions(1, 2).OrderBy(f => f.Name).ToArray();
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
	}
}
