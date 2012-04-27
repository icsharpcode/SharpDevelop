// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TreeView;
using NUnit.Framework;

namespace ICSharpCode.UnitTesting
{
	public static class Extensions
	{
		public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter,TKey> outerKeySelector, Func<TInner,TKey> innerKeySelector, Func<TOuter,TInner,TResult> resultSelector)
			where TInner : class
			where TOuter : class
		{
			var innerLookup = inner.ToLookup(innerKeySelector);
			var outerLookup = outer.ToLookup(outerKeySelector);
	
			var innerJoinItems = inner
				.Where(innerItem => !outerLookup.Contains(innerKeySelector(innerItem)))
				.Select(innerItem => resultSelector(null, innerItem));
	
			return outer
				.SelectMany(outerItem => {
				            	var innerItems = innerLookup[outerKeySelector(outerItem)];
	
				            	return innerItems.Any() ? innerItems : new TInner[] { null };
				            }, resultSelector)
				.Concat(innerJoinItems);
		}
		
		public static void OrderedInsert<T>(this IList<T> list, T item, Func<T, T, int> comparer)
		{
			int index = 0;
			while (index < list.Count && comparer(list[index], item) < 0)
				index++;
			list.Insert(index, item);
		}
		
		public static void UpdateTestClasses(this IList<TestClass> testClasses, IRegisteredTestFrameworks testFrameworks, IReadOnlyList<ITypeDefinition> oldTypes, IReadOnlyList<ITypeDefinition> newTypes)
		{
			var mappings = oldTypes.FullOuterJoin(newTypes, t => t.ReflectionName, t => t.ReflectionName, Tuple.Create);
			foreach (Tuple<ITypeDefinition, ITypeDefinition> mapping in mappings) {
				if (mapping.Item2 == null)
					testClasses.RemoveWhere(c => c.FullName == mapping.Item1.ReflectionName);
				else if (mapping.Item1 == null)
					testClasses.Add(new TestClass(testFrameworks, mapping.Item2.ReflectionName, mapping.Item2));
				else {
					var testClass = testClasses.SingleOrDefault(c => c.FullName == mapping.Item1.ReflectionName);
					if (testClass == null)
						testClasses.Add(new TestClass(testFrameworks, mapping.Item2.ReflectionName, mapping.Item2));
					else
						testClass.UpdateClass(mapping.Item2);
				}
			}
		}
	}
}
