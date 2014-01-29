// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Startup;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Runs workbench initialization.
	/// Is called by ICSharpCode.SharpDevelop.Sda and should not be called manually!
	/// </summary>
	class WorkbenchStartup
	{
		const string workbenchMemento = "WorkbenchMemento";
		const string activeContentState = "Workbench.ActiveContent";
		App app;
		
		public void InitializeWorkbench()
		{
			app = new App();
			System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
			ComponentDispatcher.ThreadIdle -= ComponentDispatcher_ThreadIdle; // ensure we don't register twice
			ComponentDispatcher.ThreadIdle += ComponentDispatcher_ThreadIdle;
			LayoutConfiguration.LoadLayoutConfiguration();
			SD.Services.AddService(typeof(IMessageLoop), new DispatcherMessageLoop(app.Dispatcher, SynchronizationContext.Current));
			InitializeWorkbench(new WpfWorkbench(), new AvalonDockLayout());
		}
		
		static void InitializeWorkbench(WpfWorkbench workbench, IWorkbenchLayout layout)
		{
			SD.Services.AddService(typeof(IWorkbench), workbench);
			
			UILanguageService.ValidateLanguage();
			
			TaskService.Initialize();
			Project.CustomToolsService.Initialize();
			
			workbench.Initialize();
			workbench.SetMemento(SD.PropertyService.NestedProperties(workbenchMemento));
			workbench.WorkbenchLayout = layout;
			
			// HACK: eagerly load output pad because pad services cannnot be instanciated from background threads
			SD.Services.AddService(typeof(IOutputPad), CompilerMessageView.Instance);
			
			var dlgMsgService = SD.MessageService as IDialogMessageService;
			if (dlgMsgService != null) {
				dlgMsgService.DialogSynchronizeInvoke = SD.MainThread.SynchronizingObject;
				dlgMsgService.DialogOwner = workbench.MainWin32Window;
			}
			
			var applicationStateInfoService = SD.GetService<ApplicationStateInfoService>();
			if (applicationStateInfoService != null) {
				applicationStateInfoService.RegisterStateGetter(activeContentState, delegate { return SD.Workbench.ActiveContent; });
			}
			
			WorkbenchSingleton.OnWorkbenchCreated();
			
			// initialize workbench-dependent services:
			NavigationService.InitializeService();
			
			workbench.ActiveContentChanged += delegate {
				Debug.WriteLine("ActiveContentChanged to " + workbench.ActiveContent);
				LoggingService.Debug("ActiveContentChanged to " + workbench.ActiveContent);
			};
			workbench.ActiveViewContentChanged += delegate {
				Debug.WriteLine("ActiveViewContentChanged to " + workbench.ActiveViewContent);
				LoggingService.Debug("ActiveViewContentChanged to " + workbench.ActiveViewContent);
			};
			workbench.ActiveWorkbenchWindowChanged += delegate {
				Debug.WriteLine("ActiveWorkbenchWindowChanged to " + workbench.ActiveWorkbenchWindow);
				LoggingService.Debug("ActiveWorkbenchWindowChanged to " + workbench.ActiveWorkbenchWindow);
			};
		}
		
		static void ComponentDispatcher_ThreadIdle(object sender, EventArgs e)
		{
			System.Windows.Forms.Application.RaiseIdle(e);
		}
		
		public void Run(IList<string> fileList)
		{
			bool didLoadSolutionOrFile = false;
			
			NavigationService.SuspendLogging();
			
			foreach (string file in fileList) {
				LoggingService.Info("Open file " + file);
				didLoadSolutionOrFile = true;
				try {
					var fullFileName = FileName.Create(Path.GetFullPath(file));
					
					if (SD.ProjectService.IsSolutionOrProjectFile(fullFileName)) {
						SD.ProjectService.OpenSolutionOrProject(fullFileName);
					} else {
						SharpDevelop.FileService.OpenFile(fullFileName);
					}
				} catch (Exception e) {
					MessageService.ShowException(e, "unable to open file " + file);
				}
			}
			
			// load previous solution
			if (!didLoadSolutionOrFile && SD.PropertyService.Get("SharpDevelop.LoadPrevProjectOnStartup", false)) {
				if (SD.FileService.RecentOpen.RecentProjects.Count > 0) {
					SD.ProjectService.OpenSolutionOrProject(SD.FileService.RecentOpen.RecentProjects[0]);
					didLoadSolutionOrFile = true;
				}
			}
			
			if (!didLoadSolutionOrFile) {
				foreach (ICommand command in AddInTree.BuildItems<ICommand>("/SharpDevelop/Workbench/AutostartNothingLoaded", null, false)) {
					try {
						command.Execute(null);
					} catch (Exception ex) {
						MessageService.ShowException(ex);
					}
				}
				StartPreloadThread();
			}
			
			NavigationService.ResumeLogging();
			
			((ParserService)SD.ParserService).StartParserThread();
			
			// finally run the workbench window ...
			app.Run(SD.Workbench.MainWindow);
			
			// save the workbench memento in the ide properties
			try {
				SD.PropertyService.SetNestedProperties(workbenchMemento, ((WpfWorkbench)SD.Workbench).CreateMemento());
			} catch (Exception e) {
				MessageService.ShowException(e, "Exception while saving workbench state.");
			}
		}
		
		#region Preload-Thread
		void StartPreloadThread()
		{
			// Wait until UI is responsive and pads are initialized before starting the thread.
			// We don't want to slow down SharpDevelop's startup, we just want to make opening
			// a project more responsive.
			// (and parallelism doesn't really help here; we're mostly waiting for the disk to load the code)
			// So we do our work in the background while the user decides which project to open.
			SD.MainThread.InvokeAsyncAndForget(
				() => new Thread(PreloadThread) { IsBackground = true, Priority = ThreadPriority.BelowNormal }.Start(),
				DispatcherPriority.ApplicationIdle
			);
		}
		
		void PreloadThread()
		{
			// Pre-load some stuff to make SharpDevelop more responsive once it is started.
			LoggingService.Debug("Preload-Thread started.");
			
			// warm up MSBuild
			string projectCode = @"
<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
  <PropertyGroup>
    <Configuration>Debug</Configuration>
    <Platform>AnyCPU</Platform>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""System"" />
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
</Project>";
			var project = new Microsoft.Build.Evaluation.Project(
				new System.Xml.XmlTextReader(new System.IO.StringReader(projectCode)), null, "4.0",
				new Microsoft.Build.Evaluation.ProjectCollection());
			
			// warm up the XSHD loader
			ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.GetDefinition("C#");
			// warm up the C# parser
			var parser = new ICSharpCode.NRefactory.CSharp.CSharpParser();
			var cu = parser.Parse(new ICSharpCode.AvalonEdit.Document.TextDocument(@"using System;
class Test {
	int SomeMethod(string a);
	void Main(string[] b) {
	   SomeMethod(b[0 + 1]);
	}
}"), "test.cs");
			// warm up the type system
			var unresolvedFile = cu.ToTypeSystem();
			var pc = new ICSharpCode.NRefactory.CSharp.CSharpProjectContent().AddOrUpdateFiles(unresolvedFile);
			pc = pc.AddAssemblyReferences(ICSharpCode.NRefactory.TypeSystem.Implementation.MinimalCorlib.Instance);
			var compilation = pc.CreateCompilation();
			// warm up the resolver
			var resolver = new ICSharpCode.NRefactory.CSharp.Resolver.CSharpAstResolver(compilation, cu, unresolvedFile);
			foreach (var node in cu.Descendants) {
				resolver.Resolve(node);
			}
			// load CSharp.Refactoring.dll
			new RedundantUsingDirectiveIssue();
			// warm up AvalonEdit (must be done on main thread)
			SD.MainThread.InvokeAsyncAndForget(
				delegate {
					object editor;
					SD.EditorControlService.CreateEditor(out editor);
					LoggingService.Debug("Preload-Thread finished.");
				}, DispatcherPriority.ApplicationIdle);
		}
		#endregion
	}
}
