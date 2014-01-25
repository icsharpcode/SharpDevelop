// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace ICSharpCode.SharpDevelop.Sda
{
	/// <summary>
	/// This class can host an instance of SharpDevelop inside another
	/// AppDomain.
	/// </summary>
	public sealed class SharpDevelopHost
	{
		#region CreateDomain
		/// <summary>
		/// Create an AppDomain capable of hosting SharpDevelop.
		/// </summary>
		public static AppDomain CreateDomain()
		{
			return AppDomain.CreateDomain("SharpDevelop.Sda", null, CreateDomainSetup());
		}
		
		/// <summary>
		/// Creates an AppDomainSetup specifying properties for an AppDomain capable of
		/// hosting SharpDevelop.
		/// </summary>
		public static AppDomainSetup CreateDomainSetup()
		{
			AppDomainSetup s = new AppDomainSetup();
			s.ApplicationBase = Path.GetDirectoryName(SdaAssembly.Location);
			s.ConfigurationFile = SdaAssembly.Location + ".config";
			s.ApplicationName = "SharpDevelop.Sda";
			return s;
		}
		#endregion
		
		#region Static helpers
		internal static Assembly SdaAssembly {
			get {
				return typeof(SharpDevelopHost).Assembly;
			}
		}
		#endregion
		
		#region SDInitStatus enum
		enum SDInitStatus
		{
			None,
			CoreInitialized,
			WorkbenchInitialized,
			Busy,
			WorkbenchUnloaded,
			AppDomainUnloaded
		}
		#endregion
		
		AppDomain appDomain;
		CallHelper helper;
		SDInitStatus initStatus;
		
		#region Constructors
		/// <summary>
		/// Create a new AppDomain to host SharpDevelop.
		/// </summary>
		public SharpDevelopHost(StartupSettings startup)
		{
			if (startup == null) {
				throw new ArgumentNullException("startup");
			}
			this.appDomain = CreateDomain();
			helper = (CallHelper)appDomain.CreateInstanceAndUnwrap(SdaAssembly.FullName, typeof(CallHelper).FullName);
			helper.InitSharpDevelopCore(new CallbackHelper(this), startup);
			initStatus = SDInitStatus.CoreInitialized;
		}
		
		/// <summary>
		/// Host SharpDevelop in the existing AppDomain.
		/// </summary>
		public SharpDevelopHost(AppDomain appDomain, StartupSettings startup)
		{
			if (appDomain == null) {
				throw new ArgumentNullException("appDomain");
			}
			if (startup == null) {
				throw new ArgumentNullException("startup");
			}
			this.appDomain = appDomain;
			helper = (CallHelper)appDomain.CreateInstanceAndUnwrap(SdaAssembly.FullName, typeof(CallHelper).FullName);
			helper.InitSharpDevelopCore(new CallbackHelper(this), startup);
			initStatus = SDInitStatus.CoreInitialized;
		}
		#endregion
		
		#region Workbench Initialization and startup
		/// <summary>
		/// Initializes the workbench (create the MainForm instance, construct menu from AddInTree etc.)
		/// and runs it using the supplied settings.
		/// This starts a new message loop for the workbench. By default the message loop
		/// is created on a new thread, but you can change the settings so that
		/// it is created on the thread calling RunWorkbench.
		/// In that case, RunWorkbench will block until SharpDevelop is shut down!
		/// </summary>
		public void RunWorkbench(WorkbenchSettings settings)
		{
			if (settings == null) {
				throw new ArgumentNullException("settings");
			}
			if (initStatus == SDInitStatus.CoreInitialized) {
				initStatus = SDInitStatus.Busy;
				helper.RunWorkbench(settings);
				if (settings.RunOnNewThread) {
					initStatus = SDInitStatus.WorkbenchInitialized;
				}
			} else {
				throw new InvalidOperationException();
			}
		}
		#endregion
		
		#region Application control
		/// <summary>
		/// Gets the list of currently opened documents.
		/// </summary>
		public ReadOnlyCollection<Document> OpenDocuments {
			get {
				if (initStatus != SDInitStatus.WorkbenchInitialized) {
					return new ReadOnlyCollection<Document>(new Document[0]);
				}
				return new ReadOnlyCollection<Document>(helper.OpenDocuments);
			}
		}
		
		/// <summary>
		/// Opens the document with the specified file name.
		/// Requires that the workbench is running.
		/// </summary>
		public Document OpenDocument(string fileName)
		{
			if (initStatus != SDInitStatus.WorkbenchInitialized) {
				throw new InvalidOperationException();
			}
			return helper.OpenDocument(fileName);
		}
		
		/// <summary>
		/// Opens the project or solution with the specified file name.
		/// Requires that the workbench is running.
		/// </summary>
		public void OpenProject(string fileName)
		{
			if (initStatus != SDInitStatus.WorkbenchInitialized) {
				throw new InvalidOperationException();
			}
			helper.OpenProject(fileName);
		}
		
		/// <summary>
		/// Gets if the specified file is a project or solution file.
		/// </summary>
		public bool IsSolutionOrProject(string fileName)
		{
			return helper.IsSolutionOrProject(fileName);
		}
		
		/// <summary>
		/// Gets/Sets whether the workbench is visible.
		/// Requires that the workbench is running.
		/// </summary>
		public bool WorkbenchVisible {
			get {
				if (initStatus != SDInitStatus.WorkbenchInitialized) {
					return false;
				} else {
					return helper.WorkbenchVisible;
				}
			}
			set {
				if (initStatus != SDInitStatus.WorkbenchInitialized) {
					throw new InvalidOperationException();
				}
				helper.WorkbenchVisible = value;
			}
		}
		
		/// <summary>
		/// Closes and unloads the workbench. The user is asked to save his work
		/// and can abort closing.
		/// Requires that the workbench is running.
		/// </summary>
		/// <param name="force">When force is used (=true), unsaved changes to documents
		/// are lost, but SharpDevelop still terminates correctly and saves changed
		/// settings.</param>
		/// <returns>True when the workbench was closed.</returns>
		public bool CloseWorkbench(bool force)
		{
			if (initStatus == SDInitStatus.CoreInitialized || initStatus == SDInitStatus.WorkbenchUnloaded) {
				// Workbench not loaded/already closed: do nothing
				return true;
			}
			if (initStatus != SDInitStatus.WorkbenchInitialized) {
				throw new InvalidOperationException();
			}
			return helper.CloseWorkbench(force);
		}
		
		/// <summary>
		/// Unload the SharpDevelop AppDomain. This will force SharpDevelop to close
		/// without saving open files or changed settings.
		/// Call CloseWorkbench before UnloadDomain to prompt the user to save documents and settings.
		/// </summary>
		public void UnloadDomain()
		{
			if (initStatus != SDInitStatus.AppDomainUnloaded) {
				if (initStatus == SDInitStatus.WorkbenchInitialized) {
					helper.KillWorkbench();
				}
				AppDomain.Unload(appDomain);
				initStatus = SDInitStatus.AppDomainUnloaded;
			}
		}
		#endregion
		
		#region CreateInstanceInTargetDomain
		/// <summary>
		/// Gets the AppDomain used to host SharpDevelop.
		/// </summary>
		public AppDomain AppDomain {
			get {
				return appDomain;
			}
		}
		
		/// <summary>
		/// Creates an instance of the specified type argument in the target AppDomain.
		/// </summary>
		/// <param name="arguments">Arguments to pass to the constructor of "T".</param>
		/// <returns>The constructed object.</returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public T CreateInstanceInTargetDomain<T>(params object[] arguments) where T : MarshalByRefObject
		{
			Type type = typeof(T);
			return (T)appDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, arguments);
		}
		
		/// <summary>
		/// Creates an instance of the specified type in the target AppDomain.
		/// </summary>
		/// <param name="type">Type to create an instance of.</param>
		/// <param name="arguments">Arguments to pass to the constructor of <paramref name="type"/>.</param>
		/// <returns>The constructed object.</returns>
		[SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
		public object CreateInstanceInTargetDomain(Type type, params object[] arguments)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (typeof(MarshalByRefObject).IsAssignableFrom(type) == false)
				throw new ArgumentException("type does not inherit from MarshalByRefObject", "type");
			return appDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, arguments);
		}
		#endregion
		
		#region Callback Events
		System.ComponentModel.ISynchronizeInvoke invokeTarget;
		
		/// <summary>
		/// Gets/Sets an object to use to synchronize all events with a thread.
		/// Use null (default) to handle all events on the thread they were
		/// raised on.
		/// </summary>
		public System.ComponentModel.ISynchronizeInvoke InvokeTarget {
			get {
				return invokeTarget;
			}
			set {
				invokeTarget = value;
			}
		}
		
		/// <summary>
		/// Event before the workbench starts running.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1713:EventsShouldNotHaveBeforeOrAfterPrefix")]
		public event EventHandler BeforeRunWorkbench;
		
		/// <summary>
		/// Event after the workbench has been unloaded.
		/// </summary>
		public event EventHandler WorkbenchClosed;
		
		/// <summary>
		/// Event when SharpDevelop starts to compile a project or solution.
		/// </summary>
		public event EventHandler StartBuild;
		
		/// <summary>
		/// Event when SharpDevelop finishes to compile a project or solution.
		/// </summary>
		public event EventHandler EndBuild;
		
		/// <summary>
		/// Event when a solution was loaded inside SharpDevelop.
		/// </summary>
		public event EventHandler SolutionLoaded;
		
		/// <summary>
		/// Event when the current solution was closed.
		/// </summary>
		public event EventHandler SolutionClosed;
		
		/// <summary>
		/// Event when a file was loaded inside SharpDevelop.
		/// </summary>
		public event EventHandler<FileEventArgs> FileLoaded;
		
		/// <summary>
		/// Event when a file was saved inside SharpDevelop.
		/// </summary>
		public event EventHandler<FileEventArgs> FileSaved;
		
		internal sealed class CallbackHelper : MarshalByRefObject
		{
			private static readonly object[] emptyObjectArray = new object[0];
			
			readonly SharpDevelopHost host;
			
			public CallbackHelper(SharpDevelopHost host)
			{
				this.host = host;
			}
			
			public override object InitializeLifetimeService()
			{
				return null;
			}
			
			private bool InvokeRequired {
				get {
					return host.invokeTarget != null && host.invokeTarget.InvokeRequired;
				}
			}
			
			private void Invoke(System.Windows.Forms.MethodInvoker method)
			{
				host.invokeTarget.BeginInvoke(method, emptyObjectArray);
			}
			
			private void Invoke(Action<string> method, string argument)
			{
				host.invokeTarget.BeginInvoke(method, new object[] { argument });
			}
			
			internal void BeforeRunWorkbench()
			{
				if (InvokeRequired) { Invoke(BeforeRunWorkbench); return; }
				host.initStatus = SDInitStatus.WorkbenchInitialized;
				if (host.BeforeRunWorkbench != null) host.BeforeRunWorkbench(host, EventArgs.Empty);
			}
			
			internal void WorkbenchClosed()
			{
				if (InvokeRequired) { Invoke(WorkbenchClosed); return; }
				host.initStatus = SDInitStatus.WorkbenchUnloaded;
				if (host.WorkbenchClosed != null) host.WorkbenchClosed(host, EventArgs.Empty);
			}
			
			internal void StartBuild()
			{
				if (InvokeRequired) { Invoke(StartBuild); return; }
				if (host.StartBuild != null) host.StartBuild(host, EventArgs.Empty);
			}
			
			internal void EndBuild()
			{
				if (InvokeRequired) { Invoke(EndBuild); return; }
				if (host.EndBuild != null) host.EndBuild(host, EventArgs.Empty);
			}
			
			internal void SolutionLoaded()
			{
				if (InvokeRequired) { Invoke(SolutionLoaded); return; }
				if (host.SolutionLoaded != null) host.SolutionLoaded(host, EventArgs.Empty);
			}
			
			internal void SolutionClosed()
			{
				if (InvokeRequired) { Invoke(SolutionClosed); return; }
				if (host.SolutionClosed != null) host.SolutionClosed(host, EventArgs.Empty);
			}
			
			internal void FileLoaded(string fileName)
			{
				if (InvokeRequired) { Invoke(FileLoaded, fileName); return; }
				if (host.FileLoaded != null) host.FileLoaded(host, new FileEventArgs(fileName));
			}
			
			internal void FileSaved(string fileName)
			{
				if (InvokeRequired) { Invoke(FileSaved, fileName); return; }
				if (host.FileSaved != null) host.FileSaved(host, new FileEventArgs(fileName));
			}
		}
		#endregion
	}
}
