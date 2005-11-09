// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Printing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Xml;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Undo;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;

using ICSharpCode.Core;
using ICSharpCode.FormDesigner.Services;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

using System.CodeDom;
using System.CodeDom.Compiler;

using Microsoft.CSharp;
using Microsoft.VisualBasic;

namespace ICSharpCode.FormDesigner
{
	public class FormDesignerViewContent : AbstractSecondaryViewContent, IClipboardHandler, IUndoHandler, IHasPropertyContainer, IContextHelpProvider
	{
		protected bool failedDesignerInitialize;
		
		protected IViewContent viewContent;
		protected Hashtable resources = new Hashtable();
		
		protected ITextEditorControlProvider textAreaControlProvider;
		
		Panel p = new Panel();
		DesignSurface designSurface;
		
		IDesignerLoaderProvider loaderProvider;
		IDesignerGenerator generator;
		DesignerResourceService designerResourceService;
		
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
		public FormDesignerViewContent(IViewContent viewContent, IDesignerLoaderProvider loaderProvider, IDesignerGenerator generator)
		{
			if (!FormKeyHandler.inserted) {
				FormKeyHandler.Insert();
			}
			
			this.loaderProvider    = loaderProvider;
			this.generator = generator;
			p.BackColor    = Color.White;
			
			this.viewContent             = viewContent;
			this.textAreaControlProvider = viewContent as ITextEditorControlProvider;
		}
		
		void LoadDesigner()
		{
			LoggingService.Info("Form Designer: BEGIN INITIALIZE");
			
			DefaultServiceContainer serviceContainer = new DefaultServiceContainer();
			serviceContainer.AddService(typeof(System.Windows.Forms.Design.IUIService), new UIService());
			serviceContainer.AddService(typeof(System.Drawing.Design.IToolboxService), ToolboxProvider.ToolboxService);
			
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IMenuCommandService), new ICSharpCode.FormDesigner.Services.MenuCommandService(p, serviceContainer));
			serviceContainer.AddService(typeof(IHelpService), new HelpService());
			serviceContainer.AddService(typeof(System.Drawing.Design.IPropertyValueUIService), new PropertyValueUIService());
			designerResourceService = new DesignerResourceService(this.resources);
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IResourceService), designerResourceService);
			AmbientProperties ambientProperties = new AmbientProperties();
			serviceContainer.AddService(typeof(AmbientProperties), ambientProperties);
			serviceContainer.AddService(typeof(ITypeResolutionService), ToolboxProvider.TypeResolutionService);
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IDesignerEventService), new DesignerEventService());
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IDesignerOptionService), new ICSharpCode.FormDesigner.Services.DesignerOptionService());
			serviceContainer.AddService(typeof(ITypeDiscoveryService), new TypeDiscoveryService());
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IDesignerOptionService), new System.Windows.Forms.Design.WindowsFormsDesignerOptionService());
			
			serviceContainer.AddService(typeof(MemberRelationshipService), new DefaultMemberRelationshipService());
			
			designSurface = new DesignSurface(serviceContainer);
						
			ICSharpCode.FormDesigner.Services.EventBindingService eventBindingService = new ICSharpCode.FormDesigner.Services.EventBindingService(designSurface);
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IEventBindingService), eventBindingService);
			
			designerResourceService.Host = Host;
			designerResourceService.FileName = viewContent.FileName;
			serviceContainer.AddService(typeof(IDesignerHost), Host);
			
			DesignerLoader designerLoader = loaderProvider.CreateLoader(generator);
			designSurface.BeginLoad(designerLoader);
			
			generator.Attach(this);
			
			IComponentChangeService componentChangeService = (IComponentChangeService)designSurface.GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentChanged += delegate { viewContent.IsDirty = true; };
			
			LoggingService.Info("Form Designer: END INITIALIZE");
		}
		
		void UnloadDesigner()
		{
			generator.Detach();
			p.Controls.Clear();
			// We cannot dispose the design surface now because of SD2-451:
			// When the switch to the source view was triggered by a double-click on an event
			// in the PropertyPad, "InvalidOperationException: The container cannot be disposed
			// at design time" is thrown.
			// This is solved by calling dispose after the double-click event has been processed.
			p.BeginInvoke(new MethodInvoker(designSurface.Dispose));
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
					ICSharpCode.SharpDevelop.Dom.HelpProvider.ShowHelp(ctl.GetType().FullName);
				}
			}
		}
		
		public void Reload()
		{
			try {
				LoadDesigner();
				
				if (designSurface != null && p.Controls.Count == 0) {
					Control designer = designSurface.View as Control;
					designer.Dock = DockStyle.Fill;
					p.Controls.Add(designer);
				}
			} catch (Exception e) {
				failedDesignerInitialize = true;
				TextBox errorText = new TextBox();
				errorText.Multiline = true;
				if (e.InnerException is FormDesignerLoadException)
					errorText.Text = e.InnerException.Message;
				else if (e is FormDesignerLoadException)
					errorText.Text = e.Message;
				else
					errorText.Text = e.ToString();
				errorText.Dock = DockStyle.Fill;
				p.Controls.Add(errorText);
				Control title = new Label();
				title.Text = "Failed to load designer. Check the source code for syntax errors.";
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
			designerResourceService.SerializationStarted(true);
			designSurface.Flush();
			designerResourceService.SerializationEnded(true);
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
			textAreaControlProvider.TextEditorControl.ActiveTextAreaControl.JumpTo(lineNumber, 255);
		}
		
		public void ShowSourceCode(IComponent component, EventDescriptor edesc, string eventMethodName)
		{
			int position;
			generator.InsertComponentEvent(component, edesc, eventMethodName, "", out position);
			ShowSourceCode(position);
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
			Reload();
			IsFormDesignerVisible = true;
			foreach(AxSideTab tab in ToolboxProvider.SideTabs) {
				if (!SharpDevelopSideBar.SideBar.Tabs.Contains(tab)) {
					SharpDevelopSideBar.SideBar.Tabs.Add(tab);
				}
			}
			SharpDevelopSideBar.SideBar.Refresh();
			propertyContainer.Host = Host;
			UpdateSelectableObjects();
		}
		
		public override void Dispose()
		{
			if (IsFormDesignerVisible) {
				Deselected();
			}
			base.Dispose();
		}
		
		public override void Deselected()
		{
			LoggingService.Info("Deselected form designer, unloading...");
			propertyContainer.Clear();
			IsFormDesignerVisible = false;
			foreach(AxSideTab tab in ToolboxProvider.SideTabs) {
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
			if (IsFormDesignerVisible)
				MergeFormChanges();
		}
		
		public override void NotifyAfterSave(bool successful)
		{
			if (successful) {
				if (designerResourceService != null) {
					designerResourceService.Save();
				}
			}
		}
		
		protected void UpdateSelectableObjects()
		{
			propertyContainer.SelectableObjects = Host.Container.Components;
			ISelectionService selectionService = (ISelectionService)Host.GetService(typeof(ISelectionService));
			if (selectionService != null) {
				propertyContainer.SelectedObject = selectionService.PrimarySelection;
			}
		}
		
		public bool IsFormDesignerVisible = false;
		
		#region IUndoHandler impelementation
		public bool EnableUndo {
			get {
				return true;
			}
		}
		public bool EnableRedo {
			get {
				return true;
			}
		}
		public virtual void Undo()
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Undo);
		}
		
		public virtual void Redo()
		{
			IMenuCommandService menuCommandService = (IMenuCommandService)designSurface.GetService(typeof(IMenuCommandService));
			menuCommandService.GlobalInvoke(StandardCommands.Redo);
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
	}
}
