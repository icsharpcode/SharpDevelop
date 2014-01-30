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
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Profiler.Controller.Data;

namespace ICSharpCode.Profiler.Controller
{
	/// <summary>
	/// Holds all settings for the profiler chosen by the user.
	/// </summary>
	public class ProfilerOptions
	{
		/// <summary>
		/// Defines the default size of the shared memory.
		/// </summary>
		public const int DefaultSharedMemorySize = 64 * 1024 * 1024; // 64 mb
		
		/// <summary>
		/// Defines a list of default performance counters.
		/// </summary>
		public static readonly PerformanceCounterDescriptor[] DefaultCounters = new[] {
			new PerformanceCounterDescriptor("Process", "% Processor Time", "_Total", ".", 0, 0, null, "%", "0.00"),
			new PerformanceCounterDescriptor("Process", "IO Data Bytes/sec", "_Total", ".", 0, null, null, "bytes/sec", "#,##0")
		};
		
		bool enableDC;
		bool enableDCAtStart;
		bool dotNotProfileDotNetInternals;
		bool combineRecursiveFunction;
		bool trackEvents;
		int sharedMemorySize;
		
		PerformanceCounterDescriptor[] counters;
		
		/// <summary>
		/// Gets the performance counters selected for monitoring.
		/// </summary>
		public PerformanceCounterDescriptor[] Counters {
			get { return counters; }
		}
		
		/// <summary>
		/// Gets whether .NET internal calls are profiled or not.
		/// </summary>
		public bool DoNotProfileDotNetInternals {
			get { return dotNotProfileDotNetInternals; }
		}
		
		/// <summary>
		/// Gets whether recursive functions calls are combined or not.
		/// </summary>
		public bool CombineRecursiveFunction {
			get { return combineRecursiveFunction; }
		}
		
		/// <summary>
		/// Gets whether data collection is enabled during profiling sessions.
		/// </summary>
		public bool EnableDC {
			get { return enableDC; }
		}
		
		/// <summary>
		/// Gets whether data collection is enabled at the start of the profiling session.
		/// </summary>
		public bool EnableDCAtStart {
			get { return enableDCAtStart; }
		}
		
		/// <summary>
		/// Gets the size of the shared memory.
		/// </summary>
		public int SharedMemorySize {
			get { return sharedMemorySize; }
		}
		
		/// <summary>
		/// Gets whether events should be tracked or not.
		/// </summary>
		public bool TrackEvents {
			get { return trackEvents; }
		}
		
		/// <summary>
		/// Creates new ProfilerOptions using the selected settings.
		/// </summary>
		public ProfilerOptions(bool enableDC = true,
		                       int sharedMemorySize = DefaultSharedMemorySize,
		                       bool profileDotNetInternals = false,
		                       bool combineRecursiveFunction = false,
		                       bool enableDCAtStart = true,
		                       bool trackEvents = false,
		                       IEnumerable<PerformanceCounterDescriptor> counters = null)
		{
			this.enableDC = enableDC;
			this.sharedMemorySize = sharedMemorySize;
			this.dotNotProfileDotNetInternals = profileDotNetInternals;
			this.combineRecursiveFunction = combineRecursiveFunction;
			this.enableDCAtStart = enableDCAtStart;
			this.trackEvents = trackEvents;
			this.counters = (counters ?? DefaultCounters).ToArray();
		}
	}
}
