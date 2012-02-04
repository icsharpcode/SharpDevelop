// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	[TestFixture]
	public class GoToSelectedMethodWithNoLocationTestFixture
	{
		MockTestTreeView treeView;
		GotoDefinitionCommand gotoDefinitionCommand;
		MockFileService fileService;
		MockMethod method;
		
		[SetUp]
		public void Init()
		{
			method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			method.DeclaringType.CompilationUnit.FileName = @"c:\projects\mytest.cs";
			
			treeView = new MockTestTreeView();
			treeView.SelectedMember = method;
			fileService = new MockFileService();
			gotoDefinitionCommand = new GotoDefinitionCommand(fileService);
			gotoDefinitionCommand.Owner = treeView;
			gotoDefinitionCommand.Run();
		}
		
		[Test]
		public void FileIsOpenedInsteadOfJumpingToFileLocation()
		{
			Assert.AreEqual(@"c:\projects\mytest.cs", fileService.FileOpened);
		}
	}
}
