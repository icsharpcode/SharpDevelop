// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		/// Gets the list of direct child items in this solution folder.
		/// </summary>
		IMutableModelCollection<ISolutionItem> Items { get; }
	}
}
