// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class PropertyPad : AbstractPadContent, IContextHelpProvider
	{
		static PropertyPad instance;
		
		// an empty container used to reset the property grid
		readonly PropertyContainer emptyContainer = new PropertyContainer(false);
		
		// The IDE container used to connect the grid to a designer host
		readonly IDEContainer ideContainer = new IDEContainer();
		
		PropertyContainer activeContainer;
		
		internal static PropertyContainer ActiveContainer {
			get { return instance.activeContainer; }
		}
		
		void SetActiveContainer(PropertyContainer pc)
		{
			if (activeContainer == pc)
				return;
			if (pc == null)
				pc = emptyContainer;
			activeContainer = pc;
			
			UpdateHostIfActive(pc);
			UpdateSelectedObjectIfActive(pc);
			UpdateSelectableIfActive(pc);
			UpdatePropertyGridReplacementContent(pc);
		}
		
		internal static void UpdateSelectedObjectIfActive(PropertyContainer container)
		{
			if (instance == null) return;
			if (instance.activeContainer != container)
				return;
			//LoggingService.Debug("UpdateSelectedObjectIfActive");
			if (container.SelectedObjects != null)
				instance.SetDesignableObjects(container.SelectedObjects);
			else
				instance.SetDesignableObject(container.SelectedObject);
		}
		
		internal static void UpdateHostIfActive(PropertyContainer container)
		{
			if (instance == null) return;
			if (instance.activeContainer != container)
				return;
			//LoggingService.Debug("UpdateHostIfActive");
			if (instance.host == container.Host)
				return;
			if (instance.host != null)
				instance.RemoveHost(instance.host);
			if (container.Host != null)
				instance.SetDesignerHost(container.Host);
		}
		
		internal static void UpdateSelectableIfActive(PropertyContainer container)
		{
			if (instance == null) return;
			if (instance.activeContainer != container)
				return;
			//LoggingService.Debug("UpdateSelectableIfActive");
			instance.SetSelectableObjects(container.SelectableObjects);
		}
		
		internal static void UpdatePropertyGridReplacementContent(PropertyContainer container)
		{
			if (instance == null) return;
			if (instance.activeContainer != container)
				return;
			//LoggingService.Debug("UpdatePropertyGridReplacementControl");
			if (container.PropertyGridReplacementContent != null) {
				instance.contentControl.SetContent(container.PropertyGridReplacementContent);
			} else {
				instance.contentControl.SetContent(instance.panel);
			}
		}
		
		System.Windows.Controls.ContentPresenter contentControl;
		Panel panel;
		ComboBox comboBox;
		PropertyGrid grid;
		IDesignerHost host;
		
		/// <summary>
		/// Gets the underlying property grid. Returns null if the property pad has not yet been created.
		/// </summary>
		public static PropertyGrid Grid {
			get {
				if (instance == null)
					return null;
				else
					return instance.grid;
			}
		}
		
		public static event PropertyValueChangedEventHandler PropertyValueChanged;
		public static event EventHandler SelectedObjectChanged;
		public static event SelectedGridItemChangedEventHandler SelectedGridItemChanged;
		
		public override object Control {
			get {
				return contentControl;
			}
		}
		
		IHasPropertyContainer previousContent;
		
		void WorkbenchActiveContentChanged(object sender, EventArgs e)
		{
			IHasPropertyContainer c = WorkbenchSingleton.Workbench.ActiveContent as IHasPropertyContainer;
			if (c == null) {
				if (previousContent == null) {
					c = WorkbenchSingleton.Workbench.ActiveViewContent as IHasPropertyContainer;
				} else {
					// if the previous content is no longer visible, we have to remove the active container
					if (previousContent is IViewContent && previousContent != WorkbenchSingleton.Workbench.ActiveViewContent) {
						c = null;
					} else {
						c = previousContent;
					}
				}
			}
			if (c != null) {
				SetActiveContainer(c.PropertyContainer);
			} else {
				SetActiveContainer(null);
			}
			previousContent = c;
		}
		
		public PropertyPad()
		{
			instance = this;
			contentControl = new System.Windows.Controls.ContentPresenter();
			panel = new Panel();
			
			grid = new PropertyGrid();
			grid.PropertySort = PropertyService.Get("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", false) ? PropertySort.Alphabetical : PropertySort.CategorizedAlphabetical;
			grid.Dock = DockStyle.Fill;
			
			grid.SelectedObjectsChanged += delegate(object sender, EventArgs e) {
				if (SelectedObjectChanged != null)
					SelectedObjectChanged(sender, e);
			};
			grid.SelectedGridItemChanged += delegate(object sender, SelectedGridItemChangedEventArgs e) {
				if (SelectedGridItemChanged != null)
					SelectedGridItemChanged(sender, e);
			};
			
			comboBox = new ComboBox();
			comboBox.Dock = DockStyle.Top;
			comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBox.DrawMode = DrawMode.OwnerDrawFixed;
			comboBox.Sorted = false;
			
			comboBox.DrawItem += new DrawItemEventHandler(ComboBoxDrawItem);
			comboBox.MeasureItem += new MeasureItemEventHandler(ComboBoxMeasureItem);
			comboBox.SelectedIndexChanged += new EventHandler(ComboBoxSelectedIndexChanged);
			
			panel.Controls.Add(grid);
			panel.Controls.Add(comboBox);
			contentControl.SetContent(panel);
			
			ProjectService.SolutionClosed += SolutionClosedEvent;
			
			grid.PropertyValueChanged += new PropertyValueChangedEventHandler(PropertyChanged);
			grid.ContextMenuStrip = MenuService.CreateContextMenu(this, "/SharpDevelop/Views/PropertyPad/ContextMenu");
			
			LoggingService.Debug("PropertyPad created");
			WorkbenchSingleton.Workbench.ActiveContentChanged += WorkbenchActiveContentChanged;
			// it is possible that ActiveContent changes fires before ActiveViewContent.
			// if the new content is not a IHasPropertyContainer and we listen only to ActiveContentChanged,
			// we might display the PropertyPad of a no longer active view content
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveContentChanged;
			WorkbenchActiveContentChanged(null, null);
		}
		
		/// <summary>
		/// Returns a new collection of objects sorted by their ISite name otherwise by their type name.
		/// </summary>
		public static ICollection SortObjectsBySiteName(ICollection objects)
		{
			List<object> unsortedObjects = new List<object>();
			foreach (object o in objects) {
				unsortedObjects.Add(o);
			}
			unsortedObjects.Sort(CompareObjectsBySiteName);
			return unsortedObjects.ToArray();
		}
		
		/// <summary>
		/// Compares two objects by their ISite name, otherwise by their type name.
		/// </summary>
		static int CompareObjectsBySiteName(object x, object y)
		{
			return String.Compare(GetObjectSiteName(x), GetObjectSiteName(y));
		}

		/// <summary>
		/// Gets the site name otherwise the name of the type.
		/// </summary>
		static string GetObjectSiteName(object o)
		{
			if(o != null) {
				IComponent component = o as IComponent;
				if (component != null) {
					ISite site = component.Site;
					if (site != null) {
						return site.Name;
					}
				}
				return o.GetType().ToString();
			}
			return String.Empty;
		}
		
		void SolutionClosedEvent(object sender, EventArgs e)
		{
			SetDesignableObjects(null);
		}
		
		/// <summary>
		/// Wrap objects in the combobox in order to replace their ToString() implementation.
		/// </summary>
		class ComboBoxItemWrapper
		{
			// This class is important because the default ToString() for some WinForms controls throws exceptions in some circumstances.
			// e.g. System.NullReferenceException
			// in System.Windows.Forms.ToolStripControlHost.get_Text()
			// in System.Windows.Forms.ToolStripItem.ToString()
			// in System.Convert.ToString(Object value, IFormatProvider provider)
			// in System.Windows.Forms.ListControl.GetItemText(Object item)
			// in System.Windows.Forms.ComboBox.NativeAdd(Object item)
			// in System.Windows.Forms.ComboBox.ObjectCollection.AddInternal(Object item)
			// in System.Windows.Forms.ComboBox.ObjectCollection.Add(Object item)
			
			// We need this even though we're using owner drawing.
			
			public readonly object Item;
			
			public ComboBoxItemWrapper(object item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				this.Item = item;
			}
			
			public override int GetHashCode()
			{
				return Item.GetHashCode();
			}
			
			public override bool Equals(object obj)
			{
				ComboBoxItemWrapper o = obj as ComboBoxItemWrapper;
				return o != null && this.Item.Equals(o.Item);
			}
			
			public override string ToString()
			{
				IComponent component = Item as IComponent;
				if (component != null) {
					ISite site = component.Site;
					if (site != null) {
						return site.Name + " - " + Item.GetType().ToString();
					}
				}
				return Item.GetType().ToString();
			}
		}
		
		object GetComboBoxItem(int index)
		{
			return ((ComboBoxItemWrapper)comboBox.Items[index]).Item;
		}
		
		void ComboBoxMeasureItem(object sender, MeasureItemEventArgs mea)
		{
			if (mea.Index < 0 || mea.Index >= comboBox.Items.Count) {
				mea.ItemHeight = comboBox.Font.Height;
				return;
			}
			object item = GetComboBoxItem(mea.Index);
			SizeF size = mea.Graphics.MeasureString(item.GetType().ToString(), comboBox.Font);
			
			mea.ItemHeight = (int)size.Height;
			mea.ItemWidth  = (int)size.Width;
			
			if (item is IComponent) {
				ISite site = ((IComponent)item).Site;
				if (site != null) {
					string name = site.Name;
					using (Font f = new Font(comboBox.Font, FontStyle.Bold)) {
						mea.ItemWidth += (int)mea.Graphics.MeasureString(name + "-", f).Width;
					}
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
			
			object item = GetComboBoxItem(dea.Index);
			int   xPos  = dea.Bounds.X;
			
			if (item is IComponent) {
				ISite site = ((IComponent)item).Site;
				if (site != null) {
					string name = site.Name;
					using (Font f = new Font(comboBox.Font, FontStyle.Bold)) {
						g.DrawString(name, f, stringColor, xPos, dea.Bounds.Y);
						xPos += (int)g.MeasureString(name + "-", f).Width;
					}
				}
			}
			
			string typeString = item.GetType().ToString();
			g.DrawString(typeString, comboBox.Font, stringColor, xPos, dea.Bounds.Y);
		}
		bool inUpdate = false;
		
		void ComboBoxSelectedIndexChanged(object sender, EventArgs e)
		{
			if (!inUpdate && host!=null) {
				ISelectionService selectionService = (ISelectionService)host.GetService(typeof(ISelectionService));
				if (selectionService != null) {
					if (comboBox.SelectedIndex >= 0) {
						selectionService.SetSelectedComponents(new object[] { GetComboBoxItem(comboBox.SelectedIndex) });
					} else {
						SetDesignableObject(null);
						selectionService.SetSelectedComponents(new object[] { });
					}
				}
			}
		}
		
		void SelectedObjectsChanged()
		{
			if (grid.SelectedObjects != null && grid.SelectedObjects.Length == 1) {
				for (int i = 0; i < comboBox.Items.Count; ++i) {
					if (grid.SelectedObject == GetComboBoxItem(i)) {
						comboBox.SelectedIndex = i;
					}
				}
			} else {
				comboBox.SelectedIndex = -1;
			}
		}
		
		public override void Dispose()
		{
			base.Dispose();
			if (grid != null) {
				this.ideContainer.Disconnect();
				ProjectService.SolutionClosed -= SolutionClosedEvent;
				try {
					grid.SelectedObjects = null;
				} catch {}
				grid.Dispose();
				grid = null;
				instance = null;
			}
		}
		
		void SetDesignableObject(object obj)
		{
			inUpdate = true;
			grid.SelectedObject  = obj;
			SelectedObjectsChanged();
			inUpdate = false;
		}
		
		void SetDesignableObjects(object[] obj)
		{
			inUpdate = true;
			grid.SelectedObjects = obj;
			SelectedObjectsChanged();
			inUpdate = false;
		}
		
		/// <summary>
		/// Refreshes the property pad if the specified item is active.
		/// </summary>
		public static void RefreshItem(object obj)
		{
			WorkbenchSingleton.AssertMainThread();
			if (instance != null && instance.grid.SelectedObjects.Contains(obj)) {
				instance.inUpdate = true;
				instance.grid.SelectedObjects = instance.grid.SelectedObjects;
				instance.inUpdate = false;
			}
		}
		
		void RemoveHost(IDesignerHost host)
		{
			this.host = null;
			this.ideContainer.Disconnect();
		}
		
		void SetDesignerHost(IDesignerHost host)
		{
			this.host = host;
			if (host != null) {
				this.ideContainer.ConnectGridAndHost(grid, host);
				grid.PropertyTabs.AddTabType(typeof(System.Windows.Forms.Design.EventsTab), PropertyTabScope.Document);
			} else {
				this.ideContainer.Disconnect();
			}
		}
		
		void SetSelectableObjects(ICollection coll)
		{
			inUpdate = true;
			try {
				comboBox.Items.Clear();
				if (coll != null) {
					foreach (object obj in SortObjectsBySiteName(coll)) {
						comboBox.Items.Add(new ComboBoxItemWrapper(obj));
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
			LoggingService.Info("Show help on property pad");
			GridItem gridItem = grid.SelectedGridItem;
			if (gridItem != null) {
				Type component = gridItem.PropertyDescriptor.ComponentType;
				if (component != null) {
					ICSharpCode.SharpDevelop.Dom.IClass c = ParserService.CurrentProjectContent.GetClass(component.FullName, 0);
					if (c != null) {
						foreach (ICSharpCode.SharpDevelop.Dom.IProperty p in c.DefaultReturnType.GetProperties()) {
							if (gridItem.PropertyDescriptor.Name == p.Name) {
								HelpProvider.ShowHelp(p);
								return;
							}
						}
						HelpProvider.ShowHelp(c);
					}
				}
			}
		}
		#endregion
		
		void PropertyChanged(object sender, PropertyValueChangedEventArgs e)
		{
			OnPropertyValueChanged(sender, e);
		}
		
		void OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
		{
			if (PropertyValueChanged != null) {
				PropertyValueChanged(sender, e);
			}
		}
	}
}
