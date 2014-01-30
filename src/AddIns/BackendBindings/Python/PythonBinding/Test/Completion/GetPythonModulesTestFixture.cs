// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
