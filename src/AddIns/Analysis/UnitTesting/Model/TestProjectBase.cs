// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Base class for <see cref="ITestProject"/> implementations.
	/// </summary>
	public abstract class TestProjectBase : TestBase, ITestProject
	{
		IProject project;
		
		public TestProjectBase(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			BindResultToCompositeResultOfNestedTests();
		}
		
		public IProject Project {
			get { return project; }
		}
		
		public override ITestProject ParentProject {
			get { return this; }
		}
		
		public override string DisplayName {
			get { return project.Name; }
		}
		
		public virtual ITest GetTestForEntity(IEntity entity)
		{
			return null;
		}
		
		public virtual IBuildable GetBuildableForTesting()
		{
			return project;
		}
		
		public void NotifyParseInformationChanged(IUnresolvedFile oldUnresolvedFile, IUnresolvedFile newUnresolvedFile)
		{
			if (!NestedTestsInitialized)
				return;
		}
		
		public abstract Task RunTestsAsync(IEnumerable<ITest> tests, TestExecutionOptions options, IProgressMonitor progressMonitor);
	}
}
