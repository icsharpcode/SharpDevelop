// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Hornung.ResourceToolkit.Gui
{
	/// <summary>
	/// Describes an object that applies a filter condition to items.
	/// </summary>
	public interface IFilter<T>
	{
		/// <summary>
		/// Determines if the specified item matches the current filter criteria.
		/// </summary>
		/// <param name="item">The item to test.</param>
		/// <returns><c>true</c>, if the specified item matches the current filter criteria, otherwise <c>false</c>.</returns>
		bool IsMatch(T item);
	}
}
