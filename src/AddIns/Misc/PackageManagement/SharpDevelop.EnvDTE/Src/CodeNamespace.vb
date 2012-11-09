' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeNamespace
		ReadOnly Property Kind() As vsCMElement
		ReadOnly Property FullName() As String
		ReadOnly Property Name() As String
		ReadOnly Property Members() As CodeElements
	End Interface
End Namespace