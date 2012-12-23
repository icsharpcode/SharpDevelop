' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeProperty2
		Inherits CodeProperty
		
		ReadOnly Property ReadWrite() As vsCMPropertyKind
		ReadOnly Property Parameters() As CodeElements
	End Interface
End Namespace