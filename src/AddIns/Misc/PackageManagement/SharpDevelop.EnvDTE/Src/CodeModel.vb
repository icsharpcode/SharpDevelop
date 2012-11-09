' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeModel
		ReadOnly Property CodeElements() As CodeElements
		ReadOnly Property Language() As String
		
		Function CodeTypeFromFullName(name As String) As CodeType
	End Interface
End Namespace