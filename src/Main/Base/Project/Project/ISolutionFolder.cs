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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Represents a solution folder.
	/// </summary>
	public interface ISolutionFolder : ISolutionItem
	{
		/// <summary>
		/// Gets/Sets the name of the folder.
		/// </summary>
		/// <exception cref="ArgumentException">newName is not a valid solution name.</exception>
		/// <remarks>
		/// For the solution itself, setting this property will rename the .sln file.
		/// </remarks>
		string Name { get; set; }
		
		/* 				if (solution.Name == newName)
					return;
				if (!FileService.CheckFileName(newName))
					return;
				string newFileName = Path.Combine(solution.Directory, newName + ".sln");
				if (!FileService.RenameFile(solution.FileName, newFileName, false)) {
					return;
				}
				solution.FileName = newFileName;
				solution.Name = newName;
		 */
		
		/// <summary>
		/// Gets whether this solution folder is the ancestor of the specified solution item.
		/// </summary>
		bool IsAncestorOf(ISolutionItem item);
		
		/// <summary>
		/// Gets the list of direct child items in this solution folder.
		/// </summary>
		IMutableModelCollection<ISolutionItem> Items { get; }
		
		/// <summary>
		/// Loads an existing project from disk and adds it to this solution.
		/// </summary>
		/// <param name="fileName">Path to the project file</param>
		/// <exception cref="ProjectLoadException">The specified file is not a valid project file</exception>
		/// <exception cref="System.IO.IOException">Error reading from the specified project file</exception>
		IProject AddExistingProject(FileName fileName);
		
		/* 		if (solutionFolderNode.Solution.SolutionFolders.Any(
				folder => string.Equals(folder.IdGuid, newProject.IdGuid, StringComparison.OrdinalIgnoreCase)))
			{
				LoggingService.Warn("ProjectService.AddProject: Duplicate IdGuid detected");
				newProject.IdGuid = Guid.NewGuid();
			}
			*/
		
		/// <summary>
		/// Adds a link to a file as a solution item.
		/// </summary>
		ISolutionFileItem AddFile(FileName fileName);
		
		/// <summary>
		/// Creates a new solution folder with the specified name.
		/// </summary>
		ISolutionFolder CreateFolder(string name);
	}
}
