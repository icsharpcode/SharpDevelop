// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeInterfaceTests
	{
		ProjectContentHelper helper;
		CodeInterface codeInterface;
		
		[SetUp]
		public void Init()
		{
			helper = new ProjectContentHelper();
		}
		
		void CreateInterface()
		{
			IClass c = helper.AddInterfaceToProjectContent("MyInterface");
			codeInterface = new CodeInterface(helper.ProjectContent, c);
		}
		
		[Test]
		public void Kind_Interface_ReturnsInterface()
		{
			CreateInterface();
			
			vsCMElement kind = codeInterface.Kind;
			
			Assert.AreEqual(vsCMElement.vsCMElementInterface, kind);
		}
	}
}
