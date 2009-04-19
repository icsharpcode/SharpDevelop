// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
		/// Closes and disposes the underlying data structure.
		/// </summary>
		void Close();
		
		/// <summary>
		/// The processor speed read from the registry of the computer where the profiling session was created on.
		/// The processor frequency is measured in MHz.
		/// </summary>
		int ProcessorFrequency { get; set; }
	}
}
