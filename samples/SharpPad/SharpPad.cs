using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SharpPad
{
	public partial class SharpPad
	{
		const string SharpPadFileFilter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*";
		
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new SharpPad());
		}
		
		public SharpPad()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
		}
		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			Close();
		}
		
		void OpenToolStripMenuItemClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog()) {
				dialog.Filter = SharpPadFileFilter;
				dialog.FilterIndex = 0;
				if (DialogResult.OK == dialog.ShowDialog()) {
					textEditorControl.LoadFile(dialog.FileName);
				}
			}
		}
		
		void SaveAsToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			SaveAs();
		}
		
		void SaveAs()
		{
			using (SaveFileDialog dialog = new SaveFileDialog()) {
				dialog.Filter = SharpPadFileFilter;
				dialog.FilterIndex = 0;
				if (DialogResult.OK == dialog.ShowDialog()) {
					textEditorControl.SaveFile(dialog.FileName);
					textEditorControl.FileName = dialog.FileName;
				}
			}
		}
		
		void SaveToolStripMenuItemClick(object sender, System.EventArgs e)
		{
			if (textEditorControl.FileName != null) {
				textEditorControl.SaveFile(textEditorControl.FileName);
			} else {
				SaveAs();
			}
		}
	}
}
