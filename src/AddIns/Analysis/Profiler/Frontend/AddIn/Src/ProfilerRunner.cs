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
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.Profiler.AddIn.Dialogs;
using ICSharpCode.Profiler.AddIn.OptionPanels;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Profiler.AddIn
{
	/// <summary>
	/// Description of ProfilerRunner.
	/// </summary>
	public class ProfilerRunner : IDisposable
	{
		public event EventHandler RunFinished;
		ProfilerControlWindow controlWindow;
		
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
				this.profiler = new Controller.Profiler(startInfo, this.database.GetWriter(), OptionWrapper.CreateProfilerOptions());
			} else {
				this.database = null;
				this.writer = writer;
				this.profiler = new Controller.Profiler(startInfo, writer, OptionWrapper.CreateProfilerOptions());
			}
			
			PrintProfilerOptions();
			this.profiler.RegisterFailed += delegate { MessageService.ShowError("${res:AddIns.Profiler.Messages.RegisterFailed}"); };
			this.profiler.DeregisterFailed += delegate { MessageService.ShowError("${res:AddIns.Profiler.Messages.UnregisterFailed}"); };
			this.profiler.OutputUpdated += delegate { SetOutputText(profiler.ProfilerOutput); };
			this.profiler.SessionEnded += delegate { FinishSession(); };
		}
		
		void PrintProfilerOptions()
		{
			var options = OptionWrapper.CreateProfilerOptions();
			LoggingService.Info("Profiler settings:");
			LoggingService.Info("Shared memory size: " + options.SharedMemorySize + " (" + (options.SharedMemorySize / 1024 / 1024) + " MB)");
			LoggingService.Info("Combine recursive calls: " + options.CombineRecursiveFunction);
			LoggingService.Info("Enable DC: " + options.EnableDC);
			LoggingService.Info("Profile .NET internals: " + (!options.DoNotProfileDotNetInternals));
			LoggingService.Info("Track events: " + options.TrackEvents);
		}
		
		void FinishSession()
		{
			try {
				using (AsynchronousWaitDialog dlg = AsynchronousWaitDialog.ShowWaitDialog(StringParser.Parse("${res:AddIns.Profiler.Messages.PreparingForAnalysis}"), true)) {
					profiler.Dispose();
					SD.MainThread.InvokeAsyncAndForget(() => { controlWindow.AllowClose = true; controlWindow.Close(); });
					if (database != null) {
						database.WriteTo(writer, progress => {
						                 	dlg.Progress = progress;
						                 	return !dlg.CancellationToken.IsCancellationRequested;
						                 });
						writer.Close();
						database.Close();
					} else {
						writer.Close();
					}
					
					if (!dlg.CancellationToken.IsCancellationRequested)
						OnRunFinished(EventArgs.Empty);
				}
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		public Process Run()
		{
			SD.Workbench.GetPad(typeof(CompilerMessageView)).BringPadToFront();
			controlWindow = new ProfilerControlWindow(this);
			Process p = profiler.Start();
			controlWindow.Show();
			return p;
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
				if (MessageService.AskQuestion("${res:AddIns.Profiler.Messages.NoStartableProjectWantToProfileStartupProject}")) {
					currentProj = ProjectService.OpenSolution.StartupProject as AbstractProject;
					if (currentProj == null) {
						MessageService.ShowError("${res:AddIns.Profiler.Messages.NoStartableProjectFound}");
						return null;
					}
				} else
					return null;
			}
			if (!File.Exists(currentProj.OutputAssemblyFullPath)) {
				MessageService.ShowError("${res:AddIns.Profiler.Messages.FileNotFound}");
				return null;
			}
			
			ProcessStartInfo startInfo;
			try {
				startInfo = currentProj.CreateStartInfo();
			} catch (ProjectStartException ex) {
				MessageService.ShowError(ex.Message);
				return null;
			}
			ProfilerRunner runner = new ProfilerRunner(startInfo, true, writer);
			return runner;
		}
		
		#region MessageView Management
		static MessageViewCategory profileCategory;
		
		static void EnsureProfileCategory()
		{
			if (profileCategory == null) {
				MessageViewCategory.Create(ref profileCategory, "Profile", StringParser.Parse("${res:AddIns.Profiler.MessageViewCategory}"));
			}
		}
		
		public static void SetOutputText(string text)
		{
			EnsureProfileCategory();
			profileCategory.SetText(StringParser.Parse(text));
		}
		
		public static void AppendOutputText(string text)
		{
			EnsureProfileCategory();
			profileCategory.AppendText(StringParser.Parse(text));
		}
		
		public static void AppendOutputLine(string text)
		{
			EnsureProfileCategory();
			profileCategory.AppendLine(StringParser.Parse(text));
		}
		#endregion
		
		bool isDisposed = false;
		
		public void Dispose()
		{
			if (!isDisposed) {
				profiler.Dispose();
				controlWindow.Close();
				
				isDisposed = true;
			}
		}
	}
}
