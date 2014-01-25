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
		/// <remarks>SharpDevelop will change an item's GUID in order to automatically solve GUID conflicts.</remarks>
		Guid IdGuid { get; set; }
		
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
