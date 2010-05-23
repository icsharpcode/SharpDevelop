// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
			treeView.SelectedMethod = method;
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
