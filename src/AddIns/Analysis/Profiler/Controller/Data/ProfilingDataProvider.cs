// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// Base implementation for a ProfilingDataProvider.
	/// </summary>
	public abstract class ProfilingDataProvider
	{
		/// <summary>
		/// Closes and disposes the provider.
		/// </summary>
		/// <remarks>
		/// Using a closed provider results in an ObjectDisposedException.
		/// Calls that are active during the call to Close() will either throw
		/// an ObjectDisposedException or finish successfully.
		/// </remarks>
		public abstract void Close();
		
		/// <summary>
		/// Returns the root-CallTreeNode for a range of datasets.
		/// </summary>
		/// <param name="startIndex">The index of the first dataset to return.</param>
		/// <param name="endIndex">The index of the last dataset to return.</param>
		public abstract CallTreeNode GetRoot(int startIndex, int endIndex);
		
		/// <summary>
		/// Returns a name mapping for a given name id.
		/// </summary>
		public abstract NameMapping GetMapping(int nameId);
		
		/// <summary>
		/// Gets all datasets in the session.
		/// </summary>
		public abstract ReadOnlyCollection<IProfilingDataSet> DataSets { get; }
		
		/// <summary>
		/// The processor speed read from the registry of the computer where the profiling session was created on.
		/// The processor frequency is measured in MHz.
		/// </summary>
		public abstract int ProcessorFrequency { get; }
		
		/// <summary>
		/// Sets a property consisting of a name-value pair. (e. g. version number)
		/// Setting a property to null will remove the property.
		/// </summary>
		public abstract void SetProperty(string name, string value);
		
		/// <summary>
		/// Gets the value of a property identified by its name.
		/// </summary>
		public abstract string GetProperty(string name);
		
		/// <summary>
		/// Returns the list of all calls in a specified range of datasets.
		/// </summary>
		public virtual IQueryable<CallTreeNode> GetAllCalls(int startIndex, int endIndex)
		{
			return GetRoot(startIndex, endIndex).Descendants;
		}
		
		/// <summary>
		/// Returns the list of all functions called in a specified range of datasets.
		/// </summary>
		public virtual IQueryable<CallTreeNode> GetFunctions(int startIndex, int endIndex)
		{
			return GetAllCalls(startIndex, endIndex).Where(c => !c.IsThread).MergeByName();
		}
		
		/// <summary>
		/// Returns the list of performance counters used in all datasets.
		/// </summary>
		public abstract PerformanceCounterDescriptor[] GetPerformanceCounters();
		
		/// <summary>
		/// Returns the list of all values collected by a performance counter identified by its index.
		/// </summary>
		public abstract float[] GetPerformanceCounterValues(int index);
		
		/// <summary>
		/// Returns the list of all events for a dataset.
		/// </summary>
		public abstract EventDataEntry[] GetEventDataEntries(int index);
	}
}
