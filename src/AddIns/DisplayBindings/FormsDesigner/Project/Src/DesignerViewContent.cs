// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner.Services;
using ICSharpCode.FormsDesigner.UndoRedo;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.Widgets.SideBar;

namespace ICSharpCode.FormsDesigner
{
	public class FormsDesignerViewContent : AbstractSecondaryViewContent, IClipboardHandler, IUndoHandler, IHasPropertyContainer, IContextHelpProvider
	{
		protected bool failedDesignerInitialize;
		
		protected IViewContent viewContent;
		protected ITextEditorControlProvider textAreaControlProvider;
		
		Panel p = new Panel();
		DesignSurface designSurface;
		bool disposing;
		
		// Switch to the Windows Forms tab by default.
		string activeTabName = "Windows Forms";
		
		IDesignerLoaderProvider loaderProvider;
		IDesignerGenerator generator;
		DesignerResourceService designerResourceService;
		FormsDesignerUndoEngine undoEngine;
		
		public override Control Control {
			get {
				return p;
			}
		}
		
		public override string TabPageText {
			get {
				return "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
			}
		}
		
		public DesignSurface DesignSurface {
			get {
				return designSurface;
			}
		}
		
		public TextEditorControl TextEditorControl {
			get {
				return textAreaControlProvider.TextEditorControl;
			}
		}
		public IDocument Document {
			get {
				return TextEditorControl.Document;
			}
		}
		
