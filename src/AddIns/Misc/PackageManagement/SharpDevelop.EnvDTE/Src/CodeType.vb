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

Namespace EnvDTE
	Public Interface CodeType
		Inherits CodeElement
		
		Property Access() As vsCMAccess
		ReadOnly Property FullName() As String
		ReadOnly Property Members() As CodeElements
		ReadOnly Property Bases() As CodeElements
		ReadOnly Property Attributes() As CodeElements
		ReadOnly Property [Namespace]() As CodeNamespace
		ReadOnly Property ProjectItem() As ProjectItem
		
		''' <summary>
		''' Returns true if the current type matches the fully qualified name or any of its
		''' base types are a match.
		''' </summary>
		ReadOnly Property IsDerivedFrom (fullName As String) As Boolean
	End Interface
End Namespace