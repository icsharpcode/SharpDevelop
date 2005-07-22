// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;


using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class PropertyPad : AbstractPadContent, IHelpProvider
	{
		static PropertyPad instance;
		
		public static PropertyPad Instance {
			get {
				return instance;
			}
		}
		
		static PropertyContainer activeContainer;
		
		static void SetActiveContainer(PropertyContainer pc)
		{
			if (activeContainer == pc)
				return;
			if (pc == null)
				return;
			activeContainer = pc;
			UpdateSelectedObjectIfActive(pc);
			UpdateHostIfActive(pc);
			UpdateSelectableIfActive(pc);
		}
		
		internal static void UpdateSelectedObjectIfActive(PropertyContainer container)
		{
			if (activeContainer != container) return;
			if (container.SelectedObjects != null)
				SetDesignableObjects(container.SelectedObjects);
			else
				SetDesignableObject(container.SelectedObject);
		}
		
		internal static void UpdateHostIfActive(PropertyContainer container)
		{
			if (activeContainer != container)
				return;
			if (host == container.Host)
				return;
			if (host != null)
				RemoveHost(host);
			if (container.Host != null)
				SetDesignerHost(container.Host);
		}
		
		internal static void UpdateSelectableIfActive(PropertyContainer container)
		{
			if (activeContainer != container)
				return;
			SetSelectableObjects(container.SelectableObjects);
		}
		
		static Panel         panel   = null;
		static ComboBox      comboBox = null;
		static PropertyGrid  grid = null;
		static IDesignerHost host = null;
		
		public static PropertyGrid Grid {
			get {
				return grid;
			}
		}
		
		public static event PropertyValueChangedEventHandler PropertyValueChanged;
		public static event EventHandler                     SelectedObjectChanged;
		
		public override Control Control {
			get {
				return panel;
			}
		}
		
		static PropertyPad()
		{
			grid = new PropertyGrid();
			grid.PropertySort = PropertyService.Get("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", false) ? PropertySort.Alphabetical : PropertySort.CategorizedAlphabetical;
			grid.Dock = DockStyle.Fill;

			comboBox = new ComboBox();
			comboBox.Dock = DockStyle.Top;
			comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBox.DrawMode = DrawMode.OwnerDrawFixed;
			comboBox.Sorted = true;
			
			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += WorkbenchWindowChanged;
		}
		
		static void WorkbenchWindowChanged(object sender, EventArgs e)
		{
			IHasPropertyContainer c = WorkbenchSingleton.Workbench.ActiveContent as IHasPropertyContainer;
			if (c == null) return;
			SetActiveContainer(c.PropertyContainer);
		}
		
		public PropertyPad()
		{
			instance = this;
			panel = new Panel();
						
			comboBox.DrawItem += new DrawItemEventHandler(ComboBoxDrawItem);
			comboBox.MeasureItem += new MeasureItemEventHandler(ComboBoxMeasureItem);
			comboBox.SelectedIndexChanged += new EventHandler(ComboBoxSelectedIndexChanged);
		
			
			panel.Controls.Add(grid);
			panel.Controls.Add(comboBox);
			
			ProjectService.SolutionClosed += new EventHandler(CombineClosedEvent);
			
			grid.PropertyValueChanged += new PropertyValueChangedEventHandler(PropertyChanged);
			grid.ContextMenuStrip = MenuService.CreateContextMenu(this, "/SharpDevelop/Views/PropertyPad/ContextMenu");
		}
		
		void CombineClosedEvent(object sender, EventArgs e)
		{
			SetDesignableObjects(null);
		}
		
		void ComboBoxMeasureItem(object sender, MeasureItemEventArgs mea)
		{
			if (mea.Index < 0 || mea.Index >= comboBox.Items.Count) {
				mea.ItemHeight = comboBox.Font.Height;
				return;
			}
			object item = comboBox.Items[mea.Index];
			SizeF size = mea.Graphics.MeasureString(item.GetType().ToString(), comboBox.Font);
			
			mea.ItemHeight = (int)size.Height;
			mea.ItemWidth  = (int)size.Width;
			
			if (item is IComponent) {
				ISite site = ((IComponent)item).Site;
				if (site != null) {	
					string name = site.Name;
					Font f = new Font(comboBox.Font, FontStyle.Bold);
					mea.ItemWidth += (int)mea.Graphics.MeasureString(name + "-", f).Width;
				}
			}
		}
		
		void ComboBoxDrawItem(object sender, DrawItemEventArgs dea)
		{
			if (dea.Index < 0 || dea.Index >= comboBox.Items.Count) {
				return;
			}
			Graphics g = dea.Graphics;
			Brush stringColor = SystemBrushes.ControlText;
			
			if ((dea.State & DrawItemState.Selected) == DrawItemState.Selected) {
				if ((dea.State & DrawItemState.Focus) == DrawItemState.Focus) {
					g.FillRectangle(SystemBrushes.Highlight, dea.Bounds);
					stringColor = SystemBrushes.HighlightText;
				} else {
					g.FillRectangle(SystemBrushes.Window, dea.Bounds);
				}
			} else {
				g.FillRectangle(SystemBrushes.Window, dea.Bounds);
			}
			
			object item = comboBox.Items[dea.Index];
			int   xPos  = dea.Bounds.X;
			
			if (item is IComponent) {
				ISite site = ((IComponent)item).Site;
				if (site != null) {	
					string name = site.Name;
					Font f = new Font(comboBox.Font, FontStyle.Bold);
					g.DrawString(name, f, stringColor, xPos, dea.Bounds.Y);
					xPos += (int)g.MeasureString(name + "-", f).Width;
				}
			}
			
			string typeString = item.GetType().ToString();
			g.DrawString(typeString, comboBox.Font, stringColor, xPos, dea.Bounds.Y);
		}
		static bool inUpdate = false;
		
		void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!inUpdate) {
				if (host!=null) {
					ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
					if (comboBox.SelectedIndex >= 0) {
						selectionService.SetSelectedComponents(new object[] {comboBox.Items[comboBox.SelectedIndex] });
          				} else {
						SetDesignableObject(null);
						selectionService.SetSelectedComponents(new object[] { });
					}
				}
				if (SelectedObjectChanged != null) {
					SelectedObjectChanged(this, EventArgs.Empty);
				}
			}
		}
		
		static void SelectedObjectsChanged()
		{
			if (grid.SelectedObjects != null && grid.SelectedObjects.Length == 1) {
				for (int i = 0; i < comboBox.Items.Count; ++i) {
					if (grid.SelectedObject == comboBox.Items[i]) {
						comboBox.SelectedIndex = i;
					}
				}
			} else {
				comboBox.SelectedIndex = -1;
			}
		}
		
		public override void RedrawContent()
		{
			grid.Refresh();
		}
		
		public override void Dispose()
		{
			base.Dispose();
			if (grid != null) {
				try {
					grid.SelectedObjects = null;
				} catch {}
				grid.Dispose();
				grid = null;
			}
		}
		
		static void SetDesignableObject(object obj)
		{
			if (grid != null) {
				grid.SelectedObject  = obj;
				SelectedObjectsChanged();
			}
		}
		
		static void SetDesignableObjects(object[] obj)
		{
			if (grid != null) {
				grid.SelectedObjects = obj;
				SelectedObjectsChanged();
			}
		}
		
		static void RemoveHost(IDesignerHost host)
		{
			PropertyPad.host = null;
			grid.Site = null;
			
			ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
			if (selectionService != null) {
				selectionService.SelectionChanging -= new EventHandler(SelectionChangingHandler);
				selectionService.SelectionChanged  -= new EventHandler(SelectionChangedHandler);
			}
			
			host.TransactionClosed -= new DesignerTransactionCloseEventHandler(TransactionClose);
			
			IComponentChangeService componentChangeService = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
			if (componentChangeService != null) {
				componentChangeService.ComponentAdded   -= new ComponentEventHandler(UpdateSelectedObjects);
				componentChangeService.ComponentRemoved -= new ComponentEventHandler(UpdateSelectedObjects);
				componentChangeService.ComponentRename  -= new ComponentRenameEventHandler(UpdateSelectedObjectsOnRename);
			}
		}
		
		static void SetDesignerHost(IDesignerHost host)
		{
			PropertyPad.host = host;
			if (host != null) {
				grid.Site = (new IDEContainer(host)).CreateSite(grid);
				grid.PropertyTabs.AddTabType(typeof(System.Windows.Forms.Design.EventsTab), PropertyTabScope.Document);
			
				ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
				if (selectionService != null) {
					selectionService.SelectionChanging += new EventHandler(SelectionChangingHandler);
					selectionService.SelectionChanged  += new EventHandler(SelectionChangedHandler);
				}
			
				host.TransactionClosed += new DesignerTransactionCloseEventHandler(TransactionClose);
			
				IComponentChangeService componentChangeService = (IComponentChangeService)host.GetService(typeof(IComponentChangeService));
				if (componentChangeService != null) {
					componentChangeService.ComponentAdded   += new ComponentEventHandler(UpdateSelectedObjects);
					componentChangeService.ComponentRemoved += new ComponentEventHandler(UpdateSelectedObjects);
					componentChangeService.ComponentRename  += new ComponentRenameEventHandler(UpdateSelectedObjectsOnRename);
				}
			} else {
				grid.Site = null;
			}
		}
		
		static void SetSelectableObjects(ICollection coll)
		{
			inUpdate = true;
			try {
				comboBox.Items.Clear();
				if (coll != null) {
					foreach (object obj in coll) {
						comboBox.Items.Add(obj);
					}
				}
				SelectedObjectsChanged();
			} finally {
				inUpdate = false;
			}
		}
		
		
		
		#region ICSharpCode.SharpDevelop.Gui.IHelpProvider interface implementation
		public void ShowHelp()
		{
			if (host != null) {
				IHelpService helpService = (IHelpService)host.GetService(typeof(IHelpService));
				helpService.ShowHelpFromKeyword(null);
			}
		}
		#endregion
		
		
		static bool shouldUpdateSelectableObjects = false;
		static void TransactionClose(object sender, DesignerTransactionCloseEventArgs e)
		{
			if (shouldUpdateSelectableObjects) {
				if (host != null) {
					SetSelectableObjects(host.Container.Components);
				}
				shouldUpdateSelectableObjects = false;
			}
			
		}
		
		static void UpdateSelectedObjects(object sender, ComponentEventArgs e)
		{
			shouldUpdateSelectableObjects = true;
		}
		
		static void UpdateSelectedObjectsOnRename(object sender, ComponentRenameEventArgs e)
		{
			shouldUpdateSelectableObjects = true;
		}
		
		static void SelectionChangingHandler(object sender, EventArgs args)
		{
		}

		static void SelectionChangedHandler(object sender, EventArgs args)
		{
			ISelectionService selectionService = sender as ISelectionService;
			if (selectionService != null) {
				ICollection selection = selectionService.GetSelectedComponents();
				object[] selArray = new object[selection.Count];
				selection.CopyTo(selArray, 0);
				
				inUpdate = true;
				try {
					grid.SelectedObjects = selArray;
					
					SelectedObjectsChanged();
				} catch (Exception) {
					
				} finally {
					inUpdate = false;
				}
			}
		}
		
		void PropertyChanged(object sender, PropertyValueChangedEventArgs e)
		{
			OnPropertyValueChanged(sender, e);
		}
		
		void OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
		{
			if(PropertyValueChanged != null) {
				PropertyValueChanged(sender, e);
			}
		}
	}
}
