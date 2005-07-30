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
	public class FormDesignerViewContent : AbstractViewContent, ISecondaryViewContent, IClipboardHandler, IUndoHandler, IHasPropertyContainer
	{
		protected bool failedDesignerInitialize;
		
		protected IViewContent viewContent;
		protected Hashtable    resources = null;
		
		protected              ITextEditorControlProvider textAreaControlProvider;
		
		protected string       compilationErrors;
		
		Panel p = new Panel();
		DesignSurface designSurface;
		
		DesignerLoader    loader;
		IDesignerGenerator generator;
		
		public override string FileName {
			get {
				string fileName = textAreaControlProvider.TextEditorControl.FileName;
				return fileName == null ? viewContent.UntitledName : fileName;
			}
		}
		
		public override Control Control {
			get {
				return p;
			}
		}
		
		public override bool IsDirty {
			get {
				if (viewContent == null) {
					return false;
				}
				return viewContent.IsDirty;
			}
			set {
				if (viewContent != null) {
					viewContent.IsDirty = value;
				}
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
				return (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
			}
		}
		public FormDesignerViewContent(IViewContent viewContent, DesignerLoader loader, IDesignerGenerator generator)
		{
			this.loader    = loader;
			this.generator = generator;
			p.BackColor    = Color.White;
			
			this.viewContent             = viewContent;
			this.textAreaControlProvider = viewContent as ITextEditorControlProvider;
		}
		
		bool isInitialized = false;
		
		void Initialize()
		{
			if (isInitialized) return;
			isInitialized = true;
			DefaultServiceContainer serviceContainer = new DefaultServiceContainer();
			serviceContainer.AddService(typeof(System.Windows.Forms.Design.IUIService), new UIService());
			serviceContainer.AddService(typeof(System.Drawing.Design.IToolboxService), ToolboxProvider.ToolboxService);
			
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IMenuCommandService), new ICSharpCode.FormDesigner.Services.MenuCommandService(p, serviceContainer));
			serviceContainer.AddService(typeof(IHelpService), new HelpService());
			serviceContainer.AddService(typeof(System.Drawing.Design.IPropertyValueUIService), new PropertyValueUIService());
			DesignerResourceService designerResourceService = new DesignerResourceService(this.resources);
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IResourceService), designerResourceService);
			AmbientProperties ambientProperties = new AmbientProperties();
			serviceContainer.AddService(typeof(AmbientProperties), ambientProperties);
			serviceContainer.AddService(typeof(ITypeResolutionService), ToolboxProvider.TypeResolutionService);
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IDesignerEventService), new DesignerEventService());
//			serviceContainer.AddService(typeof(System.ComponentModel.Design.IDesignerOptionService), new ICSharpCode.FormDesigner.Services.DesignerOptionService());
			
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IDesignerOptionService), new System.Windows.Forms.Design.WindowsFormsDesignerOptionService());
			
			serviceContainer.AddService(typeof(MemberRelationshipService), new DefaultMemberRelationshipService());
			
			
			ICSharpCode.FormDesigner.Services.EventBindingService eventBindingService = new ICSharpCode.FormDesigner.Services.EventBindingService();
			serviceContainer.AddService(typeof(System.ComponentModel.Design.IEventBindingService), eventBindingService);
			
			designSurface = new DesignSurface(serviceContainer);
			eventBindingService.ServiceProvider = designSurface;
			designerResourceService.Host = Host;
			
			designSurface.BeginLoad(loader);
			
			designSurface.Flush();
			
			generator.Attach(this);
		}
		
		PropertyContainer propertyContainer = new PropertyContainer();
		
		public PropertyContainer PropertyContainer {
			get {
				return propertyContainer;
			}
		}
		
		public override void Load(string fileName)
		{
		}
		
		public void Reload()
		{
			Initialize();
			bool dirty = viewContent.IsDirty;
//	TODO:		
//			loader.TextContent = Document.TextContent;
			
			try {
				if (designSurface != null && p.Controls.Count == 0) {
					Control designer = designSurface.View as Control;
					designer.Dock = DockStyle.Fill;
					p.Controls.Add(designer);
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		
		protected virtual void MergeFormChanges()
		{
			if (this.failedDesignerInitialize) {
				return;
			}
			bool isDirty = IsDirty;
			generator.MergeFormChanges();
			IsDirty = isDirty;
		}
		
		public  void ShowSourceCode()
		{
			WorkbenchWindow.SwitchView(0);
		}
		
		public  void ShowSourceCode(int lineNumber)
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
		
		public override void Deselected()
		{
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
//			DeselectAllComponents();
		}
		
		public void NotifyAfterSave(bool successful)
		{
//			//ifko: save the resources if there are any
//			if (successful) {
//				DesignerResourceService designerResourceService = (DesignerResourceService)designSurface.GetService(typeof(System.ComponentModel.Design.IResourceService));
//				if (designerResourceService != null) {
//					designerResourceService.Save();
//				}
//			}
		}

		public void NotifyBeforeSave()
		{
			MergeFormChanges();
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
