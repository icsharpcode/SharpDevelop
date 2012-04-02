// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		Solution solution;
		
		public RootUnitTestNode(Solution solution)
		{
			this.solution = solution;
			ProjectService.ProjectAdded += OnProjectAdded;
			ProjectService.ProjectRemoved += OnProjectRemoved;
			ParserService.LoadSolutionProjectsThreadEnded += delegate { LoadChildren(); };
			LazyLoading = true;
		}

		void OnProjectRemoved(object sender, ProjectEventArgs e)
		{
			LoadChildren();
		}

		void OnProjectAdded(object sender, ProjectEventArgs e)
		{
			LoadChildren();
		}
		
		protected override void LoadChildren()
		{
			this.Children.Clear();
			if (!ParserService.LoadSolutionProjectsThreadRunning)
				this.Children.AddRange(solution.Projects.Where(p => p.IsTestProject()).Select(p => new ProjectUnitTestNode(new TestProject(p))));
		}
		
		public override object Text {
			get { return ResourceService.GetString("ICSharpCode.UnitTesting.AllTestsTreeNode.Text"); }
		}
	}
	
	public static class Extensions
	{
		public static bool IsTestProject(this IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (project.ProjectContent == null)
				return false;
			return ParserService.GetCompilation(project).FindType(typeof(TestAttribute)).Kind != TypeKind.Unknown;
		}
	}
}
