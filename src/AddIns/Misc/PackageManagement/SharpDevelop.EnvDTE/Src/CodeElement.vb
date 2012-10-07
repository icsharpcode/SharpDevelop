' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeElement
		ReadOnly Property Name() As String
		ReadOnly Property Language() As String
		ReadOnly Property InfoLocation() As vsCMInfoLocation
		ReadOnly Property DTE() As DTE
		ReadOnly Property Kind() As vsCMElement
		
		' default is vsCMPart.vsCMPartWholeWithAttributes
		Function GetStartPoint() As TextPoint
		Function GetEndPoint() As TextPoint
	End Interface
End Namespace