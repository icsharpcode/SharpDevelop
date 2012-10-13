' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface Solution
		ReadOnly Property FullName() As String
		ReadOnly Property FileName() As String
		ReadOnly Property IsOpen() As Boolean
		ReadOnly Property Projects() As Projects
		ReadOnly Property Globals() As Globals
		ReadOnly Property SolutionBuild() As SolutionBuild
		ReadOnly Property Properties As Properties
		
		Function FindProjectItem(fileName As String) As ProjectItem
	End Interface
End Namespace
