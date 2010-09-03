// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Hornung.ResourceToolkit.Gui
{
	/// <summary>
	/// Describes an object that can filter items by registering
	/// one or more <see cref="IFilter"/> objects.
	/// </summary>
	public interface IFilterHost<T>
	{
		/// <summary>
		/// Registers a new filter with the filter host, if the filter is not already registered,
		/// or signals that the filter condition of the specified filter has changed.
		/// </summary>
		/// <param name="filter">The filter to be registered.</param>
		void RegisterFilter(IFilter<T> filter);
		
		/// <summary>
		/// Removes the specified filter from the filter host, if it is currently registered there.
		/// </summary>
		/// <param name="filter">The filter to be removed.</param>
		void UnregisterFilter(IFilter<T> filter);
	}
}
