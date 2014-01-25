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
