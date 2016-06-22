// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
