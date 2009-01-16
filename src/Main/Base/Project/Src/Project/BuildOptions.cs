// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public delegate void BuildCallback(BuildResults results);
	
	/// <summary>
	/// Specifies options for building a single project.
	/// </summary>
	public class ProjectBuildOptions
	{
		BuildTarget target;
		IDictionary<string, string> properties = new SortedList<string, string>();
		
		public BuildTarget Target {
			get { return target; }
		}
		
		public IDictionary<string, string> Properties {
			get { return properties; }
		}
		
		public ProjectBuildOptions(BuildTarget target)
		{
			this.target = target;
		}
		
		/// <summary>
		/// Specifies the project configuration used for the build.
		/// </summary>
		public string Configuration { get; set; }
		
		/// <summary>
		/// Specifies the project platform used for the build.
		/// </summary>
		public string Platform { get; set; }
	}
	
	/// <summary>
	/// Specifies options when starting a build.
	/// </summary>
	public class BuildOptions
	{
		public static bool ShowErrorListAfterBuild {
			get {
				return PropertyService.Get("SharpDevelop.ShowErrorListAfterBuild", true);
			}
			set {
				PropertyService.Set("SharpDevelop.ShowErrorListAfterBuild", value);
			}
		}
		
		public static int DefaultParallelProjectCount {
			get {
				return PropertyService.Get("SharpDevelop.BuildParallelProjectCount", Math.Min(4, Environment.ProcessorCount));
			}
			set {
				PropertyService.Set("SharpDevelop.BuildParallelProjectCount", value);
			}
		}
		
		IDictionary<string, string> globalAdditionalProperties = new SortedList<string, string>();
		IDictionary<string, string> projectAdditionalProperties = new SortedList<string, string>();
		
		/// <summary>
		/// Specifies whether dependencies should be built.
		/// </summary>
		public bool BuildDependentProjects { get; set; }
		
		/// <summary>
		/// Specifies the solution configuration used for the build.
		/// </summary>
		public string SolutionConfiguration { get; set; }
		
		/// <summary>
		/// Specifies the solution platform used for the build.
		/// </summary>
		public string SolutionPlatform { get; set; }
		
		/// <summary>
		/// Specifies the number of projects that should be built in parallel.
		/// </summary>
		public int ParallelProjectCount { get; set; }
		
		public BuildOptions(BuildTarget target, BuildCallback callback)
		{
			this.callback = callback;
			this.projectTarget = target;
			this.TargetForDependencies = target;
			
			this.BuildDependentProjects = true;
			this.ParallelProjectCount = DefaultParallelProjectCount;
		}
		
		readonly BuildCallback callback;
		
		/// <summary>
		/// Gets the method to call when the build has finished.
		/// </summary>
		public BuildCallback Callback {
			get { return callback; }
		}
		
		readonly BuildTarget projectTarget;
		
		/// <summary>
		/// The target to build for the project being built.
		/// </summary>
		public BuildTarget ProjectTarget {
			get { return projectTarget; }
		}
		
		/// <summary>
		/// The target to build for dependencies of the project being built.
		/// </summary>
		public BuildTarget TargetForDependencies { get; set; }
		
		/// <summary>
		/// Additional properties used for the build, both for the project being built and its dependencies.
		/// </summary>
		public IDictionary<string, string> GlobalAdditionalProperties {
			get { return globalAdditionalProperties; }
		}
		
		/// <summary>
		/// Additional properties used only for the project being built but not for its dependencies.
		/// </summary>
		public IDictionary<string, string> ProjectAdditionalProperties {
			get { return projectAdditionalProperties; }
		}
	}
}
