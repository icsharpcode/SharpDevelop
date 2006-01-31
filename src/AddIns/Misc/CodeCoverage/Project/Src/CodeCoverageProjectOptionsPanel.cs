// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageProjectOptionsPanel : AbstractProjectOptionPanel
	{
		static readonly string AssemblyListTextBoxName = "assemblyListTextBox";
		
		TextBox assemblyListTextBox;
		
		public CodeCoverageProjectOptionsPanel()
		{
		}
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.CodeCoverage.Resources.CodeCoverageProjectOptionsPanel.xfrm"));
			InitializeHelper();
			
			assemblyListTextBox = (TextBox)ControlDictionary[AssemblyListTextBoxName];
			
			ReadNCoverSettings();
			
			assemblyListTextBox.TextChanged += AssemblyListTextBoxTextChanged;
		}
		
		public override bool StorePanelContents()
		{
			SaveNCoverSettings();
			IsDirty = false;
			return true;
		}
		
		void AssemblyListTextBoxTextChanged(object sender, EventArgs e)
		{
			IsDirty = true;
		}
		
		void SaveNCoverSettings()
		{
			NCoverSettings settings = new NCoverSettings();
			settings.AssemblyList = assemblyListTextBox.Text;
			settings.Save(NCoverSettings.GetFileName(project));
		}
		
		void ReadNCoverSettings()
		{
			string settingsFileName = NCoverSettings.GetFileName(project);
			if (File.Exists(settingsFileName)) {
				NCoverSettings settings = new NCoverSettings(settingsFileName);
				assemblyListTextBox.Text = settings.AssemblyList;
			}
		}
	}
}
