' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public MustInherit Class CodeElementBase
		Inherits MarshalByRefObject
		
		ReadOnly Property IsDerivedFrom(fullName As String) As Boolean
			Get
				Return GetIsDerivedFrom(fullName)
			End Get
		End Property
		
		Protected MustOverride Function GetIsDerivedFrom(ByVal fullName As String) As Boolean
	End Class
End Namespace