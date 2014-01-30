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
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	#pragma warning disable 618
	public class AbstractOptionPanel : BaseSharpDevelopUserControl, IDialogPanel
	{
		bool   wasActivated = false;
		bool   isFinished   = true;
		object customizationObject;
		
		public Control Control {
			get {
				return this;
			}
		}
		
		public bool WasActivated {
			get {
				return wasActivated;
			}
		}
		
		public object CustomizationObject {
			get {
				return customizationObject;
			}
			set {
				customizationObject = value;
				OnCustomizationObjectChanged();
			}
		}
		
		public virtual bool EnableFinish {
			get {
				return isFinished;
			}
			set {
				if (isFinished != value) {
					isFinished = value;
					OnEnableFinishChanged();
				}
			}
		}
		
		
//		public AbstractOptionPanel(string fileName) : base(fileName)
//		{
//		}
		
		public AbstractOptionPanel()
		{
		}
		
		
		public virtual bool ReceiveDialogMessage(DialogMessage message)
		{
			switch (message) {
				case DialogMessage.Activated:
					if (!wasActivated) {
						LoadPanelContents();
						wasActivated = true;
					}
					break;
				case DialogMessage.OK:
					if (wasActivated) {
						return StorePanelContents();
					}
					break;
			}
			
			return true;
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
			AbstractOptionPanel panel;
			Control target;
			string filter;
			TextBoxEditMode textBoxEditMode;
			
			public BrowseButtonEvent(AbstractOptionPanel panel, Control target, string filter, TextBoxEditMode textBoxEditMode)
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
			AbstractOptionPanel panel;
			string target;
			string description;
			TextBoxEditMode textBoxEditMode;
			
			internal BrowseFolderEvent(AbstractOptionPanel panel, string target, string description, TextBoxEditMode textBoxEditMode)
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
				
				string path = SD.FileService.BrowseForFolder(description, startLocation);
				if (panel.baseDirectory != null) {
					path = FileUtility.GetRelativePath(panel.baseDirectory, path);
				}
				if (!path.EndsWith("\\", StringComparison.Ordinal) && !path.EndsWith("/", StringComparison.Ordinal))
					path += "\\";
				if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty) {
					panel.ControlDictionary[target].Text = path;
				} else {
					panel.ControlDictionary[target].Text = MSBuildInternals.Escape(path);
				}
			}
		}
		
		
		protected virtual void OnEnableFinishChanged()
		{
			if (EnableFinishChanged != null) {
				EnableFinishChanged(this, null);
			}
		}
		protected virtual void OnCustomizationObjectChanged()
		{
			if (CustomizationObjectChanged != null) {
				CustomizationObjectChanged(this, null);
			}
		}
		
		public event EventHandler CustomizationObjectChanged;
		public event EventHandler EnableFinishChanged;
	}
}
