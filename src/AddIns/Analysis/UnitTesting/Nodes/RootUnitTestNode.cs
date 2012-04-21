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
}
