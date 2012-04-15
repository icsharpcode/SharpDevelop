// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TreeView;

namespace ICSharpCode.UnitTesting
{
	public class ProjectUnitTestNode : UnitTestBaseNode
	{
		TestProject project;
		
		public TestProject Project {
			get { return project; }
		}
		
		public ProjectUnitTestNode(TestProject project)
		{
			this.project = project;
			project.TestClasses.CollectionChanged += TestClassesCollectionChanged;
			LazyLoading = true;
		}

		void TestClassesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (TestClass c in e.NewItems) {
						var node = FindNamespace(c.Namespace);
						if (node == null) {
							node = new NamespaceUnitTestNode(c.Namespace);
							Children.OrderedInsert(node, (a, b) => string.CompareOrdinal(a.Text.ToString(), b.Text.ToString()));
						}
						node.Children.OrderedInsert(new ClassUnitTestNode(c), (a, b) => string.CompareOrdinal(a.Text.ToString(), b.Text.ToString()));
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (TestClass c in e.OldItems) {
						var node = FindNamespace(c.Namespace);
						if (node == null) continue;
						node.Children.RemoveWhere(n => n is ClassUnitTestNode && ((ClassUnitTestNode)n).TestClass.FullName == c.FullName);
						if (node.Children.Count == 0)
							Children.Remove(node);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					LoadChildren();
					break;
			}
		}
		
		NamespaceUnitTestNode FindNamespace(string @namespace)
		{
			foreach (var node in Children.OfType<NamespaceUnitTestNode>()) {
				// TODO use language-specific StringComparer
				if (string.Equals(node.Namespace, @namespace, StringComparison.Ordinal))
					return node;
			}
			return null;
		}
		
		protected override void LoadChildren()
		{
			Children.Clear();
			foreach (var g in project.TestClasses.Select(c => new ClassUnitTestNode(c)).GroupBy(tc => tc.TestClass.Namespace)) {
				var namespaceNode = new NamespaceUnitTestNode(g.Key);
				namespaceNode.Children.AddRange(g);
				Children.Add(namespaceNode);
			}
		}
		
		public override object Text {
			get { return project.Project.Name; }
		}
	}
	
	public class NamespaceUnitTestNode : UnitTestBaseNode
	{
		string name;
		
		public string Namespace {
			get { return name; }
		}
		
		public NamespaceUnitTestNode(string name)
		{
			this.name = name;
		}
		
		public override object Text {
			get { return string.IsNullOrEmpty(name) ? "<default>" : name; }
		}
	}
}
