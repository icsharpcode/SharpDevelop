// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows;
using AvalonDock;
using System.Windows.Media.Imaging;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Workbench layout using the AvalonDock library.
	/// </summary>
	sealed class AvalonDockLayout : IWorkbenchLayout
	{
		WpfWorkbench workbench;
		DockingManager dockingManager = new DockingManager();
		DocumentPane documentPane = new DocumentPane();
		
		public DockingManager DockingManager {
			get { return dockingManager; }
		}
		
		public DocumentPane DocumentPane {
			get { return documentPane; }
		}
		
		public AvalonDockLayout()
		{
			dockingManager.Content = documentPane;
		}
		
		public event EventHandler ActiveWorkbenchWindowChanged;
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				return null;
			}
		}
		
		public object ActiveContent {
			get {
				return null;
			}
		}
		
		public void Attach(IWorkbench workbench)
		{
			this.workbench = (WpfWorkbench)workbench;
			this.workbench.mainContent.Content = dockingManager;
		}
		
		public void Detach()
		{
			this.workbench.mainContent.Content = null;
		}
		
		public void ShowPad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public void ActivatePad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public void ActivatePad(string fullyQualifiedTypeName)
		{
			throw new NotImplementedException();
		}
		
		public void HidePad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public void UnloadPad(PadDescriptor content)
		{
			throw new NotImplementedException();
		}
		
		public bool IsVisible(PadDescriptor padContent)
		{
			throw new NotImplementedException();
		}
		
		public void RedrawAllComponents()
		{
		}
		
		public IWorkbenchWindow ShowView(IViewContent content)
		{
			AvalonWorkbenchWindow window = new AvalonWorkbenchWindow(this);
			window.ViewContents.Add(content);
			documentPane.Items.Add(window);
			dockingManager.Show(window);
			return window;
		}
		
		public void LoadConfiguration()
		{
			try {
				if (File.Exists(LayoutConfiguration.CurrentLayoutFileName)) {
					dockingManager.RestoreLayout(LayoutConfiguration.CurrentLayoutFileName);
				} else {
					LoadDefaultLayoutConfiguration();
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
				// ignore errors loading configuration
			}
		}
		
		void LoadDefaultLayoutConfiguration()
		{
			if (File.Exists(LayoutConfiguration.CurrentLayoutTemplateFileName)) {
				dockingManager.RestoreLayout(LayoutConfiguration.CurrentLayoutTemplateFileName);
			}
		}
		
		public void StoreConfiguration()
		{
			try {
				LayoutConfiguration current = LayoutConfiguration.CurrentLayout;
				if (current != null && !current.ReadOnly) {
					string configPath = Path.Combine(PropertyService.ConfigDirectory, "layouts");
					Directory.CreateDirectory(configPath);
					dockingManager.SaveLayout(Path.Combine(configPath, current.FileName));
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
	}
}
