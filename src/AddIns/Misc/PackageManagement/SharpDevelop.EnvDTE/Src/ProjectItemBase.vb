' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public MustInherit Class ProjectItemBase
		Inherits MarshalByRefObject
		
		ReadOnly Property FileNames(index As Short) As String
			Get
				Return GetFileNames(index)
			End Get
		End Property
		
		Protected MustOverride Function GetFileNames(ByVal index As Short) As String
	End Class
End Namespace