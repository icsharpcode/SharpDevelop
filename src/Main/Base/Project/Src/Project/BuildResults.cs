// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

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
		MSBuildAlreadyRunning
	}
	
	/// <summary>
	/// Class wrapping the results of a build run.
	/// </summary>
	public class BuildResults
	{
		List<BuildError> errors = new List<BuildError>();
		BuildResultCode result;
		
		public List<BuildError> Errors {
			get {
				return errors;
			}
		}
		
		public BuildResultCode Result {
			get {
				return result;
			}
			set {
				result = value;
			}
		}
	}
}
