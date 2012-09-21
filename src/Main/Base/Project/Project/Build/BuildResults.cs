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
		
		BuildResultCode result;
		int errorCount, warningCount, messageCount;
		
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
				if (error.IsMessage)
					messageCount++;
				else if (error.IsWarning)
					warningCount++;
				else
					errorCount++;
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
		
		public int MessageCount {
			get { return messageCount; }
		}
	}
}
