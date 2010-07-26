/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier
 * Datum: 09.04.2007
 * Zeit: 17:01
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.Reports.Core
{
	/// <summary>
	/// Description of EditorDialog.
	/// </summary>
	/// 
	public interface ITextEditorDialog
	{
		DialogResult ShowDialog();
		string TextValue {get;}
	}
	
	
	
	public partial class TextEditorDialog : Form,ITextEditorDialog
	{
		string textValue;
	
		
		private TextEditorDialog()
		{
			InitializeComponent();
		}
		
		
		public TextEditorDialog(string text,string header):this()
		{
			this.textValue = text;
			this.textBox1.Text = this.textValue;
			if (String.IsNullOrEmpty (header)) {
				this.Text = this.Name;
			} else {
				this.Text = header;
			}
		}
		
		
		public string TextValue {
			get { 
				return textValue.Trim();
			}
		}
		
		
		void OkButtonClick(object sender, EventArgs e)
		{
			this.textValue = this.textBox1.Text;
		}
		
	}
}
