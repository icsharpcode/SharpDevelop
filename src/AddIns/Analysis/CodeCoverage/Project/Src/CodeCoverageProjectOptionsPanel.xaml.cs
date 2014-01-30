// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
