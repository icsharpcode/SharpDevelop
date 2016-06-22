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
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	[SDService("SD.AddInTree")]
	public interface IAddInTree
	{
		/// <summary>
		/// Gets the AddIns that are registered for this AddIn tree.
		/// </summary>
		IReadOnlyList<AddIn> AddIns { get; }
		
		/// <summary>
		/// Gets a dictionary of registered doozers.
		/// </summary>
		ConcurrentDictionary<string, IDoozer> Doozers { get; }
		
		/// <summary>
		/// Gets a dictionary of registered condition evaluators.
		/// </summary>
		ConcurrentDictionary<string, IConditionEvaluator> ConditionEvaluators { get; }
		
		/// <summary>
		/// Builds the items in the path. Ensures that all items have the type T.
		/// </summary>
		/// <param name="path">A path in the addin tree.</param>
		/// <param name="parameter">A parameter that gets passed into the doozer and condition evaluators.</param>
		/// <param name="throwOnNotFound">If true, throws a <see cref="TreePathNotFoundException"/>
		/// if the path is not found. If false, an empty list is returned when the
		/// path is not found.</param>
		IReadOnlyList<T> BuildItems<T>(string path, object parameter, bool throwOnNotFound = true);
		
		/// <summary>
		/// Builds a single item in the addin tree.
		/// </summary>
		/// <param name="path">A path to the item in the addin tree.</param>
		/// <param name="parameter">A parameter that gets passed into the doozer and condition evaluators.</param>
		/// <exception cref="TreePathNotFoundException">The path does not
		/// exist or does not point to an item.</exception>
		object BuildItem(string path, object parameter);
		
		object BuildItem(string path, object parameter, IEnumerable<ICondition> additionalConditions);
		
		/// <summary>
		/// Gets the <see cref="AddInTreeNode"/> representing the specified path.
		/// </summary>
		/// <param name="path">The path of the AddIn tree node</param>
		/// <param name="throwOnNotFound">
		/// If set to <c>true</c>, this method throws a
		/// <see cref="TreePathNotFoundException"/> when the path does not exist.
		/// If set to <c>false</c>, <c>null</c> is returned for non-existing paths.
		/// </param>
		AddInTreeNode GetTreeNode(string path, bool throwOnNotFound = true);
	}
}
