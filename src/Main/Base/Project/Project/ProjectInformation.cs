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
using System.ComponentModel;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Base class for <see cref="ProjectCreateInformation"/> and <see cref="ProjectLoadInformation"/>.
	/// </summary>
	public class ProjectInformation
	{
		public ProjectInformation(ISolution solution, FileName fileName)
		{
			if (solution == null)
				throw new ArgumentNullException("solution");
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			this.Solution = solution;
			this.FileName = fileName;
			this.ProjectName = fileName.GetFileNameWithoutExtension();
			this.ProjectSections = new List<SolutionSection>();
			this.ConfigurationMapping = new ConfigurationMapping();
			var solutionConfig = solution.ActiveConfiguration;
			// In unit tests, ActiveConfiguration maybe return null
			if (solutionConfig.Configuration != null && solutionConfig.Platform != null)
				this.ActiveProjectConfiguration = this.ConfigurationMapping.GetProjectConfiguration(solution.ActiveConfiguration);
			else
				this.ActiveProjectConfiguration = new ConfigurationAndPlatform("Debug", "AnyCPU");
			this.InitializeTypeSystem = true;
		}
		
		public ISolution Solution { get; private set; }
		public FileName FileName { get; private set; }
		
		/// <summary>
		/// Specifies the mapping between solution configurations and project configurations.
		/// </summary>
		public ConfigurationMapping ConfigurationMapping { get; set; }
		
		/// <summary>
		/// Specified the project configuration that should be initially active when the project is loaded.
		/// </summary>
		public ConfigurationAndPlatform ActiveProjectConfiguration { get; set; }
		
		public IList<SolutionSection> ProjectSections { get; private set; }
		public string ProjectName { get; set; }
		
		/// <summary>
		/// Specified the ID GUID for the project.
		/// If this property isn't set (stays at Guid.Empty), the project should generate a new GUID. (see AbstractProject ctor)
		/// </summary>
		public Guid IdGuid { get; set; }
		
		/// <summary>
		/// Specifies the type GUID for the project.
		/// Necessary for both project creation and loading.
		/// </summary>
		public Guid TypeGuid { get; set; }
		
		/// <summary>
		/// Specifies whether to initialize the type system for the project.
		/// The default is <c>true</c>.
		/// </summary>
		public bool InitializeTypeSystem { get; set; }
	}
	
	/// <summary>
	/// Parameter object for loading an existing project.
	/// </summary>
	public class ProjectLoadInformation : ProjectInformation
	{
		internal bool? upgradeToolsVersion;
		
		IProgressMonitor progressMonitor = new DummyProgressMonitor();
		
		/// <summary>
		/// Gets/Sets the progress monitor used during the load.
		/// This property never returns null.
		/// </summary>
		public IProgressMonitor ProgressMonitor {
			get { return progressMonitor; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				progressMonitor = value;
			}
		}
		
		public ProjectLoadInformation(ISolution parentSolution, FileName fileName, string projectName)
			: base(parentSolution, fileName)
		{
			if (projectName == null)
				throw new ArgumentNullException("projectName");
			this.ProjectName = projectName;
		}
	}
	
	/// <summary>
	/// This class holds all information the language binding need to create
	/// a predefined project for their language, if no project template for a
	/// specific language is avaiable, the language binding shouldn't care about
	/// this stuff.
	/// </summary>
	public class ProjectCreateInformation : ProjectInformation
	{
		public ProjectCreateInformation(ISolution solution, FileName outputFileName)
			: base(solution, outputFileName)
		{
			this.IdGuid = Guid.NewGuid();
			this.RootNamespace = string.Empty;
		}
		
		public string RootNamespace { get; set; }
		public TargetFramework TargetFramework { get; set; }
	}
}
