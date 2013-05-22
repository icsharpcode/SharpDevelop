// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Represents an item within a solution folder.
	/// This may be a file, a project, or another solution folder.
	/// </summary>
	public interface ISolutionItem
	{
		/// <summary>
		/// Gets the parent folder.
		/// This property will return null for the solution (which acts as the top-level folder).
		/// It will also return null for folders that were removed from their parent.
		/// </summary>
		/// <remarks>
		/// The parent folder of an item can change, e.g. when a project is moved into a different folder.
		/// The setter of this property should be used by the <see cref="ISolutionFolder"/> implementation only.
		/// </remarks>
		ISolutionFolder ParentFolder { get; set; }
		
		/// <summary>
		/// Gets the parent solution.
		/// This property is thread-safe, and will never returns null.
		/// </summary>
		/// <remarks>
		/// The parent solution of a solution item cannot change; a new instance of the item must be created
		/// in order to move the item to another solution.
		/// </remarks>
		ISolution ParentSolution { get; }
		
		/// <summary>
		/// Gets the ID GUID of this solution item.
		/// </summary>
		Guid IdGuid { get; }
		
		/// <summary>
		/// Gets the type GUID of this solution item.
		/// </summary>
		Guid TypeGuid { get; }
	}
	
	public interface ISolutionFileItem : ISolutionItem
	{
		FileName FileName { get; set; }
	}
}
