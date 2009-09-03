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
using System.Windows.Threading;

namespace ICSharpCode.Profiler.AddIn.OptionsPanels
{
	/// <summary>
	/// Description of General
	/// </summary>
	public class General : AbstractOptionPanel
	{
		GeneralOptionsPanel panel;
		
		static Properties properties = PropertyService.Get("ProfilerOptions", new Properties());
		
		public General()
		{
			ElementHost host = new ElementHost();
			host.Dock = DockStyle.Fill;
			host.Child = panel = new GeneralOptionsPanel();
			this.Controls.Add(host);
		}
		
		public override void LoadPanelContents()
		{
			try {
				panel.SetOptionValue("EnableDC", !properties.Get("EnableDC", true));
				panel.SetOptionValue("SharedMemorySize", properties.Get("SharedMemorySize", ProfilerOptions.DefaultSharedMemorySize) / 1024 / 1024);
				panel.SetOptionValue("DoNotProfileNetInternals", properties.Get("DoNotProfileNetInternals", false));
				panel.SetOptionValue("CombineRecursiveFunction", properties.Get("CombineRecursiveFunction", false));
				panel.SetOptionValue("EnableDCAtStart", properties.Get("EnableDCAtStart", true));
				base.LoadPanelContents();
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		
		public override bool StorePanelContents()
		{
			try {
				properties.Set("EnableDC", !panel.GetOptionValue<bool>("EnableDC"));
				properties.Set("SharedMemorySize", (int)panel.GetOptionValue<double>("SharedMemorySize") * 1024 * 1024);
				properties.Set("DoNotProfileNetInternals", panel.GetOptionValue<bool>("DoNotProfileNetInternals"));
				properties.Set("CombineRecursiveFunction", panel.GetOptionValue<bool>("CombineRecursiveFunction"));
				properties.Set("EnableDCAtStart", panel.GetOptionValue<bool>("EnableDCAtStart"));
				return base.StorePanelContents();
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
			
			return false;
		}
		
		public static ProfilerOptions CreateProfilerOptions()
		{
			return new ProfilerOptions(
				properties.Get("EnableDC", true),
				properties.Get("SharedMemorySize", ProfilerOptions.DefaultSharedMemorySize),
				properties.Get("DoNotProfileNetInternals", false),
				properties.Get("CombineRecursiveFunction", false),
				properties.Get("EnableDCAtStart", true)
			);
		}
	}
}
