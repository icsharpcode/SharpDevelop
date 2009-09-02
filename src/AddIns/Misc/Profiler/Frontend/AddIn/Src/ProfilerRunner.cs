// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn.OptionsPanels;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using System.IO;

namespace ICSharpCode.Profiler.AddIn
{
	/// <summary>
	/// Description of ProfilerRunner.
	/// </summary>
	public class ProfilerRunner
	{
		public event EventHandler RunFinished;
		
		protected virtual void OnRunFinished(EventArgs e)
		{
			if (RunFinished != null) {
				RunFinished(this, e);
			}
		}
		
		Controller.Profiler profiler;
		IProfilingDataWriter writer;
		TempFileDatabase database;
		
		public ICSharpCode.Profiler.Controller.Profiler Profiler {
			get { return profiler; }
		}
		
		/// <summary>
		/// Creates a new ProfilerRunner using a ProcessStartInfo and a data writer.
		/// </summary>
		public ProfilerRunner(ProcessStartInfo startInfo, bool useTempFileDatabase, IProfilingDataWriter writer)
		{
			if (writer == null)
				throw new ArgumentNullException("writer");
			if (startInfo == null)
				throw new ArgumentNullException("startInfo");
			
			if (useTempFileDatabase) {
				this.database = new TempFileDatabase();
				this.writer = writer;
				this.profiler = new Controller.Profiler(startInfo, this.database.GetWriter(), General.CreateProfilerOptions());
			} else {
				this.database = null;
				this.writer = writer;
				this.profiler = new Controller.Profiler(startInfo, writer, General.CreateProfilerOptions());
			}
			
			PrintProfilerOptions();
			
			this.profiler.RegisterFailed += delegate { MessageService.ShowError("Could not register the profiler into COM Registry. Cannot start profiling!"); };
			this.profiler.DeregisterFailed += delegate { MessageService.ShowError("Could not unregister the profiler from COM Registry!"); };
			this.profiler.OutputUpdated += delegate { SetOutputText(profiler.ProfilerOutput); };
			this.profiler.SessionEnded += delegate { FinishSession(); };
		}
		
		void PrintProfilerOptions()
		{
			var options = General.CreateProfilerOptions();
			LoggingService.Info("Profiler settings:");
			LoggingService.Info("Shared memory size: " + options.SharedMemorySize + " (" + (options.SharedMemorySize / 1024 / 1024) + " MB)");
			LoggingService.Info("Combine recursive calls: " + options.CombineRecursiveFunction);
			LoggingService.Info("Enable DC: " + options.EnableDC);
			LoggingService.Info("Profile .NET internals: " + (!options.DoNotProfileDotNetInternals));
		}
		
		void FinishSession()
		{
			using (AsynchronousWaitDialog dlg = AsynchronousWaitDialog.ShowWaitDialog("Preparing for analysis", true)) {
				profiler.Dispose();
				if (database != null) {
					database.WriteTo(writer, progress => !dlg.IsCancelled);
					writer.Close();
					database.Close();
				} else {
					writer.Close();
				}
				
				if (!dlg.IsCancelled)
					OnRunFinished(EventArgs.Empty);
			}
		}
		
		public void Run()
		{
			WorkbenchSingleton.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
			profiler.Start();
		}
		
		public void Stop()
		{
			profiler.Stop();
		}
		
		public static ProfilerRunner CreateRunner(IProfilingDataWriter writer)
		{
			AbstractProject currentProj = ProjectService.CurrentProject as AbstractProject;
			
			if (currentProj == null)
				return null;
			
			if (!currentProj.IsStartable) {
				if (MessageService.AskQuestion("This project cannot be started. Do you want to profile the solution's StartUp project instead?")) {
					currentProj = ProjectService.OpenSolution.StartupProject as AbstractProject;
					if (currentProj == null) {
						MessageService.ShowError("No startable project was found. Aborting ...");
						return null;
					}
				} else
					return null;
			}
			if (!File.Exists(currentProj.OutputAssemblyFullPath)) {
				MessageService.ShowError("This project cannot be started because the executable file was not found, " +
				                         "please ensure that the project and all its depencies are built correctly!");
				return null;
			}
			
			ProfilerRunner runner = new ProfilerRunner(currentProj.CreateStartInfo(), true, writer);
			return runner;
		}
		
		#region MessageView Management
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
		#endregion
	}
}
