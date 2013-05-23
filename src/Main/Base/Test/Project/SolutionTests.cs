// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Execution;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop
{
	[TestFixture]
	public class SolutionTests
	{
		[Test, Ignore]
		public void UpdateMSBuildProperties_SolutionHasFileName_SolutionDefinesSolutionDirMSBuildPropertyWithDirectoryEndingInForwardSlash()
		{
			/*CreateSolution();
			solution.FileName = @"d:\projects\MyProject\MySolution.sln";
			
			solution.UpdateMSBuildProperties();
			
			ProjectPropertyInstance property = solution.MSBuildProjectCollection.GetGlobalProperty("SolutionDir");
			string solutionDir = property.EvaluatedValue;
			
			string expectedSolutionDir = @"d:\projects\MyProject\";
			Assert.AreEqual(expectedSolutionDir, solutionDir);*/
		}
	}
}
