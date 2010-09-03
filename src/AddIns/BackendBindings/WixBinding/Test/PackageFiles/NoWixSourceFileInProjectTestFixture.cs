// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class NoWixSourceFileInProjectTestFixture : ITextFileReader, IWixDocumentWriter
	{
		MockWixPackageFilesView view;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			p.Name = "MySetup";
			view = new MockWixPackageFilesView();
			view.ContextMenuEnabled = true;
			WixPackageFilesEditor editor = new WixPackageFilesEditor(view, this, this);
			editor.ShowFiles(p);
		}
		
		[Test]
		public void NoSourceFileFoundMessageDisplayed()
		{
			Assert.IsTrue(view.IsNoSourceFileFoundMessageDisplayed);
		}
		
		[Test]
		public void WixProjectNameDisplayed()
		{
			Assert.AreEqual("MySetup", view.NoSourceFileFoundProjectName);
		}
		
		[Test]
		public void NoRootDirectoryAdded()
		{
			Assert.AreEqual(0, view.DirectoriesAdded.Count);
		}
		
		public TextReader Create(string fileName)
		{
			return null;
		}
		
		public void Write(WixDocument document)
		{	
		}
		
		[Test]
		public void ContextMenuDisabledInPackageFilesView()
		{
			Assert.IsFalse(view.ContextMenuEnabled);
		}
	}
}
