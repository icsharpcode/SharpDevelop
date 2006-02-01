' <file>
'     <copyright see="prj:///doc/copyright.txt"/>
'     <license see="prj:///doc/license.txt"/>
'     <owner name="John Simons" email="John.SIMONS@dewr.gov.au"/>
'     <version>$Revision$</version>
' </file>

Imports System.Runtime.InteropServices

<ComVisible(True)> _
Public Class JScriptExternal
	Dim jScriptGlobals As New JScriptGlobals
	
	Public ReadOnly Property Globals() As JScriptGlobals
		Get
			Return jScriptGlobals
		End Get
	End Property
End Class

<ComVisible(True)> _
Public Class JScriptGlobals
	Dim variableValueCol As New Dictionary(Of String, String)
	Dim variablePersistCol As New Dictionary(Of String, Boolean)
	
	Public Property VariableValueCollection() As Dictionary(Of String, String)
		Get
			Return variableValueCol
		End Get
		Set
			variableValueCol = value
		End Set
	End Property
	
	Public Property VariablePersistCollection() As Dictionary(Of String, Boolean)
		Get
			Return variablePersistCol
		End Get
		Set
			variablePersistCol = value
		End Set
	End Property
	
	Public Function VariableExists(ByVal key As String) As Boolean
		Return variableValueCol.ContainsKey(key)
	End Function
	
	Public Default Property VariableValue(ByVal key As String) As Object
		Get
			Return variableValueCol(key)
		End Get
		Set
			variableValueCol(key) = value
		End Set
	End Property
	
	Public Property VariablePersists(ByVal key As String) As Boolean
		Get
			Return variablePersistCol(key)
		End Get
		Set
			variablePersistCol(key) = value
		End Set
	End Property
End Class
