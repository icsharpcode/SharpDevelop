// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// Describes a CallTreeNode which allows direct access to the shared memory of an existing Profiler. It depends on an existing Profiler.
	/// </summary>
	sealed unsafe class UnmanagedCallTreeNode32 : CallTreeNode
	{
		FunctionInfo *data;
		CallTreeNode parent;
		const ulong CpuCycleMask = 0x007fffffffffffffL;
		UnmanagedProfilingDataSet dataSet;
		
		internal UnmanagedCallTreeNode32(UnmanagedProfilingDataSet dataSet, FunctionInfo *data, CallTreeNode parent)
		{
			this.data = data;
			this.dataSet = dataSet;
			this.parent = parent;
		}
		
		public override System.Linq.IQueryable<CallTreeNode> Children {
			get {
				dataSet.VerifyAccess();
				
				List<UnmanagedCallTreeNode32> children = new List<UnmanagedCallTreeNode32>();

				TargetProcessPointer32* childrenPtr = FunctionInfo.GetChildren32(data);
				for (int i = 0; i <= data->LastChildIndex; i++)
				{
					FunctionInfo* child = dataSet.GetFunctionInfo(childrenPtr[i]);
					if (child != null)
						children.Add(new UnmanagedCallTreeNode32(dataSet, child, this));
				}
				
				children.Sort((a,b) => a.Index.CompareTo(b.Index));
				
				return children.Cast<CallTreeNode>().AsQueryable(); // TODO : remove Cast<> in .NET 4.0
			}
		}
		
		public override NameMapping NameMapping {
			get {
				return this.dataSet.GetMapping(this.data->Id);
			}
		}

		public override int RawCallCount {
			get {
				dataSet.VerifyAccess(); // need to verify before deferencing data
				return this.data->CallCount;
			}
		}

		public int Index {
			get {
				dataSet.VerifyAccess(); // need to verify before deferencing data
				return (int)(this.data->TimeSpent >> 56);
			}
		}

		public override bool IsActiveAtStart {
			get	{
				dataSet.VerifyAccess(); // need to verify before deferencing data
				return (this.data->TimeSpent & ((ulong)1 << 55)) != 0;
			}
		}

		public override long CpuCyclesSpent {
			get	{
				dataSet.VerifyAccess(); // need to verify before deferencing data
				return (long)(this.data->TimeSpent & CpuCycleMask);
			}
		}

		public override CallTreeNode Parent {
			get {
				return this.parent;
			}
		}
		
		public override double TimeSpent {
			get {
				return this.CpuCyclesSpent / (1000.0 * this.dataSet.ProcessorFrequency);
			}
		}
		
		public override double TimeSpentSelf {
			get {
				return this.CpuCyclesSpentSelf / (1000.0 * this.dataSet.ProcessorFrequency);
			}
		}

		public override CallTreeNode Merge(IEnumerable<CallTreeNode> nodes)
		{
			throw new NotImplementedException();
		}
		
		public override IQueryable<CallTreeNode> Callers {
			get {
				return GetCallers().AsQueryable();
			}
		}
		
		IEnumerable<CallTreeNode> GetCallers()
		{
			if (parent != null) yield return parent;
		}
		
		public override bool Equals(CallTreeNode other)
		{
			if (other is UnmanagedCallTreeNode32) {
				UnmanagedCallTreeNode32 node = other as UnmanagedCallTreeNode32;
				
				return node.data == this.data;
			}
			
			return false;
		}
		
		public override int GetHashCode()
		{
			return (new IntPtr(data)).GetHashCode();
		}
	}
}
