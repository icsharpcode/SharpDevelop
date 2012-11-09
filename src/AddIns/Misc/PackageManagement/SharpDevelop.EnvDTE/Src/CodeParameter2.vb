' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeParameter2
		Inherits CodeParameter
		
		ReadOnly Property ParameterKind() As vsCMParameterKind
	End Interface
End Namespace