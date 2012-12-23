' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface ProjectObject
		
		ReadOnly Property References() As References
		ReadOnly Property Project() As Project
		ReadOnly Property DTE() As DTE
	End Interface
End Namespace