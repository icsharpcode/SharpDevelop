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
using System.Linq;
using System.Collections.Generic;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// Filters the data being written to remove NUnit internals and show the unit tests more clearly.
	/// </summary>
	public class UnitTestWriter : IProfilingDataWriter
	{
		IProfilingDataWriter targetWriter;
		HashSet<string> unitTestNames;
		
		/// <summary>
		/// Creates a new UnitTestWriter instance.
		/// </summary>
		/// <param name="targetWriter">The target IProfilingDataWriter where the output should be written to.</param>
		/// <param name="unitTestNames">The fully qualified names of the unit test methods.</param>
		public UnitTestWriter(IProfilingDataWriter targetWriter, string[] unitTestNames)
		{
			if (targetWriter == null)
				throw new ArgumentNullException("targetWriter");
			
			if (unitTestNames == null)
				throw new ArgumentNullException("unitTestNames");
			
			this.targetWriter = targetWriter;
			this.unitTestNames = new HashSet<string>(unitTestNames);
		}
		
		sealed class UnitTestDataSet : IProfilingDataSet
		{
			public UnitTestDataSet(CallTreeNode root, bool isFirst)
			{
				this.RootNode = root;
				this.IsFirst = isFirst;
			}
			
			public bool IsFirst { get; private set; }
			
			public CallTreeNode RootNode { get; private set; }
		}
		
		/// <inheritdoc/>
		public int ProcessorFrequency {
			get { return targetWriter.ProcessorFrequency; }
			set { targetWriter.ProcessorFrequency = value; }
		}
		
		/// <inheritdoc/>
		public void WriteDataSet(IProfilingDataSet dataSet)
		{
			if (dataSet == null)
				throw new ArgumentNullException("dataSet");
			
			List<CallTreeNode> list = new List<CallTreeNode>();

			FindUnitTests(dataSet.RootNode, list);
			
			if (list.Count > 0) {
				targetWriter.WriteDataSet(
					new UnitTestDataSet(new UnitTestRootCallTreeNode(list), dataSet.IsFirst)
				);
			} else {
				// proposed fix for http://community.sharpdevelop.net/forums/t/10533.aspx
				// discuss with Daniel
				targetWriter.WriteDataSet(dataSet);
			}
		}
		
		bool IsUnitTest(NameMapping name)
		{
			return unitTestNames.Contains(name.Name);
		}
		
		void FindUnitTests(CallTreeNode parentNode, IList<CallTreeNode> list)
		{
			if (IsUnitTest(parentNode.NameMapping)) {
				list.Add(parentNode);
				return;
			}
			
			foreach (var node in parentNode.Children) {
				FindUnitTests(node, list);
			}
		}
		
		/// <inheritdoc/>
		public void WriteMappings(IEnumerable<NameMapping> mappings)
		{
			targetWriter.WriteMappings(mappings);
		}
		
		/// <inheritdoc/>
		public void Close()
		{
			targetWriter.Close();
		}
		
		/// <inheritdoc/>
		public void WritePerformanceCounterData(IEnumerable<PerformanceCounterDescriptor> counters)
		{
			targetWriter.WritePerformanceCounterData(counters);
		}
		
		/// <inheritdoc/>
		public void WriteEventData(IEnumerable<EventDataEntry> events)
		{
			targetWriter.WriteEventData(events);
		}
		
		/// <inheritdoc/>
		public int DataSetCount {
			get { return targetWriter.DataSetCount; }
		}
	}
}
