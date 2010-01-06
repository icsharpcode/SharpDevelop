// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using IronPython.Modules;
using NUnit.Framework;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class GetTypeForPythonModuleTestFixture
	{
		StandardPythonModules modules;

		[SetUp]
		public void Init()
		{
			 modules = new StandardPythonModules();
		}
		
		[Test]
		public void GetTypeReturnsNullForUnknownModuleName()
		{
			Assert.IsNull(modules.GetTypeForModule("unknown"));
		}
		
		[Test]
		public void GetTypeReturnsSysModuleTypeForSysModuleName()
		{
			Assert.AreEqual(typeof(SysModule), modules.GetTypeForModule("sys"));
		}
	}
}
