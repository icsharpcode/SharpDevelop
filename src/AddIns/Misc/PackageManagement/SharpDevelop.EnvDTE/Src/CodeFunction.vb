' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeFunction
		Inherits CodeElement
		
		Property Access() As vsCMAccess
		ReadOnly Property Parameters() As CodeElements
		ReadOnly Property Type() As CodeTypeRef2
		ReadOnly Property Attributes() As CodeElements
		Property CanOverride() As Boolean
		ReadOnly Property FunctionKind() As vsCMFunction
		ReadOnly Property IsShared() As Boolean
		ReadOnly Property MustImplement() As Boolean
	End Interface
End Namespace