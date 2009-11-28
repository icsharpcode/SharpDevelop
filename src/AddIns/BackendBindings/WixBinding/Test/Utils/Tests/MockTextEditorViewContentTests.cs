// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
