// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;

namespace NoGoop.ObjBrowser.GuiDesigner
{
	public class ToolboxService : IToolboxService
	{
		protected String _selectedCategory;
		
		protected void ComponentsCreated(Object sender, ToolboxComponentsCreatedEventArgs e)
		{
			foreach (IComponent c in e.Components) {
				if (c == null)
					continue;
				ObjectBrowser.GetTopLevelObjectNode().AddNewObject(c, null);
			}
		}
		public void AddCreator(ToolboxItemCreatorCallback cb, String str, IDesignerHost host)
		{
			//Console.WriteLine("TBS:addCreator ");
		}
		
		public void AddCreator(ToolboxItemCreatorCallback cb, String str)
		{
			//Console.WriteLine("TBS:addCreator ");
		}
		
		public void AddLinkedToolboxItem(ToolboxItem item, String name, IDesignerHost host)
		{
			//Console.WriteLine("TBS:AddLinkedToolBoxItem ");
		}
		
		public void AddLinkedToolboxItem(ToolboxItem item, IDesignerHost host)
		{
			//Console.WriteLine("TBS:AddLinkedToolBoxItem ");
		}
		
		public void AddToolboxItem(ToolboxItem item, String name)
		{
			//Console.WriteLine("TBS:AddToolBoxItem ");
		}
		
		public void AddToolboxItem(ToolboxItem item)
		{
			//Console.WriteLine("TBS:AddToolBoxItem ");
		}
		
		public ToolboxItem DeserializeToolboxItem(Object obj)
		{
			return DeserializeToolboxItem(obj, null);
		}
		
		// We don't really deserialize a tool box item, we fabricate
		// it is the drag source is a node that has a type.  Otherwise
		// we just return null indicating this is not from the toolbox
		public ToolboxItem DeserializeToolboxItem(Object obj, IDesignerHost host)
		{
			//Console.WriteLine("TBS:DeserializeToolBoxItem " + obj);
			if (!(obj is IDataObject))
				return null;
			IDataObject data = (IDataObject)obj;
			bool found = false;
			IDragDropItem sourceNode = null;
			// Look for a node that represents a type, this is either
			// a constructor or a type node
			foreach (Type t in BrowserTree.DragSourceTypes) {
				sourceNode = (IDragDropItem)data.GetData(t);
				if (sourceNode != null && sourceNode is ITargetType) {
					found = true;
					break;
				}
			}
			if (!found)
				return null;
			Type type = ((ITargetType)sourceNode).Type;
			if (!(typeof(Control).IsAssignableFrom(type)))
				return null;
			IDesignSurfaceNode dNode = (IDesignSurfaceNode)sourceNode;
			if (!dNode.OnDesignSurface)
				return null;
			ToolboxItem ti = new ToolboxItem(type);
			ti.ComponentsCreated += new ToolboxComponentsCreatedEventHandler(ComponentsCreated);
			return ti;
		}
		
		public ToolboxItem GetSelectedToolboxItem()
		{
			//Console.WriteLine("TBS:GetSelectedToolboxItem ");
			return null;
		}
		
		public ToolboxItem GetSelectedToolboxItem(IDesignerHost host)
		{
			//Console.WriteLine("TBS:GetSelectedToolboxItem ");
			return null;
		}
		
		public ToolboxItemCollection GetToolboxItems(String name)
		{
			//Console.WriteLine("TBS:GetToolboxItems");
			return null;
		}
		
		public ToolboxItemCollection GetToolboxItems(String name, IDesignerHost host)
		{
			//Console.WriteLine("TBS:GetToolboxItems");
			return null;
		}
		
		public ToolboxItemCollection GetToolboxItems(IDesignerHost host)
		{
			//Console.WriteLine("TBS:GetToolboxItems");
			return null;
		}
		
		public ToolboxItemCollection GetToolboxItems()
		{
			//Console.WriteLine("TBS:GetToolboxItems");
			return null;
		}
		
		public bool IsSupported(Object obj, ICollection col)
		{
			//Console.WriteLine("TBS:IsSupported ");
			return false;
		}
		
		public bool IsSupported(Object obj, IDesignerHost host)
		{
			//Console.WriteLine("TBS:IsSupported ");
			return false;
		}
		
		public bool IsToolboxItem(Object obj)
		{
			//Console.WriteLine("TBS:IsToolboxItem ");
			return true;
		}
		
		public bool IsToolboxItem(Object obj, IDesignerHost host)
		{
			//Console.WriteLine("TBS:IsToolboxItem ");
			return true;
		}
		
		public void Refresh()
		{
			//Console.WriteLine("TBS:Refresh ");
		}
		
		public void RemoveCreator(String name)
		{
			//Console.WriteLine("TBS:RemoveCreator ");
		}
		
		public void RemoveCreator(String name, IDesignerHost host)
		{
			//Console.WriteLine("TBS:RemoveCreator ");
		}
		
		public void RemoveToolboxItem(ToolboxItem item)
		{
			//Console.WriteLine("TBS:RemoveToolboxItem ");
		}
		
		public void RemoveToolboxItem(ToolboxItem item,
									 String str)
		{
			//Console.WriteLine("TBS:RemoveToolboxItem ");
		}
		
		public void RemoveToolboxItem(ToolboxItem item, IDesignerHost host)
		{
			//Console.WriteLine("TBS:RemoveToolboxItem ");
		}
		
		public void SelectedToolboxItemUsed()
		{
			//Console.WriteLine("TBS:SelectedToolboxItemUsed ");
		}
		
		public Object SerializeToolboxItem(ToolboxItem item)
		{
			//Console.WriteLine("TBS:SelectedToolboxItemUsed ");
			return item;
		}
		
		public bool SetCursor()
		{
			//Console.WriteLine("TBS:SetCursor");
			return false;
		}
		
		public void SetSelectedToolboxItem(ToolboxItem item)
		{
			//Console.WriteLine("TBS:SetSelectedToolboxItem");
		}
		
		public CategoryNameCollection CategoryNames {
			get {
				return null;
			}
		}
		
		public String SelectedCategory {
			get {
				return _selectedCategory;
			}
			set {
				_selectedCategory = value;
			}
		}
	}
}
