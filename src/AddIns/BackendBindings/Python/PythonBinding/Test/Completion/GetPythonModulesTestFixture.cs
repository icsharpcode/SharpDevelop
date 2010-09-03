// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Tests the standard Python module names can be determined
	/// for IronPython.
	/// </summary>
	[TestFixture]
	public class GetPythonModulesTestFixture
	{
		List<ICompletionEntry> moduleNames;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			moduleNames = new PythonStandardModules();
		}
		
		[Test]
		public void ContainsSysModuleName()
		{
			Assert.Contains(new NamespaceEntry("sys"), moduleNames);
		}
		
		[Test]
		public void SysModuleInListOfModulesOnlyOnce()
		{
			int countOccurrencesOfSysModuleName = 0;
			foreach (ICompletionEntry entry in moduleNames) {
				if (entry.Name == "sys") {
					countOccurrencesOfSysModuleName++;
				}
			}
			Assert.AreEqual(1, countOccurrencesOfSysModuleName);
		}
		
		[Test]
		public void ContainsBuiltInModule()
		{
			Assert.Contains(new NamespaceEntry("__builtin__"), moduleNames, "Module names: " + WriteList(moduleNames));
		}
		
		[Test]
		public void ContainsMathModule()
		{
			Assert.Contains(new NamespaceEntry("math"), moduleNames);
		}
		
		static string WriteList(List<ICompletionEntry> items)
		{
			StringBuilder text = new StringBuilder();
			foreach (ICompletionEntry item in items) {
				text.AppendLine(item.Name);
			}
			return text.ToString();
		}
	}
}
