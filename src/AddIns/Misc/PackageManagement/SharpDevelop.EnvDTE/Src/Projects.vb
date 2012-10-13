' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface Projects
		Inherits IEnumerable
		
		''' <summary>
		''' Index of 1 returns the first project.
		''' </summary>
		Function Item(index As Object) As Project
		
		ReadOnly Property Count() As Integer
	End Interface
End Namespace