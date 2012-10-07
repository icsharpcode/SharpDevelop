' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface Reference
		ReadOnly Property Name() As String
		ReadOnly Property Path() As String
		ReadOnly Property SourceProject() As Project
		
		Sub Remove()
	End Interface
End Namespace