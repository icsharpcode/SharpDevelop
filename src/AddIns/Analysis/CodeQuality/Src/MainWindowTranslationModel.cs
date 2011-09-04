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
	public class MainWindowTranslationViewModel :ViewModelBase
	{
		
		public MainWindowTranslationViewModel():base()
		{
			this.Title = "Code Quality Analysis";
			this.OpenAssembly = "Open Assembly";
			this.DependencyGraph = "Dependency Graph";
		}
		
		
		public string Title {get;private set;}
		
		public string OpenAssembly {get; private set;}
		
		public string DependencyGraph {get; private set;}
		
		private string fileName;
		
		public string FileName {
			get { return fileName; }
			set { fileName = value;
			base.RaisePropertyChanged(() =>FileName);}
		}
		
		private Visibility progressbarVisibly ;
		
		public Visibility ProgressbarVisible {
			get { return progressbarVisibly; }
			set { progressbarVisibly = value;
				base.RaisePropertyChanged(() =>ProgressbarVisible);
				}
		}
		
		private Visibility assemblyStatsVisible;
		
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
		
		/*
		#region OpenAssembly
		
		public ICommand OpenAssemblyCommand
        {
            get { return new RelayCommand(SaveAssemblyExecute, CanSaveAssemblyExecute); }
        }
		  
		Boolean CanSaveAssemblyExecute()
        {
            return true;
        }
		
		
		void SaveAssemblyExecute()
        {
           var fileDialog = new SaveFileDialog()
			{
				Filter = "PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|BMP (*.bmp)|*.bmp|TIFF (.tiff)|*.tiff"
			};

			fileDialog.ShowDialog();

			if (String.IsNullOrEmpty(fileDialog.FileName))
				return;

			// render it
			var renderBitmap = new RenderTargetBitmap((int)graphLayout.ActualWidth,
			                                          (int)graphLayout.ActualHeight,
			                                          96d,
			                                          96d,
			                                          PixelFormats.Default);
			renderBitmap.Render(graphLayout);

			using (var outStream = new FileStream(fileDialog.FileName, FileMode.Create))
			{
				BitmapEncoder encoder;

				switch (fileDialog.FilterIndex)
				{
					case 1:
						encoder = new PngBitmapEncoder();
						break;
					case 2:
						encoder = new JpegBitmapEncoder();
						break;
					case 3:
						encoder = new GifBitmapEncoder();
						break;
					case 4:
						encoder = new BmpBitmapEncoder();
						break;
					case 5:
						encoder = new TiffBitmapEncoder();
						break;
					default:
						encoder = new PngBitmapEncoder();
						break;
				}
				
				encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
				encoder.Save(outStream);
			}
        }
		#endregion
		*/
	}
}
