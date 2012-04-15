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
	/// <summary>
	/// Description of RootUnitTestNode.
	/// </summary>
	public class RootUnitTestNode : UnitTestBaseNode
	{
		public RootUnitTestNode()
		{
			TestService.TestableProjects.CollectionChanged += TestService_TestableProjects_CollectionChanged;
			LazyLoading = true;
		}

		void TestService_TestableProjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					Children.AddRange(e.NewItems.OfType<TestProject>().Select(p => new ProjectUnitTestNode(p)));
					break;
				case NotifyCollectionChangedAction.Remove:
					Children.RemoveWhere(node => node is ProjectUnitTestNode && e.OldItems.OfType<TestProject>().Any(p => p.Project == ((ProjectUnitTestNode)node).Project));
					break;
				case NotifyCollectionChangedAction.Reset:
					LoadChildren();
					break;
			}
		}
		
		protected override void LoadChildren()
		{
			Children.Clear();
			Children.AddRange(TestService.TestableProjects.Select(p => new ProjectUnitTestNode(p)));
		}
		
		public override object Text {
			get { return ResourceService.GetString("ICSharpCode.UnitTesting.AllTestsTreeNode.Text"); }
		}
	}
	
	public static class Extensions
	{
		static readonly ITypeReference testAttribute = new GetClassTypeReference("NUnit.Framework", "TestAttribute", 0);
		
		public static bool IsTestProject(this IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (project.ProjectContent == null)
				return false;
			return testAttribute.Resolve(SD.ParserService.GetCompilation(project).TypeResolveContext).Kind != TypeKind.Unknown;
		}
		
		public static bool HasTests(this ITypeDefinition type, ICompilation compilation)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			var testAttribute = Extensions.testAttribute.Resolve(compilation.TypeResolveContext);
			return type.Methods.Any(m => m.Attributes.Any(a => a.AttributeType.Equals(testAttribute)));
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
