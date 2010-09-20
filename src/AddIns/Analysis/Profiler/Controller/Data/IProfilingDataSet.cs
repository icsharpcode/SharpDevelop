// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		/// Gets whether this dataset is the first dataset of a profiling run.
		/// </summary>
		bool IsFirst { get; }
		
		/// <summary>
		/// Gets the root node of the dataset.
		/// </summary>
		CallTreeNode RootNode { get; }
	}
}
