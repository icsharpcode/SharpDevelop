// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ICSharpCode.Core;
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
		}
		
		protected override void LoadChildren()
		{
			base.LoadChildren();
		}
		
		public override object Text {
			get { return project.Name; }
		}
	}
}
