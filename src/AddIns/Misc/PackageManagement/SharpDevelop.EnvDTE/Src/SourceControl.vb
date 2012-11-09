' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface SourceControl
		Function IsItemCheckedOut(itemName As String) As Boolean
		Function IsItemUnderSCC(itemName As String) As Boolean
		Function CheckOutItem(itemName As String) As Boolean
	End Interface
End Namespace