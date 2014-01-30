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
