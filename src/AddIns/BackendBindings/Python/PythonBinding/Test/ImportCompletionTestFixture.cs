// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop;
using System;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests
{
	/// <summary>
	/// Tests the code completion after an "import" statement.
	/// </summary>
	[TestFixture]
	public class ImportCompletionTestFixture
	{
		DerivedPythonCodeCompletionBinding codeCompletionBinding;
		bool handlesImportKeyword;
		SharpDevelopTextAreaControl textAreaControl;
		TextEditorAdapter textEditor;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, String.Empty);
			}
			textAreaControl = new SharpDevelopTextAreaControl();
			textEditor = new TextEditorAdapter(textAreaControl);
			codeCompletionBinding = new DerivedPythonCodeCompletionBinding();
			handlesImportKeyword = codeCompletionBinding.HandleKeyword(textEditor, "import");
		}
		
		[Test]
		public void HandlesImportKeyWord()
		{
			Assert.IsTrue(handlesImportKeyword);
		}
		
		[Test]
		public void UnknownKeywordNotHandled()
		{
			Assert.IsFalse(codeCompletionBinding.HandleKeyword(textEditor, "Unknown"));
		}
		
		[Test]
		public void HandlesUppercaseImportKeyword()
		{
			Assert.IsTrue(codeCompletionBinding.HandleKeyword(textEditor, "IMPORT"));
		}
		
		[Test]
		public void NullKeyword()
		{
			Assert.IsFalse(codeCompletionBinding.HandleKeyword(textEditor, null));
		}
		
		[Test]
		public void CtrlSpaceCompletionDataProviderCreated()
		{
			Assert.IsTrue(codeCompletionBinding.IsCtrlSpaceCompletionDataProviderCreated);
		}
		
		[Test]
		public void CodeCompletionWindowDisplayed()
		{
			Assert.IsTrue(codeCompletionBinding.IsCodeCompletionWindowDisplayed);
		}
		
		[Test]
		[Ignore("Broken since ITextEditor introduction")]
		public void TextAreaControlUsedToDisplayCodeCompletionWindow()
		{
			Assert.AreSame(textAreaControl, codeCompletionBinding.TextAreaControlUsedToShowCompletionWindow);
		}
		
		[Test]
		public void CompletionProviderUsedWhenDisplayingCodeCompletionWindow()
		{
			Assert.AreSame(codeCompletionBinding.CtrlSpaceCompletionDataProvider, codeCompletionBinding.CompletionProviderUsedWhenDisplayingCodeCompletionWindow);
		}
		
		[Test]
		public void CompletionCharacterIsSpace()
		{
			Assert.AreEqual(' ', codeCompletionBinding.CompletionCharacter);
		}
		
		[Test]
		public void ExpressionContextIsImportable()
		{
			Assert.AreEqual(ExpressionContext.Importable, codeCompletionBinding.ExpressionContext);
		}
	}
}
