Namespace My
	
	<Global.Microsoft.VisualBasic.HideModuleName()>
	Module MyWpfExtension
		<System.ThreadStatic()> Private m_computer As MyComputer
		<System.ThreadStatic()> Private m_user As MyUser
		<System.ThreadStatic()> Private m_windows As MyWindows
		<System.ThreadStatic()> Private m_log As MyLog
		
		''' <summary>
		''' Returns the application object for the running application
		''' </summary>
		Friend ReadOnly Property Application() As Application
			Get
				Return CType(Global.System.Windows.Application.Current, Application)
			End Get
		End Property
		
		''' <summary>
		''' Returns information about the host computer.
		''' </summary>
		Friend ReadOnly Property Computer() As Global.Microsoft.VisualBasic.Devices.Computer
			Get
				If m_computer Is Nothing Then
					m_computer = New MyComputer()
				End If
				Return m_computer
			End Get
		End Property
		
		''' <summary>
		''' Returns information for the current user.  If you wish to run the application with the current
		''' Windows user credentials, call My.User.InitializeWithWindowsUser().
		''' </summary>
		Friend ReadOnly Property User() As Global.Microsoft.VisualBasic.ApplicationServices.User
			Get
				If m_user Is Nothing Then
					m_user = New MyUser()
				End If
				Return m_user
			End Get
		End Property
		
		''' <summary>
		''' Returns the application log. The listeners can be configured by the application's configuration file.
		''' </summary>
		Friend ReadOnly Property Log() As Global.Microsoft.VisualBasic.Logging.Log
			Get
				If m_log Is Nothing Then
					m_log = New MyLog()
				End If
				Return m_log
			End Get
		End Property
		
		''' <summary>
		''' Returns the collection of Windows defined in the project.
		''' </summary>
		Friend ReadOnly Property Windows() As MyWindows
			<Global.System.Diagnostics.DebuggerHidden()>
			Get
				If m_windows Is Nothing Then
					m_windows = New MyWindows()
				End If
				Return m_windows
			End Get
		End Property
		
		<Global.Microsoft.VisualBasic.MyGroupCollection("System.Windows.Window", "Create__Instance__", "Dispose__Instance__", "My.MyWpfExtenstionModule.Windows")>
		Friend NotInheritable Class MyWindows
			<Global.System.Diagnostics.DebuggerHidden()>
			Private Shared Function Create__Instance__(Of T As {New, Global.System.Windows.Window})(ByVal Instance As T) As T
				If Instance Is Nothing Then
					If m_WindowBeingCreated IsNot Nothing Then
						If m_WindowBeingCreated.ContainsKey(GetType(T)) = True Then
							Throw New Global.System.InvalidOperationException("The window cannot be accessed via My.Windows from the Window constructor.")
						End If
					Else
						m_WindowBeingCreated = New Global.System.Collections.Hashtable()
					End If
					m_WindowBeingCreated.Add(GetType(T), Nothing)
					Return New T()
					m_WindowBeingCreated.Remove(GetType(T))
				Else
					Return Instance
				End If
			End Function
			
			<Global.System.Diagnostics.DebuggerHidden()>
			Private Sub Dispose__Instance__(Of T As Global.System.Windows.Window)(ByRef instance As T)
				instance = Nothing
			End Sub
			
			<Global.System.ThreadStatic()> Private Shared m_WindowBeingCreated As Global.System.Collections.Hashtable
		End Class
	End Module
End Namespace

Partial Class MyComputer
	Inherits Global.Microsoft.VisualBasic.Devices.Computer
End Class

Partial Class MyUser
	Inherits Global.Microsoft.VisualBasic.ApplicationServices.User
End Class

Partial Class MyLog
	Inherits Global.Microsoft.VisualBasic.Logging.Log
End Class

Partial Class Application
    Inherits Global.System.Windows.Application
    
    Friend ReadOnly Property Info() As Global.Microsoft.VisualBasic.ApplicationServices.AssemblyInfo
        <Global.System.Diagnostics.DebuggerHidden()>
        Get
            Return New Global.Microsoft.VisualBasic.ApplicationServices.AssemblyInfo(Global.System.Reflection.Assembly.GetExecutingAssembly())
        End Get
    End Property
End Class