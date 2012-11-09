' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeTypeRef
		ReadOnly Property AsFullName() As String
		ReadOnly Property AsString() As String
		ReadOnly Property Parent() As CodeElement
		ReadOnly Property CodeType() As CodeType
		ReadOnly Property TypeKind() As vsCMTypeRef
	End Interface
End Namespace