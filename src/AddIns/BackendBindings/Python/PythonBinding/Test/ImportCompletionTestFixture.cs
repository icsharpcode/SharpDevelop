// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
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
		ICSharpCode.SharpDevelop.Editor.ITextEditor textEditor;
		DerivedPythonCodeCompletionBinding codeCompletionBinding;
		bool handlesImportKeyword;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PropertyService.InitializeServiceForUnitTests();
			codeCompletionBinding = new DerivedPythonCodeCompletionBinding();
			textEditor = new AvalonEditTextEditorAdapter(new ICSharpCode.AvalonEdit.TextEditor());
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
			Assert.AreSame(textEditor, codeCompletionBinding.TextAreaControlUsedToShowCompletionWindow);
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
