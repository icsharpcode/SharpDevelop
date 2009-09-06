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
		
		public T GetOptionValue<T>(string name)
		{
			object o = null;
			
			switch (name) {
				case "SharedMemorySize":
					o = this.slSharedMemorySize.Value;
					break;
				case "EnableDC":
					o = this.chkEnableDC.IsChecked;
					break;
				case "DoNotProfileNetInternals":
					o = this.chkDoNotProfileNetInternals.IsChecked;
					break;
				case "CombineRecursiveFunction":
					o = this.chkCombineRecursiveCalls.IsChecked;
					break;
				case "EnableDCAtStart":
					o = this.chkEnableDCAtStartup.IsChecked;
					break;
				default:
					throw new NotSupportedException("value '" + name + "' is not supported!");
			}
			
			return (T)o;
		}

		public void SetOptionValue<T>(string name, T value)
		{
			object o = value;
			
			switch (name) {
				case "SharedMemorySize":
					this.slSharedMemorySize.Value = (int)o;
					break;
				case "EnableDC":
					this.chkEnableDC.IsChecked = (bool)o;
					break;
				case "DoNotProfileNetInternals":
					this.chkDoNotProfileNetInternals.IsChecked = (bool)o;
					break;
				case "CombineRecursiveFunction":
					this.chkCombineRecursiveCalls.IsChecked = (bool)o;
					break;
				case "EnableDCAtStart":
					this.chkEnableDCAtStartup.IsChecked = (bool)o;
					break;
				default:
					throw new NotSupportedException("value '" + name + "' is not supported!");
			}
		}
	}
}