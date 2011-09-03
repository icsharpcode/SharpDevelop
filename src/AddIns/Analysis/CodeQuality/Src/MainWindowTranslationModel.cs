/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.09.2011
 * Time: 13:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

using ICSharpCode.CodeQualityAnalysis.Controls;
using ICSharpCode.CodeQualityAnalysis.Utility;
using Microsoft.Win32;

namespace ICSharpCode.CodeQualityAnalysis
{
	/// <summary>
	/// Description of MainWindowViewModel.
	/// </summary>
	public class MainWindowTranslationViewModel :ViewModelBase
	{
		
		public MainWindowTranslationViewModel():base()
		{
			this.Title = "Code Quality Analysis";
			this.OpenAssembly = "OpenAssembly";
		}
		
		public string Title {get;private set;}
		
		public string OpenAssembly {get; private set;}
		
		
		#region OpenAssembly
		/*
		public ICommand OpenAssemblyCommand
        {
            get { return new RelayCommand(OpenAssemblyExecute, CanOpenAssemblyExecute); }
        }
		  
		Boolean CanOpenAssemblyExecute()
        {
            return true;
        }
		
		
		void OpenAssemblyExecute()
        {
            var fileDialog = new OpenFileDialog
			{
				Filter = "Component Files (*.dll, *.exe)|*.dll;*.exe"
			};

			fileDialog.ShowDialog();

			if (String.IsNullOrEmpty(fileDialog.FileName))
				return;
			
			progressBar.Visibility = Visibility.Visible;
			assemblyStats.Visibility = Visibility.Hidden;
			fileAssemblyLoading.Text = System.IO.Path.GetFileName(fileDialog.FileName);
			
			var worker = new BackgroundWorker();
			worker.DoWork += (source, args) => MetricsReader = new MetricsReader(fileDialog.FileName);
			worker.RunWorkerCompleted += (source, args) => {
				
				progressBar.Visibility = Visibility.Hidden;
				assemblyStats.Visibility = Visibility.Visible;
				mainTabs.IsEnabled = true;
				Helper.FillTree(definitionTree, metricsReader.MainModule);
				
				FillMatrix();
			};
			
			worker.RunWorkerAsync();
        }
		
		private void FillMatrix()
		{
			var matrix = new DependencyMatrix();

			foreach (var ns in metricsReader.MainModule.Namespaces) {
				matrix.HeaderRows.Add(new Cell<INode>(ns));
				foreach (var type in ns.Types) {
					matrix.HeaderRows.Add(new Cell<INode>(type));
				}
				matrix.HeaderColumns.Add(new Cell<INode>(ns));
				foreach (var type in ns.Types) {
					matrix.HeaderColumns.Add(new Cell<INode>(type));
				}
			}

			//matrixControl.Matrix = matrix;
			//matrixControl.DrawTree(metricsReader.MainModule);
		}
		*/
		#endregion
	}
}
