// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
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
	public class FormsDesignerViewContent : AbstractSecondaryViewContent, IClipboardHandler, IUndoHandler, IHasPropertyContainer, IContextHelpProvider, IToolsHost
	{
		protected bool failedDesignerInitialize;
		
		protected IViewContent viewContent;
		protected ITextEditorControlProvider textAreaControlProvider;
		
		Panel p = new Panel();
		DesignSurface designSurface;
		bool disposing;
		
		IDesignerLoaderProvider loaderProvider;
		IDesignerGenerator generator;
		DesignerResourceService designerResourceService;
		FormsDesignerUndoEngine undoEngine;
		
		public override object Content {
			get {
				return p;
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
		
		internal new ICollection<OpenedFile> Files {
			get { return base.Files; }
		}
		
		public IDesignerHost Host {
			get {
				if (designSurface == null)
					return null;
				return (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
			}
		}
		public FormsDesignerViewContent(IViewContent viewContent, IDesignerLoaderProvider loaderProvider, IDesignerGenerator generator)
			: base(viewContent)
		{
			this.TabPageText = "${res:FormsDesigner.DesignTabPages.DesignTabPage}";
			
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
				designerResourceService = new DesignerResourceService(this);
			}
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IResourceService), designerResourceService);
			AmbientProperties ambientProperties = new AmbientProperties();
			serviceContainer.AddService(typeof(AmbientProperties), ambientProperties);
			serviceContainer.AddService(typeof(ITypeResolutionService), new TypeResolutionService(viewContent.PrimaryFileName));
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IDesignerEventService), new DesignerEventService());
			serviceContainer.AddService(typeof(DesignerOptionService), new SharpDevelopDesignerOptionService());
			serviceContainer.AddService(typeof(ITypeDiscoveryService), new TypeDiscoveryService());
			serviceContainer.AddService(typeof(MemberRelationshipService), new DefaultMemberRelationshipService());
			
			designSurface = new DesignSurface(serviceContainer);
			
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IMenuCommandService), new ICSharpCode.FormsDesigner.Services.MenuCommandService(p, designSurface));
			ICSharpCode.FormsDesigner.Services.EventBindingService eventBindingService = new ICSharpCode.FormsDesigner.Services.EventBindingService(designSurface);
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IEventBindingService), eventBindingService);
			
			DesignerLoader designerLoader = loaderProvider.CreateLoader(generator);
			designSurface.BeginLoad(designerLoader);
			
			generator.Attach(this);
			
			undoEngine = new FormsDesignerUndoEngine(Host);
			
			IComponentChangeService componentChangeService = (IComponentChangeService)designSurface.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentChanged += MakeDirty;
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
			
			UpdatePropertyPad();
			PropertyPad.PropertyValueChanged += PropertyValueChanged;
			
			hasUnmergedChanges = false;
			
			LoggingService.Info("Form Designer: END INITIALIZE");
		}
		
		bool hasUnmergedChanges;
		
		void MakeDirty(object sender, ComponentChangedEventArgs args)
		{
			hasUnmergedChanges = true;
			this.PrimaryFile.MakeDirty();
			designerResourceService.MarkResourceFilesAsDirty();
		}
		
		public override void Load(OpenedFile file, System.IO.Stream stream)
		{
			if (file == PrimaryFile) {
				base.Load(file, stream);
			} else {
				designerResourceService.Load(file, stream);
			}
		}
		
		public override void Save(OpenedFile file, System.IO.Stream stream)
		{
			if (file == PrimaryFile) {
				base.Save(file, stream);
			} else {
				if (hasUnmergedChanges) SaveToPrimary();
				designerResourceService.Save(file, stream);
			}
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
			PropertyPad.PropertyValueChanged -= PropertyValueChanged;
			generator.Detach();
			bool savedIsDirty = this.PrimaryFile.IsDirty;
			p.Controls.Clear();
			this.PrimaryFile.IsDirty = savedIsDirty;
			
			// We cannot dispose the design surface now because of SD2-451:
			// When the switch to the source view was triggered by a double-click on an event
			// in the PropertyPad, "InvalidOperationException: The container cannot be disposed
			// at design time" is thrown.
			// This is solved by calling dispose after the double-click event has been processed.
			if (designSurface != null) {
				if (disposing) {
					designSurface.Dispose();
				} else {
					p.BeginInvoke(new MethodInvoker(designSurface.Dispose));
				}
				designSurface = null;
			}
		}
		
		PropertyContainer propertyContainer = new PropertyContainer();
		
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
		
		public void ShowHelp()
		{
			if (Host == null) {
				return;
			}
			
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
				
				bool savedIsDirty = this.PrimaryFile.IsDirty;
				if (designSurface != null && p.Controls.Count == 0) {
					Control designer = designSurface.View as Control;
					designer.Dock = DockStyle.Fill;
					p.Controls.Add(designer);
				}
				this.PrimaryFile.IsDirty = savedIsDirty;
			} catch (Exception e) {
				failedDesignerInitialize = true;
				TextBox errorText = new TextBox();
				errorText.ScrollBars = ScrollBars.Both;
				errorText.Multiline = true;
				if (e.InnerException is FormsDesignerLoadException) {
					errorText.Text = e.InnerException.Message;
				} else if (e is FormsDesignerLoadException) {
					errorText.Text = e.Message;
				} else if (designSurface != null && !designSurface.IsLoaded && designSurface.LoadErrors != null) {
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
			bool isDirty = this.PrimaryFile.IsDirty;
			LoggingService.Info("Merging form changes...");
			designSurface.Flush();
			LoggingService.Info("Finished merging form changes");
			hasUnmergedChanges = false;
			this.PrimaryFile.IsDirty = isDirty;
		}
		
		public void ShowSourceCode()
		{
			this.WorkbenchWindow.ActiveViewContent = this.PrimaryViewContent;
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
		
		/*
		protected override void OnViewActivated(EventArgs e)
		{
			LoggingService.Info("Designer.OnViewActived 1");
			base.OnViewActivated(e); // calls Load() if required
			LoggingService.Info("Designer.OnViewActived 2");
			
			IsFormsDesignerVisible = true;
			AddSideBars();
			PropertyPad.PropertyValueChanged += PropertyValueChanged;
			SetActiveSideTab();
			UpdatePropertyPad();
			LoggingService.Info("Designer.OnViewActived 3");
		}
		 */
		
		/*
		protected override void OnViewDeactivated(EventArgs e)
		{
			LoggingService.Info("Designer.OnViewDeactivated");
			
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
		 */
		
		public override void Dispose()
		{
			disposing = true;
			base.Dispose();
		}
		
		protected override void LoadFromPrimary()
		{
			UnloadDesigner();
			Reload();
		}
		
		protected override void SaveToPrimary()
		{
			MergeFormChanges();
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
			if (Host != null) {
				propertyContainer.Host = Host;
				propertyContainer.SelectableObjects = Host.Container.Components;
				ISelectionService selectionService = (ISelectionService)Host.GetService(typeof(ISelectionService));
				if (selectionService != null) {
					UpdatePropertyPadSelection(selectionService);
				}
			}
		}
		
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
		bool IsMenuCommandEnabled(CommandID commandID)
		{
			if (designSurface == null) {
				return false;
			}
			
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			if (menuCommandService == null) {
				return false;
			}
			
			System.ComponentModel.Design.MenuCommand menuCommand = menuCommandService.FindCommand(commandID);
			if (menuCommand == null) {
				return false;
			}
			
			//int status = menuCommand.OleStatus;
			return menuCommand.Enabled;
		}
		
		public bool EnableCut {
			get {
				return IsMenuCommandEnabled(StandardCommands.Cut);
			}
		}
		
		public bool EnableCopy {
			get {
				return IsMenuCommandEnabled(StandardCommands.Copy);
			}
		}
		
		const string ComponentClipboardFormat = "CF_DESIGNERCOMPONENTS";
		public bool EnablePaste {
			get {
				return IsMenuCommandEnabled(StandardCommands.Paste);
			}
		}
		
		public bool EnableDelete {
			get {
				return IsMenuCommandEnabled(StandardCommands.Delete);
			}
		}
		
		public bool EnableSelectAll {
			get {
				return designSurface != null;
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
			if (!propertyContainer.IsActivePropertyContainer)
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
		
		public virtual Control ToolsControl {
			get { return ToolboxProvider.FormsDesignerSideBar; }
		}
	}
}
