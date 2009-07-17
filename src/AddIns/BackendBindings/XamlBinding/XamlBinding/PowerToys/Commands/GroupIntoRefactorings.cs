// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.XamlBinding.PowerToys.Commands
{
	public class GroupIntoBorderBuilder : AbstractMenuItemBuilder
	{
		public override ICollection BuildItems(Codon codon, object owner)
		{
			List<MenuItem> items = new List<MenuItem>();
			
			items.Add(CreateItem("Border", delegate { new GroupIntoBorderWithoutChild().Run(); }));
			items.Add(CreateItem("Border With Root Grid", delegate { new GroupIntoBorderWithGrid().Run(); }));
			items.Add(CreateItem("Border With Root StackPanel - Vertical", delegate { new GroupIntoBorderWithStackPanelVertical().Run(); }));
			items.Add(CreateItem("Border With Root StackPanel - Horizontal", delegate { new GroupIntoBorderWithStackPanelHorizontal().Run(); }));
			
			return items.OrderBy(item => item.Header).ToList();
		}
	}
	
	class GroupIntoBorderWithoutChild : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("Border", CompletionDataHelper.WpfXamlNamespace));
		}
	}
	
	class GroupIntoBorderWithGrid : GroupIntoBase
	{
		protected override bool GroupInto(XElement parent, IEnumerable<XElement> children)
		{
			XElement newParent = new XElement(XName.Get("Grid", CompletionDataHelper.WpfXamlNamespace));
			XElement newItem = new XElement(XName.Get("Border", CompletionDataHelper.WpfXamlNamespace), newParent);
			
			newParent.Add(children);
			parent.Add(newItem);
			
			foreach (var child in children)
				child.Remove();
			
			return true;
		}
	}
	
	class GroupIntoBorderWithStackPanelVertical : GroupIntoBase
	{
		protected override bool GroupInto(XElement parent, IEnumerable<XElement> children)
		{
			XElement newParent = new XElement(XName.Get("StackPanel", CompletionDataHelper.WpfXamlNamespace));
			XElement newItem = new XElement(XName.Get("Border", CompletionDataHelper.WpfXamlNamespace), newParent);
			
			newParent.SetAttributeValue("Orientation", "Vertical");
			
			newParent.Add(children);
			parent.Add(newItem);
			
			foreach (var child in children)
				child.Remove();
			
			return true;
		}
	}
	
	class GroupIntoBorderWithStackPanelHorizontal : GroupIntoBase
	{
		protected override bool GroupInto(XElement parent, IEnumerable<XElement> children)
		{
			XElement newParent = new XElement(XName.Get("StackPanel", CompletionDataHelper.WpfXamlNamespace));
			XElement newItem = new XElement(XName.Get("Border", CompletionDataHelper.WpfXamlNamespace), newParent);
			
			newParent.SetAttributeValue("Orientation", "Horizontal");
			
			newParent.Add(children);
			parent.Add(newItem);
			
			foreach (var child in children)
				child.Remove();
			
			return true;
		}
	}

	public abstract class AbstractMenuItemBuilder : IMenuItemBuilder
	{
		public abstract ICollection BuildItems(Codon codon, object owner);
		
		protected MenuItem CreateItem(string header, Action clickAction)
		{
			MenuItem item = new MenuItem() { Header = header };
			item.Click += new RoutedEventHandler(delegate { clickAction(); });
			return item;
		}
	}

	abstract class GroupIntoBase : XamlMenuCommand
	{
		protected override bool Refactor(ITextEditor editor, XDocument document)
		{
			if (editor.SelectionLength == 0) {
				MessageService.ShowError("Nothing selected!");
				return false;
			}
			
			Location startLoc = editor.Document.OffsetToPosition(editor.SelectionStart);
			Location endLoc = editor.Document.OffsetToPosition(editor.SelectionStart + editor.SelectionLength);
			
			var selectedItems = (from item in document.Root.Descendants()
			                     where item.IsInRange(startLoc, endLoc) select item).ToList();
			
			if (selectedItems.Any()) {
				var parent = selectedItems.First().Parent;
				var items = selectedItems.Where(i => i.Parent == parent);
				
				return GroupInto(parent, items);
			}
			
			return false;
		}
		
		protected abstract bool GroupInto(XElement parent, IEnumerable<XElement> children);
	}

	abstract class SimpleGroupIntoBase : GroupIntoBase
	{
		protected sealed override bool GroupInto(XElement parent, IEnumerable<XElement> children)
		{
			XElement newItem = CreateParent();
			
			newItem.Add(children);
			parent.Add(newItem);
			
			foreach (var child in children)
				child.Remove();
			
			return true;
		}
		
		protected abstract XElement CreateParent();
	}

	public class GroupIntoBuilder : AbstractMenuItemBuilder
	{
		public override ICollection BuildItems(Codon codon, object owner)
		{
			List<MenuItem> items = new List<MenuItem>();
			
			items.Add(CreateItem("Grid", delegate { new GroupIntoGrid().Run(); }));
			items.Add(CreateItem("Canvas", delegate { new GroupIntoCanvas().Run(); }));
			items.Add(CreateItem("DockPanel", delegate { new GroupIntoDockPanel().Run(); }));
			items.Add(CreateItem("ScrollViewer With Root Grid", delegate { new GroupIntoScrollViewerWithGrid().Run(); }));
			items.Add(CreateItem("StackPanel - Vertical", delegate { new GroupIntoStackPanelVertical().Run(); }));
			items.Add(CreateItem("StackPanel - Horizontal", delegate { new GroupIntoStackPanelHorizontal().Run(); }));
			items.Add(CreateItem("UniformGrid", delegate { new GroupIntoUniformGrid().Run(); }));
			items.Add(CreateItem("Viewbox", delegate { new GroupIntoViewbox().Run(); }));
			items.Add(CreateItem("WrapPanel", delegate { new GroupIntoWrapPanel().Run(); }));
			items.Add(CreateItem("GroupBox With Root Grid", delegate { new GroupIntoGroupBoxWithGrid().Run(); }));
			
			return items.OrderBy(item => item.Header).ToList();
		}
	}
	
	class GroupIntoGrid : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("Grid", CompletionDataHelper.WpfXamlNamespace));
		}
	}
	
	class GroupIntoCanvas : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("Canvas", CompletionDataHelper.WpfXamlNamespace));
		}
	}
	
	class GroupIntoDockPanel : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("DockPanel", CompletionDataHelper.WpfXamlNamespace));
		}
	}
	
	class GroupIntoUniformGrid : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("UniformGrid", CompletionDataHelper.WpfXamlNamespace));
		}
	}
	
	class GroupIntoViewbox : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("Viewbox", CompletionDataHelper.WpfXamlNamespace));
		}
	}
	
	class GroupIntoWrapPanel : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("WrapPanel", CompletionDataHelper.WpfXamlNamespace));
		}
	}
	
	class GroupIntoStackPanelVertical : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			var element = new XElement(XName.Get("StackPanel", CompletionDataHelper.WpfXamlNamespace));
			element.SetAttributeValue("Orientation", "Vertical");
			return element;
		}
	}
	
	class GroupIntoStackPanelHorizontal : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			var element = new XElement(XName.Get("StackPanel", CompletionDataHelper.WpfXamlNamespace));
			element.SetAttributeValue("Orientation", "Horizontal");
			return element;
		}
	}
	
	class GroupIntoScrollViewerWithGrid : GroupIntoBase
	{
		protected override bool GroupInto(XElement parent, IEnumerable<XElement> children)
		{
			XElement newParent = new XElement(XName.Get("Grid", CompletionDataHelper.WpfXamlNamespace));
			XElement newItem = new XElement(XName.Get("ScrollViewer", CompletionDataHelper.WpfXamlNamespace), newParent);
			
			newParent.Add(children);
			parent.Add(newItem);
			
			foreach (var child in children)
				child.Remove();
			
			return true;
		}
	}
	
	class GroupIntoGroupBoxWithGrid : GroupIntoBase
	{
		protected override bool GroupInto(XElement parent, IEnumerable<XElement> children)
		{
			XElement newParent = new XElement(XName.Get("Grid", CompletionDataHelper.WpfXamlNamespace));
			XElement newItem = new XElement(XName.Get("GroupBox", CompletionDataHelper.WpfXamlNamespace), newParent);
			
			newParent.Add(children);
			parent.Add(newItem);
			
			foreach (var child in children)
				child.Remove();
			
			return true;
		}
	}
}
