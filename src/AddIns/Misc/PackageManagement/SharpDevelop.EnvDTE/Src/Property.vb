' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface [Property]
		ReadOnly Property Name() As String
		Property Value() As Object
		Property [Object]() As Object
	End Interface
End Namespace