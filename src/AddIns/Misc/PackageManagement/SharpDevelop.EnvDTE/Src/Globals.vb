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
	
	''' <summary>
	''' EnvDTE.Globals class defined in VB.NET so multiple parameterized properties can be defined
	''' which are not supported in C#. This allows Powershell to use the properties as methods:
	''' 
	''' $dte.Solution.Globals.VariablePersists("MyVariable") = $true
	''' $dte.Solution.Globals.VariablePersists("MyVariable")
	''' $dte.Solution.Globals.VariableValue("MyVariable") = "path/to/tool"
	''' $dte.Solution.Globals.VariablePersists("MyVariable") = $true
	''' </summary>
	Public MustInherit Class Globals
		Public Property VariableValue(ByVal name As String) As Object
			Get
				Return GetVariableValue(name)
			End Get
			Set
				SetVariableValue(name, value)
			End Set
		End Property
		
		Protected MustOverride Function GetVariableValue(ByVal name As String) As Object
		Protected MustOverride Sub SetVariableValue(ByVal name As String, ByVal value As Object)
		
		Public Property VariablePersists(ByVal name As String) As Boolean
			Get
				Return GetVariablePersists(name)
			End Get
			Set
				SetVariablePersists(name, value)
			End Set
		End Property
		
		Protected MustOverride Function GetVariablePersists(ByVal name As String) As Boolean
		Protected MustOverride Sub SetVariablePersists(ByVal name As String, ByVal value As Boolean)
		
		Public ReadOnly Property VariableExists(ByVal name As string) As Boolean
			Get
				Return GetVariableExists(name)
			End Get
		End Property
		
		Protected MustOverride Function GetVariableExists(ByVal name As String) As Boolean
	End Class
End Namespace