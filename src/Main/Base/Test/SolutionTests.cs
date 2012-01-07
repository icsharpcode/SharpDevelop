// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Execution;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class SolutionTests
	{
		Solution solution;
		
		void CreateSolution()
		{
			IProjectChangeWatcher fakeWatcher = MockRepository.GenerateStub<IProjectChangeWatcher>();
			solution = new Solution(fakeWatcher);
		}
		
		[Test]
		public void UpdateMSBuildProperties_SolutionHasFileName_SolutionDefinesSolutionDirMSBuildPropertyWithDirectoryEndingInForwardSlash()
		{
			CreateSolution();
			solution.FileName = @"d:\projects\MyProject\MySolution.sln";
			
			solution.UpdateMSBuildProperties();
			
			ProjectPropertyInstance property = solution.MSBuildProjectCollection.GetGlobalProperty("SolutionDir");
			string solutionDir = property.EvaluatedValue;
			
			string expectedSolutionDir = @"d:\projects\MyProject\";
			Assert.AreEqual(expectedSolutionDir, solutionDir);
		}
	}
}
