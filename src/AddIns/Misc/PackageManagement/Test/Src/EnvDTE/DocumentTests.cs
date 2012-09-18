// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
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
