' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeInterface
		Inherits CodeType
		
		Function AddFunction(name As String, kind As vsCMFunction, type As Object, Optional Position As Object = Nothing, Optional Access As vsCMAccess = vsCMAccess.vsCMAccessPublic) As CodeFunction
	End Interface
End Namespace