// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum BuildResultCode
	{
		None,
		/// <summary>Build finished successful.</summary>
		Success,
		/// <summary>A build error occurred, see BuildResults.Error collection</summary>
		Error,
		/// <summary>A project build file is not valid</summary>
		BuildFileError,
		/// <summary>Build was not executed because another build is running</summary>
		MSBuildAlreadyRunning,
		/// <summary>Build was cancelled.</summary>
		Cancelled
	}
	
	/// <summary>
	/// Class wrapping the results of a build run.
	/// </summary>
	public class BuildResults
	{
		List<BuildError> errors = new List<BuildError>();
		ReadOnlyCollection<BuildError> readOnlyErrors;
		
		List<IBuildable> builtProjects = new List<IBuildable>();
		ReadOnlyCollection<IBuildable> readOnlyBuiltProjects;
		
		BuildResultCode result;
		int errorCount, warningCount;
		
		/// <summary>
		/// Adds a build error/warning to the results.
		/// This method is thread-safe.
		/// </summary>
		public void Add(BuildError error)
		{
			if (error == null)
				throw new ArgumentNullException("error");
			lock (errors) {
				readOnlyErrors = null;
				errors.Add(error);
				if (error.IsWarning)
					warningCount++;
				else
					errorCount++;
			}
		}
		
		/// <summary>
		/// Adds a project to the list of built projects.
		/// This method is thread-safe.
		/// </summary>
		public void AddBuiltProject(IBuildable buildable)
		{
			if (buildable == null)
				throw new ArgumentNullException("buildable");
			lock (builtProjects) {
				readOnlyBuiltProjects = null;
				builtProjects.Add(buildable);
			}
		}
		
		/// <summary>
		/// Gets the list of build errors or warnings.
		/// This property is thread-safe.
		/// </summary>
		public ReadOnlyCollection<BuildError> Errors {
			get {
				lock (errors) {
					if (readOnlyErrors == null) {
						readOnlyErrors = Array.AsReadOnly(errors.ToArray());
					}
					return readOnlyErrors;
				}
			}
		}
		
		/// <summary>
		/// Gets the list of projects that were built. This property is thread-safe.
		/// </summary>
		public ReadOnlyCollection<IBuildable> BuiltProjects {
			get { 
				lock (builtProjects) {
					if (readOnlyBuiltProjects == null) {
						readOnlyBuiltProjects = Array.AsReadOnly(builtProjects.ToArray());
					}
					return readOnlyBuiltProjects;
				}
			}
		}
		
		public BuildResultCode Result {
			get { return result; }
			set { result = value; }
		}
		
		public int ErrorCount {
			get { return errorCount; }
		}
		
		public int WarningCount {
			get { return warningCount; }
		}
	}
}
