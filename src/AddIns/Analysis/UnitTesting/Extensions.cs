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
		static readonly ITypeReference testAttribute = new GetClassTypeReference("NUnit.Framework", "TestAttribute", 0);
		static readonly ITypeReference testCaseAttribute = new GetClassTypeReference("NUnit.Framework", "TestCaseAttribute", 0);
		
		public static bool IsTestProject(this IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (project.ProjectContent == null)
				return false;
			return testAttribute.Resolve(SD.ParserService.GetCompilation(project).TypeResolveContext).Kind != TypeKind.Unknown;
		}
		
		public static bool IsTestMethod(this IMethod method, ICompilation compilation)
		{
			if (method == null)
				throw new ArgumentNullException("method");
			var testAttribute = Extensions.testAttribute.Resolve(compilation.TypeResolveContext);
			var testCaseAttribute = Extensions.testCaseAttribute.Resolve(compilation.TypeResolveContext);
			return method.Attributes.Any(a => a.AttributeType.Equals(testAttribute) || a.AttributeType.Equals(testCaseAttribute));
		}
		
		public static bool HasTests(this ITypeDefinition type, ICompilation compilation)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			var testAttribute = Extensions.testAttribute.Resolve(compilation.TypeResolveContext);
			var testCaseAttribute = Extensions.testCaseAttribute.Resolve(compilation.TypeResolveContext);
			return type.Methods.Any(m => m.Attributes.Any(a => a.AttributeType.Equals(testAttribute) || a.AttributeType.Equals(testCaseAttribute)));
		}
		
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
	}
}
