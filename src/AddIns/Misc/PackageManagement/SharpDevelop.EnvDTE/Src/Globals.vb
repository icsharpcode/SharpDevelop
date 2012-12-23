' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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