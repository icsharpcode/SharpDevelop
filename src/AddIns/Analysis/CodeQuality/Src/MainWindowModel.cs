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

using ICSharpCode.CodeQualityAnalysis.Controls;
using ICSharpCode.CodeQualityAnalysis.Utility;
using Microsoft.Win32;

namespace ICSharpCode.CodeQualityAnalysis
{
	/// <summary>
	/// Description of MainWindowViewModel.
	/// </summary>
	
	
	public class MainWindowViewModel :ViewModelBase
	{
		
		public MainWindowViewModel():base()
		{
			this.FrmTitle = "$Code Quality Analysis";
			this.btnOpenAssembly = "$Open Assembly";
			
			#region MainTab
			this.TabDependencyGraph = "$Dependency Graph";
			this.TabDependencyMatrix = "$Dependency Matrix";
			this.TabMetrics = "$Metrics";
			#endregion
			MetrixTabEnable = false;
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
			base.RaisePropertyChanged(() =>this.MainModule);}			                         	                      
		}
	}
}
