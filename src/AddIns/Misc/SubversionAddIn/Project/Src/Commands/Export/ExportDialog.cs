using System;
using System.Reflection;
using System.Drawing;
using System.Text;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using NSvn.Common;
using NSvn.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.Svn.Commands
{
	/// <summary>
	/// Description of Form1.
	/// </summary>
	public class ExportDialog : BaseSharpDevelopForm
	{
		
		public Revision Revision {
			get {
				if (ControlDictionary["revisionComboBox"].Text == "Date") {
					return Revision.FromDate(((DateTimePicker)ControlDictionary["dateTimePicker"]).Value);
				}
				return Revision.Parse(ControlDictionary["revisionComboBox"].Text);
			}
		}
		
		public bool SourceIsLocalDirectory {
			get {
				return ((RadioButton)ControlDictionary["fromLocalDirRadioButton"]).Checked;
			}
		}
		
		public string Source {
			get {
				return SourceIsLocalDirectory ? ControlDictionary["sourceDirectoryTextBox"].Text : ControlDictionary["urlTextBox"].Text;
			}
		}
		
		public string Destination {
			get {
				return ControlDictionary["localDirectoryTextBox"].Text;
			}
		}
		
		public bool NonRecursive {
			get {
				return ((CheckBox)ControlDictionary["nonRecursiveCheckBox"]).Checked;
			}
		}
		
		public ExportDialog()
		{
			SetupFromXmlStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("ICSharpCode.Svn.Resources.ExportDialog.xfrm"));
			((ComboBox)ControlDictionary["revisionComboBox"]).Items.AddRange(new string[] {
				"Head",
				"Committed",
				"Base",
				"Previous",
				"Working",
				"Date"
			});
			((ComboBox)ControlDictionary["revisionComboBox"]).Text = "Head";
			((ComboBox)ControlDictionary["revisionComboBox"]).TextChanged += new EventHandler(RevisionComboBoxTextChanged);
			RevisionComboBoxTextChanged(this, EventArgs.Empty);
			
			Get<RadioButton>("fromLocalDir").Checked = true;
			Get<RadioButton>("fromLocalDir").CheckedChanged += new EventHandler(FromLocalDirRadioButtonCheckedChanged);
			FromLocalDirRadioButtonCheckedChanged(this, EventArgs.Empty);
			
			ControlDictionary["sourceDirectoryBrowseButton"].Click += new EventHandler(SourceDirectoryBrowseButtonClick);
			ControlDictionary["localDirectoryBrowseButton"].Click  += new EventHandler(LocalDirectoryBrowseButtonClick);
		}
		
		void RevisionComboBoxTextChanged(object sender, EventArgs e)
		{
			ControlDictionary["dateTimePicker"].Enabled = ControlDictionary["revisionComboBox"].Text == "Date";
		}
		
		void FromLocalDirRadioButtonCheckedChanged(object sender, EventArgs e)
		{
			ControlDictionary["urlTextBox"].Visible = !SourceIsLocalDirectory;
			ControlDictionary["sourceDirectoryTextBox"].Visible = ControlDictionary["sourceDirectoryBrowseButton"].Visible = SourceIsLocalDirectory;
			
			if (ControlDictionary["urlTextBox"].Visible) {
				ControlDictionary["urlLabel"].Text = "&URL:";
			} else {
				ControlDictionary["urlLabel"].Text = "&Source directory:";
			}
		}
		
		void SourceDirectoryBrowseButtonClick(object sender, EventArgs e)
		{
			FolderDialog fdiag = new FolderDialog();
			
			if (fdiag.DisplayDialog("Select source directory.") == DialogResult.OK) {
				ControlDictionary["sourceDirectoryTextBox"].Text = fdiag.Path;
			}
		}
		
		void LocalDirectoryBrowseButtonClick(object sender, EventArgs e)
		{
			FolderDialog fdiag = new FolderDialog();
			
			if (fdiag.DisplayDialog("Select destination directory.") == DialogResult.OK) {
				ControlDictionary["localDirectoryTextBox"].Text = fdiag.Path;
			}
		}
	}
}
