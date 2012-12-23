' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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