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
	public class GoToSelectedBaseClassMethodTestFixture
	{
		MockTestTreeView treeView;
		GotoDefinitionCommand gotoDefinitionCommand;
		MockFileService fileService;
		MockMethod baseClassMethod;
		
		[SetUp]
		public void Init()
		{
			baseClassMethod = MockMethod.CreateMockMethodWithoutAnyAttributes();
			baseClassMethod.DeclaringType.CompilationUnit.FileName = @"c:\projects\mytest.cs";
			
			MockClass derivedClass = MockClass.CreateMockClassWithoutAnyAttributes();
			derivedClass.CompilationUnit.FileName = @"d:\projects\myderivedtestclass.cs";
			
			int methodBeginLine = 3; // 1 based.
			int methodBeginColumn = 6; // 1 based.
			baseClassMethod.Region = new DomRegion(methodBeginLine, methodBeginColumn);
			
			BaseTestMember baseTestMethod = new BaseTestMember(derivedClass, baseClassMethod);
			
			treeView = new MockTestTreeView();
			treeView.SelectedMember = baseTestMethod;
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
	}
}
