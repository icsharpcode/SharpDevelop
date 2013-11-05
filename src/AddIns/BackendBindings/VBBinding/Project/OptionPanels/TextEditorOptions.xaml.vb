' Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
' This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

Imports System.Collections.Generic
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports ICSharpCode.Core

Namespace OptionPanels
	''' <summary>
	''' Interaction logic for TextEditorOptions.xaml
	''' </summary>
	Public Partial Class TextEditorOptions
		Public Sub New()
			InitializeComponent()
		End Sub

		Public Shared Property EnableEndConstructs() As Boolean
			Get
				Return PropertyService.[Get]("VBBinding.TextEditor.EnableEndConstructs", True)
			End Get
			Set
				PropertyService.[Set]("VBBinding.TextEditor.EnableEndConstructs", value)
			End Set
		End Property

		Public Shared Property EnableCasing() As Boolean
			Get
				Return PropertyService.[Get]("VBBinding.TextEditor.EnableCasing", True)
			End Get
			Set
				PropertyService.[Set]("VBBinding.TextEditor.EnableCasing", value)
			End Set
		End Property
	End Class
End Namespace
