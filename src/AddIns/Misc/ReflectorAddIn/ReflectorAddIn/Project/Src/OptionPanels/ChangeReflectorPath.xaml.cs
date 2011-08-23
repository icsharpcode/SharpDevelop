
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ReflectorAddIn.OptionPanels
{
	public partial class ChangeReflectorPath : OptionPanel
	{
		public ChangeReflectorPath()
		{
			InitializeComponent();
			
			Loaded += delegate { ShowStatus(); };
		}
		
		void ShowStatus()
		{
			string path = PropertyService.Get(ReflectorSetupHelper.ReflectorExePathPropertyName);
			
			if (string.IsNullOrEmpty(path)) {
				StatusLabel.Text = StringParser.Parse("${res:ReflectorAddIn.ReflectorPathNotSet}");
			} else {
				StatusLabel.Text = StringParser.Parse("${res:ReflectorAddIn.IdeOptions.ReflectorFoundInPath}")
					+ Environment.NewLine + path;
			}
		}
		
		void FindReflectorPathClick(object sender, EventArgs e)
		{
			ReflectorSetupHelper.OpenReflectorExeFullPathInteractiver();
			ShowStatus();
		}
	}
}