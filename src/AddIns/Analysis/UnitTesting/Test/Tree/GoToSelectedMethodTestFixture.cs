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
			treeView.SelectedMethod = method;
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
			treeView.SelectedMethod = null;
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
