using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ICSharpCode.Profiler.Controller;
using ICSharpCode.Profiler.Controller.Data;
using ICSharpCode.Profiler.Controller.Queries;
using ICSharpCode.Profiler.Controls;
using Microsoft.Win32;

namespace ICSharpCode.Profiler.Frontend
{
	/// <summary>
	/// Interaktionslogik für Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		Profiler.Controller.Profiler profiler;
		ProfilingDataProvider provider;
		TempFileDatabase database;
		
		private void btnRun_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Programs|*.exe|All files|*.*";
			dlg.DefaultExt = ".exe";
			if (!(dlg.ShowDialog() ?? false))
				return;
			string path = dlg.FileName;

			// remove UI before disposing profiler
			//this.timeLine.ValuesList.Clear();
			if (this.provider != null)
				this.provider.Close();

			if (this.profiler != null)
				this.profiler.Dispose();
			
			string pathToDb = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(Profiler.Controller.Profiler).Assembly.Location), "output.sdps");
			if (File.Exists(pathToDb))
				File.Delete(pathToDb);
			
			this.database = new TempFileDatabase();
			
			this.profiler = new Profiler.Controller.Profiler(path, database.GetWriter(), new ProfilerOptions());
			profiler.RegisterFailed += delegate { MessageBox.Show("register failed"); };
			profiler.DeregisterFailed += delegate { MessageBox.Show("deregister failed"); };

			this.profiler.OutputUpdated += profiler_OutputUpdated;
			this.profiler.SessionEnded += profiler_SessionEnded;

			this.profiler.Start();

			this.btnRun.IsEnabled = false;
			this.btnStop.IsEnabled = true;
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (profiler != null)
				profiler.Dispose();
		}

		private void btnStop_Click(object sender, RoutedEventArgs e)
		{
			this.profiler.Stop();

			this.btnRun.IsEnabled = true;
			this.btnStop.IsEnabled = false;
		}

		void profiler_SessionEnded(object sender, EventArgs e)
		{
			string pathToDb = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(typeof(Profiler.Controller.Profiler).Assembly.Location), "output.sdps");
			ProfilingDataSQLiteWriter writer = new ProfilingDataSQLiteWriter(pathToDb, false, null);
			this.database.WriteTo(writer, progress => true);
			writer.Close();
			this.database.Close();
			this.provider = new ProfilingDataSQLiteProvider(pathToDb);

			this.Dispatcher.Invoke(
				(Action)(
					() => {
						try {
							this.treeView.Provider = this.provider;
							RefreshUI(0, 0);
							this.timeLine.IsEnabled = true;
							//this.timeLine.ValuesList.Clear();
							//this.timeLine.ValuesList.AddRange(this.provider.DataSets.Select(i => i.CpuUsage));
						} catch (Exception ex) {
							Debug.WriteLine(ex.ToString());
							MessageBox.Show(ex.ToString());
						}
					}
				)
			);
		}

		void RefreshUI(int startIndex, int endIndex)
		{
			if (startIndex > endIndex) {
				int help = startIndex;
				startIndex = endIndex;
				endIndex = help;
			}

			if ((endIndex < 0 && endIndex >= provider.DataSets.Count) &&
			    (startIndex < 0 && startIndex >= provider.DataSets.Count))
				return;
			
			treeView.CurrentQuery = "from t in Threads select t";

			treeView.SetRange(startIndex, endIndex);

			this.btnRun.IsEnabled = true;
			this.btnStop.IsEnabled = false;
		}

		void profiler_OutputUpdated(object sender, EventArgs e)
		{
			this.Dispatcher.Invoke(
				(Action)(delegate() {
				         	this.txtOutput.Text = this.profiler.ProfilerOutput;
				         }));
		}

		public Window1()
		{
			InitializeComponent();

			this.btnStop.IsEnabled = false;
			//this.timeLine.IsEnabled = false;
			this.treeView.Reporter = new ErrorReporter(HandleError);
			
			this.CommandBindings.Add(new CommandBinding(ApplicationCommands.SelectAll, ExecuteSelectAll, CanExecuteSelectAll));
		}
		
		void ExecuteSelectAll(object sender, ExecutedRoutedEventArgs e)
		{
			if (this.timeLine.IsEnabled) {
				//this.timeLine.SelectedStartIndex = 0;
				//this.timeLine.SelectedEndIndex = this.timeLine.ValuesList.Count;
			}
			e.Handled = true;
		}
		
		void CanExecuteSelectAll(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = this.timeLine.IsEnabled;// && this.timeLine.ValuesList.Count > 0;
			e.Handled = true;
		}
		
		void HandleError(IEnumerable<CompilerError> errors)
		{
			this.Dispatcher.Invoke(
				(Action)(delegate() {
				         	foreach (CompilerError error in errors)
				         		txtOutput.AppendText(error.ToString() + Environment.NewLine);
				         }));
		}

		private void timeLine_RangeChanged(object sender, RangeEventArgs e)
		{
			RefreshUI(e.StartIndex, e.EndIndex);
		}

		private void btnLoadSession_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "SharpDevelop Profiling Session|*.sdps|All files|*.*";
			dlg.DefaultExt = ".sdps";
			if (!(dlg.ShowDialog() ?? false))
				return;
			if (this.provider != null)
				this.provider.Close();
			this.provider = ProfilingDataSQLiteProvider.FromFile(dlg.FileName);
			this.treeView.Provider = this.provider;
			this.treeView.CurrentQuery = "from t in Threads select t";
			treeView.SetRange(0, this.provider.DataSets.Count);
			this.timeLine.IsEnabled = true;
			//this.timeLine.ValuesList.Clear();
			//this.timeLine.ValuesList.AddRange(provider.DataSets.Select(i => i.CpuUsage));
		}

		internal void LogString(string text, bool clear)
		{
			if (clear)
				this.txtOutput.Clear();
			this.txtOutput.AppendText(text);
		}
	}
}
