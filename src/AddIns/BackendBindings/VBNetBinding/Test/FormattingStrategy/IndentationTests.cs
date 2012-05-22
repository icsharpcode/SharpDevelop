// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.AvalonEdit;
using System;
using ICSharpCode.AvalonEdit;
using ICSharpCode.Core;
using NUnit.Framework;

namespace ICSharpCode.VBNetBinding.Tests
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
		public void SimpleInterfaceWithModifierTest()
		{
			string code = @"Public Interface t
Sub Test()
Sub Test2()
End Interface";
			
			string expectedCode = @"Public Interface t
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
		public void SD1775_Fix()
		{
			string expected = @"Public Class Test
	Private Sub Tester()
		B = New Burger With {Cheese = True, _
			Lettuce = True, _
			Tomato = True, _
			CookLevel = MeatCookLevel.WellDone}
	End Sub
End Class";
			
			string code = @"Public Class Test
Private Sub Tester()
B = New Burger With {Cheese = True, _
Lettuce = True, _
Tomato = True, _
CookLevel = MeatCookLevel.WellDone}
End Sub
End Class";
			
			RunFormatTest(code, expected);
		}
		
		[Test]
		public void Simple()
		{
			string expected = @"'
' Created by SharpDevelop.
' User: Siegfried
' Date: 25.02.2011
' Time: 14:41
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
	' This file controls the behaviour of the application.
	Partial Class MyApplication
		Public Sub New()
			MyBase.New(AuthenticationMode.Windows)
			Me.IsSingleInstance = False
			Me.EnableVisualStyles = True
			Me.SaveMySettingsOnExit = True
			Me.ShutDownStyle = ShutdownMode.AfterMainFormCloses
		End Sub
		
		Protected Overrides Sub OnCreateMainForm()
			Me.MainForm = My.Forms.MainForm
		End Sub
	End Class
End Namespace";
			
			string code = @"'
' Created by SharpDevelop.
' User: Siegfried
' Date: 25.02.2011
' Time: 14:41
'
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
' This file controls the behaviour of the application.
Partial Class MyApplication
Public Sub New()
MyBase.New(AuthenticationMode.Windows)
Me.IsSingleInstance = False
Me.EnableVisualStyles = True
Me.SaveMySettingsOnExit = True
Me.ShutDownStyle = ShutdownMode.AfterMainFormCloses
End Sub

Protected Overrides Sub OnCreateMainForm()
Me.MainForm = My.Forms.MainForm
End Sub
End Class
End Namespace";
			
			RunFormatTest(code, expected);
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
		
		[Test, Ignore]
		// TODO : find out why unit test does not work, but normal run works
		public void FunctionLambda()
		{
			string expected = @"Public Class Test
	Private Sub Tester()
		Dim increment1 = Function(x) x + 1
		Dim increment2 = Function(x)
		                 	Return x + 2
		                 End Function
	End Sub
End Class";
			
			string code = @"Public Class Test
Private Sub Tester()
Dim increment1 = Function(x) x + 1
Dim increment2 = Function(x)
Return x + 2
End Function
End Sub
End Class";
			
			RunFormatTest(code, expected);
		}
		
		[Test, Ignore]
		// TODO : find out why unit test does not work, but normal run works
		public void SubLambda()
		{
			string expected = @"Public Class Test
	Private Sub Tester()
		Dim writeline1 = Sub(x) Console.WriteLine(x)
		Dim writeline2 = Sub(x)
		                 	Console.WriteLine(x)
		                 End Sub
	End Sub
End Class";
			
			string code = @"Public Class Test
Private Sub Tester()
Dim writeline1 = Sub(x) Console.WriteLine(x)
Dim writeline2 = Sub(x)
Console.WriteLine(x)
End Sub
End Sub
End Class";
			
			RunFormatTest(code, expected);
		}
		
		[Test]
		public void Region()
		{
			string expected = @"Module Program
	#Region Test
	
	#End Region
	
	Sub asdf()
		
	End Sub
End Module";
			
			string code = @"Module Program
#Region Test

#End Region

Sub asdf()

End Sub
End Module";
			
			RunFormatTest(code, expected);
		}
		
		[Test]
		public void Attribute()
		{
			string expected = @"Module Core
	<STAThreadAttribute> _
	Sub Main
		
	End Sub
End Module";
			
			string code = @"Module Core
<STAThreadAttribute> _
Sub Main

End Sub
End Module";
			
			RunFormatTest(code, expected);
		}
		
		[Test]
		public void Attribute2()
		{
			string expected = @"Module Core
	<AttributeUsage(
		bla.bla
		)> _
	Sub Main
		
	End Sub
End Module";
			
			string code = @"Module Core
<AttributeUsage(
bla.bla
)> _
Sub Main

End Sub
End Module";
			
			RunFormatTest(code, expected);
		}
		
		[Test]
		public void ForNextOneLine()
		{
			string expected = @"Module Core
	Sub Main
		Dim a = 1
		For i = 0 To 10 : Console.WriteLine(i) : Next
		Dim b = 2
	End Sub
End Module";
			
			string code = @"Module Core
Sub Main
Dim a = 1
For i = 0 To 10 : Console.WriteLine(i) : Next
Dim b = 2
End Sub
End Module";
			
			RunFormatTest(code, expected);
		}
		
		[Test]
		public void RandomNext()
		{
			string expected = @"Module Core
	Public Function GetRandomNumber( _
		Optional ByVal Low As Integer = 1, _
		Optional ByVal High As Integer = 100) As Integer
		' Returns a random number,
		' between the optional Low and High parameters
		Return objRandom.Next(Low, High + 1)
	End Function
End Module";
			
			string code = @"Module Core
Public Function GetRandomNumber( _
Optional ByVal Low As Integer = 1, _
Optional ByVal High As Integer = 100) As Integer
' Returns a random number,
' between the optional Low and High parameters
Return objRandom.Next(Low, High + 1)
End Function
End Module";
			
			RunFormatTest(code, expected);
		}
		
		void RunFormatTest(string code, string expectedCode)
		{
			AvalonEditTextEditorAdapter editor = new AvalonEditTextEditorAdapter(new TextEditor());
			editor.Document.Text = code;
			VBNetFormattingStrategy formattingStrategy = new VBNetFormattingStrategy();
			formattingStrategy.IndentLines(editor, 1, editor.Document.TotalNumberOfLines);
			
			Console.WriteLine(editor.Document.Text);
			
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
