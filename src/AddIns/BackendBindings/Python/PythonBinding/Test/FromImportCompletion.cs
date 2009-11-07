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
	public class FromImportCompletion
	{
		DerivedPythonCodeCompletionBinding codeCompletionBinding;
		bool handlesImportKeyword;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			PropertyService.InitializeServiceForUnitTests();
			codeCompletionBinding = new DerivedPythonCodeCompletionBinding();
			var editor = new AvalonEditTextEditorAdapter(new ICSharpCode.AvalonEdit.TextEditor());
			handlesImportKeyword = codeCompletionBinding.HandleKeyword(editor, "from");
		}
		
		[Test]
		public void HandlesImportKeyWord()
		{
			Assert.IsTrue(handlesImportKeyword);
		}
		
		[Test]
		public void ExpressionContextIsImportable()
		{
			Assert.AreEqual(ExpressionContext.Importable, codeCompletionBinding.ExpressionContext);
		}		
	}
}
