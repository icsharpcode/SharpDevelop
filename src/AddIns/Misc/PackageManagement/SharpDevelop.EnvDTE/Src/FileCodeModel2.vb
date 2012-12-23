' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface FileCodeModel2
		ReadOnly Property CodeElements() As CodeElements
		
		Sub AddImport(name As String, Optional position As Object = Nothing, Optional [alias] As String = Nothing)
	End Interface
End Namespace