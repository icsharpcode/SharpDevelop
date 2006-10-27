// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui.OptionPanels;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageProjectOptionsPanel : AbstractProjectOptionPanel
	{
		static readonly string AssemblyListTextBoxName = "assemblyListTextBox";
		static readonly string ExcludedAttributesListTextBoxName = "excludedAttributesTextBox";
		
		TextBox assemblyListTextBox;
		TextBox excludedAttributesListTextBox;
		
		public CodeCoverageProjectOptionsPanel()
		{
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.CodeCoverage.Resources.CodeCoverageProjectOptionsPanel.xfrm"));
			InitializeHelper();
			
			assemblyListTextBox = (TextBox)ControlDictionary[AssemblyListTextBoxName];
			excludedAttributesListTextBox = (TextBox)ControlDictionary[ExcludedAttributesListTextBoxName];
			
			ReadNCoverSettings();
			
			assemblyListTextBox.TextChanged += TextBoxTextChanged;
			excludedAttributesListTextBox.TextChanged += TextBoxTextChanged;
		}
		
		public override bool StorePanelContents()
		{
			SaveNCoverSettings();
			IsDirty = false;
			return true;
		}
		
		void TextBoxTextChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}
		
		void SaveNCoverSettings()
		{
			NCoverSettings settings = new NCoverSettings();
			settings.AssemblyList = assemblyListTextBox.Text;
			settings.ExcludedAttributesList = excludedAttributesListTextBox.Text;
			settings.Save(NCoverSettings.GetFileName(project));
		}
		
		void ReadNCoverSettings()
		{
			string settingsFileName = NCoverSettings.GetFileName(project);
			if (File.Exists(settingsFileName)) {
				NCoverSettings settings = new NCoverSettings(settingsFileName);
				assemblyListTextBox.Text = settings.AssemblyList;
				excludedAttributesListTextBox.Text = settings.ExcludedAttributesList;
			}
		}
	}
}
