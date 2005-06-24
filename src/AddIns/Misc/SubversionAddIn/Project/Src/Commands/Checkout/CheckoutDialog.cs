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
	public class CheckoutDialog : BaseSharpDevelopForm
	{
		
		public Revision Revision {
			get {
				if (ControlDictionary["revisionComboBox"].Text == "Date") {
					return Revision.FromDate(((DateTimePicker)ControlDictionary["dateTimePicker"]).Value);
				}
				return Revision.Parse(ControlDictionary["revisionComboBox"].Text);
			}
		}
		
		public string Source {
			get {
				return ControlDictionary["urlTextBox"].Text;
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
		
		public CheckoutDialog()
		{
			SetupFromXmlStream(Assembly.GetCallingAssembly().GetManifestResourceStream("ExportDialog.xfrm"));
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
			
			ControlDictionary["localDirectoryBrowseButton"].Click  += new EventHandler(LocalDirectoryBrowseButtonClick);
		}
		
		void RevisionComboBoxTextChanged(object sender, EventArgs e)
		{
			ControlDictionary["dateTimePicker"].Enabled = ControlDictionary["revisionComboBox"].Text == "Date";
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
