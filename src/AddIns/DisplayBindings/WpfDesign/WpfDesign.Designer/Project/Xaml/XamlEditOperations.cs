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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.Designer.Extensions;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	/// <summary>
	/// Deals with operations on controls which also require access to internal XML properties of the XAML Document.
	/// </summary>
	public class XamlEditOperations
	{
		readonly XamlDesignContext _context;
		readonly XamlParserSettings _settings;
		
		
		readonly char _delimeter = Convert.ToChar(0x7F);
		
		/// <summary>
		/// Delimet character to seperate different piece of Xaml's
		/// </summary>
		public char Delimeter {
			get { return _delimeter; }
		}

		public XamlEditOperations(XamlDesignContext context, XamlParserSettings settings)
		{
			this._context = context;
			this._settings = settings;
		}
		
		/// <summary>
		/// Copy <paramref name="designItems"/> from the designer to clipboard.
		/// </summary>
		public void Cut(ICollection<DesignItem> designItems)
		{
			Clipboard.Clear();
			string cutXaml = "";
			var changeGroup = _context.OpenGroup("Cut " + designItems.Count + " elements", designItems);
			foreach (var item in designItems)
			{
				if (item != null && item != _context.RootItem)
				{
					XamlDesignItem xamlItem = item as XamlDesignItem;
					if (xamlItem != null) {
						cutXaml += XamlStaticTools.GetXaml(xamlItem.XamlObject);
						cutXaml += _delimeter;
					}
				}
			}
			ModelTools.DeleteComponents(designItems);
			Clipboard.SetText(cutXaml, TextDataFormat.Xaml);
			changeGroup.Commit();
		}
		
		/// <summary>
		/// Copy <paramref name="designItems"/> from the designer to clipboard.
		/// </summary>
		public void Copy(ICollection<DesignItem> designItems)
		{
			Clipboard.Clear();
			string copiedXaml = "";
			var changeGroup = _context.OpenGroup("Copy " + designItems.Count + " elements", designItems);
			foreach (var item in designItems)
			{
				if (item != null)
				{
					XamlDesignItem xamlItem = item as XamlDesignItem;
					if (xamlItem != null) {
						copiedXaml += XamlStaticTools.GetXaml(xamlItem.XamlObject);
						copiedXaml += _delimeter;
					}
				}
			}
			Clipboard.SetText(copiedXaml, TextDataFormat.Xaml);
			changeGroup.Commit();
		}
		
		/// <summary>
		/// Paste items from clipboard into the designer.
		/// </summary>
		public void Paste()
		{
			bool pasted = false;
			string combinedXaml = Clipboard.GetText(TextDataFormat.Xaml);
			IEnumerable<string> xamls = combinedXaml.Split(_delimeter);
			xamls = xamls.Where(xaml => xaml != "");

			DesignItem parent = _context.Services.Selection.PrimarySelection;
			DesignItem child = _context.Services.Selection.PrimarySelection;
			
			XamlDesignItem rootItem = _context.RootItem as XamlDesignItem;
			var pastedItems = new Collection<DesignItem>();
			foreach(var xaml in xamls) {
				var obj = XamlParser.ParseSnippet(rootItem.XamlObject, xaml, _settings);
				if(obj!=null) {
					DesignItem item = _context._componentService.RegisterXamlComponentRecursive(obj);
					if (item != null)
						pastedItems.Add(item);
				}
			}
			
			if (pastedItems.Count != 0) {
				var changeGroup = _context.OpenGroup("Paste " + pastedItems.Count + " elements", pastedItems);
				while (parent != null && pasted == false) {
					if (parent.ContentProperty != null) {
						if (parent.ContentProperty.IsCollection) {
							if (CollectionSupport.CanCollectionAdd(parent.ContentProperty.ReturnType, pastedItems.Select(item => item.Component)) && parent.GetBehavior<IPlacementBehavior>()!=null) {
								AddInParent(parent, pastedItems);
								pasted = true;
							}
						} else if (pastedItems.Count == 1 && parent.ContentProperty.Value == null && parent.ContentProperty.ValueOnInstance == null && DefaultPlacementBehavior.CanContentControlAdd((ContentControl)parent.View)) {
							AddInParent(parent, pastedItems);
							pasted = true;
						}
						if(!pasted)
							parent=parent.Parent;
					} else {
						parent = parent.Parent;
					}
				}

				while (pasted == false) {
					if (child.ContentProperty != null) {
						if (child.ContentProperty.IsCollection) {
							foreach (var col in child.ContentProperty.CollectionElements) {
								if (col.ContentProperty != null && col.ContentProperty.IsCollection) {
									if (CollectionSupport.CanCollectionAdd(col.ContentProperty.ReturnType, pastedItems.Select(item => item.Component))) {
										pasted = true;
									}
								}
							}
							break;
						} else if (child.ContentProperty.Value != null) {
							child = child.ContentProperty.Value;
						} else if (pastedItems.Count == 1) {
							child.ContentProperty.SetValue(pastedItems.First().Component);
							pasted = true;
							break;
						} else
							break;
					} else
						break;
				}
				changeGroup.Commit();
			}
		}
		
		/// <summary>
		/// Adds Items under a parent given that the content property is collection and can add types of <paramref name="pastedItems"/>
		/// </summary>
		/// <param name="parent">The Parent element</param>
		/// <param name="pastedItems">The list of elements to be added</param>
		void AddInParent(DesignItem parent,IList<DesignItem> pastedItems)
		{
			IEnumerable<Rect> rects = pastedItems.Select(i => new Rect(new Point(0, 0), new Point((double)i.Properties["Width"].ValueOnInstance, (double)i.Properties["Height"].ValueOnInstance)));
			var operation = PlacementOperation.TryStartInsertNewComponents(parent, pastedItems, rects.ToList(), PlacementType.PasteItem);
			ISelectionService selection = _context.Services.Selection;
			selection.SetSelectedComponents(pastedItems);
			if(operation!=null)
				operation.Commit();
		}
	}
}
