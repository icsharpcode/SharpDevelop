// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using ICSharpCode.Core;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Profiler.AddIn.OptionsPanels
{
	/// <summary>
	/// Description of General
	/// </summary>
	public class General : OptionPanel
	{
		GeneralOptionsPanel panel;
		
		static Properties properties = PropertyService.Get("ProfilerOptions", new Properties());
		
		public General()
		{
			panel = new GeneralOptionsPanel();
		}
		
		public override object Control {
			get {
				return panel;
			}
		}
		
		public override void LoadOptions()
		{
			panel.SetOptionValue<bool>("EnableDC", !properties.Get("EnableDC", true));
			panel.SetOptionValue<double>("SharedMemorySize", properties.Get("SharedMemorySize", ProfilerOptions.SHARED_MEMORY_SIZE) / 1024 / 1024);
			panel.SetOptionValue<bool>("DoNotProfileNetInternals", properties.Get("DoNotProfileNetInternals", false));
			panel.SetOptionValue<bool>("CombineRecursiveFunction", properties.Get("CombineRecursiveFunction", false));
		}
		
		public override bool SaveOptions()
		{
			properties.Set("EnableDC", !panel.GetOptionValue<bool>("EnableDC"));
			properties.Set("SharedMemorySize", (int)panel.GetOptionValue<double>("SharedMemorySize") * 1024 * 1024);
			properties.Set("DoNotProfileNetInternals", panel.GetOptionValue<bool>("DoNotProfileNetInternals"));
			properties.Set("CombineRecursiveFunction", panel.GetOptionValue<bool>("CombineRecursiveFunction"));
			return true;
		}
		
		public static ProfilerOptions CreateProfilerOptions()
		{
			return new ProfilerOptions(properties.Get("EnableDC", true),
			                           properties.Get("SharedMemorySize", ProfilerOptions.SHARED_MEMORY_SIZE),
			                           properties.Get("DoNotProfileNetInternals", false),
			                           properties.Get("CombineRecursiveFunction", false)
			                          );
		}
	}
}
