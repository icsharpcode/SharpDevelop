/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 09.06.2012
 * Time: 15:01
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows.Controls;

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Interaction logic for CodeCoverageOptionsPanelXaml.xaml
	/// </summary>
	public partial class CodeCoverageProjectOptionsPanel :  ProjectOptionPanel
	{
		public CodeCoverageProjectOptionsPanel()
		{
			InitializeComponent();
		}
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			ReadPartCoverSettings();
			base.Load(project, configuration, platform);
		}
		
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			SavePartCoverSettings();
			return base.Save(project, configuration, platform);
		}
		
		
		void ReadPartCoverSettings()
		{
			string settingsFileName = OpenCoverSettings.GetFileName(base.Project);
			if (File.Exists(settingsFileName)) {
				OpenCoverSettings settings = new OpenCoverSettings(settingsFileName);
				includeListTextBox.Text = ConvertToMultLineString(settings.Include);
				excludeListTextBox.Text = ConvertToMultLineString(settings.Exclude);
			}
		}
		
		
		private void SavePartCoverSettings()
		{
			OpenCoverSettings settings = new OpenCoverSettings();
			settings.Include.AddRange(MakeStringArray(includeListTextBox.Text));
			settings.Exclude.AddRange(MakeStringArray(excludeListTextBox.Text));
			settings.Save(OpenCoverSettings.GetFileName(base.Project));
		}
		
		
		private string[] MakeStringArray(string str)
		{
			return str.Split(new char[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
		}
		
		/// <summary>
		/// Each item in the string collection is added as a separate line
		/// followed by a carriage return and line feed except the last
		/// item.
		/// </summary>
		private static string ConvertToMultLineString(StringCollection items)
		{
			StringBuilder text = new StringBuilder();
			foreach (String item in items) {
				text.Append(item);
				text.Append("\r\n");
			}
			return text.ToString().Trim();
		}
		
		
		void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			IsDirty = true;
		}
	}
}