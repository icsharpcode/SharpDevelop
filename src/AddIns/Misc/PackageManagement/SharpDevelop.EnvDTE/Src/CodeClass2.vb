' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeClass2
		Inherits CodeClass
		
		ReadOnly Property PartialClasses() As CodeElements
		ReadOnly Property IsGeneric() As Boolean
		Property ClassKind() As vsCMClassKind
		ReadOnly Property IsAbstract() As Boolean
	End Interface
End Namespace