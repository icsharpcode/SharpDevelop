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
	public class GoToSelectedMethodTestFixture
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
			
			int methodBeginLine = 3; // 1 based.
			int methodBeginColumn = 6; // 1 based.
			method.Region = new DomRegion(methodBeginLine, methodBeginColumn);
			
			treeView = new MockTestTreeView();
			treeView.SelectedMember = method;
			fileService = new MockFileService();
			gotoDefinitionCommand = new GotoDefinitionCommand(fileService);
			gotoDefinitionCommand.Owner = treeView;
			gotoDefinitionCommand.Run();
		}
		
		[Test]
		public void MethodIsJumpedTo()
		{
			int line = 2; // zero based.
			int col = 5; // zero based.
			FilePosition expectedFilePos = new FilePosition(@"c:\projects\mytest.cs", line, col);
			
			Assert.AreEqual(expectedFilePos, fileService.FilePositionJumpedTo);
		}
		
		[Test]
		public void ExceptionNotThrownWhenSelectedTestTreeMethodIsNull()
		{
			treeView.SelectedMember = null;
			Assert.DoesNotThrow(delegate { gotoDefinitionCommand.Run(); });
		}
		
		[Test]
		public void ExceptionNotThrownWhenCommandOwnerIsNull()
		{
			gotoDefinitionCommand.Owner = null;
			Assert.DoesNotThrow(delegate { gotoDefinitionCommand.Run(); });
		}
	}
}
