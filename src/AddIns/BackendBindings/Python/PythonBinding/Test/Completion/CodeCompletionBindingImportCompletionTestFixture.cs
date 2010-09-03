// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	/// <summary>
	/// Tests the code completion after an "import" statement.
	/// </summary>
	[TestFixture]
	public class CodeCompletionBindingImportCompletionTestFixture
	{
		DerivedPythonCodeCompletionBinding codeCompletionBinding;
		bool handlesImportKeyword;
		AvalonEditTextEditorAdapter textEditor;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, String.Empty);
			}
			textEditor = new AvalonEditTextEditorAdapter(new ICSharpCode.AvalonEdit.TextEditor());
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
		public void CompletionDataProviderCreated()
		{
			Assert.IsTrue(codeCompletionBinding.IsCompletionDataProviderCreated);
		}
		
		[Test]
		public void CodeCompletionWindowDisplayed()
		{
			Assert.IsTrue(codeCompletionBinding.IsCodeCompletionWindowDisplayed);
		}
		
		[Test]
		public void TextAreaControlUsedToDisplayCodeCompletionWindow()
		{
			Assert.AreSame(textEditor, codeCompletionBinding.TextEditorUsedToShowCompletionWindow);
		}
		
		[Test]
		public void CompletionProviderUsedWhenDisplayingCodeCompletionWindow()
		{
			Assert.AreSame(codeCompletionBinding.CompletionDataProvider, codeCompletionBinding.CompletionProviderUsedWhenDisplayingCodeCompletionWindow);
		}
		
		[Test]
		public void CompletionCharacterIsSpace()
		{
			Assert.AreEqual(' ', codeCompletionBinding.CompletionCharacter);
		}
	}
}
