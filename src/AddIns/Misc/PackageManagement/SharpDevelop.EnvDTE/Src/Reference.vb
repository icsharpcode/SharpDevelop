' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface Reference
		ReadOnly Property Identity() As String
		ReadOnly Property Name() As String
		ReadOnly Property Path() As String
		ReadOnly Property PublicKeyToken As String
		ReadOnly Property SourceProject() As Project
		ReadOnly Property StrongName As Boolean
		
		Sub Remove()
	End Interface
End Namespace