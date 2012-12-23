' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface ProjectItem
		ReadOnly Property Name() As String
		Property Kind() As String
		ReadOnly Property SubProject() As Project
		ReadOnly Property Properties() As Global.EnvDTE.Properties
		ReadOnly Property ContainingProject() As Project
		ReadOnly Property ProjectItems() As ProjectItems
		ReadOnly Property FileCodeModel() As FileCodeModel2
		ReadOnly Property Document() As Document
		ReadOnly Property FileNames(index As Short) As String
		ReadOnly Property FileCount() As Short
		ReadOnly Property Collection As ProjectItems
		
		Sub Delete()
		Sub Remove()
		Function Open(viewKind As String) As Global.EnvDTE.Window
	End Interface
End Namespace