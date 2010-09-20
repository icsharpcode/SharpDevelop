// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a namespace in the TestTreeView.
	/// </summary>
	public class TestNamespaceTreeNode : TestTreeNode
	{
		string ns = String.Empty;
		string namespacePrefix = String.Empty;
		string fullNamespace;
		TestClassCollection testClasses;
		List<TestNamespaceTreeNode> namespaceChildNodes = new List<TestNamespaceTreeNode>();
		ExtTreeNode dummyNode;
		
		/// <summary>
		/// Creates a new TestNamespaceTreeNode
		/// </summary>
		/// <remarks>
		/// Note that the Namespace child nodes are added in
		/// the constructor not whilst the node is expanding
		/// via the Initialize method. This is so the icon for the
		/// node can be updated even if the parent node is not 
		/// expanded. The alternative is to have each namespace node,
		/// even if it does not have any class child nodes, to
		/// store all the classes that are below it in the tree and
		/// update the icon based on their results. The assumption
		/// is that there are fewer namespace nodes than classes so
		/// adding the namespace nodes here does not matter. 
		/// </remarks>
		/// <param name="namespacePrefix">The first part of the
		/// namespace (including any dot characters) before this 
		/// particular namespace.</param>
		/// <param name="name">The name of the namespace without any
		/// dot characters (e.g. the name at this particular 
		/// location in the tree).</param>
		public TestNamespaceTreeNode(TestProject testProject, string namespacePrefix, string name) 
			: base(testProject, name)
		{
			ns = name;
			this.namespacePrefix = namespacePrefix;
			fullNamespace = GetFullNamespace(namespacePrefix, ns);
			GetTestClasses();
			testProject.TestClasses.TestClassAdded += TestClassAdded;
			testProject.TestClasses.TestClassRemoved += TestClassRemoved;
			
			// Add namespace nodes - do not add them on node expansion.
			foreach (string namespaceName in TestProject.GetChildNamespaces(fullNamespace)) {
				TestNamespaceTreeNode node = new TestNamespaceTreeNode(TestProject, fullNamespace, namespaceName);
				node.AddTo(this);
				namespaceChildNodes.Add(node);
				node.ImageIndexChanged += TestNamespaceNodeImageIndexChanged;
			}
			
			// Add a dummy node if there are no namespaces since
			// there might be class nodes which will be added 
			// lazily when the node is expanded.
			if (namespaceChildNodes.Count == 0) {
				dummyNode = new ExtTreeNode();
				Nodes.Add(dummyNode);
			}
			
			UpdateImageListIndex();
		}
		
		/// <summary>
		/// Creates a new TestNamespaceTreeNode with the specified
		/// namespace name. This node will have no namespace prefix.
		/// </summary>
		public TestNamespaceTreeNode(TestProject testProject, string name) 
			: this(testProject, String.Empty, name)
		{
		}
		
		/// <summary>
		/// Frees any resources held by this class.
		/// </summary>
		public override void Dispose()
		{
			if (!IsDisposed) {
				TestProject.TestClasses.TestClassAdded -= TestClassAdded;
				TestProject.TestClasses.TestClassRemoved -= TestClassRemoved;
				testClasses.ResultChanged -= TestClassesResultChanged;
			}
			base.Dispose();
		}
		
		/// <summary>
		/// Gets whether this namespace node is considered empty. If
		/// the node has been expanded then the node is empty if it
		/// has no child nodes. If it has not been expanded then there
		/// will be a dummy child node or a set of namespace
		/// nodes but no test class nodes.
		/// </summary>
		public bool IsEmpty {
			get {
				if (isInitialized) {
					return Nodes.Count == 0;
				} else if (dummyNode != null) {
					return testClasses.Count == 0;
				} else {
					return Nodes.Count == 0 && testClasses.Count == 0;
				}
			}
		}
		
		/// <summary>
		/// Gets the full namespace of this tree node. This includes any
		/// parent namespaces prefixed to the namespace associated
		/// with this tree node.
		/// </summary>
		public string FullNamespace {
			get {
				return fullNamespace;
			}
		}
			
		/// <summary>
		/// Adds the test class nodes for this namespace when the
		/// node is expanded.
		/// </summary>
		protected override void Initialize()
		{
			if (dummyNode != null) {
				Nodes.Clear();
			}
			
			// Add class nodes for this namespace.
			foreach (TestClass c in testClasses) {
				TestClassTreeNode classNode = new TestClassTreeNode(TestProject, c);
				classNode.AddTo(this);
			}
			
			// Sort the nodes.
			SortChildNodes();
		}
		
		/// <summary>
		/// Adds the child namespace to the namespace prefix.
		/// </summary>
		static string GetFullNamespace(string prefix, string name)
		{
			if (prefix.Length > 0) {
				return String.Concat(prefix, ".", name);
			}
			return name;
		}
		
		/// <summary>
		/// Updates this node's icon because one of its child namespace nodes
		/// has changed.
		/// </summary>
		void TestNamespaceNodeImageIndexChanged(object source, EventArgs e)
		{
			UpdateImageListIndex();
		}
		
		/// <summary>
		/// Gets the test classes for this namespace.
		/// </summary>
		void GetTestClasses()
		{
			testClasses = new TestClassCollection();
			foreach (TestClass c in TestProject.GetTestClasses(fullNamespace)) {
				testClasses.Add(c);
			}
			testClasses.ResultChanged += TestClassesResultChanged;
		}
		
		/// <summary>
		/// The overall test result for the classes in this namespace
		/// have changed so the node's icon is updated.
		/// </summary>
		void TestClassesResultChanged(object source, EventArgs e)
		{
			UpdateImageListIndex();
		}
		
		/// <summary>
		/// Determines the image list index for this node based
		/// on the current state of all the child nodes.
		/// </summary>
		/// <remarks>
		/// Since the test classes overall test result is
		/// available via the TestClassCollection we do not
		/// need to sum all the individual test classes.
		/// </remarks>
		void UpdateImageListIndex()
		{
			int ignoredCount = 0;
			int passedCount = 0;
			int failedCount = 0;
			
			// Count the passes, failures and ignores for the
			// namespace child nodes.
			foreach (ExtTreeNode node in namespaceChildNodes) {
				switch (node.ImageIndex) {
					case (int)TestTreeViewImageListIndex.TestFailed:
						failedCount++;
						break;
					case (int)TestTreeViewImageListIndex.TestIgnored:
						ignoredCount++;
						break;
					case (int)TestTreeViewImageListIndex.TestPassed:
						passedCount++;
						break;
				}
			}
			
			// Check the passes, failures and ignores for the
			// test classes that belong to this namespace.
			switch (testClasses.Result) {
				case TestResultType.Failure:
					failedCount++;
					break;
				case TestResultType.Success:
					passedCount++;
					break;
				case TestResultType.Ignored:
					ignoredCount++;
					break;
			}
			
			// Work out the total number of passes we are expecting
			int total = namespaceChildNodes.Count;
			if (testClasses.Count > 0) {
				// Only add one for the testClasses since the
				// overall pass or failure is indicated via
				// the TestClassCollection so we do not need
				// to add all the test classes.
				total++;
			}
			
			// Determine the correct image list index for this node.
			if (failedCount > 0) {
				UpdateImageListIndex(TestResultType.Failure);
			} else if (ignoredCount > 0) {
				UpdateImageListIndex(TestResultType.Ignored);
			} else if (passedCount == total) {
				UpdateImageListIndex(TestResultType.Success);
			} else {
				UpdateImageListIndex(TestResultType.None);
			}
		}
		
		/// <summary>
		/// A new test class has been added to the project so a new 
		/// tree node is added if the class belongs to this namespace.
		/// </summary>
		void TestClassAdded(object source, TestClassEventArgs e)
		{
			if (e.TestClass.Namespace == fullNamespace) {
				// Add test class to our monitored test classes.
				testClasses.Add(e.TestClass);
				
				// Add a new tree node.
				TestClassTreeNode classNode = new TestClassTreeNode(TestProject, e.TestClass);
				classNode.AddTo(this);
				
				// Sort the nodes.
				SortChildNodes();
			} else if (isInitialized && NamespaceStartsWith(e.TestClass.Namespace)) {
				// Check if there is a child namespace node for the class.
				string childNamespace = TestClass.GetChildNamespace(e.TestClass.Namespace, fullNamespace);
				if (!NamespaceNodeExists(childNamespace)) {
					// Add a new namespace node.
					TestNamespaceTreeNode node = new TestNamespaceTreeNode(TestProject, fullNamespace, childNamespace);
					node.AddTo(this);
					
					// Sort the nodes.
					SortChildNodes();
				}
			}
		}
		
		/// <summary>
		/// Determines whether the namespace for this node starts
		/// with the namespace specified.
		/// </summary>
		bool NamespaceStartsWith(string ns)
		{
			return ns.StartsWith(String.Concat(fullNamespace, "."));
		}
		
		/// <summary>
		/// A test class has been removed from the project so the
		/// corresponding tree node is removed if it belongs to this 
		/// namespace.
		/// </summary>
		void TestClassRemoved(object source, TestClassEventArgs e)
		{
			if (e.TestClass.Namespace == fullNamespace) {
				// Remove test class from our monitored test classes.
				testClasses.Remove(e.TestClass);
			
				// Remove the corresponding tree node.
				foreach (ExtTreeNode node in Nodes) {
					TestClassTreeNode classNode = node as TestClassTreeNode;
					if (classNode != null && classNode.Text == e.TestClass.Name) {
						classNode.Remove();
						classNode.Dispose();
						break;
					}
				}
				
				// Remove this namespace node if there are no more child nodes.
				RemoveIfEmpty();
			}
		}
		
		/// <summary>
		/// Removes this node if it has no child nodes. This
		/// method also calls the same method on the parent
		/// namespace node so it can check whether it should
		/// remove itself.
		/// </summary>
		void RemoveIfEmpty()
		{
			if (IsEmpty) {
				Remove();
				Dispose();
				TestNamespaceTreeNode parentNode = Parent as TestNamespaceTreeNode;
				if (parentNode != null) {
					parentNode.RemoveIfEmpty();
				}
			}
		}
	}
}
