// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ICSharpCode.CodeQualityAnalysis.Utility;
using ICSharpCode.CodeQualityAnalysis.Utility.Localizeable;
using ICSharpCode.CodeQualityAnalysis.Utility.Queries;
using ICSharpCode.SharpDevelop.Widgets;

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
	
	
	public class MainWindowViewModel :ViewModelBase
	{
		BaseQuery query;
		
		public MainWindowViewModel():base()
		{
			this.FrmTitle = "Code Quality Analysis";
			this.btnOpenAssembly = "Open Assembly";
			
			#region MainTab
			this.TabDependencyGraph = "Dependency Graph";
			this.TabDependencyMatrix = "Dependency Matrix";
			this.TabMetrics = "Metrics";
			#endregion
			
			ActivateMetrics = new RelayCommand(ActivateMetricsExecute);
			ExecuteSelectedItemWithCommand = new RelayCommand (ExecuteSelectedItem);
			
		}
		
		
		public string FrmTitle {get;private set;}
		
		public string btnOpenAssembly {get; private set;}
		
		#region Main TabControl
		
		public string TabDependencyGraph {get; private set;}
		public string TabDependencyMatrix {get; private set;}
		public string TabMetrics {get;private set;}
		
		TabItem selectedTabItem;
		
		public TabItem SelectedTabItem {
			get { return selectedTabItem; }
			set { selectedTabItem = value;
				ResetTreeMap();
				selectedMetricsLevel = 0;
				base.RaisePropertyChanged(() => SelectedTabItem);}
		}
		
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
		
		
		public bool MetrixTabEnable
		{
			get {return SelectedTreeNode != null;}
		}
		
		string typeInfo;
		
		public string TypeInfo {
			get { return typeInfo; }
			set { typeInfo = value;
				base.RaisePropertyChanged(() =>this.TypeInfo);
			}
		}
		
		private AssemblyNode mainModule;
		
		public AssemblyNode MainModule {
			get { return mainModule; }
			set {
				mainModule = value;
				base.RaisePropertyChanged(() =>this.MainModule);
				Summary = String.Format("Module Name: {0}  Namespaces: {1}  Types {2} Methods: {3}  Fields: {4}",
				                        mainModule.Name,
				                        mainModule.Namespaces.Count(),
				                        mainModule.TypesCount,
				                        mainModule.MethodsCount,
				                        mainModule.FieldsCount);
			}
		}
		
		
		private INode selectedTreeNode;
		
		public INode SelectedTreeNode {
			get { return selectedTreeNode; }
			set { selectedTreeNode = value;
				base.RaisePropertyChanged(() =>this.SelectedTreeNode);
				base.RaisePropertyChanged(() =>this.MetrixTabEnable);
				Summary = UpdateToolStrip();
			}
		}
		
		
		string UpdateToolStrip()
		{
//			var t = SelectedTreeNode as Type;
//			if (t != null)
//			{
//				return string.Format("Type Namer {0}  Methods {1} Fields {2}",
//				                     t.Name,
//				                     t.GetAllMethods().Count(),
//				                     t.GetAllFields().Count());
//			}
//			var ns = SelectedTreeNode as Namespace;
//			if ( ns != null) {
//				return string.Format("Namespace Name {0}  Types : {1}  Methods: {2} Fields : {3}",
//				                     ns.Name,
//				                     ns.Types.Count,
//				                     ns.GetAllMethods().Count(),
//				                     ns.GetAllFields().Count());
//			}
			return String.Empty;
		}
		
	
		#region MetricsLevel Combo Left ComboBox
		
		public MetricsLevel MetricsLevel {
			get {return MetricsLevel;}
		}
		
		private MetricsLevel selectedMetricsLevel;
		
		public MetricsLevel SelectedMetricsLevel {
			get { return selectedMetricsLevel; }
			set { selectedMetricsLevel = value;
				base.RaisePropertyChanged(() =>this.SelectedMetricsLevel);
				ResetTreeMap();
			}
		}
		
		
		public ICommand ActivateMetrics {get;private set;}
	
		void ActivateMetricsExecute ()
		{
			itemsWithCommand.Clear();
			
			switch (SelectedMetricsLevel) {
				case MetricsLevel.Assembly:
					query = new QueryAssembly(MainModule);
					break;
				case MetricsLevel.Namespace:
					query = new QueryNameSpace(MainModule);
					break;
				case MetricsLevel.Type:
					query = new QueryType(MainModule);
					break;
				case MetricsLevel.Method:
					query = new QueryMethod(MainModule);
					break;
				default:
					throw new Exception("Invalid value for MetricsLevel");
			}
			ItemsWithCommand = query.GetQueryList();
		}
		
		#endregion
		
		#region Metrics Combo > Right Combobox
		
		List<ItemWithFunc> itemsWithCommand;
		
		public List<ItemWithFunc> ItemsWithCommand {
			get {
				if (itemsWithCommand == null) {
					itemsWithCommand = new List<ItemWithFunc>();
				}
				return itemsWithCommand;
			}
			set { 
				itemsWithCommand = value;
				base.RaisePropertyChanged(() => ItemsWithCommand);}
		}
		
		ItemWithFunc selectedItemWithCommand;
		
		public ItemWithFunc SelectedItemWithCommand {
			get { return selectedItemWithCommand; }
			set { selectedItemWithCommand = value;
				base.RaisePropertyChanged(() => SelectedItemWithCommand);}
		}
		
		public ICommand ExecuteSelectedItemWithCommand {get; private set;}
		
		void ExecuteSelectedItem()
		{
			if (SelectedItemWithCommand != null) {
				TreeValueProperty ="NumericValue";
				var list = SelectedItemWithCommand.Action.Invoke();
				if (list != null ) {
					Nodes = new ObservableCollection<TreeMapViewModel>(list);
					Summary = String.Format("Total number of Elements <{0}> '0'-values <{1}> No of Displayed Elements <{2}>        ",
					                        query.TotalElements,
					                        query.RemovedElements,
					                        Nodes.Count);
				}
			}
		}
		
		#endregion
		
		#region ShowTreeMap Treemap
		
		private ObservableCollection<TreeMapViewModel> nodes;
		
		public ObservableCollection<TreeMapViewModel> Nodes {
			get {
				if (nodes == null)
				{
					nodes = new ObservableCollection<TreeMapViewModel>();
				}
				return nodes;
			}
			set { nodes = value;
				base.RaisePropertyChanged(() =>this.Nodes);}
		}
		
		
		private string treeValueProperty ;
		
		public string TreeValueProperty {
			get { return treeValueProperty; }
			set { treeValueProperty = value;
				base.RaisePropertyChanged(() =>this.TreeValueProperty);}
		}
		
		
		void ResetTreeMap()
		{
			Nodes.Clear();
			ItemsWithCommand.Clear();
			
			base.RaisePropertyChanged(() => Nodes);
			base.RaisePropertyChanged(() => ItemsWithCommand);
		}
		
		
		#endregion
		
		#region ToolStrip and ToolTip
		
		string summary;
		
		public string Summary {
			get { return summary; }
			set { summary = value;
				base.RaisePropertyChanged(() => Summary);
			}
		}
		
		#endregion
	}
}
