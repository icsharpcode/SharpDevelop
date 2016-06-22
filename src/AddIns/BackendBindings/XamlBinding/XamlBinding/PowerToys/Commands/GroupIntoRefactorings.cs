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
	public class GroupIntoBorderWithoutChild : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("Border", currentWpfNamespace));
		}
	}
	
	public class GroupIntoBorderWithGrid : GroupIntoBase
	{
		protected override bool GroupInto(XElement parent, IEnumerable<XElement> children)
		{
			XElement newParent = new XElement(XName.Get("Grid", currentWpfNamespace));
			XElement newItem = new XElement(XName.Get("Border", currentWpfNamespace), newParent);
			
			newParent.Add(children);
			parent.Add(newItem);
			
			foreach (var child in children)
				child.Remove();
			
			return true;
		}
	}
	
	public class GroupIntoBorderWithStackPanelVertical : GroupIntoBase
	{
		protected override bool GroupInto(XElement parent, IEnumerable<XElement> children)
		{
			XElement newParent = new XElement(XName.Get("StackPanel", currentWpfNamespace));
			XElement newItem = new XElement(XName.Get("Border", currentWpfNamespace), newParent);
			
			newParent.SetAttributeValue("Orientation", "Vertical");
			
			newParent.Add(children);
			parent.Add(newItem);
			
			foreach (var child in children)
				child.Remove();
			
			return true;
		}
	}
	
	public class GroupIntoBorderWithStackPanelHorizontal : GroupIntoBase
	{
		protected override bool GroupInto(XElement parent, IEnumerable<XElement> children)
		{
			XElement newParent = new XElement(XName.Get("StackPanel", currentWpfNamespace));
			XElement newItem = new XElement(XName.Get("Border", currentWpfNamespace), newParent);
			
			newParent.SetAttributeValue("Orientation", "Horizontal");
			
			newParent.Add(children);
			parent.Add(newItem);
			
			foreach (var child in children)
				child.Remove();
			
			return true;
		}
	}

	public abstract class GroupIntoBase : XamlMenuCommand
	{
		protected string currentWpfNamespace;
		
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
				
				currentWpfNamespace = parent.GetCurrentNamespaces()
					.First(i => CompletionDataHelper.WpfXamlNamespaces.Contains(i));
				
				var items = selectedItems.Where(i => i.Parent == parent);
				
				return GroupInto(parent, items);
			}
			
			return false;
		}
		
		protected abstract bool GroupInto(XElement parent, IEnumerable<XElement> children);
	}

	public abstract class SimpleGroupIntoBase : GroupIntoBase
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

	public class GroupIntoGrid : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("Grid", currentWpfNamespace));
		}
	}
	
	public class GroupIntoCanvas : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("Canvas", currentWpfNamespace));
		}
	}
	
	public class GroupIntoDockPanel : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("DockPanel", currentWpfNamespace));
		}
	}
	
	public class GroupIntoUniformGrid : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("UniformGrid", currentWpfNamespace));
		}
	}
	
	public class GroupIntoViewbox : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("Viewbox", currentWpfNamespace));
		}
	}
	
	public class GroupIntoWrapPanel : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			return new XElement(XName.Get("WrapPanel", currentWpfNamespace));
		}
	}
	
	public class GroupIntoStackPanelVertical : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			var element = new XElement(XName.Get("StackPanel", currentWpfNamespace));
			element.SetAttributeValue("Orientation", "Vertical");
			return element;
		}
	}
	
	public class GroupIntoStackPanelHorizontal : SimpleGroupIntoBase
	{
		protected override XElement CreateParent()
		{
			var element = new XElement(XName.Get("StackPanel", currentWpfNamespace));
			element.SetAttributeValue("Orientation", "Horizontal");
			return element;
		}
	}
	
	public class GroupIntoScrollViewerWithGrid : GroupIntoBase
	{
		protected override bool GroupInto(XElement parent, IEnumerable<XElement> children)
		{
			XElement newParent = new XElement(XName.Get("Grid", currentWpfNamespace));
			XElement newItem = new XElement(XName.Get("ScrollViewer", currentWpfNamespace), newParent);
			
			newParent.Add(children);
			parent.Add(newItem);
			
			foreach (var child in children)
				child.Remove();
			
			return true;
		}
	}
	
	public class GroupIntoGroupBoxWithGrid : GroupIntoBase
	{
		protected override bool GroupInto(XElement parent, IEnumerable<XElement> children)
		{
			XElement newParent = new XElement(XName.Get("Grid", currentWpfNamespace));
			XElement newItem = new XElement(XName.Get("GroupBox", currentWpfNamespace), newParent);
			
			newParent.Add(children);
			parent.Add(newItem);
			
			foreach (var child in children)
				child.Remove();
			
			return true;
		}
	}
}