		public IDesignerHost Host {
			get {
				if (designSurface == null)
					return null;
				return (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
			}
		}
		public FormsDesignerViewContent(IViewContent viewContent, IDesignerLoaderProvider loaderProvider, IDesignerGenerator generator)
		{
			if (!FormKeyHandler.inserted) {
				FormKeyHandler.Insert();
			}
			
			this.loaderProvider    = loaderProvider;
			this.generator = generator;
			p.BackColor    = Color.White;
			p.RightToLeft = RightToLeft.No;
			// Make sure auto-scaling is based on the correct font.
			// This is required on Vista, I don't know why it works correctly in XP
			p.Font = Control.DefaultFont;
			
			this.viewContent             = viewContent;
			this.textAreaControlProvider = viewContent as ITextEditorControlProvider;
		}
		
		public override void SwitchedTo()
		{
			if (IsFormsDesignerVisible) {
				AddSideBars();
			}
		}
		
		void LoadDesigner()
		{
			LoggingService.Info("Form Designer: BEGIN INITIALIZE");
			
			DefaultServiceContainer serviceContainer = new DefaultServiceContainer();
			serviceContainer.AddService(typeof(System.Windows.Forms.Design.IUIService), new UIService());
			serviceContainer.AddService(typeof(System.Drawing.Design.IToolboxService), ToolboxProvider.ToolboxService);
			
			serviceContainer.AddService(typeof(IHelpService), new HelpService());
			serviceContainer.AddService(typeof(System.Drawing.Design.IPropertyValueUIService), new PropertyValueUIService());
			
			// Do not re-initialize the resource service on every load
			// because of SD2-1107.
			// The service holds cached resource file contents which
			// may not have been written to disk yet if the user switched
			// between source and design view without saving.
			if (designerResourceService == null) {
				designerResourceService = new DesignerResourceService(viewContent.FileName);
			}
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IResourceService), designerResourceService);
			AmbientProperties ambientProperties = new AmbientProperties();
			serviceContainer.AddService(typeof(AmbientProperties), ambientProperties);
			serviceContainer.AddService(typeof(ITypeResolutionService), new TypeResolutionService(viewContent.FileName));
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IDesignerEventService), new DesignerEventService());
			serviceContainer.AddService(typeof(DesignerOptionService), new SharpDevelopDesignerOptionService());
			serviceContainer.AddService(typeof(ITypeDiscoveryService), new TypeDiscoveryService());
			serviceContainer.AddService(typeof(MemberRelationshipService), new DefaultMemberRelationshipService());
			
			designSurface = new DesignSurface(serviceContainer);
			
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IMenuCommandService), new ICSharpCode.FormsDesigner.Services.MenuCommandService(p, designSurface));
			ICSharpCode.FormsDesigner.Services.EventBindingService eventBindingService = new ICSharpCode.FormsDesigner.Services.EventBindingService(designSurface);
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IEventBindingService), eventBindingService);
			
			designerResourceService.Host = Host;
			
			DesignerLoader designerLoader = loaderProvider.CreateLoader(generator);
			designSurface.BeginLoad(designerLoader);
			
			generator.Attach(this);
			
			undoEngine = new FormsDesignerUndoEngine(Host);
			
			IComponentChangeService componentChangeService = (IComponentChangeService)designSurface.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentChanged += delegate { viewContent.IsDirty = true; };
			componentChangeService.ComponentAdded   += ComponentListChanged;
			componentChangeService.ComponentRemoved += ComponentListChanged;
			componentChangeService.ComponentRename  += ComponentListChanged;
			this.Host.TransactionClosed += TransactionClose;
			
			ISelectionService selectionService = (ISelectionService)designSurface.GetService(typeof(ISelectionService));
			selectionService.SelectionChanged  += SelectionChangedHandler;
			
			if (IsTabOrderMode) { // fixes SD2-1015
				tabOrderMode = false; // let ShowTabOrder call the designer command again
				ShowTabOrder();
			}
			
			LoggingService.Info("Form Designer: END INITIALIZE");
		}
		
		bool shouldUpdateSelectableObjects = false;
		
		void TransactionClose(object sender, DesignerTransactionCloseEventArgs e)
		{
			if (shouldUpdateSelectableObjects) {
				// update the property pad after the transaction is *really* finished
				// (including updating the selection)
				p.BeginInvoke(new MethodInvoker(UpdatePropertyPad));
				shouldUpdateSelectableObjects = false;
			}
		}
		
		void ComponentListChanged(object sender, EventArgs e)
		{
			shouldUpdateSelectableObjects = true;
		}
		
		void UnloadDesigner()
		{
			generator.Detach();
			bool savedIsDirty = viewContent.IsDirty;
			p.Controls.Clear();
			viewContent.IsDirty = savedIsDirty;
			// We cannot dispose the design surface now because of SD2-451:
			// When the switch to the source view was triggered by a double-click on an event
			// in the PropertyPad, "InvalidOperationException: The container cannot be disposed
			// at design time" is thrown.
			// This is solved by calling dispose after the double-click event has been processed.
			if (disposing) {
				designSurface.Dispose();
			} else {
				p.BeginInvoke(new MethodInvoker(designSurface.Dispose));
			}
			designSurface = null;
		}
		
		PropertyContainer propertyContainer = new PropertyContainer();
		
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
		
		public void ShowHelp()
		{
			ISelectionService selectionService = (ISelectionService)Host.GetService(typeof(ISelectionService));
			if (selectionService != null) {
				Control ctl = selectionService.PrimarySelection as Control;
				if (ctl != null) {
					ICSharpCode.SharpDevelop.HelpProvider.ShowHelp(ctl.GetType().FullName);
				}
			}
		}
		
		public void Reload()
		{
			try {
				failedDesignerInitialize = false;
				LoadDesigner();
				
				bool savedIsDirty = viewContent.IsDirty;
				if (designSurface != null && p.Controls.Count == 0) {
					Control designer = designSurface.View as Control;
					designer.Dock = DockStyle.Fill;
					p.Controls.Add(designer);
				}
				viewContent.IsDirty = savedIsDirty;
			} catch (Exception e) {
				failedDesignerInitialize = true;
				TextBox errorText = new TextBox();
				errorText.Multiline = true;
				if (e.InnerException is FormsDesignerLoadException) {
					errorText.Text = e.InnerException.Message;
				} else if (e is FormsDesignerLoadException) {
					errorText.Text = e.Message;
				} else if (!designSurface.IsLoaded && designSurface.LoadErrors != null) {
					errorText.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.ErrorLoadingDesigner}\r\n\r\n");
					foreach(Exception le in designSurface.LoadErrors) {
						errorText.Text += le.ToString();
						errorText.Text += "\r\n";
					}
				} else {
					errorText.Text = e.ToString();
				}
				
				errorText.Dock = DockStyle.Fill;
				p.Controls.Add(errorText);
				Control title = new Label();
				title.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.FormDesigner.LoadErrorCheckSourceCodeForErrors}");
				title.Dock = DockStyle.Top;
				p.Controls.Add(title);
			}
		}
		
		public virtual void MergeFormChanges()
		{
			if (this.failedDesignerInitialize) {
				return;
			}
			bool isDirty = viewContent.IsDirty;
			LoggingService.Info("Merging form changes...");
			designSurface.Flush();
			LoggingService.Info("Finished merging form changes");
			viewContent.IsDirty = isDirty;
		}
		
		public void ShowSourceCode()
		{
			WorkbenchWindow.SwitchView(0);
		}
		
		public void ShowSourceCode(int lineNumber)
		{
			ShowSourceCode();
			textAreaControlProvider.TextEditorControl.ActiveTextAreaControl.JumpTo(lineNumber - 1);
		}
		
		public void ShowSourceCode(IComponent component, EventDescriptor edesc, string eventMethodName)
		{
			int position;
			string file;
			bool eventCreated = generator.InsertComponentEvent(component, edesc, eventMethodName, "", out file, out position);
			if (eventCreated) {
				if (FileUtility.IsEqualFileName(file, this.TextEditorControl.FileName)) {
					ShowSourceCode(position);
				} else {
					FileService.JumpToFilePosition(file, position - 1, 0);
				}
			}
		}
		
		public ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			return generator.GetCompatibleMethods(edesc);
		}
		
		public ICollection GetCompatibleMethods(EventInfo edesc)
		{
			return generator.GetCompatibleMethods(edesc);
		}
		
		public override void Selected()
		{
			PropertyPad.PropertyValueChanged += PropertyValueChanged;
			Reload();
			IsFormsDesignerVisible = true;
			AddSideBars();
			SetActiveSideTab();
			UpdatePropertyPad();
		}
		
		public override void Dispose()
		{
			disposing = true;
			if (IsFormsDesignerVisible) {
				Deselecting();
			}
			base.Dispose();
		}
		
		public override void Deselecting()
		{
			// can happen if form designer is disposed and then deselected
			if (!IsFormsDesignerVisible)
				return;
			LoggingService.Info("Deselecting form designer, unloading..." + viewContent.TitleName);
			PropertyPad.PropertyValueChanged -= PropertyValueChanged;
			propertyContainer.Clear();
			IsFormsDesignerVisible = false;
			activeTabName = String.Empty;
			if (SharpDevelopSideBar.SideBar.ActiveTab != null && ToolboxProvider.SideTabs.Contains(SharpDevelopSideBar.SideBar.ActiveTab)) {
				activeTabName = SharpDevelopSideBar.SideBar.ActiveTab.Name;
			}
			foreach(SideTab tab in ToolboxProvider.SideTabs) {
				if (!SharpDevelopSideBar.SideBar.Tabs.Contains(tab)) {
					return;
				}
				SharpDevelopSideBar.SideBar.Tabs.Remove(tab);
			}
			SharpDevelopSideBar.SideBar.Refresh();
			if (!failedDesignerInitialize) {
				MergeFormChanges();
				textAreaControlProvider.TextEditorControl.Refresh();
			}
			UnloadDesigner();
			LoggingService.Info("Unloading form designer finished");
		}
		
		public override void NotifyBeforeSave()
		{
			base.NotifyBeforeSave();
			if (IsFormsDesignerVisible)
				MergeFormChanges();
		}
		
		public override void NotifyAfterSave(bool successful)
		{
			base.NotifyAfterSave(successful);
			if (successful) {
				if (designerResourceService != null) {
					designerResourceService.Save(viewContent.FileName);
				}
			}
		}
		
		public override void NotifyFileNameChanged()
		{
			base.NotifyFileNameChanged();
			if (designerResourceService != null) {
				designerResourceService.FormFileName = viewContent.FileName;
			}
		}
		
		void SelectionChangedHandler(object sender, EventArgs args)
		{
			UpdatePropertyPadSelection((ISelectionService)sender);
		}
		
		void UpdatePropertyPadSelection(ISelectionService selectionService)
		{
			ICollection selection = selectionService.GetSelectedComponents();
			object[] selArray = new object[selection.Count];
			selection.CopyTo(selArray, 0);
			propertyContainer.SelectedObjects = selArray;
		}
		
		protected void UpdatePropertyPad()
		{
			if (IsFormsDesignerVisible) {
				propertyContainer.Host = Host;
				propertyContainer.SelectableObjects = Host.Container.Components;
				ISelectionService selectionService = (ISelectionService)Host.GetService(typeof(ISelectionService));
				if (selectionService != null) {
					UpdatePropertyPadSelection(selectionService);
				}
			}
		}
		
		public bool IsFormsDesignerVisible = false;
		
		#region IUndoHandler implementation
		public bool EnableUndo {
			get {
				if (undoEngine != null) {
					return undoEngine.EnableUndo;
				}
				return false;
			}
		}
		public bool EnableRedo {
			get {
				if (undoEngine != null) {
					return undoEngine.EnableRedo;
				}
				return false;
			}
		}
		public virtual void Undo()
		{
			if (undoEngine != null) {
				undoEngine.Undo();
			}
		}
		
		public virtual void Redo()
		{
			if (undoEngine != null) {
				undoEngine.Redo();
			}
		}
		#endregion
		
		#region IClipboardHandler implementation
		public bool EnableCut {
			get {
				//ISelectionService selectionService = (ISelectionService)designSurface.GetService(typeof(ISelectionService));
				//return selectionService.SelectionCount >= 0 && selectionService.PrimarySelection != host.RootComponent;
				IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
				System.ComponentModel.Design.MenuCommand menuCommand = menuCommandService.FindCommand(StandardCommands.Cut);
				if (menuCommand == null) {
					return false;
				}
				int status = menuCommand.OleStatus;
				return menuCommand.Enabled;
			}
		}
		
		public bool EnableCopy {
			get {
				//ISelectionService selectionService = (ISelectionService)designSurface.GetService(typeof(ISelectionService));
				//return selectionService.SelectionCount >= 0 && selectionService.PrimarySelection != host.RootComponent;
				IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
				System.ComponentModel.Design.MenuCommand menuCommand = menuCommandService.FindCommand(StandardCommands.Copy);
				if (menuCommand == null) {
					return false;
				}
				int status = menuCommand.OleStatus;
				return menuCommand.Enabled;
			}
		}
		
		const string ComponentClipboardFormat = "CF_DESIGNERCOMPONENTS";
		public bool EnablePaste {
			get {
				IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
				System.ComponentModel.Design.MenuCommand menuCommand = menuCommandService.FindCommand(StandardCommands.Paste);
				if (menuCommand == null) {
					return false;
				}
				int status = menuCommand.OleStatus;
				return menuCommand.Enabled;
			}
		}
		
		public bool EnableDelete {
			get {
				IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
				System.ComponentModel.Design.MenuCommand menuCommand = menuCommandService.FindCommand(StandardCommands.Delete);
				if (menuCommand == null) {
					return false;
				}
				int status = menuCommand.OleStatus;
				return menuCommand.Enabled;
			}
		}
		
		public bool EnableSelectAll {
			get {
				return true;
			}
		}
		
		public void Cut()
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Cut);
		}
		
		public void Copy()
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Copy);
		}
		
		public void Paste()
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Paste);
		}
		
		public void Delete()
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Delete);
		}
		
		public void SelectAll()
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.SelectAll);
		}
		#endregion
		
		#region Tab Order Handling
		bool tabOrderMode = false;
		public virtual bool IsTabOrderMode {
			get {
				return tabOrderMode;
			}
		}
		
		public virtual void ShowTabOrder()
		{
			if (!IsTabOrderMode) {
				IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
				menuCommandService.GlobalInvoke(StandardCommands.TabOrder);
				tabOrderMode = true;
			}
		}
		
		public virtual void HideTabOrder()
		{
			if (IsTabOrderMode) {
				IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
				menuCommandService.GlobalInvoke(StandardCommands.TabOrder);
				tabOrderMode = false;
			}
		}
		#endregion
		
		/// <summary>
		/// Reloads the form designer if the language property has changed.
		/// </summary>
		void PropertyValueChanged(object source, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem == null || e.OldValue == null)
				return;
			if (e.ChangedItem.GridItemType == GridItemType.Property) {
				if (e.ChangedItem.PropertyDescriptor.Name == "Language") {
					if (!e.OldValue.Equals(e.ChangedItem.Value)) {
						LoggingService.Debug("Reloading designer due to language change.");
						propertyContainer.Clear();
						if (!failedDesignerInitialize) {
							MergeFormChanges();
						}
						UnloadDesigner();
						Reload();
						UpdatePropertyPad();
					}
				}
			}
		}
		
		void AddSideBars()
		{
			foreach(SideTab tab in ToolboxProvider.SideTabs) {
				if (!SharpDevelopSideBar.SideBar.Tabs.Contains(tab)) {
					SharpDevelopSideBar.SideBar.Tabs.Add(tab);
				}
			}
			SharpDevelopSideBar.SideBar.Refresh();
		}
		
		void SetActiveSideTab()
		{
			if (activeTabName.Length == 0) {
				return;
			}
			
			foreach(SideTab tab in ToolboxProvider.SideTabs) {
				if (activeTabName == tab.Name) {
					SharpDevelopSideBar.SideBar.ActiveTab = tab;
					return;
				}
			}
		}
	}
}
