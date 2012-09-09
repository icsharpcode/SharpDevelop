// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// NUnit test project.
	/// </summary>
	public class NUnitTestProject : TestProjectBase
	{
		public NUnitTestProject(IProject project) : base(project)
		{
		}
		
		public override Task RunTestsAsync(IEnumerable<ITest> tests, TestExecutionOptions options, IProgressMonitor progressMonitor)
		{
			throw new NotImplementedException();
		}
	}
}
