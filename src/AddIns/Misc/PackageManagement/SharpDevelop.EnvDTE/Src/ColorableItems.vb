' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface ColorableItems
		ReadOnly Property Name() As String
		Property Bold() As Boolean
		Property Foreground() As UInteger
		Property Background() As UInt32
	End Interface
End Namespace