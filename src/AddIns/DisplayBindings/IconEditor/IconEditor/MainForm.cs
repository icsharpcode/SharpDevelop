// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace IconEditor
{
	public partial class MainForm
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			//ScanIcons(new DirectoryInfo(@"d:\net\icons"));
			Application.Run(new MainForm());
		}
		
		// scan for invalid or special icons
		static void ScanIcons(DirectoryInfo dir)
		{
			foreach (DirectoryInfo subdir in dir.GetDirectories()) {
				ScanIcons(subdir);
			}
			foreach (FileInfo file in dir.GetFiles("*.ico")) {
				try {
					IconFile f = new IconFile(file.OpenRead());
					if (f.AvailableColorDepths.Contains(24)) {
						Debug.WriteLine(file.FullName + " - 24");
					}
				} catch (InvalidIconException ex) {
					Debug.WriteLine(file.FullName + " - " + ex.Message);
				}
			}
		}
		
		static void Save(string fileName, Stream data)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
				byte[] buffer = new byte[4096];
				int c;
				do {
					c = data.Read(buffer, 0, buffer.Length);
					if (c != 0)
						fs.Write(buffer, 0, c);
				} while (c != 0);
			}
		}
		
		EditorPanel pnl = new EditorPanel();
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			pnl.Dock = DockStyle.Fill;
			Controls.Add(pnl);
			Controls.SetChildIndex(pnl, 0);
			
			//IconFile f = new IconFile(@"D:\net\icons\WinXP\Internet.ico");
			//IconFile f = new IconFile(@"c:\temp\example-vista-icon-2.ico");
			//IconFile f = new IconFile(@"D:\NET\Icons\exit.ico");
			//pnl.ShowFile(f);
		}
		
		void HelpToolStripButtonClick(object sender, System.EventArgs e)
		{
			MessageBox.Show(@"SharpDevelop IconEditor:
(C) Daniel Grunwald, 2006

Contact me: daniel@danielgrunwald.de", "IconEditor");
		}
		
		string fileName;
		
		void NewToolStripButtonClick(object sender, System.EventArgs e)
		{
			fileName = null;
			pnl.ShowFile(new IconFile());
		}
		
		void OpenToolStripButtonClick(object sender, System.EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK) {
				try {
					pnl.ShowFile(new IconFile(openFileDialog.FileName));
					fileName = openFileDialog.FileName;
				} catch (InvalidIconException ex) {
					MessageBox.Show("Invalid icon file: " + ex.Message);
				} catch (IOException ex) {
					MessageBox.Show("Error opening icon file: " + ex.Message);
				}
			}
		}
		
		void SaveToolStripButtonClick(object sender, EventArgs e)
		{
			MessageBox.Show("not implemented");
			//pnl.SaveIcon(@"c:\temp\save.ico");
		}
	}
}
