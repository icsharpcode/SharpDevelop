' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface CodeElements
		Inherits IEnumerable
		
		ReadOnly Property Count() As Integer
		
		Function Item(index As Object) As CodeElement
		
		Function GetEnumerator() As IEnumerator
	End Interface
End Namespace