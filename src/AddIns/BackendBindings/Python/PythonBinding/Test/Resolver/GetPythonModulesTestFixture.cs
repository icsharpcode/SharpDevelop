// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;
using ICSharpCode.PythonBinding;

namespace PythonBinding.Tests.Resolver
{
	/// <summary>
	/// Tests the standard Python module names can be determined
	/// from the IronPython assembly.
	/// </summary>
	[TestFixture]
	public class GetPythonModulesTestFixture
	{
		string[] moduleNames;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			StandardPythonModules modules = new StandardPythonModules();
			moduleNames = modules.GetNames();
		}
		
		[Test]
		public void ContainsSysModuleName()
		{
			Assert.Contains("sys", moduleNames);
		}
		
		[Test]
		public void ContainsBuiltInModule()
		{
			Assert.Contains("__builtin__", moduleNames);
		}
	}
}
