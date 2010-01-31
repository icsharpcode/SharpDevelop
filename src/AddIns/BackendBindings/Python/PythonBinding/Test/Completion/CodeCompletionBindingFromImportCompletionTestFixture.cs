// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests
{
	/// <summary>
	/// Tests that the From keyword is correctly identified as a 
	/// importable code completion keyword.
	/// </summary>
	[TestFixture]
	public class FromImportCompletionTestFixture
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
			handlesImportKeyword = codeCompletionBinding.HandleKeyword(textEditor, "from");
		}
		
		[Test]
		public void HandlesImportKeyWord()
		{
			Assert.IsTrue(handlesImportKeyword);
		}
	}
}
