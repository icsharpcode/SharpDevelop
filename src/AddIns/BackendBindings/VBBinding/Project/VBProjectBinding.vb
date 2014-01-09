' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Imports System.Xml
Imports ICSharpCode.SharpDevelop.Project

Public Class VBProjectBinding
	Implements IProjectBinding
	Public Const LanguageName As String = "VB"

	Public Function LoadProject(info As ProjectLoadInformation) As IProject Implements IProjectBinding.LoadProject
		Return New VBProject(info)
	End Function

	Public Function CreateProject(info As ProjectCreateInformation) As IProject Implements IProjectBinding.CreateProject
		Return New VBProject(info)
	End Function

	Public ReadOnly Property HandlingMissingProject() As Boolean Implements IProjectBinding.HandlingMissingProject
		Get
			Return False
		End Get
	End Property
End Class
