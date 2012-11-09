' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface DTE
		ReadOnly Property Version() As String
		ReadOnly Property Solution() As Solution
		ReadOnly Property ItemOperations() As ItemOperations
		ReadOnly Property ActiveSolutionProjects() As Object
		ReadOnly Property SourceControl() As SourceControl
		
		Function Properties(category As String, page As String) As Properties
	End Interface
End Namespace