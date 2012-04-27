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
			if (e.PropertyName == "TestResult")
				RaisePropertyChanged("Icon");
		}

		void TestClassesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					foreach (TestClass c in e.NewItems) {
						if (c.Namespace == "<invalid>") continue;
						string namespaceSuffix = GetNamespaceSuffix(c.Namespace, project.Project.RootNamespace);
						if (string.IsNullOrEmpty(namespaceSuffix)) {
							Children.OrderedInsert(new ClassUnitTestNode(c), (a, b) => string.CompareOrdinal(a.Text.ToString(), b.Text.ToString()));
						} else {
							var node = FindNamespace(namespaceSuffix);
							if (node == null) {
								node = new NamespaceUnitTestNode(namespaceSuffix);
								Children.OrderedInsert(node, (a, b) => string.CompareOrdinal(a.Text.ToString(), b.Text.ToString()));
							}
							node.Children.OrderedInsert(new ClassUnitTestNode(c), (a, b) => string.CompareOrdinal(a.Text.ToString(), b.Text.ToString()));
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (TestClass c in e.OldItems) {
						if (c.Namespace == "<invalid>") continue;
						string namespaceSuffix = GetNamespaceSuffix(c.Namespace, project.Project.RootNamespace);
						if (string.IsNullOrEmpty(namespaceSuffix)) {
							Children.RemoveWhere(n => n is ClassUnitTestNode && ((ClassUnitTestNode)n).TestClass.FullName == c.FullName);
						} else {
							UnitTestBaseNode node = FindNamespace(namespaceSuffix);
							if (node == null) continue;
							node.Children.RemoveWhere(n => n is ClassUnitTestNode && ((ClassUnitTestNode)n).TestClass.FullName == c.FullName);
							while (node.Children.Count == 0) {
								var parent = (UnitTestBaseNode)node.Parent;
								if (parent == null) break;
								parent.Children.Remove(node);
								node = parent;
							}
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					LoadChildren();
					break;
			}
		}
		
		static string GetNamespaceSuffix(string @namespace, string rootNamespace)
		{
			if (@namespace.StartsWith(rootNamespace + ".", StringComparison.Ordinal))
				return @namespace.Substring(rootNamespace.Length + 1);
			if (@namespace.Equals(rootNamespace, StringComparison.Ordinal))
				return "";
			return @namespace;
		}
		
		NamespaceUnitTestNode FindNamespace(string @namespace)
		{
			var parent = FindNamespace(@namespace, this);
			NamespaceUnitTestNode newNode;
			string rootNamespace = "";
			if (parent is NamespaceUnitTestNode) {
				var namespaceNode = (NamespaceUnitTestNode)parent;
				rootNamespace = namespaceNode.Namespace;
				if (namespaceNode.Namespace.Equals(@namespace, StringComparison.OrdinalIgnoreCase))
					return namespaceNode;
			}
			parent.Children.Add(CreateMissingNamespaceNodes(out newNode, @namespace, rootNamespace));
			return newNode;
		}
		
		NamespaceUnitTestNode CreateMissingNamespaceNodes(out NamespaceUnitTestNode newNode, string @namespace, string rootNamespace)
		{
			newNode = new NamespaceUnitTestNode(@namespace);
			NamespaceUnitTestNode result = newNode;
			int dot = @namespace.LastIndexOf('.');
			@namespace = dot > -1 ? @namespace.Substring(0, dot) : @namespace;
			while (dot > -1 && !rootNamespace.Equals(@namespace, StringComparison.OrdinalIgnoreCase)) {
				var node = result;
				result = new NamespaceUnitTestNode(@namespace);
				result.Children.Add(node);
				dot = @namespace.LastIndexOf('.');
				@namespace = dot > -1 ? @namespace.Substring(0, dot) : @namespace;
			}
			return result;
		}
		
		UnitTestBaseNode FindNamespace(string @namespace, UnitTestBaseNode parent)
		{
			foreach (var node in parent.Children.OfType<NamespaceUnitTestNode>()) {
				if (@namespace.StartsWith(node.Namespace + ".", StringComparison.OrdinalIgnoreCase) || @namespace.Equals(node.Namespace, StringComparison.OrdinalIgnoreCase))
					return FindNamespace(@namespace, node);
			}
			return parent;
		}
		
		protected override void LoadChildren()
		{
			Children.Clear();
			foreach (var g in project.TestClasses.Select(c => new ClassUnitTestNode(c)).GroupBy(tc => tc.TestClass.Namespace)) {
				UnitTestBaseNode node;
				if (g.Key == "<invalid>") continue;
				string namespaceName = GetNamespaceSuffix(g.Key, project.Project.RootNamespace);
				if (string.IsNullOrEmpty(namespaceName))
					node = this;
				else
					node = FindNamespace(namespaceName);
				node.Children.AddRange(g);
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
