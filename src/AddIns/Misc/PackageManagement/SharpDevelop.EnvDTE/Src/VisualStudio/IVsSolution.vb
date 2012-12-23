' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Imports System.Runtime.InteropServices

Namespace Microsoft.VisualStudio.Shell.Interop
	Public Interface IVsSolution
		Function GetProjectOfUniqueName(uniqueName As String, <OutAttribute> ByRef hierarchy As IVsHierarchy) As Integer
	End Interface
End Namespace