// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace ICSharpCode.Profiler.Controller.Data
{
	/// <summary>
	/// Used to write profiling data to a format (e. g. SQLite DB, binary).
	/// Instance members of this interface are not thread-safe.
	/// </summary>
	public interface IProfilingDataWriter
	{
		/// <summary>
		/// Writes an unmanaged dataset.
		/// </summary>
		/// <param name="dataSet">The data set to write.</param>
		void WriteDataSet(IProfilingDataSet dataSet);
		
		/// <summary>
		/// Writes an amount of name mappings.
		/// </summary>
		void WriteMappings(IEnumerable<NameMapping> mappings);
		
		/// <summary>
		/// Writes an amount of performance counters and the collected values.
		/// </summary>
		void WritePerformanceCounterData(IEnumerable<PerformanceCounterDescriptor> counters);
		
		/// <summary>
		/// Writes an amount of events.
		/// </summary>
		void WriteEventData(IEnumerable<EventDataEntry> events);
		
		/// <summary>
		/// Closes and disposes the underlying data structure.
		/// </summary>
		void Close();
		
		/// <summary>
		/// The processor speed read from the registry of the computer where the profiling session was created on.
		/// The processor frequency is measured in MHz.
		/// </summary>
		int ProcessorFrequency { get; set; }
		
		/// <summary>
		/// Gets the number of datasets that have been written by this IProfilingDataWriter.
		/// </summary>
		int DataSetCount { get; }
	}
}
