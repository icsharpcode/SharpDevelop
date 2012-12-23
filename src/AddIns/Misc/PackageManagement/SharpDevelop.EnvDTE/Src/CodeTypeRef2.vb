' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeTypeRef2
		Inherits CodeTypeRef
		
		ReadOnly Property IsGeneric As Boolean
	End Interface
End Namespace