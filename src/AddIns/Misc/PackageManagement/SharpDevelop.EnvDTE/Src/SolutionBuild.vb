' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface SolutionBuild
		ReadOnly Property ActiveConfiguration() As SolutionConfiguration
		ReadOnly Property StartupProjects As Object
		
		''' <summary>
		''' Returns the number of projects that failed to build.
		''' </summary>
		ReadOnly Property LastBuildInfo() As Integer
		
		Sub BuildProject(solutionConfiguration As String, projectUniqueName As String, waitForBuildToFinish As Boolean)
	End Interface
End Namespace