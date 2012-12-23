' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface Properties
		Inherits IEnumerable
		
		Function Item(propertyName As String) As [Property]
	End Interface
End Namespace