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
	/// Description of UnitTestWriter.
	/// </summary>
	public class UnitTestWriter : IProfilingDataWriter
	{
		IProfilingDataWriter targetWriter;
		string[] unitTestNames;
		
		public UnitTestWriter(IProfilingDataWriter targetWriter, string[] unitTestNames)
		{
			if (targetWriter == null)
				throw new ArgumentNullException("targetWriter");
			
			if (unitTestNames == null)
				throw new ArgumentNullException("unitTestNames");
			
			this.targetWriter = targetWriter;
			this.unitTestNames = unitTestNames;
		}
		
		sealed class UnitTestDataSet : IProfilingDataSet
		{
			public UnitTestDataSet(CallTreeNode root, bool isFirst, double cpuUsage)
			{
				this.RootNode = root;
				this.IsFirst = isFirst;
				this.CpuUsage = cpuUsage;
			}
			
			public double CpuUsage { get; private set; }
			
			public bool IsFirst { get; private set; }
			
			public CallTreeNode RootNode { get; private set; }
		}
		
		public int ProcessorFrequency {
			get { return this.targetWriter.ProcessorFrequency; }
			set { this.targetWriter.ProcessorFrequency = value; }
		}
		
		public void WriteDataSet(IProfilingDataSet dataSet)
		{
			if (dataSet == null)
				throw new ArgumentNullException("dataSet");
			
			List<CallTreeNode> list = new List<CallTreeNode>();

			FindUnitTests(dataSet.RootNode, list);
			
			if (list.Count > 0) {
				this.targetWriter.WriteDataSet(
					new UnitTestDataSet(new UnitTestRootCallTreeNode(list), dataSet.IsFirst, dataSet.CpuUsage)
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
		
		public void WriteMappings(System.Collections.Generic.IEnumerable<NameMapping> mappings)
		{
			this.targetWriter.WriteMappings(mappings);
		}
		
		public void Close()
		{
			this.targetWriter.Close();
		}
	}
}
