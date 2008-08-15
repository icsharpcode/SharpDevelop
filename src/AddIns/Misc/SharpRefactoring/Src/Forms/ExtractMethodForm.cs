/*
 * Erstellt mit SharpDevelop.
 * Benutzer: HP
 * Datum: 12.11.2007
 * Zeit: 18:46
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;

namespace SharpRefactoring.Forms
{
	/// <summary>
	/// Description of ExtractMethodForm.
	/// </summary>
	public partial class ExtractMethodForm : Form
	{
		public ExtractMethodForm(string name, string preview)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

            this.txtName.Text = name;
            this.txtPreview.Text = preview;
			
            txtName_TextChanged(null, EventArgs.Empty);
            
            this.txtName.SelectAll();
		}

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Text = this.txtName.Text;
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            string text = this.txtPreview.Text;

            if (string.IsNullOrEmpty(text))
                return;

            string afterName = text.Substring(text.IndexOf('('));
            
            string type = text.Split(' ')[0];

            this.txtPreview.Text = type + " " + this.txtName.Text + afterName;
        }
	}
}
