' Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy of this
' software and associated documentation files (the "Software"), to deal in the Software
' without restriction, including without limitation the rights to use, copy, modify, merge,
' publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
' to whom the Software is furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all copies or
' substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
' INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
' PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
' FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
' OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Linq
Imports ICSharpCode.SharpDevelop
Imports ICSharpCode.SharpDevelop.Dom
Imports ICSharpCode.SharpDevelop.Gui
Imports ICSharpCode.SharpDevelop.Gui.OptionPanels
Imports ICSharpCode.SharpDevelop.Project

Namespace OptionPanels
	''' <summary>
	''' Interaction logic for ProjectImportsOptions.xaml
	''' </summary>
	Public Partial Class ProjectImports
		Inherits ProjectOptionPanel

		Public Sub New()
			InitializeComponent()
		End Sub

		#Region "override"

		Protected Overrides Sub Initialize()
			ProjectItems = New ObservableCollection(Of String)()
			NameSpaceItems = New ObservableCollection(Of String)()

			For Each item As ProjectItem In MyBase.Project.Items
				If item.ItemType = ItemType.Import Then
					ProjectItems.Add(item.Include)
				End If
			Next


			Dim projectContent As IProjectContent = ParserService.GetProjectContent(MyBase.Project)
			For Each refProjectContent As IProjectContent In projectContent.ThreadSafeGetReferencedContents()
				AddNamespaces(refProjectContent)
			Next
			AddNamespaces(projectContent)
		End Sub


		Protected Overrides Function Save(project As MSBuildBasedProject, configuration As String, platform As String) As Boolean
			Dim [imports] As New List(Of ProjectItem)()
			For Each item As ProjectItem In project.Items
				If item.ItemType = ItemType.Import Then
					[imports].Add(item)
				End If
			Next

			For Each item As ImportProjectItem In [imports]
				ProjectService.RemoveProjectItem(project, item)
			Next

			For Each importedNamespace As String In ProjectItems
				ProjectService.AddProjectItem(project, New ImportProjectItem(project, importedNamespace))
			Next

			Return MyBase.Save(project, configuration, platform)
		End Function

		#End Region


		Private m_projectItems As ObservableCollection(Of String)

		Public Property ProjectItems() As ObservableCollection(Of String)
			Get
				Return m_projectItems
			End Get
			Set
				m_projectItems = value
				MyBase.RaisePropertyChanged(Function() ProjectItems)
			End Set
		End Property

		Private m_selectedProjectItem As String

		Public Property SelectedProjectItem() As String
			Get
				Return m_selectedProjectItem
			End Get
			Set
				m_selectedProjectItem = value
				MyBase.RaisePropertyChanged(Function() SelectedProjectItem)
				RemoveButtonEnable = True
				AddButtonEnable = False
			End Set
		End Property

		Private m_nameSpaceItems As ObservableCollection(Of String)

		Public Property NameSpaceItems() As ObservableCollection(Of String)
			Get
				Return m_nameSpaceItems
			End Get
			Set
				m_nameSpaceItems = value
				MyBase.RaisePropertyChanged(Function() NameSpaceItems)
			End Set
		End Property


		Private m_selectedNameSpace As String

		Public Property SelectedNameSpace() As String
			Get
				Return m_selectedNameSpace
			End Get
			Set
				m_selectedNameSpace = value
				MyBase.RaisePropertyChanged(Function() SelectedNameSpace)
				AddButtonEnable = True
			End Set
		End Property


		Private m_addButtonEnable As Boolean

		Public Property AddButtonEnable() As Boolean
			Get
				Return m_addButtonEnable
			End Get
			Set
				m_addButtonEnable = value
				MyBase.RaisePropertyChanged(Function() AddButtonEnable)
			End Set
		End Property

		Private m_removeButtonEnable As Boolean

		Public Property RemoveButtonEnable() As Boolean
			Get
				Return m_removeButtonEnable
			End Get
			Set
				m_removeButtonEnable = value
				MyBase.RaisePropertyChanged(Function() RemoveButtonEnable)
			End Set
		End Property



		Private Sub AddNamespaces(projectContent As IProjectContent)
			For Each projectNamespace As String In projectContent.NamespaceNames
				If Not String.IsNullOrEmpty(projectNamespace) Then

					If Not NameSpaceItems.Contains(projectNamespace) Then
						NameSpaceItems.Add(projectNamespace)
					End If
				End If
			Next
		End Sub

		Private Sub AddButton_Click(sender As Object, e As RoutedEventArgs)
			ProjectItems.Add(SelectedNameSpace)
			IsDirty = True
		End Sub

		Private Sub RemoveButton_Click(sender As Object, e As RoutedEventArgs)
			ProjectItems.Remove(SelectedProjectItem)
			SelectedProjectItem = Nothing
			RemoveButtonEnable = False
			AddButtonEnable = False
			IsDirty = True
		End Sub
	End Class
End Namespace
