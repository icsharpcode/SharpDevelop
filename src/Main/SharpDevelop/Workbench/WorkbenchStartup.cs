// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Threading;
using ICSharpCode.Core;
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
		App app;
		
		public void InitializeWorkbench()
		{
			app = new App();
			System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
			ComponentDispatcher.ThreadIdle -= ComponentDispatcher_ThreadIdle; // ensure we don't register twice
			ComponentDispatcher.ThreadIdle += ComponentDispatcher_ThreadIdle;
			LayoutConfiguration.LoadLayoutConfiguration();
			SD.Services.AddService(typeof(IMessageLoop), new DispatcherMessageLoop(app.Dispatcher, SynchronizationContext.Current));
			WorkbenchSingleton.InitializeWorkbench(new WpfWorkbench(), new AvalonDockLayout());
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
					string fullFileName = Path.GetFullPath(file);
					
					IProjectLoader loader = ProjectService.GetProjectLoader(fullFileName);
					if (loader != null) {
						loader.Load(fullFileName);
					} else {
						SharpDevelop.FileService.OpenFile(fullFileName);
					}
				} catch (Exception e) {
					MessageService.ShowException(e, "unable to open file " + file);
				}
			}
			
			// load previous solution
			if (!didLoadSolutionOrFile && PropertyService.Get("SharpDevelop.LoadPrevProjectOnStartup", false)) {
				if (SD.FileService.RecentOpen.RecentProjects.Count > 0) {
					ProjectService.LoadSolution(SD.FileService.RecentOpen.RecentProjects[0]);
					didLoadSolutionOrFile = true;
				}
			}
			
			if (!didLoadSolutionOrFile) {
				foreach (ICommand command in AddInTree.BuildItems<ICommand>("/Workspace/AutostartNothingLoaded", null, false)) {
					try {
						command.Run();
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
				PropertyService.SetNestedProperties(workbenchMemento, SD.Workbench.CreateMemento());
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
			SD.MainThread.InvokeAsync(
				() => new Thread(PreloadThread) { IsBackground = true, Priority = ThreadPriority.BelowNormal }.Start(),
				DispatcherPriority.ApplicationIdle
			).FireAndForget();
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
			var pc = new ICSharpCode.NRefactory.CSharp.CSharpProjectContent().UpdateProjectContent(null, unresolvedFile);
			pc = pc.AddAssemblyReferences(new[] { ICSharpCode.NRefactory.TypeSystem.Implementation.MinimalCorlib.Instance });
			var compilation = pc.CreateCompilation();
			// warm up the resolver
			var resolver = new ICSharpCode.NRefactory.CSharp.Resolver.CSharpAstResolver(compilation, cu, unresolvedFile);
			foreach (var node in cu.Descendants) {
				resolver.Resolve(node);
			}
			// warm up AvalonEdit (must be done on main thread)
			SD.MainThread.InvokeAsync(
				delegate {
					object editor;
					SD.EditorControlService.CreateEditor(out editor);
					LoggingService.Debug("Preload-Thread finished.");
				}, DispatcherPriority.Background);
		}
		#endregion
	}
}
