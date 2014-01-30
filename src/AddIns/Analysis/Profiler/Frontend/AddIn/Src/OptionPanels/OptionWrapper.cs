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
using ICSharpCode.Core;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Profiler.AddIn.OptionPanels
{
	public static class OptionWrapper
	{
		static Properties properties = SD.PropertyService.Get("ProfilerOptions", new Properties());
		
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
