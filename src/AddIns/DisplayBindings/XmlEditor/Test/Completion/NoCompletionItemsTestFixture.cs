// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
