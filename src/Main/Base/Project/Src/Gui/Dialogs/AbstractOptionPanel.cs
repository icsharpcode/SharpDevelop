// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class AbstractOptionPanel : BaseSharpDevelopUserControl, IDialogPanel
	{
		bool   wasActivated = false;
		bool   isFinished   = true;
		object customizationObject = null;
		
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
		
		public virtual object CustomizationObject {
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
		
		protected void ConnectBrowseButton(string browseButton, string target)
		{
			ConnectBrowseButton(browseButton, target, "${res:SharpDevelop.FileFilter.AllFiles}|*.*");
		}
		protected void ConnectBrowseButton(string browseButton, string target, string fileFilter)
		{
			if (ControlDictionary[browseButton] == null) {
				
				MessageService.ShowError(browseButton + " not found!");
				return;
			}
			if (ControlDictionary[target] == null) {
				
				MessageService.ShowError(target + " not found!");
				return;
			}
			ControlDictionary[browseButton].Click += new EventHandler(new BrowseButtonEvent(this, target, fileFilter).Event);
		}
		
		protected void ConnectBrowseFolder(string browseButton, string target)
		{
			ConnectBrowseFolder(browseButton, target, "${res:Dialog.ProjectOptions.SelectFolderTitle}");
		}
		protected void ConnectBrowseFolder(string browseButton, string target, string description)
		{
			if (ControlDictionary[browseButton] == null) {
				MessageService.ShowError(browseButton + " not found!");
				return;
			}
			if (ControlDictionary[target] == null) {
				MessageService.ShowError(target + " not found!");
				return;
			}
			
			ControlDictionary[browseButton].Click += new EventHandler(new BrowseFolderEvent(this, target, description).Event);
		}
		
		protected class BrowseButtonEvent
		{
			AbstractOptionPanel panel;
			string target;
			string filter;
			
			public BrowseButtonEvent(AbstractOptionPanel panel, string target, string filter)
			{
				this.panel  = panel;
				this.filter = filter;
				this.target = target;
			}
			
			public void Event(object sender, EventArgs e)
			{
				using (OpenFileDialog fdiag = new OpenFileDialog()) {
					fdiag.Filter      = StringParser.Parse(filter);
					fdiag.Multiselect = false;
					
					if(fdiag.ShowDialog() == DialogResult.OK) {
						string file = fdiag.FileName;
						if (panel.baseDirectory != null) {
							file = FileUtility.GetRelativePath(panel.baseDirectory, file);
						}
						panel.ControlDictionary[target].Text = file;
					}
				}
			}
		}
		
		class BrowseFolderEvent
		{
			AbstractOptionPanel panel;
			string target;
			string description;
			
			public BrowseFolderEvent(AbstractOptionPanel panel, string target, string description)
			{
				this.panel  = panel;
				this.description = description;
				this.target = target;
			}
			
			public void Event(object sender, EventArgs e)
			{
				string startLocation = panel.baseDirectory;
				if (startLocation != null) {
					startLocation = FileUtility.GetAbsolutePath(startLocation, panel.ControlDictionary[target].Text);
				}
				
				using (FolderBrowserDialog fdiag = FileService.CreateFolderBrowserDialog(description, startLocation)) {
					if (fdiag.ShowDialog() == DialogResult.OK) {
						string path = fdiag.SelectedPath;
						if (panel.baseDirectory != null) {
							path = FileUtility.GetRelativePath(panel.baseDirectory, path);
						}
						if (!path.EndsWith("\\") && !path.EndsWith("/"))
							path += "\\";
						panel.ControlDictionary[target].Text = path;
					}
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
