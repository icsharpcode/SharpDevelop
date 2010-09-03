// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using IronPython.Modules;
using NUnit.Framework;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class GetTypeForPythonModuleTestFixture
	{
		PythonStandardModules modules;
		PythonStandardModuleType sysModuleType;

		[SetUp]
		public void Init()
		{
			 modules = new PythonStandardModules();
			 sysModuleType = modules.GetModuleType("sys");
		}
		
		[Test]
		public void GetTypeReturnsNullForUnknownModuleName()
		{
			Assert.IsNull(modules.GetModuleType("unknown"));
		}
		
		[Test]
		public void GetTypeReturnsSysModuleTypeForSysModuleName()
		{
			Assert.AreEqual(typeof(SysModule), modules.GetModuleType("sys").Type);
		}
		
		[Test]
		public void GetModuleTypeReturnsSysModuleTypeForSysModuleName()
		{
			Assert.AreEqual(typeof(SysModule), sysModuleType.Type);
		}
		
		[Test]
		public void GetModuleTypeReturnsSysModuleNameForSysModuleName()
		{
			Assert.AreEqual("sys", sysModuleType.Name);
		}
	}
}
