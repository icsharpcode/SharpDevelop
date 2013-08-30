// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using NUnit.Framework;
using Rhino.Mocks;

namespace PackageManagement.Tests
{
	[TestFixture]
	public class NuGetPackageRestoreCommandLineTests
	{
		NuGetPackageRestoreCommandLine commandLine;
		
		void CreateCommandLineWithSolution(string fileName)
		{
			IPackageManagementSolution solution = MockRepository.GenerateStub<IPackageManagementSolution>();
			solution.Stub(s => s.FileName).Return(fileName);
			commandLine = new NuGetPackageRestoreCommandLine(solution);
		}
		
		[Test]
		public void Arguments_RestoreSolution_SolutionFullFileNameUsed()
		{
			CreateCommandLineWithSolution(@"d:\projects\MySolution\MySolution.sln");
			
			string arguments = commandLine.Arguments;
			
			Assert.AreEqual("restore \"d:\\projects\\MySolution\\MySolution.sln\"", arguments);
		}
		
		[Test]
		public void ToString_RestoreSolution_FullCommandLineReturned()
		{
			CreateCommandLineWithSolution(@"d:\projects\MySolution\MySolution.sln");
			commandLine.Command = @"d:\sharpdevelop\NuGet.exe";
			
			string fullCommandLine = commandLine.ToString();
			
			Assert.AreEqual("\"d:\\sharpdevelop\\NuGet.exe\" restore \"d:\\projects\\MySolution\\MySolution.sln\"", fullCommandLine);
		}
	}
}
