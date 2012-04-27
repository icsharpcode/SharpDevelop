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
		string name;
		
		public string Namespace {
			get { return name; }
		}
		
		public NamespaceUnitTestNode(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("name");
			this.name = name;
		}
		
		public override object Text {
			get { return name.Substring(name.LastIndexOf('.') + 1); }
		}
		
		internal override TestResultType TestResultType {
			get { return TestResultType.None; }
		}
	}
}
