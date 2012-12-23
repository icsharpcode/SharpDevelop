' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Enum vsCMParameterKind
		vsCMParameterKindNone       = 0
		vsCMParameterKindIn         = 1
		vsCMParameterKindRef        = 2
		vsCMParameterKindOut        = 4
		vsCMParameterKindOptional   = 8
		vsCMParameterKindParamArray = 16
	End Enum
End Namespace