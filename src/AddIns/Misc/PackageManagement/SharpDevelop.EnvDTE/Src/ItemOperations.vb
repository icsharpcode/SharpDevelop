' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface ItemOperations
		Sub OpenFile(fileName As String)
		Sub Navigate(url As String)
		Function NewFile(fileName As String) As Window
	End Interface
End Namespace