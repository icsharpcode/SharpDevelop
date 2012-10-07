' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface EditPoint
		Inherits TextPoint
		
		Sub ReplaceText(pointOrCount As Object, text As String, flags As Integer)
		Sub Insert(text As String)
	End Interface
End Namespace