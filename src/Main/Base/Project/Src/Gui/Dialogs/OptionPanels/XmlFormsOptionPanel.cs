// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	[Obsolete("XML Forms are obsolete")]
	public class XmlFormsOptionPanel : BaseSharpDevelopUserControl, IOptionPanel
	{
		public object Owner { get; set; }
		
		public object Control {
			get {
				return this;
			}
		}
		
		bool wasActivated;
		
		void IOptionPanel.LoadOptions()
		{
			if (!wasActivated) {
				wasActivated = true;
				LoadPanelContents();
			}
		}
		
		bool IOptionPanel.SaveOptions()
		{
			return StorePanelContents();
		}
		
		public virtual void LoadPanelContents()
		{
			
		}
		
		public virtual bool StorePanelContents()
		{
			return true;
		}
		
		protected string baseDirectory;
		
		protected void ConnectBrowseButton(string browseButton, string target, string fileFilter, TextBoxEditMode textBoxEditMode)
		{
			if (ControlDictionary[browseButton] == null) {
				
				MessageService.ShowError(browseButton + " not found!");
				return;
			}
			if (ControlDictionary[target] == null) {
				MessageService.ShowError(target + " not found!");
				return;
			}
			ControlDictionary[browseButton].Click += new EventHandler(new BrowseButtonEvent(this, ControlDictionary[target], fileFilter, textBoxEditMode).Event);
		}
		
		protected void ConnectBrowseFolder(string browseButton, string target, TextBoxEditMode textBoxEditMode)
		{
			ConnectBrowseFolder(browseButton, target, "${res:Dialog.ProjectOptions.SelectFolderTitle}", textBoxEditMode);
		}
		protected void ConnectBrowseFolder(string browseButton, string target, string description, TextBoxEditMode textBoxEditMode)
		{
			if (ControlDictionary[browseButton] == null) {
				MessageService.ShowError(browseButton + " not found!");
				return;
			}
			if (ControlDictionary[target] == null) {
				MessageService.ShowError(target + " not found!");
				return;
			}
			
			ControlDictionary[browseButton].Click += new EventHandler(new BrowseFolderEvent(this, target, description, textBoxEditMode).Event);
		}
		protected void BrowseForFile(Control target, string filter, TextBoxEditMode textBoxEditMode)
		{
			if (target == null) {
				throw new ArgumentNullException("target");
			}
			new BrowseButtonEvent(this, target, filter, textBoxEditMode).Event(null, null);
		}
		
		sealed class BrowseButtonEvent
		{
			XmlFormsOptionPanel panel;
			Control target;
			string filter;
			TextBoxEditMode textBoxEditMode;
			
			public BrowseButtonEvent(XmlFormsOptionPanel panel, Control target, string filter, TextBoxEditMode textBoxEditMode)
			{
				this.panel  = panel;
				this.filter = filter;
				this.target = target;
				this.textBoxEditMode = textBoxEditMode;
			}
			
			public void Event(object sender, EventArgs e)
			{
				using (OpenFileDialog fdiag = new OpenFileDialog()) {
					fdiag.Filter      = StringParser.Parse(filter);
					fdiag.Multiselect = false;
					try {
						string initialDir = System.IO.Path.GetDirectoryName(System.IO.Path.Combine(panel.baseDirectory, target.Text));
						if (FileUtility.IsValidPath(initialDir) && System.IO.Directory.Exists(initialDir)) {
							fdiag.InitialDirectory = initialDir;
						}
					} catch {}
					if(fdiag.ShowDialog() == DialogResult.OK) {
						string file = fdiag.FileName;
						if (panel.baseDirectory != null) {
							file = FileUtility.GetRelativePath(panel.baseDirectory, file);
						}
						if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
							target.Text = file;
						} else {
							target.Text = MSBuildInternals.Escape(file);
						}
					}
				}
			}
		}
		
		sealed class BrowseFolderEvent
		{
			XmlFormsOptionPanel panel;
			string target;
			string description;
			TextBoxEditMode textBoxEditMode;
			
			internal BrowseFolderEvent(XmlFormsOptionPanel panel, string target, string description, TextBoxEditMode textBoxEditMode)
			{
				this.panel  = panel;
				this.description = description;
				this.target = target;
				this.textBoxEditMode = textBoxEditMode;
			}
			
			public void Event(object sender, EventArgs e)
			{
				string startLocation = panel.baseDirectory;
				if (startLocation != null) {
					string text = panel.ControlDictionary[target].Text;
					if (textBoxEditMode == TextBoxEditMode.EditRawProperty)
						text = MSBuildInternals.Unescape(text);
					startLocation = FileUtility.GetAbsolutePath(startLocation, text);
				}
				
				using (FolderBrowserDialog fdiag = FileService.CreateFolderBrowserDialog(description, startLocation)) {
					if (fdiag.ShowDialog() == DialogResult.OK) {
						string path = fdiag.SelectedPath;
						if (panel.baseDirectory != null) {
							path = FileUtility.GetRelativePath(panel.baseDirectory, path);
						}
						if (!path.EndsWith("\\") && !path.EndsWith("/"))
							path += "\\";
						if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
							panel.ControlDictionary[target].Text = path;
						} else {
							panel.ControlDictionary[target].Text = MSBuildInternals.Escape(path);
						}
					}
				}
			}
		}
	}
}
