/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 11.11.2007
 * Zeit: 22:54
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportRootDesigner.
	/// </summary>

	public class ReportRootDesigner: DocumentDesigner
	{
		private ICollection currentSelection;
		private IDesignerHost host;
		private MenuCommandService menuCommandService;
		private IToolboxService	toolboxService;
		private ISelectionService selectionService;
		private IComponentChangeService componentChangeService;
		private List<BaseSection> sections;
		private ReportSettings reportSettings;
		private RootReportModel rootReportModel;
		
		public ReportRootDesigner()
		{
		}
		

		public override void Initialize(IComponent component)
		{
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
//				this.componentChangeService.ComponentChanging += new ComponentChangingEventHandler(OnComponentChanging);
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
		
			this.selectionService = (ISelectionService)GetService(typeof(ISelectionService));
			if (this.selectionService != null)
			{
				this.selectionService.SetSelectedComponents(new object[] {component}, SelectionTypes.Replace);
				this.selectionService.SelectionChanged += new EventHandler(OnSelectionChanged);
			}
		
			this.host = (IDesignerHost)GetService(typeof(IDesignerHost));
			
			this.menuCommandService = (MenuCommandService)host.GetService(typeof(MenuCommandService));
			if (host != null)
			{
				host.LoadComplete += new EventHandler(OnLoadComplete);
			}
			//Dragdropp only allowed in Section
			this.Control.AllowDrop = false;
		}
		
		
		public override SelectionRules SelectionRules {
			get { 
				return SelectionRules.BottomSizeable;
			}
		}
		
		
		protected override void PostFilterProperties(IDictionary properties)
		{
			DesignerHelper.RemoveProperties(properties);
			string [] s = new string[]{"Visible","BackColor","Text","MaximumSize","MinimumSize","Size",
				"AutoScaleDimensions","DataBindings"};
			DesignerHelper.Remove(properties,s);
			base.PostFilterProperties(properties);
		}
		
		
		private void ShowMessage(Exception e)
		{
			base.DisplayError(e);
			IUIService s = (IUIService)host.GetService(typeof(IUIService));
			if (s != null) {
				s.ShowError(e);
			}
		}
		
//		private new void DisplayError(Exception ex)
//		{
//			MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Fehler im Designer", MessageBoxButtons.OK, MessageBoxIcon.Error);
//		}
		
		
		private void InitializeGUI()
		{
			this.rootReportModel = host.Container.Components[0] as RootReportModel;
			reportSettings = host.Container.Components[1] as ReportSettings;
			this.rootReportModel.PageMargin = new System.Drawing.Printing.Margins(this.reportSettings.LeftMargin,reportSettings.RightMargin,
			                                                                      reportSettings.TopMargin,reportSettings.BottomMargin);
		
			this.rootReportModel.Page = new Rectangle(0,0,
			                                          this.reportSettings.PageSize.Width,
			                                          this.reportSettings.PageSize.Height);
			this.rootReportModel.Invalidate();
		}
		
		
		public override bool CanParent(ControlDesigner controlDesigner)
		{
			return base.CanParent(controlDesigner);
		}
		
		
		public override bool CanBeParentedTo(IDesigner parentDesigner)
		{
			return base.CanBeParentedTo(parentDesigner);
		}
		
		
		
		private void OnSectionSizeChanged (object sender, EventArgs e)
		{
			this.RecalculateSections();
		}
		
		private void RecalculateSections()
		{
			int locY = 50;
			if (this.reportSettings == null) {
				reportSettings = host.Container.Components[1] as ReportSettings;
			}
			
			foreach (BaseSection s in sections)
			{
				s.Location = new Point(this.reportSettings.LeftMargin,locY);
				locY = locY + s.Size.Height + GlobalsDesigner.GabBetweenSection;
			}
			this.Control.Invalidate();
		}
		
		#region Events
		
		private void OnLoadComplete(object sender, EventArgs e)
		{
			IDesignerHost host = (IDesignerHost)sender;
			host.LoadComplete -= new EventHandler(this.OnLoadComplete);
			InitializeGUI();
		}
		
		
		private void OnComponentAdded(object sender, ComponentEventArgs ce)
		{
			BaseSection section = ce.Component as BaseSection;
			
			if (section != null) {
				this.sections.Add(section);
				section.SizeChanged += new EventHandler( OnSectionSizeChanged);
				foreach (Control cc in section.Controls) {
					AddToHost(cc);
					this.host.Container.Add(cc);
				}
				
				this.Control.Controls.Add(section);
				RecalculateSections();
			}
		}
		
		
		private void AddToHost (Control ctrl)
		{
			if (ctrl.Controls.Count > 0) {
				foreach (Control c1 in ctrl.Controls) {
					AddToHost (c1);
				}
			}
			this.host.Container.Add(ctrl as IComponent);
		}
			
		/*
		private void OnComponentRemoving (object sender, ComponentEventArgs ce)
		
//			System.Console.WriteLine("RootDesigner:OnComponentRemoving {0}",ce.Component.ToString());
		}
		
		
		private void OnComponentRemoved (object sender, ComponentEventArgs ce)
		{
//			System.Console.WriteLine("RootDesigner:OnComponentRemoved {0}",ce.Component.ToString());
		}
		*/
		
		private void OnComponentChanged(object sender, ComponentChangedEventArgs ce)
		{
			System.Console.WriteLine("RootDesigner:OnComponentChanged {0} from {1} to {2}",ce.Component.ToString(),ce.OldValue,ce.NewValue);
			
//			MemberDescriptor m = ce.Member;
			if (ce.Member.Name == "Name") {
				AbstractItem item = ce.Component as AbstractItem;
				if (item != null) {
					item.Name = ce.NewValue.ToString();
				}
			}
			
			BaseSection section = ce.Component as BaseSection;
			if (section != null) {
				foreach (BaseSection s in sections)
				{
					if (s.Name == section.Name) {
						s.Size = section.Size;
					}
				}
				RecalculateSections();
			}
		}
		
		
		private void OnComponentChanging(object sender, ComponentChangingEventArgs ce)
		{
//			System.Console.WriteLine("RootDesigner:OnComponentChanging");
		}
		
		
		private void OnSelectionChanged(object sender, EventArgs e)
		{
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
		
		
		public IToolboxService ToolboxService
		{
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
		protected override void Dispose(bool disposing)
		{
            if (disposing)
            {
                IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                if (cs != null)
                {
                    cs.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                    cs.ComponentChanging -= new ComponentChangingEventHandler(OnComponentChanging);
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
