' Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy of this
' software and associated documentation files (the "Software"), to deal in the Software
' without restriction, including without limitation the rights to use, copy, modify, merge,
' publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
' to whom the Software is furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all copies or
' substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
' INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
' PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
' FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
' OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.

Imports System.Text
Imports ICSharpCode.SharpDevelop.Project
Imports Microsoft.Build.Framework
Imports System.IO

''' <summary>
''' Fixes SD2-995 : Special characters not correctly encoded for languages others than English
''' </summary>
<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId := "Vbc")> _
Public NotInheritable Class VbcEncodingFixingLogger
	Implements IMSBuildLoggerFilter
	Public Function CreateFilter(context As IMSBuildLoggerContext, nextFilter As IMSBuildChainedLoggerFilter) As IMSBuildChainedLoggerFilter Implements IMSBuildLoggerFilter.CreateFilter
		Return New VbcLoggerImpl(nextFilter)
	End Function

	Private NotInheritable Class VbcLoggerImpl
		Implements IMSBuildChainedLoggerFilter
		ReadOnly nextFilter As IMSBuildChainedLoggerFilter

		Public Sub New(nextFilter As IMSBuildChainedLoggerFilter)
			Me.nextFilter = nextFilter
		End Sub

		Private Shared Function FixEncoding(text As String) As String
			If text Is Nothing Then
				Return text
			End If
			Return Encoding.[Default].GetString(ICSharpCode.SharpDevelop.ProcessRunner.OemEncoding.GetBytes(text))
		End Function

		Public Sub HandleError([error] As BuildError) Implements IMSBuildChainedLoggerFilter.HandleError
			[error].ErrorText = FixEncoding([error].ErrorText)
			[error].FileName = FixEncoding([error].FileName)
			[error].Column = FixColumn([error].FileName, [error].Line, [error].Column)
			nextFilter.HandleError([error])
		End Sub

		Public Sub HandleBuildEvent(e As Microsoft.Build.Framework.BuildEventArgs) Implements IMSBuildChainedLoggerFilter.HandleBuildEvent
			nextFilter.HandleBuildEvent(e)
			If TypeOf e Is TaskFinishedEventArgs AndAlso lastFile IsNot Nothing Then
				lastFile.Close()
				lastFile = Nothing
			End If
		End Sub

		Private lastFileName As String, lastLineText As String
		Private lastFile As StreamReader
		Private lastLine As Integer

		' fixes SD-1746 - VB compiler errors are shown in incorrect column if the line contains tabs
		' (http://bugtracker.sharpdevelop.net/issue/ViewIssue.aspx?id=1746&PROJID=4)
		Private Function FixColumn(fileName As String, line As Integer, column As Integer) As Integer
			If Not File.Exists(fileName) OrElse line < 1 OrElse column < 1 Then
				Return column
			End If

			If fileName <> lastFileName OrElse line < lastLine OrElse lastFile Is Nothing Then
				If lastFile IsNot Nothing Then
					lastFile.Close()
				End If
				lastFile = New StreamReader(fileName)
				lastFileName = fileName
				lastLineText = ""
				lastLine = 0
			End If

			While lastLine < line AndAlso lastLineText IsNot Nothing
				lastLineText = lastFile.ReadLine()
				lastLine += 1
			End While

			If Not String.IsNullOrEmpty(lastLineText) Then
				Dim i As Integer = 0
				While i < column AndAlso i < lastLineText.Length
					If lastLineText(i) = ControlChars.Tab Then
						column -= 3
					End If
					i += 1
				End While
			End If

			Return column
		End Function
	End Class
End Class
