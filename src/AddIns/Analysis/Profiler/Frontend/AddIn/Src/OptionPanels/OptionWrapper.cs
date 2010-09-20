// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.Profiler.Controller.Data;

namespace ICSharpCode.Profiler.AddIn.OptionPanels
{
	public static class OptionWrapper
	{
		static Properties properties = PropertyService.Get("ProfilerOptions", new Properties());
		
		public static int SharedMemorySize {
			get { return properties.Get("SharedMemorySize", ProfilerOptions.DefaultSharedMemorySize) / 1024 / 1024; }
			set { properties.Set("SharedMemorySize", value * 1024 * 1024); }
		}
		
		public static bool EnableDC {
			get { return !properties.Get("EnableDC", true); }
			set { properties.Set("EnableDC", !value); }
		}
		
		public static bool DoNotProfileNetInternals {
			get { return properties.Get("DoNotProfileNetInternals", false); }
			set { properties.Set("DoNotProfileNetInternals", value); }
		}
		
		public static bool CombineRecursiveCalls {
			get { return properties.Get("CombineRecursiveFunction", false); }
			set { properties.Set("CombineRecursiveFunction", value); }
		}
		
		public static bool EnableDCAtStart {
			get { return properties.Get("EnableDCAtStart", true); }
			set { properties.Set("EnableDCAtStart", value); }
		}
		
		public static bool TrackEvents {
			get { return properties.Get("TrackEvents", false); }
			set { properties.Set("TrackEvents", value); }
		}
		
		public static ProfilerOptions CreateProfilerOptions()
		{
			return new ProfilerOptions(
				properties.Get("EnableDC", true),
				properties.Get("SharedMemorySize", ProfilerOptions.DefaultSharedMemorySize),
				properties.Get("DoNotProfileNetInternals", false),
				properties.Get("CombineRecursiveFunction", false),
				properties.Get("EnableDCAtStart", true),
				properties.Get("TrackEvents", false),
				ProfilerOptions.DefaultCounters
			);
		}
	}
}
