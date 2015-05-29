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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Printing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.Core;
using ICSharpCode.Reporting.Addin.DesignableItems;
using ICSharpCode.Reporting.Addin.Globals;
using ICSharpCode.Reporting.Addin.TypeProvider;

namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of ReportRootDesigner.
	/// </summary>

	class ReportRootDesigner: DocumentDesigner{
		ICollection currentSelection;
		IDesignerHost host;
		 MenuCommandService menuCommandService;
		IToolboxService	toolboxService;
		ISelectionService selectionService;
		IComponentChangeService componentChangeService;
		List<BaseSection> sections;
		ReportSettings reportSettings;
		RootReportModel rootReportModel;
		

		void ShowMessage(Exception e){
			DisplayError(e);
			var uiService = (IUIService)host.GetService(typeof(IUIService));
			if (uiService != null) {
				uiService.ShowError(e);
			}
		}
		
		
		void InitializeGUI(){
			reportSettings = host.Container.Components[1] as ReportSettings;
			InitializeRootReportModel();
		}
		
		
		void InitializeRootReportModel (){
			rootReportModel = host.Container.Components[0] as RootReportModel;
			rootReportModel.PageMargin = CalculateMargins();
			rootReportModel.Page = new Rectangle(new Point(0,0), reportSettings.PageSize);
			rootReportModel.Landscape = this.reportSettings.Landscape;
			rootReportModel.Invalidate();
		}
			
		
		Margins CalculateMargins (){
			return new Margins(reportSettings.LeftMargin,reportSettings.RightMargin,
			                   reportSettings.TopMargin,reportSettings.BottomMargin);
		}
			
		#region overrides
		
		public override void Initialize(IComponent component){
			base.Initialize(component);
			sections = new List<BaseSection>();
			
			// We need to listen to change events.  If a shape changes,
			// we need to invalidate our view.
			//
		
			this.componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			if (this.componentChangeService != null)
			{
				this.componentChangeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
//				this.componentChangeService.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
//				this.componentChangeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
				this.componentChangeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
				this.componentChangeService.ComponentChanging += new ComponentChangingEventHandler(OnComponentChanging);
			}
	
	
			// Add the menu commands we support.  We must be a member of the VSIP program to
			// define new menu items, but we can handle any item located within the StandardCommands
			// class because Visual Studio already defines them.
			//
			
			menuCommandService = (MenuCommandService)GetService(typeof(MenuCommandService));
			/*
			if (menuCommandService != null)
			{
				/*
				m_menuCommands = new MenuCommand[]
					{
						new MenuCommand(new EventHandler(OnMenuCut), StandardCommands.Cut),
						new MenuCommand(new EventHandler(OnMenuCopy), StandardCommands.Copy),
//						new ImmediateMenuCommand(new EventHandler(OnMenuPasteStatus), new EventHandler(OnMenuPaste), StandardCommands.Paste),
						new MenuCommand(new EventHandler(OnMenuDelete), StandardCommands.Delete)
					};

				foreach(MenuCommand mc in m_menuCommands)
				{
					m_menuCommandService.AddCommand(mc);
				}
			
				System.Console.WriteLine("RootDesigner menuService set");
			}
		*/
			// Select our base shape.  By default there is nothing selected but that looks
			// strange (the property grid is empty).
			//
		
			selectionService = (ISelectionService)GetService(typeof(ISelectionService));
			if (this.selectionService != null)
			{
				this.selectionService.SetSelectedComponents(new object[] {component}, SelectionTypes.Replace);
				this.selectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
			}
		
			host = (IDesignerHost)GetService(typeof(IDesignerHost));
			
			menuCommandService = (MenuCommandService)host.GetService(typeof(MenuCommandService));
			if (host != null)
			{
				host.LoadComplete += OnLoadComplete;
			}
			//Dragdropp only allowed in Section
			this.Control.AllowDrop = false;
		}
		
		
		public override SelectionRules SelectionRules {
			get { 
				return SelectionRules.BottomSizeable;
			}
		}
		
		
		protected override void PostFilterProperties(IDictionary properties){	
			TypeProviderHelper.RemoveProperties(properties);
			var s = new string[]{"Visible","BackColor",
				"Text","MaximumSize","MinimumSize",
				"Size","AutoScaleDimensions",
				"DataBindings"};
			TypeProviderHelper.Remove(properties,s);
			base.PostFilterProperties(properties);
		}	
		
		#endregion
		
		
		
		#region Events
		
		void OnSectionSizeChanged (object sender, EventArgs e){
			RecalculateSections();
		}
		
		
		void RecalculateSections(){
			int locY = 50;
			// disable once ConvertIfStatementToNullCoalescingExpression
			if (reportSettings == null) {
				reportSettings = host.Container.Components[1] as ReportSettings;
			}
			
			foreach (BaseSection section in sections){
				section.Location = new Point(reportSettings.LeftMargin,locY);
				locY = locY + section.Size.Height + DesignerGlobals.GabBetweenSection;
			}
			Control.Invalidate();
		}
		
		
		
		void OnLoadComplete(object sender, EventArgs e){
			var host = (IDesignerHost)sender;
			host.LoadComplete -= OnLoadComplete;
			InitializeGUI();
		}
		
		
		void OnComponentAdded(object sender, ComponentEventArgs ce){
			var section = ce.Component as BaseSection;
			
			if (section != null) {
				sections.Add(section);
				section.SizeChanged += OnSectionSizeChanged;
				foreach (Control ctrl in section.Controls) {
					AddToHost(ctrl);
					host.Container.Add(ctrl);
				}				
				Control.Controls.Add(section);
				RecalculateSections();
			}
		}
		
		
		void AddToHost (Control ctrl){
			foreach (Control c1 in ctrl.Controls) {
				AddToHost (c1);
			}
			host.Container.Add(ctrl as IComponent);
		}
			
		
		void OnComponentChanged(object sender, ComponentChangedEventArgs ce){
			LoggingService.InfoFormatted("RootDesigner:OnComponentChanged");
			var str = String.Format(CultureInfo.CurrentCulture,"RootDesigner:OnComponentChanged <{0}> from <{1}> to <{2}>",ce.Component.ToString(),ce.OldValue,ce.NewValue);
			LoggingService.InfoFormatted(str);

			var section = ce.Component as BaseSection;
			if (section != null) {
				foreach (BaseSection s in sections){
				
					if (s.Name == section.Name) {
						s.Size = section.Size;
					}
				}
				RecalculateSections();
			}
		}
		
		
		void OnComponentChanging(object sender, ComponentChangingEventArgs ce){
			System.Console.WriteLine("RootDesigner:OnComponentChanging");
//			Host.CreateTransaction();
		}
		
		
		void OnSelectionChanged(object sender, EventArgs e){
			currentSelection = ((ISelectionService)sender).GetSelectedComponents();
		}

		#endregion
		
	
		public IDesignerHost Host {
			get {
				if (this.host == null) {
					this.host = (IDesignerHost)this.GetService(typeof(IDesignerHost));
				}
				return host; }
		}
		
		
		public IToolboxService ToolboxService{
			get
			{
				if (toolboxService == null)
				{
					toolboxService = (IToolboxService)this.GetService(typeof(IToolboxService));
				}
				return toolboxService;
			}
		}
		
		
		public ISelectionService SelectionService {
			get { 
				if (this.selectionService == null) {
					this.selectionService = (ISelectionService)this.GetService(typeof(ISelectionService));
				}
				return selectionService; }
		}
		
		
		public IComponentChangeService ComponentChangeService {
			get { 
				if (this.componentChangeService == null) {
				this.componentChangeService	= (IComponentChangeService)this.GetService(typeof(IComponentChangeService));
				}
				return componentChangeService; }
		}
		
		public MenuCommandService MenuCommandService {
			get { 
				if (this.menuCommandService == null) {
					this.menuCommandService = (MenuCommandService)Host.GetService(typeof(IMenuCommandService)); 
				}
				return (MenuCommandService)host.GetService(typeof(IMenuCommandService));
				}
		}

		
		public ICollection CurrentSelection {
			get { return currentSelection; }
		}
		
		
		#region Dispose
		protected override void Dispose(bool disposing){
            if (disposing)
            {
                var componentService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                if (componentService != null)
                {
                    componentService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                    componentService.ComponentChanging -= new ComponentChangingEventHandler(OnComponentChanging);
                }

                ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
                if (ss != null)
                {
                    ss.SelectionChanged -= new EventHandler(OnSelectionChanged);
                }
/*
                if (m_menuCommands != null && m_menuCommandService != null)
                {
                    foreach(MenuCommand mc in m_menuCommands)
                    {
                        m_menuCommandService.RemoveCommand(mc);
                    }
                    m_menuCommands = null;
                }
              */
           }

			base.Dispose(disposing);
		}
		
		#endregion
	}
}
