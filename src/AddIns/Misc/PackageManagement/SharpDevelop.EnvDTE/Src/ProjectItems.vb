' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface ProjectItems
		Inherits IEnumerable
		Sub AddFromFileCopy(filePath As String)
		
		Function Item(index As Object) As ProjectItem
		Function AddFromDirectory(directory As String) As ProjectItem
		Function AddFromFile(fileName As String) As ProjectItem
		
		ReadOnly Property Parent() As Object
		ReadOnly Property Count() As Integer
		ReadOnly Property Kind() As String
	End Interface
End Namespace