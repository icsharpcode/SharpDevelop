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
			panel.Load(properties.Get("EnableDC", true),
			           properties.Get("SharedMemorySize", ProfilerOptions.SHARED_MEMORY_SIZE) / 1024 / 1024);
			base.LoadPanelContents();
		}
		
		public override bool StorePanelContents()
		{
			properties.Set("EnableDC", !panel.GetOptionValue<bool>("EnableDC"));
			properties.Set("SharedMemorySize", (int)panel.GetOptionValue<double>("SharedMemorySize") * 1024 * 1024);
			
			return base.StorePanelContents();
		}
			
		public static ProfilerOptions CreateProfilerOptions()
		{
			return new ProfilerOptions(properties.Get("EnableDC", true),
			                           properties.Get("SharedMemorySize", ProfilerOptions.SHARED_MEMORY_SIZE),
			                           !properties.Get("PorfileDotNetInternals", true),
			                           properties.Get("CombineRecursiveFunction", false)
			                          );
		}
	}
}
