// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public enum LineTerminatorStyle {
		Windows,
		Macintosh,
		Unix
	}
	
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class LoadSavePanel : AbstractOptionPanel
	{
		const string loadUserDataCheckBox        = "loadUserDataCheckBox";
		const string createBackupCopyCheckBox    = "createBackupCopyCheckBox";
		const string lineTerminatorStyleComboBox = "lineTerminatorStyleComboBox";
		
		CheckBox autoLoadExternalChangesCheckBox, detectExternalChangesCheckBox, useRecycleBinCheckBox;
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.LoadSaveOptionPanel.xfrm"));
			
			((CheckBox)ControlDictionary[loadUserDataCheckBox]).Checked     = PropertyService.Get("SharpDevelop.LoadDocumentProperties", true);
			((CheckBox)ControlDictionary[createBackupCopyCheckBox]).Checked = FileService.SaveUsingTemporaryFile;
			
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.WindowsRadioButton}"));
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.MacintoshRadioButton}"));
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.LoadSaveOptions.UnixRadioButton}"));
			
			((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).SelectedIndex = (int)(LineTerminatorStyle)PropertyService.Get("SharpDevelop.LineTerminatorStyle", LineTerminatorStyle.Windows);
			
			autoLoadExternalChangesCheckBox = Get<CheckBox>("autoLoadExternalChanges");
			detectExternalChangesCheckBox = Get<CheckBox>("detectExternalChanges");
			useRecycleBinCheckBox = Get<CheckBox>("useRecycleBin");
			
			detectExternalChangesCheckBox.CheckedChanged += delegate {
				autoLoadExternalChangesCheckBox.Enabled = detectExternalChangesCheckBox.Checked;
			};
			autoLoadExternalChangesCheckBox.Enabled = detectExternalChangesCheckBox.Checked;
			
			detectExternalChangesCheckBox.Checked = FileChangeWatcher.DetectExternalChangesOption;
			autoLoadExternalChangesCheckBox.Checked = FileChangeWatcher.AutoLoadExternalChangesOption;
			useRecycleBinCheckBox.Checked = FileService.DeleteToRecycleBin;
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.Set("SharpDevelop.LoadDocumentProperties", ((CheckBox)ControlDictionary[loadUserDataCheckBox]).Checked);
			PropertyService.Set("SharpDevelop.LineTerminatorStyle",    (LineTerminatorStyle)((ComboBox)ControlDictionary[lineTerminatorStyleComboBox]).SelectedIndex);
			
			FileChangeWatcher.DetectExternalChangesOption = detectExternalChangesCheckBox.Checked;
			FileChangeWatcher.AutoLoadExternalChangesOption = autoLoadExternalChangesCheckBox.Checked;
			FileService.DeleteToRecycleBin = useRecycleBinCheckBox.Checked;
			FileService.SaveUsingTemporaryFile = ((CheckBox)ControlDictionary[createBackupCopyCheckBox]).Checked;
			
			return true;
		}
	}
}
