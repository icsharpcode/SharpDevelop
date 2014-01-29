// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
		
		public override IQueryable<CallTreeNode> Children {
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
				
				return children.AsQueryable();
			}
		}
		
		public override NameMapping NameMapping {
			get {
				return dataSet.GetMapping(data->Id);
			}
		}

		public override int RawCallCount {
			get {
				dataSet.VerifyAccess(); // need to verify before deferencing data
				return data->CallCount;
			}
		}

		public int Index {
			get {
				dataSet.VerifyAccess(); // need to verify before deferencing data
				return (int)(data->TimeSpent >> 56);
			}
		}

		public override bool IsActiveAtStart {
			get	{
				dataSet.VerifyAccess(); // need to verify before deferencing data
				return (data->TimeSpent & ((ulong)1 << 55)) != 0;
			}
		}

		public override long CpuCyclesSpent {
			get	{
				dataSet.VerifyAccess(); // need to verify before deferencing data
				return (long)(data->TimeSpent & CpuCycleMask);
			}
		}
		
		public override long CpuCyclesSpentSelf {
			get {
				dataSet.VerifyAccess();
				
				long result = (long)(data->TimeSpent & CpuCycleMask);
				
				TargetProcessPointer32* childrenPtr = FunctionInfo.GetChildren32(data);
				for (int i = 0; i <= data->LastChildIndex; i++)
				{
					FunctionInfo* child = dataSet.GetFunctionInfo(childrenPtr[i]);
					if (child != null)
						result -= (long)(child->TimeSpent & CpuCycleMask);
				}
				
				return result;
			}
		}

		public override CallTreeNode Parent {
			get {
				return parent;
			}
		}
		
		public override double TimeSpent {
			get {
				return CpuCyclesSpent / (1000.0 * dataSet.ProcessorFrequency);
			}
		}
		
		public override double TimeSpentSelf {
			get {
				return CpuCyclesSpentSelf / (1000.0 * dataSet.ProcessorFrequency);
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
			UnmanagedCallTreeNode32 node = other as UnmanagedCallTreeNode32;
			if (node != null) {
				return node.data == data;
			}
			
			return false;
		}
		
		public override int GetHashCode()
		{
			return (new IntPtr(data)).GetHashCode();
		}
	}
}
