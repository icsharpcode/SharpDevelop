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
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.SharpDevelop.Logging;
using ICSharpCode.SharpDevelop.Parser;

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
			// Initialize the most important services:
			var container = new SharpDevelopServiceContainer(ServiceSingleton.FallbackServiceProvider);
			container.AddService(typeof(IMessageService), new SDMessageService());
			container.AddService(typeof(ILoggingService), new log4netLoggingService());
			ServiceSingleton.ServiceProvider = container;
			
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
			
			if (properties.ApplicationRootPath != null) {
				FileUtility.ApplicationRootPath = properties.ApplicationRootPath;
			}
			
			startup.StartCoreServices();
			Assembly exe = Assembly.Load(properties.ResourceAssemblyName);
			SD.ResourceService.RegisterNeutralStrings(new ResourceManager("ICSharpCode.SharpDevelop.Resources.StringResources", exe));
			SD.ResourceService.RegisterNeutralImages(new ResourceManager("ICSharpCode.SharpDevelop.Resources.BitmapResources", exe));
			
			CommandWrapper.LinkCommandCreator = (link => new LinkCommand(link));
			CommandWrapper.WellKnownCommandCreator = Core.Presentation.MenuService.GetKnownCommand;
			CommandWrapper.RegisterConditionRequerySuggestedHandler = (eh => CommandManager.RequerySuggested += eh);
			CommandWrapper.UnregisterConditionRequerySuggestedHandler = (eh => CommandManager.RequerySuggested -= eh);
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
			
			((AssemblyParserService)SD.AssemblyParserService).DomPersistencePath = properties.DomPersistencePath;
			
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
					// see IShutdownService.Shutdown for a description of the shut down procedure
					WorkbenchSingleton.OnWorkbenchUnloaded();
					var propertyService = SD.PropertyService;
					var shutdownService = (ShutdownService)SD.ShutdownService;
					shutdownService.WaitForBackgroundTasks();
					((IDisposable)SD.Services).Dispose(); // dispose all services
					propertyService.Save();
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
			foreach (ICommand command in AddInTree.BuildItems<ICommand>("/SharpDevelop/Workbench/AutostartAfterWorkbenchInitialized", null, false)) {
				try {
					command.Execute(null);
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
					SD.MainThread.InvokeIfRequired(() => new Action<List<Document>>(GetOpenDocuments)(l));
				} else {
					GetOpenDocuments(l);
				}
				return l;
			}
		}
		void GetOpenDocuments(List<Document> l)
		{
			foreach (IViewContent vc in SD.Workbench.ViewContentCollection) {
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
				return SD.MainThread.InvokeIfRequired(() => OpenDocumentInternal(fileName));
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
				SD.MainThread.InvokeIfRequired(() => OpenProjectInternal(fileName));
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
				return SD.MainThread.InvokeIfRequired(() => CloseWorkbenchInternal(force));
			} else {
				return CloseWorkbenchInternal(force);
			}
		}
		bool CloseWorkbenchInternal(bool force)
		{
			foreach (IWorkbenchWindow window in SD.Workbench.WorkbenchWindowCollection.ToArray()) {
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
					return SD.MainThread.InvokeIfRequired<bool>(GetWorkbenchVisibleInternal);
				} else {
					return GetWorkbenchVisibleInternal();
				}
			}
			set {
				if (WorkbenchSingleton.InvokeRequired) {
					SD.MainThread.InvokeIfRequired(() => SetWorkbenchVisibleInternal(value));
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
