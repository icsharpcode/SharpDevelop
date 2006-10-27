// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

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
