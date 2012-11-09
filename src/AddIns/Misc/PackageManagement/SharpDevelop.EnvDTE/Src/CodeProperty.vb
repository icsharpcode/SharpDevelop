' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeProperty
		Inherits CodeElement
		
		Property Access() As vsCMAccess
		ReadOnly Property Parent() As CodeClass
		ReadOnly Property Attributes() As CodeElements
		ReadOnly Property Type() As CodeTypeRef
		ReadOnly Property Getter() As CodeFunction
		ReadOnly Property Setter() As CodeFunction
	End Interface
End Namespace