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
