// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>

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
			get { return this.targetWriter.ProcessorFrequency; }
			set { this.targetWriter.ProcessorFrequency = value; }
		}
		
		/// <inheritdoc/>
		public void WriteDataSet(IProfilingDataSet dataSet)
		{
			if (dataSet == null)
				throw new ArgumentNullException("dataSet");
			
			List<CallTreeNode> list = new List<CallTreeNode>();

			FindUnitTests(dataSet.RootNode, list);
			
			if (list.Count > 0) {
				this.targetWriter.WriteDataSet(
					new UnitTestDataSet(new UnitTestRootCallTreeNode(list), dataSet.IsFirst)
				);
			} else {
				this.targetWriter.WriteDataSet(
					new UnitTestDataSet(new UnitTestRootCallTreeNode(null), dataSet.IsFirst)
				);
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
		public void WriteMappings(System.Collections.Generic.IEnumerable<NameMapping> mappings)
		{
			this.targetWriter.WriteMappings(mappings);
		}
		
		/// <inheritdoc/>
		public void Close()
		{
			this.targetWriter.Close();
		}
		
		public void WritePerformanceCounterData(IEnumerable<PerformanceCounterDescriptor> counters)
		{
			this.targetWriter.WritePerformanceCounterData(counters);
		}
		
		public void WriteEventData(IEnumerable<EventDataEntry> events)
		{
			this.targetWriter.WriteEventData(events);
		}
		
		public int DataSetCount {
			get { return this.targetWriter.DataSetCount; }
		}
	}
}
