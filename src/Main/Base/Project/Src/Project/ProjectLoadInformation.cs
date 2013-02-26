// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Parameter object for loading an existing project.
	/// </summary>
	public class ProjectLoadInformation
	{
		public ISolution Solution { get; private set; }
		public FileName FileName { get; private set; }
		public ConfigurationAndPlatform Configuration { get; set; }
		public string ProjectName { get; private set; }
		public Guid TypeGuid { get; set; }
		public IList<ProjectSection> ProjectSections {get; set;}
		
		/// <summary>
		/// Whether to initialize the type system for the newly loaded project.
		/// The default is <c>true</c>.
		/// </summary>
		public bool InitializeTypeSystem { get; set; }
		#warning this property is unused?
		
		internal string Guid { get; set; }
		
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
		{
			if (parentSolution == null)
				throw new ArgumentNullException("parentSolution");
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (projectName == null)
				throw new ArgumentNullException("projectName");
			this.Solution = parentSolution;
			this.FileName = fileName;
			this.ProjectName = projectName;
			this.InitializeTypeSystem = true;
		}
	}
}
