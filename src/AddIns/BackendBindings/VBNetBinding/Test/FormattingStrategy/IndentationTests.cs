// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor;
using System;
using ICSharpCode.Core;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using VBNetBinding.FormattingStrategy;

namespace VBNetBinding.Tests.FormattingStrategy
{
	[TestFixture]
	public class IndentationTests
	{
		[Test]
		public void SimpleInterfaceTest()
		{
			string code = @"Interface t
Sub Test()
Sub Test2()
End Interface";
			
			string expectedCode = @"Interface t
	Sub Test()
	Sub Test2()
End Interface";
			
			RunFormatTest(code, expectedCode);
		}
		
		[Test]
		public void ArrayInitializerTest()
		{
			string expected = @"Public Class Test
	Private Sub Tester()
		test(asdf, _
			asdf, _
			asdf, _
			asdf)
		
		Dim test As Integer() = { _
			2,2,3,34,4,5 _
		}
	End Sub
End Class";
			
			string code = @"Public Class Test
	Private Sub Tester()
		test(asdf, _
			asdf, _
			asdf, _
			asdf)
		
		Dim test As Integer() = { _
		2,2,3,34,4,5 _
		}
	End Sub
End Class";
			
			RunFormatTest(code, expected);
		}
		
		void RunFormatTest(string code, string expectedCode)
		{
			using (TextEditorControl editor = new TextEditorControl()) {
				editor.Document.TextContent = code;
				VBFormattingStrategy formattingStrategy = new VBFormattingStrategy();
				formattingStrategy.IndentLines(editor.ActiveTextAreaControl.TextArea, 0, editor.Document.TotalNumberOfLines);
				
				Assert.AreEqual(expectedCode, editor.Document.TextContent);
			}
		}
		
		[TestFixtureSetUp]
		public void Init()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, "VBNetBindingTests");
			}
		}
	}
}
