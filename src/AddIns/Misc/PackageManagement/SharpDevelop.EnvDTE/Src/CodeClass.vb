' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeClass
		Inherits CodeType
		
		ReadOnly Property ImplementedInterfaces() As CodeElements
		Function AddVariable(name As String, type As Object, Optional Position As Object = Nothing, Optional Access As vsCMAccess = vsCMAccess.vsCMAccessPublic, Optional Location As Object = Nothing) As CodeVariable
	End Interface
End Namespace