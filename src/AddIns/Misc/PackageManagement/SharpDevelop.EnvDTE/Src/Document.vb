' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface Document
		Property Saved() As Boolean
		ReadOnly Property FullName() As String
		
		Function [Object](modelKind As String) As Object
	End Interface
End Namespace