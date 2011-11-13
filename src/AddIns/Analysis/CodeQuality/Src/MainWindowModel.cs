/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.09.2011
 * Time: 13:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.CodeQualityAnalysis.Controls;
using ICSharpCode.CodeQualityAnalysis.Utility;
using ICSharpCode.CodeQualityAnalysis.Utility.Localizeable;
using Microsoft.Win32;

namespace ICSharpCode.CodeQualityAnalysis
{
	/// <summary>
	/// Description of MainWindowViewModel.
	/// </summary>
	public enum MetricsLevel
	{
		Assembly,
		Namespace,
		Type,
		Method
	}
	
	
	public enum Metrics
	{
		[LocalizableDescription("IL Instructions")]
		ILInstructions,
		
		[LocalizableDescription("Cyclomatic Complexity")]
		CyclomaticComplexity,
		
		[LocalizableDescription("Variables")]
		Variables
	}
	
	public class MainWindowViewModel :ViewModelBase
	{
		
		public MainWindowViewModel():base()
		{
			this.FrmTitle = "Code Quality Analysis";
			this.btnOpenAssembly = "Open Assembly";
			
			#region MainTab
			this.TabDependencyGraph = "Dependency Graph";
			this.TabDependencyMatrix = "Dependency Matrix";
			this.TabMetrics = "Metrics";
			#endregion
			
			MetrixTabEnable = false;
			
			ActivateMetrics = new RelayCommand(ActivateMetricsExecute);
			ShowTreeMap = new RelayCommand(ShowTreemapExecute,CanActivateTreemap);
		}
		
		
		public string FrmTitle {get;private set;}
		
		public string btnOpenAssembly {get; private set;}
		
		#region Main TabControl
		
		public string TabDependencyGraph {get; private set;}
		public string TabDependencyMatrix {get; private set;}
		public string TabMetrics {get;private set;}
		
		#endregion
		
		
		private string fileName;
		
		public string FileName {
			get { return fileName; }
			set { fileName = value;
			base.RaisePropertyChanged(() =>FileName);}
		}
		
		
		private Visibility progressbarVisibly = Visibility.Hidden;
		
		public Visibility ProgressbarVisible {
			get { return progressbarVisibly; }
			set { progressbarVisibly = value;
				base.RaisePropertyChanged(() =>ProgressbarVisible);
				}
		}
		
		private Visibility assemblyStatsVisible= Visibility.Hidden; 
		
		public Visibility AssemblyStatsVisible {
			get { return assemblyStatsVisible; }
			set { assemblyStatsVisible = value;
				base.RaisePropertyChanged(() => AssemblyStatsVisible);
			}
		}

		bool mainTabEnable;

		public bool MainTabEnable {
			get { return mainTabEnable; }
			set { mainTabEnable = value;
				base.RaisePropertyChanged(() => MainTabEnable);
			}
		}
		
		bool metrixTabEnable;
		
		public bool MetrixTabEnable {
			get { return metrixTabEnable; }
			set { metrixTabEnable = value;
			base.RaisePropertyChanged(() => MetrixTabEnable);}
		}
		
		string typeInfo;
		
		public string TypeInfo {
			get { return typeInfo; }
			set { typeInfo = value;
				base.RaisePropertyChanged(() =>this.TypeInfo);
			}
		}
		
		private Module mainModule;
		
		public Module MainModule {
			get { return mainModule; }
			set { mainModule = value;
				base.RaisePropertyChanged(() =>this.MainModule);
			}
		}
		
		
		private ObservableCollection<INode> nodes;
		
		public ObservableCollection<INode> Nodes {
			get { return nodes; }
			set { nodes = value;
			base.RaisePropertyChanged(() =>this.Nodes);}
		}
		
		
		private string treeValueProperty ;
		
		public string TreeValueProperty {
			get { return treeValueProperty; }
			set { treeValueProperty = value;
			base.RaisePropertyChanged(() =>this.TreeValueProperty);}
		}
		
		// MetricsLevel Combo
		
		public MetricsLevel MetricsLevel {
			get {return MetricsLevel;}
		}
	
		#region ActivateMetrics
		
		public ICommand ActivateMetrics {get;private set;}
		
		bool metricsIsActive;
		
		void ActivateMetricsExecute ()
		{
			metricsIsActive = true;
		}
		
		#endregion
		
		
		// Metrics Combo
		
		public Metrics Metrics
		{
			get {return Metrics;}
		}
		
		Metrics selectedMetrics;
		
		public Metrics SelectedMetrics {
			get { return selectedMetrics; }
			set { selectedMetrics = value;
				base.RaisePropertyChanged(() =>this.SelectedMetrics);
			}
		}
		
		#region ShowTreeMap Treemap
		
		public ICommand ShowTreeMap {get;private set;}
		
		bool CanActivateTreemap()
		{
			return metricsIsActive;
		}
		
		void ShowTreemapExecute()
		{
			var r  = from ns in MainModule.Namespaces
				from type in ns.Types
				from method in type.Methods
				select method;
			Nodes = new ObservableCollection<INode>(r);
			
			switch (selectedMetrics)
			{
				case Metrics.ILInstructions:
					TreeValueProperty = "Instructions.Count";
					break;
					
				case Metrics.CyclomaticComplexity:
					TreeValueProperty = Metrics.CyclomaticComplexity.ToString();
					break;
				case Metrics.Variables:
					TreeValueProperty = Metrics.Variables.ToString();
					break;
				default:
					throw new Exception("Invalid value for Metrics");
			}
		}
		
		#endregion
	}
}
