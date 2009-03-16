using ICSharpCode.Profiler.AddIn.OptionsPanels;
using System;
using System.Globalization;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.Profiler.Controller;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Profiler.AddIn
{
	/// <summary>
	/// Provides methods for initialisation and control of the current profiling session.
	/// </summary>
	public static class ProfilerService
	{
		public static Profiler.Controller.Profiler RunCurrentProject(out string profilerSessionFilePath)
		{
			profilerSessionFilePath = Path.Combine(ProjectService.CurrentProject.Directory, @"ProfilingSessions\Session" + DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture) + ".sdps");
			
			Directory.CreateDirectory(Path.GetDirectoryName(profilerSessionFilePath));
			
			if (!ProjectService.CurrentProject.IsStartable) {
				MessageService.ShowError("This project cannot be started, please select a startable project for Profiling!");
				return null;
			}
			if (!File.Exists(ProjectService.CurrentProject.OutputAssemblyFullPath)) {
				MessageService.ShowError("This project cannot be started because the executable file was not found, " +
				                         "please ensure that the project and all its depencies are built correctly!");
				return null;
			}
			
			Profiler.Controller.Profiler profiler = InitProfiler(ProjectService.CurrentProject.OutputAssemblyFullPath, profilerSessionFilePath);
			profiler.ProfilerOptions = ProfilerOptionsWrapper.CreateProfilerOptions();
			profiler.Start();
			
			return profiler;
		}

		static Profiler.Controller.Profiler InitProfiler(string path, string outputFilePath)
		{
			Profiler.Controller.Profiler profiler = new Profiler.Controller.Profiler(path, new ProfilingDataSQLiteWriter(outputFilePath));
			
			profiler.RegisterFailed += delegate { MessageService.ShowError("regsvr32 failed"); };
			profiler.DeregisterFailed += delegate { MessageService.ShowError("regsvr32 /u failed"); };
			profiler.OutputUpdated += delegate { SetOutputText(profiler.ProfilerOutput); };
			
			return profiler;
		}
		
		static MessageViewCategory profileCategory = null;
		
		static void EnsureProfileCategory()
		{
			if (profileCategory == null) {
				MessageViewCategory.Create(ref profileCategory, "Profile", "Profile");
			}
		}
		
		public static void SetOutputText(string text)
		{
			EnsureProfileCategory();
			profileCategory.SetText(text);
		}
		
		public static void AppendOutputText(string text)
		{
			EnsureProfileCategory();
			profileCategory.AppendText(text);
		}
		
		public static void AppendOutputLine(string text)
		{
			EnsureProfileCategory();
			profileCategory.AppendLine(text);
		}
	}
}
