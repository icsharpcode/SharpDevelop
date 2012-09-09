// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Specialized;
using System.ComponentModel;
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
			project.PropertyChanged += ProjectPropertyChanged;
			LazyLoading = true;
		}

		void ProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "TestResult") {
				RaisePropertyChanged("Icon");
				RaisePropertyChanged("ExpandedIcon");
			}
		}

		void TestClassesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (TestClass c in e.NewItems) {
						if (c.Namespace == "<invalid>") continue;
						UnitTestBaseNode node = FindOrCreateNamespace(this, project.Project.RootNamespace, c.Namespace);
						node.Children.OrderedInsert(new ClassUnitTestNode(c), NodeTextComparer);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (TestClass c in e.OldItems) {
						if (c.Namespace == "<invalid>") continue;
						SharpTreeNode node = FindNamespace(this, project.Project.RootNamespace, c.Namespace);
						node.Children.RemoveAll(n => n is ClassUnitTestNode && ((ClassUnitTestNode)n).TestClass == c);
						while (node is NamespaceUnitTestNode && node.Children.Count == 0) {
							var parent = node.Parent;
							if (parent == null) break;
							parent.Children.Remove(node);
							node = parent;
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					LoadChildren();
					break;
			}
		}
		
		static UnitTestBaseNode FindOrCreateNamespace(UnitTestBaseNode parent, string parentNamespace, string @namespace)
		{
			if (parentNamespace == @namespace)
				return parent;
			foreach (var node in parent.Children.OfType<NamespaceUnitTestNode>()) {
				if (@namespace == node.NamespaceName)
					return node;
				if (@namespace.StartsWith(node.NamespaceName + ".", StringComparison.Ordinal)) {
					return FindOrCreateNamespace(node, node.NamespaceName, @namespace);
				}
			}
			// Create missing namespace node:
			
			// Figure out which part of the namespace we can remove due to the parent namespace:
			int startPos = 0;
			if (@namespace.StartsWith(parentNamespace + ".", StringComparison.Ordinal)) {
				startPos = parentNamespace.Length + 1;
			}
			// Get the next dot
			int dotPos = @namespace.IndexOf('.', startPos);
			if (dotPos < 0) {
				var newNode = new NamespaceUnitTestNode(@namespace);
				parent.Children.OrderedInsert(newNode, NodeTextComparer);
				return newNode;
			} else {
				var newNode = new NamespaceUnitTestNode(@namespace.Substring(0, dotPos));
				parent.Children.OrderedInsert(newNode, NodeTextComparer);
				return FindOrCreateNamespace(newNode, newNode.NamespaceName, @namespace);
			}
		}
		
		static UnitTestBaseNode FindNamespace(UnitTestBaseNode parent, string parentNamespace, string @namespace)
		{
			if (parentNamespace == @namespace)
				return parent;
			foreach (var node in parent.Children.OfType<NamespaceUnitTestNode>()) {
				if (@namespace == node.NamespaceName)
					return node;
				if (@namespace.StartsWith(node.NamespaceName + ".", StringComparison.Ordinal)) {
					return FindNamespace(node, node.NamespaceName, @namespace);
				}
			}
			return null;
		}
		
		protected override void LoadChildren()
		{
			Children.Clear();
			foreach (var g in project.TestClasses.Select(c => new ClassUnitTestNode(c)).GroupBy(tc => tc.TestClass.Namespace)) {
				if (g.Key == "<invalid>") continue;
				UnitTestBaseNode node = FindOrCreateNamespace(this, project.Project.RootNamespace, g.Key);
				node.Children.AddRange(g.OrderBy(NodeTextComparer));
			}
		}
		
		public override object Text {
			get { return project.Project.Name; }
		}
		
		internal override TestResultType TestResultType {
			get { return project.TestResult; }
		}
	}
}
