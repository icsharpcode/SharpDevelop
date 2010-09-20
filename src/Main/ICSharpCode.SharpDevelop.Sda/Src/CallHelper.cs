// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Sda
{
	internal sealed class CallHelper : MarshalByRefObject
	{
		SharpDevelopHost.CallbackHelper callback;
		bool useSharpDevelopErrorHandler;
		
		
		public override object InitializeLifetimeService()
		{
			return null;
		}
		
		#region Initialize Core
		public void InitSharpDevelopCore(SharpDevelopHost.CallbackHelper callback, StartupSettings properties)
		{
			// creating the service manager initializes the logging service etc.
			ICSharpCode.Core.Services.ServiceManager.Instance = new SDServiceManager();
			
			LoggingService.Info("InitSharpDevelop...");
			this.callback = callback;
			CoreStartup startup = new CoreStartup(properties.ApplicationName);
			if (properties.UseSharpDevelopErrorHandler) {
				this.useSharpDevelopErrorHandler = true;
				ExceptionBox.RegisterExceptionBoxForUnhandledExceptions();
			}
			startup.ConfigDirectory = properties.ConfigDirectory;
			startup.DataDirectory = properties.DataDirectory;
			if (properties.PropertiesName != null) {
				startup.PropertiesName = properties.PropertiesName;
			}
			AssemblyParserService.DomPersistencePath = properties.DomPersistencePath;
			
			if (properties.ApplicationRootPath != null) {
				FileUtility.ApplicationRootPath = properties.ApplicationRootPath;
			}
			
			startup.StartCoreServices();
			Assembly exe = Assembly.Load(properties.ResourceAssemblyName);
			ResourceService.RegisterNeutralStrings(new ResourceManager("Resources.StringResources", exe));
			ResourceService.RegisterNeutralImages(new ResourceManager("Resources.BitmapResources", exe));
			
			MenuCommand.LinkCommandCreator = delegate(string link) { return new LinkCommand(link); };
			MenuCommand.KnownCommandCreator = CreateICommandForWPFCommand;
			Core.Presentation.MenuService.LinkCommandCreator = MenuCommand.LinkCommandCreator;
			StringParser.RegisterStringTagProvider(new SharpDevelopStringTagProvider());
			
			LoggingService.Info("Looking for AddIns...");
			foreach (string file in properties.addInFiles) {
				startup.AddAddInFile(file);
			}
			foreach (string dir in properties.addInDirectories) {
				startup.AddAddInsFromDirectory(dir);
			}
			
			if (properties.AllowAddInConfigurationAndExternalAddIns) {
				startup.ConfigureExternalAddIns(Path.Combine(PropertyService.ConfigDirectory, "AddIns.xml"));
			}
			if (properties.AllowUserAddIns) {
				startup.ConfigureUserAddIns(Path.Combine(PropertyService.ConfigDirectory, "AddInInstallTemp"),
				                            Path.Combine(PropertyService.ConfigDirectory, "AddIns"));
			}
			
			LoggingService.Info("Loading AddInTree...");
			startup.RunInitialization();
			
			// Register events to marshal back
			Project.ProjectService.BuildStarted   += delegate { this.callback.StartBuild(); };
			Project.ProjectService.BuildFinished  += delegate { this.callback.EndBuild(); };
			Project.ProjectService.SolutionLoaded += delegate { this.callback.SolutionLoaded(); };
			Project.ProjectService.SolutionClosed += delegate { this.callback.SolutionClosed(); };
			Project.ProjectService.SolutionConfigurationChanged += delegate { this.callback.SolutionConfigurationChanged(); };
			FileUtility.FileLoaded += delegate(object sender, FileNameEventArgs e) { this.callback.FileLoaded(e.FileName); };
			FileUtility.FileSaved  += delegate(object sender, FileNameEventArgs e) { this.callback.FileSaved(e.FileName); };
			
			LoggingService.Info("InitSharpDevelop finished");
		}
		
		static ICommand CreateICommandForWPFCommand(AddIn addIn, string commandName)
		{
			var wpfCommand = Core.Presentation.MenuService.GetRegisteredCommand(addIn, commandName);
			if (wpfCommand != null)
				return new WpfCommandWrapper(wpfCommand);
			else
				return null;
		}
		
		sealed class WpfCommandWrapper : AbstractCommand
		{
			readonly System.Windows.Input.ICommand wpfCommand;
			
			public WpfCommandWrapper(System.Windows.Input.ICommand wpfCommand)
			{
				this.wpfCommand = wpfCommand;
			}
			
			public override void Run()
			{
				var routedCommand = wpfCommand as System.Windows.Input.RoutedCommand;
				if (routedCommand != null) {
					var target = System.Windows.Input.FocusManager.GetFocusedElement(WorkbenchSingleton.MainWindow);
					routedCommand.Execute(this.Owner, target);
				} else {
					wpfCommand.Execute(this.Owner);
				}
			}
			
			public override string ToString()
			{
				return "[WpfCommandWrapper " + wpfCommand + "]";
			}
		}
		#endregion
		
		#region Initialize and run Workbench
		public void RunWorkbench(WorkbenchSettings settings)
		{
			if (settings.RunOnNewThread) {
				Thread t = new Thread(RunWorkbenchInternal);
				t.SetApartmentState(ApartmentState.STA);
				t.Name = "SDmain";
				t.Start(settings);
			} else {
				RunWorkbenchInternal(settings);
			}
		}
		
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		void RunWorkbenchInternal(object settings)
		{
			WorkbenchSettings wbSettings = (WorkbenchSettings)settings;
			
			WorkbenchStartup wbc = new WorkbenchStartup();
			LoggingService.Info("Initializing workbench...");
			wbc.InitializeWorkbench();
			
			RunWorkbenchInitializedCommands();
			
			LoggingService.Info("Starting workbench...");
			Exception exception = null;
			// finally start the workbench.
			try {
				callback.BeforeRunWorkbench();
				if (Debugger.IsAttached) {
					wbc.Run(wbSettings.InitialFileList);
				} else {
					try {
						wbc.Run(wbSettings.InitialFileList);
					} catch (Exception ex) {
						exception = ex;
					}
				}
			} finally {
				LoggingService.Info("Unloading services...");
				try {
					WorkbenchSingleton.OnWorkbenchUnloaded();
					PropertyService.Save();
				} catch (Exception ex) {
					LoggingService.Warn("Exception during unloading", ex);
					if (exception == null) {
						exception = ex;
					}
				}
			}
			LoggingService.Info("Finished running workbench.");
			callback.WorkbenchClosed();
			if (exception != null) {
				const string errorText = "Unhandled exception terminated the workbench";
				LoggingService.Fatal(exception);
				if (useSharpDevelopErrorHandler) {
					System.Windows.Forms.Application.Run(new ExceptionBox(exception, errorText, true));
				} else {
					throw new RunWorkbenchException(errorText, exception);
				}
			}
		}
		
		void RunWorkbenchInitializedCommands()
		{
			foreach (ICommand command in AddInTree.BuildItems<ICommand>("/Workspace/AutostartAfterWorkbenchInitialized", null, false)) {
				try {
					command.Run();
				} catch (Exception ex) {
					// allow startup to continue if some commands fail
					MessageService.ShowException(ex);
				}
			}
		}
		#endregion
		
		public List<Document> OpenDocuments {
			get {
				List<Document> l = new List<Document>();
				if (WorkbenchSingleton.InvokeRequired) {
					WorkbenchSingleton.SafeThreadCall(new Action<List<Document>>(GetOpenDocuments), l);
				} else {
					GetOpenDocuments(l);
				}
				return l;
			}
		}
		void GetOpenDocuments(List<Document> l)
		{
			foreach (IViewContent vc in WorkbenchSingleton.Workbench.ViewContentCollection) {
				Document d = Document.FromWindow(vc);
				if (d != null) {
					l.Add(d);
				}
			}
		}
		
		/// <summary>
		/// Opens the document with the specified file name.
		/// </summary>
		public Document OpenDocument(string fileName)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction<string, Document>(OpenDocumentInternal, fileName);
			} else {
				return OpenDocumentInternal(fileName);
			}
		}
		Document OpenDocumentInternal(string fileName)
		{
			return Document.FromWindow(FileService.OpenFile(fileName));
		}
		
		public void OpenProject(string fileName)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadCall(OpenProjectInternal, fileName);
			} else {
				OpenProjectInternal(fileName);
			}
		}
		void OpenProjectInternal(string fileName)
		{
			Project.ProjectService.LoadSolutionOrProject(fileName);
		}
		
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public bool IsSolutionOrProject(string fileName)
		{
			return Project.ProjectService.HasProjectLoader(fileName);
		}
		
		public bool CloseWorkbench(bool force)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				return WorkbenchSingleton.SafeThreadFunction<bool, bool>(CloseWorkbenchInternal, force);
			} else {
				return CloseWorkbenchInternal(force);
			}
		}
		bool CloseWorkbenchInternal(bool force)
		{
			foreach (IWorkbenchWindow window in WorkbenchSingleton.Workbench.WorkbenchWindowCollection.ToArray()) {
				if (!window.CloseWindow(force))
					return false;
			}
			WorkbenchSingleton.MainWindow.Close();
			return true;
		}
		
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "needs to be run in correct AppDomain")]
		public void KillWorkbench()
		{
			Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
		}
		
		public bool WorkbenchVisible {
			get {
				if (WorkbenchSingleton.InvokeRequired) {
					return WorkbenchSingleton.SafeThreadFunction<bool>(GetWorkbenchVisibleInternal);
				} else {
					return GetWorkbenchVisibleInternal();
				}
			}
			set {
				if (WorkbenchSingleton.InvokeRequired) {
					WorkbenchSingleton.SafeThreadCall(SetWorkbenchVisibleInternal, value);
				} else {
					SetWorkbenchVisibleInternal(value);
				}
			}
		}
		bool GetWorkbenchVisibleInternal()
		{
			return WorkbenchSingleton.MainWindow.Visibility == Visibility.Visible;
		}
		void SetWorkbenchVisibleInternal(bool value)
		{
			WorkbenchSingleton.MainWindow.Visibility = value ? Visibility.Visible : Visibility.Hidden;
		}
	}
}
