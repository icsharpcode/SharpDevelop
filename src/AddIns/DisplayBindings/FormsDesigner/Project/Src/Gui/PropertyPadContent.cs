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

namespace ICSharpCode.FormsDesigner.Gui
{
	public class PropertyPadContent : Control
	{
		ComboBox comboBox;
		PropertyGrid grid;
		
		readonly IDEContainer ideContainer;
		
		public PropertyGrid Grid {
			get { return grid; }
		}
		
		IDesignerHost host;
		
		public IDesignerHost Host {
			get { return host; }
		}
		
		object selectedObject;
		object[] selectedObjects;
		
		public object SelectedObject {
			get {
				return selectedObject;
			}
			set {
				selectedObject = value;
				selectedObjects = null;
				SetDesignableObject(selectedObject);
			}
		}
		
		public object[] SelectedObjects {
			get {
				return selectedObjects;
			}
			set {
				selectedObject = null;
				selectedObjects = value;
				SetDesignableObjects(selectedObjects);
			}
		}
		
		ICollection selectableObjects;
		
		public ICollection SelectableObjects {
			get {
				return selectableObjects;
			}
			set {
				selectableObjects = value;
				SetSelectableObjects(selectableObjects);
			}
		}
		
		public event PropertyValueChangedEventHandler PropertyValueChanged;
		public event EventHandler SelectedObjectChanged;
		public event SelectedGridItemChangedEventHandler SelectedGridItemChanged;
		
		public PropertyPadContent(IDesignerHost host, IFormsDesigner designer, FormsDesignerAppDomainHost appDomainHost)
		{
			ideContainer = new IDEContainer(appDomainHost);
			
			grid = new PropertyGrid();
			grid.PropertySort = designer.DesignerOptions.PropertyGridSortAlphabetical ? PropertySort.Alphabetical : PropertySort.CategorizedAlphabetical;
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
			
			Controls.Add(grid);
			Controls.Add(comboBox);
			
//			ProjectService.SolutionClosed += SolutionClosedEvent;
			
			grid.PropertyValueChanged += new PropertyValueChangedEventHandler(PropertyChanged);
//			grid.ContextMenuStrip = MenuService.CreateContextMenu(this, "/SharpDevelop/Views/PropertyPad/ContextMenu");
			
			SetDesignerHost(host);
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
			
			object item = comboBox.Items[dea.Index];
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
			}
		}
		
		void SelectedObjectsChanged()
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
		
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (grid != null) {
				try {
					grid.SelectedObjects = null;
				} catch {}
				grid.Dispose();
				grid = null;
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
		
		void SetSelectableObjects(ICollection coll)
		{
			inUpdate = true;
			try {
				comboBox.Items.Clear();
				if (coll != null) {
					foreach (object obj in SortObjectsBySiteName(coll)) {
						comboBox.Items.Add(obj);
					}
				}
				SelectedObjectsChanged();
			} finally {
				inUpdate = false;
			}
		}
		
		#region ICSharpCode.SharpDevelop.Gui.IHelpProvider interface implementation
//		public void ShowHelp()
//		{
//			LoggingService.Info("Show help on property pad");
//			GridItem gridItem = grid.SelectedGridItem;
//			if (gridItem != null) {
//				Type component = gridItem.PropertyDescriptor.ComponentType;
//				if (component != null) {
//					ICSharpCode.SharpDevelop.Dom.IClass c = ParserService.CurrentProjectContent.GetClass(component.FullName, 0);
//					if (c != null) {
//						foreach (ICSharpCode.SharpDevelop.Dom.IProperty p in c.DefaultReturnType.GetProperties()) {
//							if (gridItem.PropertyDescriptor.Name == p.Name) {
//								HelpProvider.ShowHelp(p);
//								return;
//							}
//						}
//						HelpProvider.ShowHelp(c);
//					}
//				}
//			}
//		}
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
