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
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum SolutionFormatVersion
	{
		VS2005 = 9,
		VS2008 = 10,
		VS2010 = 11,
		VS2012 = 12
	}
	
	/// <summary>
	/// Represents a solution.
	/// </summary>
	/// <remarks>
	/// This interface is thread-safe for read-access. Background threads may read from ISolution
	/// even while the main thread is modifying the solution.
	/// However, only the main thread is allowed to modify the solution.
	/// </remarks>
	public interface ISolution : ISolutionFolder, ICanBeDirty, IConfigurable, IDisposable
	{
		Microsoft.Build.Evaluation.ProjectCollection MSBuildProjectCollection { get; }
		
		/// <summary>
		/// Gets the full path of the .sln file.
		/// </summary>
		FileName FileName { get; }
		
		event EventHandler FileNameChanged;
		
		/// <summary>
		/// Gets the full path of the directory containing the .sln file.
		/// </summary>
		DirectoryName Directory { get; }
		
		/// <summary>
		/// Gets/Sets the startup project.
		/// </summary>
		IProject StartupProject { get; set; }
		
		event EventHandler StartupProjectChanged;
		
		/// <summary>
		/// Gets all projects in the solution.
		/// </summary>
		IModelCollection<IProject> Projects { get; }
		
		/// <summary>
		/// Gets all items in the solution; including those nested within solution folders.
		/// </summary>
		/// <remarks>
		/// The enumerator performs a pre-order walk of the solution folder tree.
		/// </remarks>
		IEnumerable<ISolutionItem> AllItems { get; }
		
		/// <summary>
		/// Gets the list of global sections.
		/// These can be used to store additional data within the solution file.
		/// </summary>
		IMutableModelCollection<SolutionSection> GlobalSections { get; }
		
		/// <summary>
		/// Finds the item with the specified <see cref="ISolutionItem.IdGuid"/>;
		/// or returns null if no such item exists.
		/// </summary>
		ISolutionItem GetItemByGuid(Guid guid);
		
		/// <summary>
		/// Gets a container that can be used to store data about the solution.
		/// This data is stored in SharpDevelop's config directory, not directly with the .sln file.
		/// </summary>
		Properties Preferences { get; }
		
		/// <summary>
		/// Gets a container that can be used to store data about the solution.
		/// This data is stored along with .sln file as .sln.sdsettings.
		/// </summary>
		Properties GlobalPreferences { get; }
		
		/// <summary>
		/// This event is raised by <see cref="SavePreferences"/> immediately before the preferences are saved to disk.
		/// It can be used to set additional data on the preferences at the last moment.
		/// </summary>
		event EventHandler PreferencesSaving;
		
		/// <summary>
		/// Saves the preferences for this solution.
		/// </summary>
		void SavePreferences();
		
		/// <summary>
		/// Gets whether the solution is read-only.
		/// </summary>
		bool IsReadOnly { get; }
		
		/// <summary>
		/// Saves the solution.
		/// 
		/// This method will not save the project files. Use <see cref="ProjectService.SaveSolution"/> instead to save the solution
		/// and all open projects.
		/// </summary>
		void Save();
	}
}
