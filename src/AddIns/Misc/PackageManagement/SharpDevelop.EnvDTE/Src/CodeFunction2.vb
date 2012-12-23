' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeFunction2
		Inherits CodeFunction
		
		ReadOnly Property IsGeneric() As Boolean
		ReadOnly Property OverrideKind() As vsCMOverrideKind
	End Interface
End Namespace