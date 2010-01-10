// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using NUnit.Framework;
using ICSharpCode.PythonBinding;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Tests the standard Python module names can be determined
	/// for IronPython.
	/// </summary>
	[TestFixture]
	public class GetPythonModulesTestFixture
	{
		string[] moduleNames;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PythonStandardModules modules = new PythonStandardModules();
			moduleNames = modules.GetNames();
		}
		
		[Test]
		public void ContainsSysModuleName()
		{
			Assert.Contains("sys", moduleNames);
		}
		
		[Test]
		public void SysModuleInListOfModulesOnlyOnce()
		{
			int countOccurrencesOfSysModuleName = 0;
			foreach (string name in moduleNames) {
				if (name == "sys") {
					countOccurrencesOfSysModuleName++;
				}
			}
			Assert.AreEqual(1, countOccurrencesOfSysModuleName);
		}
		
		[Test]
		public void ContainsBuiltInModule()
		{
			Assert.Contains("__builtin__", moduleNames, "Module names: " + WriteArray(moduleNames));
		}
		
		[Test]
		public void ContainsMathModule()
		{
			Assert.Contains("math", moduleNames);
		}
		
		static string WriteArray(string[] items)
		{
			StringBuilder text = new StringBuilder();
			foreach (string item in items) {
				text.AppendLine(item);
			}
			return text.ToString();
		}
	}
}
