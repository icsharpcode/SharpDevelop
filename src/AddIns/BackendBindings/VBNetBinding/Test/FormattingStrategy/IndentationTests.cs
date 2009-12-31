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
using VBNetBinding.FormattingStrategy;

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
		
		[Test]
		public void ElseIfMultiLineContinuationTest()
		{
			string expected = @"Public Class Test
	Private Sub Tester()
		If True Then
			'CODE HERE
		Else If False And _
			False Then
			'CODE HERE1
		End If
	End Sub
End Class";
			
			string code = @"Public Class Test
Private Sub Tester()
If True Then
'CODE HERE
Else If False And _
False Then
'CODE HERE1
End If
End Sub
End Class";
			
			RunFormatTest(code, expected);
		}
		
		[Test]
		public void SelectCaseTest()
		{
						string expected = @"Public Class Test
	Private Sub Tester()
		Select Case a
			Case 0
				DoSomething()
				' TEST CASE 0
			Case 1
				'TEST CASE 1
			Case 2
				'TEST CASE 2
			Case Else
				DoElse()
		End Select
	End Sub
End Class";
			
			string code = @"Public Class Test
Private Sub Tester()
Select Case a
Case 0
DoSomething()
' TEST CASE 0
Case 1
'TEST CASE 1
Case 2
'TEST CASE 2
Case Else
DoElse()
End Select
End Sub
End Class";
			
			RunFormatTest(code, expected);
		}
		
		[Test]
		public void SelectCaseTest2()
		{
						string expected = @"Public Class Test
	Private Sub Tester()
		Select Case a
			Case 0
				DoSomething()
			Case 1
			Case 2
			Case Else
				DoElse()
		End Select
	End Sub
End Class";
			
			string code = @"Public Class Test
Private Sub Tester()
Select Case a
Case 0
DoSomething()
Case 1
Case 2
Case Else
DoElse()
End Select
End Sub
End Class";
			
			RunFormatTest(code, expected);
		}
		
		[Test]
		public void WithTest()
		{
						string expected = @"Public Class Test
	Private Sub Tester()
		With a
			If True Then
				' Test
			ElseIf False Then
				' Test3
			Else
				' Test2
			End If
		End With
	End Sub
End Class";
			
			string code = @"Public Class Test
Private Sub Tester()
With a
If True Then
' Test
ElseIf False Then
' Test3
Else
' Test2
End If
End With
End Sub
End Class";
			
			RunFormatTest(code, expected);
		}
		
		[Test]
		public void WithTest2()
		{
						string expected = @"Public Class Test
	Private Sub Tester()
		With a
			If True Then
				' Test
			ElseIf False Then
				' Test3
			Else
				Try
					DoSomething()
				Catch ex As Exception
					'Handle
				End Try
			End If
		End With
	End Sub
End Class";
			
			string code = @"Public Class Test
Private Sub Tester()
With a
If True Then
' Test
ElseIf False Then
' Test3
Else
Try
DoSomething()
Catch ex As Exception
'Handle
End Try
End If
End With
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
