// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	public class MockBuildProjectBeforeTestRun : BuildProjectBeforeExecute
	{
		bool runMethodCalled;
		
		public MockBuildProjectBeforeTestRun()
			: base(null)
		{
		}
		
		public IProject[] Projects { get; set; }
		
		public void FireBuildCompleteEvent()
		{
			base.OnBuildComplete(new EventArgs());
		}
		
		public override void Run()
		{
			runMethodCalled = true;
			LastBuildResults = new BuildResults();
		}
		
		public bool IsRunMethodCalled {
			get { return runMethodCalled; }
		}
	}
}
