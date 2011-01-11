// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public sealed class WebProjectsOptions
	{
		private WebProjectsOptions() { }
		
		private static readonly WebProjectsOptions _Instance = new WebProjectsOptions();
		
		private List<WebProjectOptions> options = new List<WebProjectOptions>();
		
		public WebProjectOptions GetWebProjectOptions(string projectName) {
			return options.Find(o => o.ProjectName == projectName);
		}
		
		public void SetWebProjectOptions(string projectName, WebProjectOptions data)
		{
			var d = GetWebProjectOptions(projectName);
			if (d == null)
			{
				if (data == null)
					data = new WebProjectOptions() { ProjectName = projectName };
				
				options.Add(data);
			}
			else
			{
				int index = options.IndexOf(d);
				options[index] = data;
			}
		}
		
		public static WebProjectsOptions Instance {
			get {
				return _Instance;
			}
		}
	}
	
	[Serializable]
	public class WebProjectOptions
	{
		[DefaultValue(null)]
		public string ProjectName { get; set; }
		
		[DefaultValue(null)]
		public WebProjectDebugData Data { get; set; }
	}
	
	[Serializable]
	public class WebProjectDebugData
	{
		[DefaultValue(null)]
		public string ProjectUrl { get; set; }
		
		[DefaultValue("8080")]
		public string Port { get; set; }
		
		[DefaultValue(WebServer.None)]
		public WebServer WebServer { get; set; }
	}
}
