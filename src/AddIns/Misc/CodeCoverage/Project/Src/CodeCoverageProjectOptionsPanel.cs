// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.OptionPanels;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageProjectOptionsPanel : AbstractProjectOptionPanel
	{
		static readonly string IncludeListTextBoxName = "includeListTextBox";
		static readonly string ExcludeListTextBoxName = "excludeListTextBox";
		
		TextBox includeListTextBox;
		TextBox excludeListTextBox;
		
		public CodeCoverageProjectOptionsPanel()
		{
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.CodeCoverage.Resources.CodeCoverageProjectOptionsPanel.xfrm"));
			InitializeHelper();
			
			includeListTextBox = (TextBox)ControlDictionary[IncludeListTextBoxName];
			excludeListTextBox = (TextBox)ControlDictionary[ExcludeListTextBoxName];
			
			ReadPartCoverSettings();
			
			includeListTextBox.TextChanged += TextBoxTextChanged;
			excludeListTextBox.TextChanged += TextBoxTextChanged;
		}
		
		public override bool StorePanelContents()
		{
			SavePartCoverSettings();
			IsDirty = false;
			return true;
		}
		
		void TextBoxTextChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}
		
		void SavePartCoverSettings()
		{
			PartCoverSettings settings = new PartCoverSettings();
			settings.Include.AddRange(RemoveEmptyStrings(includeListTextBox.Lines));
			settings.Exclude.AddRange(RemoveEmptyStrings(excludeListTextBox.Lines));
			settings.Save(PartCoverSettings.GetFileName(project));
		}
		
		void ReadPartCoverSettings()
		{
			string settingsFileName = PartCoverSettings.GetFileName(project);
			if (File.Exists(settingsFileName)) {
				PartCoverSettings settings = new PartCoverSettings(settingsFileName);
				includeListTextBox.Text = ConvertToMultLineString(settings.Include);
				excludeListTextBox.Text = ConvertToMultLineString(settings.Exclude);
			}
		}

		/// <summary>
		/// Each item in the string collection is added as a separate line
		/// followed by a carriage return and line feed except the last
		/// item.
		/// </summary>
		static string ConvertToMultLineString(StringCollection items)
		{
			StringBuilder text = new StringBuilder();
			foreach (String item in items) {
				text.Append(item);
				text.Append("\r\n");
			}
			return text.ToString().Trim();
		}
		
		/// <summary>
		/// Creates a new string array but with any lines that are empty
		/// in the original lines array removed from it.
		/// </summary>
		static string[] RemoveEmptyStrings(string[] lines)
		{
			List<string> convertedLines = new List<string>();
			foreach (string line in lines) {
				if (line.Trim().Length > 0) {
					convertedLines.Add(line);
				}
			}
			return convertedLines.ToArray();
		}
	}
}
