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
	public class NamespaceUnitTestNode : UnitTestBaseNode
	{
		readonly string shortName;
		readonly string namespaceName;
		
		/// <summary>
		/// Gets the namespace suffix (namespace portion after the root namespace)
		/// </summary>
		public string ShortName {
			get { return shortName; }
		}
		
		/// <summary>
		/// Gets the full namespace
		/// </summary>
		public string NamespaceName {
			get { return namespaceName; }
		}
		
		public NamespaceUnitTestNode(string namespaceName)
		{
			if (namespaceName == null)
				throw new ArgumentNullException("namespaceName");
			this.namespaceName = namespaceName;
			this.shortName = namespaceName.Substring(namespaceName.IndexOf('.') + 1);
		}
		
		public override object Text {
			get { return shortName; }
		}
		
		internal override TestResultType TestResultType {
			get {
				if (Children.Count == 0) return TestResultType.None;
				if (Children.OfType<UnitTestBaseNode>().Any(node => node.TestResultType == TestResultType.Failure))
					return TestResultType.Failure;
				if (Children.OfType<UnitTestBaseNode>().Any(node => node.TestResultType == TestResultType.None))
					return TestResultType.None;
				if (Children.OfType<UnitTestBaseNode>().Any(node => node.TestResultType == TestResultType.Ignored))
					return TestResultType.Ignored;
				return TestResultType.Success;
			}
		}
	}
}
