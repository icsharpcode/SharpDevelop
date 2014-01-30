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
using System.Linq;
using System.Windows.Input;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// Manages the Focus/Primary Selection using TAB for down-the-tree navigation and Shift+TAB for up-the-tree navigation. 
	/// </summary>
	class FocusNavigator
    {
		/* The Focus navigator do not involves the concept of Logical Focus or KeyBoard Focus
		 * since nothing is getting focoused on the designer except for the DesignPanel. It just changes
		 * the primary selection between the hierarchy of elements present on the designer. */
		
        private readonly DesignSurface _surface;
        private KeyBinding _tabBinding;
        private KeyBinding _shiftTabBinding;
        
        public FocusNavigator(DesignSurface surface)
        {
            this._surface=surface;
        }
        
        /// <summary>
        /// Starts the navigator on the Design surface and add bindings.
        /// </summary>
        public void Start()
        {
            var tabFocus = new RelayCommand(this.MoveFocusForward, this.CanMoveFocusForward);
            var shiftTabFocus = new RelayCommand(this.MoveFocusBack, this.CanMoveFocusBack);
            _tabBinding = new KeyBinding(tabFocus, new KeyGesture(Key.Tab));
            _shiftTabBinding = new KeyBinding(shiftTabFocus, new KeyGesture(Key.Tab, ModifierKeys.Shift));
            IKeyBindingService kbs = _surface.DesignContext.Services.GetService(typeof(IKeyBindingService)) as IKeyBindingService;
            if (kbs != null)
            {
                kbs.RegisterBinding(_tabBinding);
                kbs.RegisterBinding(_shiftTabBinding);
            }
        }
        
        /// <summary>
        /// De-register the bindings from the Design Surface
        /// </summary>
        public void End()
        {
            IKeyBindingService kbs = _surface.DesignContext.Services.GetService(typeof(IKeyBindingService)) as IKeyBindingService;
            if (kbs != null)
            {
                kbs.DeregisterBinding(_tabBinding);
                kbs.DeregisterBinding(_shiftTabBinding);
            }
        }
        
        /// <summary>
        /// Moves the Foucus down the tree.
        /// </summary>        
        void MoveFocusForward()
        {
            var designSurface = _surface;
            if (designSurface != null) {
                var context = designSurface.DesignContext;
                ISelectionService selection=context.Services.Selection;
                DesignItem item = selection.PrimarySelection;
                selection.SetSelectedComponents(selection.SelectedItems, SelectionTypes.Remove);
                if (item != GetLastElement())
                {
                    if (item.ContentProperty != null)
                    {
                        if (item.ContentProperty.IsCollection)
                        {
                            if (item.ContentProperty.CollectionElements.Count != 0)
                            {
                                if (ModelTools.CanSelectComponent(item.ContentProperty.CollectionElements.First()))
                                    selection.SetSelectedComponents(new DesignItem[] { item.ContentProperty.CollectionElements.First() }, SelectionTypes.Primary);
                                else
                                    SelectNextInPeers(item);
                            }
                            else
                                SelectNextInPeers(item);
                        }
                        else if (item.ContentProperty.Value != null) {
                            if (ModelTools.CanSelectComponent(item.ContentProperty.Value))
                                selection.SetSelectedComponents(new DesignItem[] { item.ContentProperty.Value }, SelectionTypes.Primary);
                            else
                                SelectNextInPeers(item);
                        }
                        else {
                            SelectNextInPeers(item);
                        }
                    }
                    else {
                        SelectNextInPeers(item);
                    }
                }
                else { //if the element was last element move focus to the root element to keep a focus cycle.
                    selection.SetSelectedComponents(new DesignItem[] {context.RootItem}, SelectionTypes.Primary);
                }
            }
        }
        
        /// <summary>
        /// Checks if focus navigation should be for down-the-tree be done.
        /// </summary>
        bool CanMoveFocusForward()
        {
            var designSurface = _surface;
            if (designSurface != null)
                if (Keyboard.FocusedElement == designSurface._designPanel)
                    return true;
            return false;
        }
        
        /// <summary>
        /// Moves focus up-the-tree.
        /// </summary>        
        void MoveFocusBack()
        {
            var designSurface = _surface;
            if (designSurface != null) {
                var context = designSurface.DesignContext;
                ISelectionService selection = context.Services.Selection;
                DesignItem item = selection.PrimarySelection;
                if (item != context.RootItem)
                {
                    if (item.Parent != null && item.Parent.ContentProperty.IsCollection)
                    {
                        int index = item.Parent.ContentProperty.CollectionElements.IndexOf(item);
                        if (index != 0)
                        {
                            if (ModelTools.CanSelectComponent(item.Parent.ContentProperty.CollectionElements.ElementAt(index - 1)))
                                selection.SetSelectedComponents(new DesignItem[] { item.Parent.ContentProperty.CollectionElements.ElementAt(index - 1) }, SelectionTypes.Primary);
                        }
                        else
                        {
                            if (ModelTools.CanSelectComponent(item.Parent))
                                selection.SetSelectedComponents(new DesignItem[] { item.Parent }, SelectionTypes.Primary);
                        }

                    }
                    else
                    {
                        if (ModelTools.CanSelectComponent(item.Parent))
                            selection.SetSelectedComponents(new DesignItem[] { item.Parent }, SelectionTypes.Primary);
                    }
                }
                else {// if the element was root item move focus again to the last element.
                    selection.SetSelectedComponents(new DesignItem[] { GetLastElement() }, SelectionTypes.Primary);
                }
            }
        }
        
        /// <summary>
        /// Checks if focus navigation for the up-the-tree should be done.
        /// </summary>
        bool CanMoveFocusBack()
        {
            var designSurface = _surface;
            if (designSurface != null)
                if (Keyboard.FocusedElement == designSurface._designPanel)
                    return true;
            return false;
        }
        
        /// <summary>
        /// Gets the last element in the element hierarchy.
        /// </summary>        
        DesignItem GetLastElement()
        {
            DesignItem item = _surface.DesignContext.RootItem;
            while (item != null && item.ContentProperty != null)
            {
                if (item.ContentProperty.IsCollection)
                {
                    if (item.ContentProperty.CollectionElements.Count != 0) {
                        if (ModelTools.CanSelectComponent(item.ContentProperty.CollectionElements.Last()))
                            item = item.ContentProperty.CollectionElements.Last();
                        else
                            break;
                    }
                    else
                        break;
                }
                else {
                    if (item.ContentProperty.Value != null)
                        item = item.ContentProperty.Value;
                    else
                        break;
                }
            }
            return item;
        }
        
        /// <summary>
        /// Select the next element in the element collection if <paramref name="item"/> parent's had it's content property as collection.
        /// </summary>        
        void SelectNextInPeers(DesignItem item)
        {
            ISelectionService selection = _surface.DesignContext.Services.Selection;
            if (item.Parent != null && item.Parent.ContentProperty != null)
            {
                if (item.Parent.ContentProperty.IsCollection)
                {
                    int index = item.Parent.ContentProperty.CollectionElements.IndexOf(item);
                    if (index != item.Parent.ContentProperty.CollectionElements.Count)
                        selection.SetSelectedComponents(new DesignItem[] { item.Parent.ContentProperty.CollectionElements.ElementAt(index + 1) }, SelectionTypes.Primary);
                }
            }
        }
    }
}
