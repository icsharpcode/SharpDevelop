' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Namespace EnvDTE
	Public Interface References
		Inherits IEnumerable
		
		''' <summary>
		''' This method should be in a separate AssemblyReferences class that is exposed by web projects.
		''' </summary>
		Sub AddFromGAC(assemblyName As String)
		Sub Add(path As String)
		
		Function Item(identity As String) As Reference
		Function Find(identity As String) As Reference
	End Interface
End Namespace