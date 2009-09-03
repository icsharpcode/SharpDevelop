// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// Represents a dataset from a profiling session.
	/// </summary>
	/// <remarks>
	/// All members of this class are thread-safe. However, calling members of this class
	/// might be unsafe when the data source (<see cref="ProfilingDataProvider"/> or
	/// <see cref="UnmanagedProfilingDataSet"/>) is concurrently disposed. See the documentation
	/// of the data source for details.
	/// </remarks>
	public interface IProfilingDataSet 
	{
		/// <summary>
		/// Gets the percent of CPU power used by the profilee at a certain point.
		/// </summary>
		double CpuUsage { get; }
		
		/// <summary>
		/// Gets whether this dataset is the first dataset of a profiling run.
		/// </summary>
		bool IsFirst { get; }
		
		/// <summary>
		/// Gets the root node of the dataset.
		/// </summary>
		CallTreeNode RootNode { get; }
	}
}
