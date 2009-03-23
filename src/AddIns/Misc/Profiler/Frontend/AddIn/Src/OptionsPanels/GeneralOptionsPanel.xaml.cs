using ICSharpCode.Profiler.Interprocess;
using System;
using System.Collections.Generic;
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
using System.Management;
using System.Linq;

namespace ICSharpCode.Profiler.AddIn.OptionsPanels
{
	/// <summary>
	/// Interaction logic for GeneralOptionsPanel.xaml
	/// </summary>
	public partial class GeneralOptionsPanel : UserControl
	{
		public GeneralOptionsPanel()
		{
			InitializeComponent();
		}
		
		public void Load(bool enableDC, int sharedMemorySize)
		{
			this.slSharedMemorySize.Value = sharedMemorySize;
			this.chkEnableDC.IsChecked = !enableDC;
		}
		
		public T GetOptionValue<T>(string name)
		{
			object o;
			
			switch (name) {
				case "SharedMemorySize":
					o = this.slSharedMemorySize.Value;
					break;
				case "EnableDC":
					o = this.chkEnableDC.IsChecked;
					break;
				default:
					throw new NotSupportedException("value '" + name + "' is not supported!");
			}
			
			return (T)o;
		}
	}
}