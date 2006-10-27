// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.Controls;

namespace NoGoop.ObjBrowser.Panels
{
	internal class CustDirPanel : Panel, ICustPanel
	{
		protected TextBox           _appDir;
		protected TextBox           _comDir;

		Size ICustPanel.PreferredSize {
			get {
				return new Size(500, 250);
			}
		} 

		internal CustDirPanel()
		{
			Label l;

			Text = StringParser.Parse("${res:ComponentInspector.CustDirectoriesPanel.Title}");
			Width = 400;

			// Padding
			l = new Label();
			l.Dock = DockStyle.Top;
			l.Height = 5;
			Controls.Add(l);

			_appDir = BuildDirPanel(this, StringParser.Parse("${res:ComponentInspector.CustDirectoriesPanel.ApplicationWorkingFolder}"));
			_comDir = BuildDirPanel(this,  StringParser.Parse("${res:ComponentInspector.CustDirectoriesPanel.ConvertedAssembliesFolder}"));
		}

		protected TextBox BuildDirPanel(Control parent, String name)
		{
			Panel panel = new Panel();
			panel.Dock = DockStyle.Top;
			panel.Height = 60;

			Label l = new Label();
			l.Location = new Point(0, 0);
			l.Dock = DockStyle.Left;
			l.Text = name;
			l.AutoSize = true;
			panel.Controls.Add(l);

			TextBox textBox = new TextBox();
			textBox.Location = new Point(10, 20);
			textBox.Width = ((ICustPanel)this).PreferredSize.Width - 40;
			panel.Controls.Add(textBox);
			
			// Don't have a reasonable directory browser and don't feel
			// like doing the P/Invoke for the underlying one.  Sigh.
			/*****
			Button b = new Button();
			b.Location = new Point(120, 20);
			b.Width = 20;
			b.Text = "...";
			b.Tag = textBox;
			b.Click += new EventHandler(DirButtonClicked);
			panel.Controls.Add(b);
			*****/
			
			parent.Controls.Add(panel);
			
			return textBox;
		}

		public void BeforeShow()
		{
			_appDir.Text = ComponentInspectorProperties.ApplicationWorkingDirectory;
			if (_comDir != null) {
				_comDir.Text = ComponentInspectorProperties.ConvertedAssemblyDirectory;
			}
		}

		protected bool CreateDirectory(String dirText)
		{
			try {
				Directory.CreateDirectory(dirText);
			} catch (Exception ex) {
				ErrorDialog.Show(ex,
								 "Error creating directory " + dirText,
								MessageBoxIcon.Error);
				return false;
			}
			return true;
		}

		public bool AfterShow()
		{
			if (!CreateDirectory(_appDir.Text)) {
				_appDir.Text = ComponentInspectorProperties.ApplicationWorkingDirectory;
				return false;
			}

			if (_comDir != null && !CreateDirectory(_comDir.Text)) {
				_comDir.Text = ComponentInspectorProperties.ConvertedAssemblyDirectory;
				return false;
			}

			ComponentInspectorProperties.ApplicationWorkingDirectory = _appDir.Text;
			if (_comDir != null) {
				ComponentInspectorProperties.ConvertedAssemblyDirectory = _comDir.Text;
			}
			return true;
		}
	}
}
