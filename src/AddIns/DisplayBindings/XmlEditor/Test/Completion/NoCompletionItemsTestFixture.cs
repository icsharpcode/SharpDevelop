// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class NoCompletionItemsTestFixture : NamespaceCompletionWindowTestFixtureBase
	{
		[SetUp]
		public void Init()
		{
			base.InitBase();
			
			schemas.Clear();
			
			XmlCodeCompletionBinding completionBinding = new XmlCodeCompletionBinding(associations);
			keyPressResult = completionBinding.HandleKeyPress(textEditor, '=');
		}
		
		[Test]
		public void TextEditorShowCompletionWindowNotCalledWhenNoCompletionItems()
		{
			Assert.IsFalse(textEditor.IsShowCompletionWindowMethodCalled);
		}
	}
}
