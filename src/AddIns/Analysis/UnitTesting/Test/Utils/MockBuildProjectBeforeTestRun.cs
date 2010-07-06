// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
