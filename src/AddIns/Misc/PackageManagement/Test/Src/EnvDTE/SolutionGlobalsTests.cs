// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class SolutionGlobalsTests
	{
		SolutionHelper solutionHelper;
		Globals globals;
		
		void CreateSolution()
		{
			solutionHelper = new SolutionHelper();
			globals = solutionHelper.Solution.Globals;
		}
		
		void AddVariableToExtensibilityGlobals(string name)
		{
			AddVariableToExtensibilityGlobals(name, String.Empty);
		}
		
		void AddVariableToExtensibilityGlobals(string name, string value)
		{
			solutionHelper.AddVariableToExtensibilityGlobals(name, value);
		}
		
		void AddExtensibilityGlobalsSection()
		{
			solutionHelper.AddExtensibilityGlobalsSection();
		}
		
		[Test]
		public void VariableExists_VariableExistsInExtensibilityGlobalsSection_ReturnsTrue()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("test");
			
			bool exists = globals.VariableExists("test");
			
			Assert.IsTrue(exists);
		}
		
		[Test]
		public void VariableExists_NoExtensibilityGlobalsSection_ReturnsFalse()
		{
			CreateSolution();
			
			bool exists = globals.VariableExists("test");
			
			Assert.IsFalse(exists);
		}
		
		[Test]
		public void VariableExists_ExtensibilityGlobalsSectionExistsButHasNoVariables_ReturnsFalse()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			
			bool exists = globals.VariableExists("test");
			
			Assert.IsFalse(exists);
		}
		
		[Test]
		public void VariableExists_VariableExistsInExtensibilityGlobalsSectionWithDifferentCasing_ReturnsTrue()
		{
			CreateSolution();
			AddExtensibilityGlobalsSection();
			AddVariableToExtensibilityGlobals("TEST");
			
			bool exists = globals.VariableExists("test");
			
			Assert.IsTrue(exists);
		}
	}
}
