// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>


using System;
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
			NameMapping method1 = new NameMapping(1, "r1", "m1", new List<string>());
			NameMapping method2 = new NameMapping(2, "r2", "m2", new List<string>());
			NameMapping method3 = new NameMapping(3, "r3", "m3", new List<string>());
			using (var writer = new ProfilingDataSQLiteWriter("temp.sdps", false, null)) {
				writer.ProcessorFrequency = 2000; // MHz
				writer.WriteMappings(new[] { method1, method2, method3 } );
				CallTreeNodeStub dataSet;
				dataSet = new CallTreeNodeStub {
					NameMappingValue = method1,
					AddChildren = {
						new CallTreeNodeStub {
							NameMappingValue = method2,
							RawCallCountValue = 10,
							CpuCyclesSpentValue = 500 * k
						}
					}
				};
				writer.WriteDataSet(new DataSetStub { CpuUsage = 0.3, IsFirst = true, RootNode = dataSet });
				dataSet = new CallTreeNodeStub {
					NameMappingValue = method1,
					IsActiveAtStartValue = true,
					AddChildren = {
						new CallTreeNodeStub {
							NameMappingValue = method2,
							RawCallCountValue = 0,
							IsActiveAtStartValue = true,
							CpuCyclesSpentValue = 200 * k
						},
						new CallTreeNodeStub {
							NameMappingValue = method3,
							RawCallCountValue = 1,
							IsActiveAtStartValue = true,
							CpuCyclesSpentValue = 300 * k
						}
					}
				};
				writer.WriteDataSet(new DataSetStub { CpuUsage = 0.4, IsFirst = false, RootNode = dataSet });
			}
			provider = new ProfilingDataSQLiteProvider("test.sdps");
		}
		
		[TestFixtureTearDown]
		public void FixtureCleanUp()
		{
			provider.Dispose();
			File.Delete("temp.sdps");
		}
		
		[Test]
		public void TestMethod()
		{
			// TODO: Add your test.
		}
	}
}
