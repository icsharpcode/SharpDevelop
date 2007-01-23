// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Russell Wilkins" email=""/>
//     <version>$Revision$</version>
// </file>

#region Using
using System;
using System.Collections;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Workflow.Activities;
using ICSharpCode.Core;
using System.Reflection;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
#endregion

namespace WorkflowDesigner
{
	// TODO - Replace this a class based on System.Drawing.Design.ToolboxService
	public class WorkflowToolboxService : IToolboxService
	{
		private string category = "Workflow";
		
		public WorkflowToolboxService()
		{
		}
		
		public CategoryNameCollection CategoryNames {
			get {
				return new CategoryNameCollection(new string[] { "Workflow" });
			}
		}
		
		public string SelectedCategory {
			get {
				return category;
			}
			set {
				category = value;
			}
		}
		
		public void AddCreator(ToolboxItemCreatorCallback creator, string format)
		{
			throw new NotImplementedException();
		}
		
		public void AddCreator(ToolboxItemCreatorCallback creator, string format, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public void AddToolboxItem(ToolboxItem toolboxItem)
		{
			throw new NotImplementedException();
		}
		
		public void AddToolboxItem(ToolboxItem toolboxItem, string category)
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItem DeserializeToolboxItem(object serializedObject)
		{
			return DeserializeToolboxItem(serializedObject, null);
		}
		
		public ToolboxItem DeserializeToolboxItem(object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			IDataObject dataObject = (System.Windows.Forms.IDataObject)serializedObject;

			SharpDevelopSideTabItem sti = (SharpDevelopSideTabItem)dataObject.GetData(typeof(SharpDevelopSideTabItem));
			ToolboxItem toolboxItem = (ToolboxItem)sti.Tag;

			return toolboxItem;
		}
		
		public ToolboxItem GetSelectedToolboxItem()
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItem GetSelectedToolboxItem(System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItemCollection GetToolboxItems()
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItemCollection GetToolboxItems(System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItemCollection GetToolboxItems(string category)
		{
			throw new NotImplementedException();
		}
		
		public ToolboxItemCollection GetToolboxItems(string category, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public bool IsSupported(object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			return true;
		}
		
		public bool IsSupported(object serializedObject, System.Collections.ICollection filterAttributes)
		{
			return true;
		}
		
		public bool IsToolboxItem(object serializedObject)
		{
			throw new NotImplementedException();
		}
		
		public bool IsToolboxItem(object serializedObject, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public void Refresh()
		{
			throw new NotImplementedException();
		}
		
		public void RemoveCreator(string format)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveCreator(string format, System.ComponentModel.Design.IDesignerHost host)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveToolboxItem(ToolboxItem toolboxItem)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveToolboxItem(ToolboxItem toolboxItem, string category)
		{
			throw new NotImplementedException();
		}
		
		public void SelectedToolboxItemUsed()
		{
			throw new NotImplementedException();
		}
		
		public object SerializeToolboxItem(ToolboxItem toolboxItem)
		{
			throw new NotImplementedException();
		}
		
		public bool SetCursor()
		{
			throw new NotImplementedException();
		}
		
		public void SetSelectedToolboxItem(ToolboxItem toolboxItem)
		{
			throw new NotImplementedException();
		}
		
		
	}
//	public class ToolboxService2 : System.Drawing.Design.ToolboxService
//	{
//		private CategoryNameCollection categoryNameCollection = new CategoryNameCollection(new string[] {});
//		private string selectedCategory;
//		private ArrayList items = new ArrayList(); // TODO: replace by category item lists.
//
//		public ToolboxService2() : base()
//		{
//			foreach (Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
//			{
//				if (assembly.GetName().Name == "System.Workflow.Activities")
//				{
//					ICollection toolboxItems = System.Drawing.Design.ToolboxService.GetToolboxItems(assembly.GetName());
//					LoggingService.DebugFormatted("ToolboxItems count = {0}", toolboxItems.Count);
//
//					foreach (ToolboxItem tbi in toolboxItems)
//					{
//						((IToolboxService)this).AddToolboxItem(tbi);
//					}
//				}
//
//			}
//		}
//
//
//		protected override CategoryNameCollection CategoryNames {
//			get {
//				return categoryNameCollection;
//			}
//		}
//
//		protected override string SelectedCategory {
//			get {
//				return selectedCategory;
//			}
//			set {
//				selectedCategory = value;
//			}
//		}
//
//		protected override ToolboxItemContainer SelectedItemContainer {
//			get {
//				throw new NotImplementedException();
//			}
//			set {
//				throw new NotImplementedException();
//			}
//		}
//
//		protected override System.Collections.IList GetItemContainers()
//		{
//			return items;
//		}
//
//		protected override System.Collections.IList GetItemContainers(string categoryName)
//		{
//			return items;
//		}
//
//		protected override void Refresh()
//		{
//			throw new NotImplementedException();
//		}
//
//	}
}
