// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;

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
			ICSharpCode.Core.Services.ServiceManager.LoggingService = new log4netLoggingService();
			ICSharpCode.Core.Services.ServiceManager.MessageService = WinFormsMessageService.Instance;
			
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
			ParserService.DomPersistencePath = properties.DomPersistencePath;
			
			// disable RTL: translations for the RTL languages are inactive
			RightToLeftConverter.RightToLeftLanguages = new string[0];
			
			if (properties.ApplicationRootPath != null) {
				FileUtility.ApplicationRootPath = properties.ApplicationRootPath;
			}
			
			startup.StartCoreServices();
			Assembly exe = Assembly.Load(properties.ResourceAssemblyName);
			ResourceService.RegisterNeutralStrings(new ResourceManager("Resources.StringResources", exe));
			ResourceService.RegisterNeutralImages(new ResourceManager("Resources.BitmapResources", exe));
			
			MenuCommand.LinkCommandCreator = delegate(string link) { return new LinkCommand(link); };
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
			Project.ProjectService.StartBuild += delegate { this.callback.StartBuild(); };
			Project.ProjectService.EndBuild   += delegate { this.callback.EndBuild(); };
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
			
			LoggingService.Info("Initializing workbench...");
			WorkbenchSingleton.InitializeWorkbench();
			
			LoggingService.Info("Starting workbench...");
			Exception exception = null;
			// finally start the workbench.
			try {
				StartWorkbenchCommand wbc = new StartWorkbenchCommand();
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
			if (force) {
				foreach (IWorkbenchWindow window in WorkbenchSingleton.Workbench.WorkbenchWindowCollection.ToArray()) {
					window.CloseWindow(true);
				}
			}
			WorkbenchSingleton.MainForm.Close();
			return WorkbenchSingleton.MainForm.IsDisposed;
		}
		
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void KillWorkbench()
		{
			System.Windows.Forms.Application.Exit();
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
			return WorkbenchSingleton.MainForm.Visible;
		}
		void SetWorkbenchVisibleInternal(bool value)
		{
			WorkbenchSingleton.MainForm.Visible = value;
		}
	}
}


