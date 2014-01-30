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
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Specifies options when starting a build.
	/// </summary>
	public class BuildOptions
	{
		#region static settings
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
		
		public static BuildDetection BuildOnExecute {
			get {
				return PropertyService.Get("SharpDevelop.BuildOnExecute", BuildDetection.RegularBuild);
			}
			set {
				PropertyService.Set("SharpDevelop.BuildOnExecute", value);
			}
		}
		
		public static BuildOutputVerbosity DefaultBuildOutputVerbosity {
			get {
				return PropertyService.Get("SharpDevelop.DefaultBuildOutputVerbosity", BuildOutputVerbosity.Normal);
			}
			set {
				PropertyService.Set("SharpDevelop.DefaultBuildOutputVerbosity", value);
			}
		}
		#endregion
		
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
		
		/// <summary>
		/// Gets/Sets the verbosity of build output.
		/// </summary>
		public BuildOutputVerbosity BuildOutputVerbosity { get; set; }
		
		/// <summary>
		/// Gets/Sets whether to build all projects or only modified ones.
		/// The default is to build all projects.
		/// </summary>
		public BuildDetection BuildDetection { get; set; }
		
		public BuildOptions(BuildTarget target)
		{
			this.projectTarget = target;
			this.TargetForDependencies = target;
			
			this.BuildDependentProjects = true;
			this.ParallelProjectCount = DefaultParallelProjectCount;
			this.BuildOutputVerbosity = DefaultBuildOutputVerbosity;
			this.BuildDetection = BuildDetection.RegularBuild;
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
