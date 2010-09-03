// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockTextEditorViewContentTests
	{
		MockTextEditorViewContent view;
		
		[SetUp]
		public void Init()
		{
			view = new MockTextEditorViewContent();
		}
		
		[Test]
		public void TextEditorSetCanBeRetrieved()
		{
			MockTextEditor editor = new MockTextEditor();
			view.TextEditor = editor;
			Assert.AreSame(editor, view.TextEditor);
		}
	}
}
