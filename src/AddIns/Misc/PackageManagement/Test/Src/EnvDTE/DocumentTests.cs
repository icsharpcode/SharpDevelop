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
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;
using NUnit.Framework;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class DocumentTests
	{
		IViewContent view;
		Document document;
		OpenedFile openedFile;
		
		[SetUp]
		public void Init()
		{
			openedFile = MockRepository.GenerateStub<OpenedFile>();
			view = MockRepository.GenerateStub<IViewContent>();
			view.Stub(v => v.PrimaryFile).Return(openedFile);
		}
		
		void CreateDocument(string fileName)
		{
			document = new Document(fileName, view);
		}
		
		void OpenFileIsDirty()
		{
			openedFile.IsDirty = true;
		}
		
		[Test]
		public void Saved_SetToTrue_OpenFileIsDirtySetToFalse()
		{
			CreateDocument(@"d:\projects\MyProject\program.cs");
			OpenFileIsDirty();
			
			document.Saved = true;
			
			Assert.IsFalse(openedFile.IsDirty);
		}
		
		[Test]
		public void Saved_SetToFalse_OpenFileIsDirtySetToTrue()
		{
			CreateDocument(@"d:\projects\MyProject\program.cs");
			
			document.Saved = false;
			
			Assert.IsTrue(openedFile.IsDirty);
		}
	}
}
