' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeAttribute
		Inherits CodeElement
		
		ReadOnly Property FullName() As String
		Property Value() As String
	End Interface
End Namespace