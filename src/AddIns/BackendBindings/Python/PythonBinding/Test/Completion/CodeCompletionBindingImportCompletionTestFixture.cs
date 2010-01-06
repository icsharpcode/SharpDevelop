// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	public class CodeCompletionBindingImportCompletionTestFixture
	{
		DerivedPythonCodeCompletionBinding codeCompletionBinding;
		bool handlesImportKeyword;
		SharpDevelopTextAreaControl textAreaControl;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, String.Empty);
			}
			textAreaControl = new SharpDevelopTextAreaControl();
			codeCompletionBinding = new DerivedPythonCodeCompletionBinding();
			handlesImportKeyword = codeCompletionBinding.HandleKeyword(textAreaControl, "import");
		}
		
		[Test]
		public void HandlesImportKeyWord()
		{
			Assert.IsTrue(handlesImportKeyword);
		}
		
		[Test]
		public void UnknownKeywordNotHandled()
		{
			Assert.IsFalse(codeCompletionBinding.HandleKeyword(textAreaControl, "Unknown"));
		}
		
		[Test]
		public void HandlesUppercaseImportKeyword()
		{
			Assert.IsTrue(codeCompletionBinding.HandleKeyword(textAreaControl, "IMPORT"));
		}
		
		[Test]
		public void NullKeyword()
		{
			Assert.IsFalse(codeCompletionBinding.HandleKeyword(textAreaControl, null));
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
			Assert.AreSame(textAreaControl, codeCompletionBinding.TextAreaControlUsedToShowCompletionWindow);
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
