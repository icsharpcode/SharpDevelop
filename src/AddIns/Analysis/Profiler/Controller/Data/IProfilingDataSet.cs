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
