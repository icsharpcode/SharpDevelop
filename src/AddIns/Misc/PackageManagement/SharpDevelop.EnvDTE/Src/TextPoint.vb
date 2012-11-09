' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface TextPoint
		ReadOnly Property LineCharOffset() As Integer
		ReadOnly Property Line() As Integer
		
		Function CreateEditPoint() As EditPoint
	End Interface
End Namespace