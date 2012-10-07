' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface Project
		ReadOnly Property Name() As String
		ReadOnly Property UniqueName() As String
		ReadOnly Property FileName() As String
		ReadOnly Property FullName() As String
		ReadOnly Property [Object]() As Object
		ReadOnly Property Properties() As Properties
		ReadOnly Property ProjectItems() As ProjectItems
		ReadOnly Property DTE() As DTE
		ReadOnly Property Type() As String
		ReadOnly Property Kind() As String
		ReadOnly Property CodeModel() As CodeModel
		ReadOnly Property ConfigurationManager() As ConfigurationManager
		
		Sub Save()
	End Interface
End Namespace