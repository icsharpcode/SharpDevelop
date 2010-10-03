// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public sealed class SolutionPreferences : IMementoCapable
	{
		Solution solution;
		Properties properties = new Properties();
		string startupProject = "";
		string activeConfiguration = "Debug";
		string activePlatform = "Any CPU";
		
		internal SolutionPreferences(Solution solution)
		{
			this.solution = solution;
		}
		
		public Properties Properties {
			get {
				return properties;
			}
		}
		
		public IProject StartupProject {
			get {
				if (startupProject.Length == 0)
					return null;
				foreach (IProject project in solution.Projects) {
					if (project.IdGuid.Equals(startupProject, StringComparison.OrdinalIgnoreCase))
						return project;
				}
				return null;
			}
			set {
				SetStartupProject((value != null) ? value.IdGuid : "");
			}
		}
		
		void SetStartupProject(string value)
		{
			if (value != startupProject) {
				startupProject = value;
				if (StartupProjectChanged != null) {
					StartupProjectChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler StartupProjectChanged;
		
		public string ActiveConfiguration {
			get {
				return activeConfiguration;
			}
			set {
				if (value == null) throw new ArgumentNullException();
				activeConfiguration = value;
			}
		}
		
		public string ActivePlatform {
			get {
				return activePlatform;
			}
			set {
				if (value == null) throw new ArgumentNullException();
				activePlatform = value;
			}
		}
		
		/// <summary>
		/// Creates a new memento from the state.
		/// </summary>
		Properties IMementoCapable.CreateMemento()
		{
			Properties p = properties;
			p.Set("StartupProject",      startupProject);
			p.Set("ActiveConfiguration", activeConfiguration);
			p.Set("ActivePlatform",      activePlatform);
			return p;
		}
		
		/// <summary>
		/// Sets the state to the given memento.
		/// </summary>
		void IMementoCapable.SetMemento(Properties memento)
		{
			SetStartupProject(memento.Get("StartupProject", ""));
			string configuration = memento.Get("ActiveConfiguration", activeConfiguration);
			string platform      = memento.Get("ActivePlatform", activePlatform);
			
			// validate configuration and platform:
			IList<string> available = solution.GetConfigurationNames();
			if (available.Count > 0 && !available.Contains(configuration))
				configuration = available[0];
			available = solution.GetPlatformNames();
			if (available.Count > 0 && !available.Contains(platform))
				platform = available[0];
			
			this.ActiveConfiguration = configuration;
			this.ActivePlatform = platform;

			this.properties = memento;
		}
	}
}
