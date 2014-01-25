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
	Public Interface ProjectItem
		ReadOnly Property Name() As String
		Property Kind() As String
		ReadOnly Property SubProject() As Project
		ReadOnly Property Properties() As Global.EnvDTE.Properties
		ReadOnly Property ContainingProject() As Project
		ReadOnly Property ProjectItems() As ProjectItems
		ReadOnly Property FileCodeModel() As FileCodeModel2
		ReadOnly Property Document() As Document
		ReadOnly Property FileNames(index As Short) As String
		ReadOnly Property FileCount() As Short
		ReadOnly Property Collection As ProjectItems
		
		Sub Delete()
		Sub Remove()
		Function Open(viewKind As String) As Global.EnvDTE.Window
		Sub Save(Optional fileName As String = Nothing)
	End Interface
End Namespace