' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeParameter
		Inherits CodeElement
		
		ReadOnly Property Type() As CodeTypeRef2
		ReadOnly Property Attributes() As CodeElements
	End Interface
End Namespace