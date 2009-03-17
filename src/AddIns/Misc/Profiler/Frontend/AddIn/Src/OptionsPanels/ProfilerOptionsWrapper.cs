// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Profiler.Controller;
using System;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Profiler.AddIn.OptionsPanels
{
	static class ProfilerOptionsWrapper
	{
		static Properties properties = PropertyService.Get("ProfilerOptions", new Properties());
		
		public static Properties Properties {
			get {
				return properties;
			}
		}
		
		public static bool EnableDC {
			get { return properties.Get("EnableDC", true); }
			set { properties.Set("EnableDC", value); }
		}
		
		public static int SharedMemorySize {
			get { return properties.Get("SharedMemorySize", 64 * 1024 * 1024); }
			set { properties.Set("SharedMemorySize", value); }
		}
		
		public static ProfilerOptions CreateProfilerOptions()
		{
			return new ProfilerOptions(properties.Get("EnableDC", true),
			                           properties.Get("SharedMemorySize", 64 * 1024 * 1024));
		}
	}
}
