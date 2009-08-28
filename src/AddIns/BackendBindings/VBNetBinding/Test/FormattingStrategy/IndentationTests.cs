// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using NUnit.Framework;

namespace VBNetBinding.Tests
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
		public void InterfaceWithNewLineAtEndTest()
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
		
		[Test]
		public void IfLineContinuationTest()
		{
			string expected = @"Public Class Test
	Private Sub Tester()
		If True _
			Then
			test()
		End If
	End Sub
End Class";
			
			string code = @"Public Class Test
Private Sub Tester()
If True _
Then
test()
End If
End Sub
End Class";
			
			RunFormatTest(code, expected);
		}
		
		[Test]
		public void IfMultiLineContinuationTest()
		{
			string expected = @"Public Class Test
	Private Sub Tester()
		If IsCorrect(i) _
			And _
			IsHIgherThan(i, 5) _
			Then
			test()
		End If
	End Sub
End Class";
			
			string code = @"Public Class Test
Private Sub Tester()
If IsCorrect(i) _
And _
IsHIgherThan(i, 5) _
Then
test()
End If
End Sub
End Class";
			
			RunFormatTest(code, expected);
		}
		
		void RunFormatTest(string code, string expectedCode)
		{
			AvalonEditTextEditorAdapter editor = new AvalonEditTextEditorAdapter(new TextEditor());
			editor.Document.Text = code;
			VBNetFormattingStrategy formattingStrategy = new VBNetFormattingStrategy();
			formattingStrategy.IndentLines(editor, 0, editor.Document.TotalNumberOfLines);
			
			Assert.AreEqual(expectedCode, editor.Document.Text);
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
