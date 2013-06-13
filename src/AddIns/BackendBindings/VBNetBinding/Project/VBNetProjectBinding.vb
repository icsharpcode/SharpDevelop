' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Imports System.Xml
Imports ICSharpCode.SharpDevelop.Project

Public Class VBNetProjectBinding
	Implements IProjectBinding
	Public Const LanguageName As String = "VBNet"

	Public ReadOnly Property Language() As String Implements IProjectBinding.Language
		Get
			Return LanguageName
		End Get
	End Property

	Public Function LoadProject(info As ProjectLoadInformation) As IProject Implements IProjectBinding.LoadProject
		Return New VBNetProject(info)
	End Function

	Public Function CreateProject(info As ProjectCreateInformation) As IProject Implements IProjectBinding.CreateProject
		Return New VBNetProject(info)
	End Function

	Public ReadOnly Property HandlingMissingProject() As Boolean Implements IProjectBinding.HandlingMissingProject
		Get
			Return False
		End Get
	End Property
End Class
