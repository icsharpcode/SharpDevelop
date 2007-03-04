// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	/// <summary>
	/// Manages the collection of selected components and the primary selection.
	/// Notifies components with attached DesignSite when their selection state changes.
	/// </summary>
	sealed class DefaultSelectionService : ISelectionService
	{
		HashSet<DesignItem> _selectedComponents = new HashSet<DesignItem>();
		DesignItem _primarySelection;
		
		public bool IsComponentSelected(DesignItem component)
		{
			return _selectedComponents.Contains(component);
		}
		
		public ICollection<DesignItem> SelectedItems {
			get { return _selectedComponents.Clone(); }
		}
		
		public DesignItem PrimarySelection
		{
			get { return _primarySelection; }
		}
		
		public int SelectionCount
		{
			get { return _selectedComponents.Count; }
		}
		
		public event EventHandler SelectionChanging;
		public event EventHandler<DesignItemCollectionEventArgs> SelectionChanged;
		public event EventHandler PrimarySelectionChanging;
		public event EventHandler PrimarySelectionChanged;
		
		public void SetSelectedComponents(ICollection<DesignItem> components)
		{
			SetSelectedComponents(components, SelectionTypes.Replace);
		}
		
		public void SetSelectedComponents(ICollection<DesignItem> components, SelectionTypes selectionType)
		{
			if (components == null)
				components = new DesignItem[0];
			
			if (SelectionChanging != null)
				SelectionChanging(this, EventArgs.Empty);
			
			DesignItem newPrimarySelection = _primarySelection;
			
			if (selectionType == SelectionTypes.Auto) {
				if (Keyboard.Modifiers == ModifierKeys.Control)
					selectionType = SelectionTypes.Toggle; // Ctrl pressed: toggle selection
				else if ((Keyboard.Modifiers & ~ModifierKeys.Control) == ModifierKeys.Shift)
					selectionType = SelectionTypes.Add; // Shift or Ctrl+Shift pressed: add to selection
				else
					selectionType = SelectionTypes.Primary; // otherwise: change primary selection
			}
			
			if ((selectionType & SelectionTypes.Primary) == SelectionTypes.Primary) {
				// change primary selection to first new component
				newPrimarySelection = null;
				foreach (DesignItem obj in components) {
					newPrimarySelection = obj;
					break;
				}
				
				selectionType &= ~SelectionTypes.Primary;
				if (selectionType == 0) {
					// if selectionType was only Primary, and components has only one item that
					// changes the primary selection was changed to an already-selected item,
					// then we keep the current selection.
					// otherwise, we replace it
					if (components.Count == 1 && IsComponentSelected(newPrimarySelection)) {
						// keep selectionType = 0 -> don't change the selection
					} else {
						selectionType = SelectionTypes.Replace;
					}
				}
			}
			
			HashSet<DesignItem> componentsToNotifyOfSelectionChange = new HashSet<DesignItem>();
			switch (selectionType) {
				case SelectionTypes.Add:
					// add to selection and notify if required
					foreach (DesignItem obj in components) {
						if (_selectedComponents.Add(obj))
							componentsToNotifyOfSelectionChange.Add(obj);
					}
					break;
				case SelectionTypes.Remove:
					// remove from selection and notify if required
					foreach (DesignItem obj in components) {
						if (_selectedComponents.Remove(obj))
							componentsToNotifyOfSelectionChange.Add(obj);
					}
					break;
				case SelectionTypes.Replace:
					// notify all old components:
					componentsToNotifyOfSelectionChange.AddRange(_selectedComponents);
					// set _selectedCompontents to new components
					_selectedComponents.Clear();
					foreach (DesignItem obj in components) {
						_selectedComponents.Add(obj);
						// notify the new components
						componentsToNotifyOfSelectionChange.Add(obj);
					}
					break;
				case SelectionTypes.Toggle:
					// toggle selection and notify
					foreach (DesignItem obj in components) {
						if (_selectedComponents.Contains(obj)) {
							_selectedComponents.Remove(obj);
						} else {
							_selectedComponents.Add(obj);
						}
						componentsToNotifyOfSelectionChange.Add(obj);
					}
					break;
				case 0:
					// do nothing
					break;
				default:
					throw new NotSupportedException("The selection type " + selectionType + " is not supported");
			}
			
			if (!IsComponentSelected(newPrimarySelection)) {
				// primary selection is not selected anymore - change primary selection to any other selected component
				newPrimarySelection = null;
				foreach (DesignItem obj in _selectedComponents) {
					newPrimarySelection = obj;
					break;
				}
			}
			
			// Primary selection has changed:
			if (newPrimarySelection != _primarySelection) {
				componentsToNotifyOfSelectionChange.Add(_primarySelection);
				componentsToNotifyOfSelectionChange.Add(newPrimarySelection);
				if (PrimarySelectionChanging != null) {
					PrimarySelectionChanging(this, EventArgs.Empty);
				}
				_primarySelection = newPrimarySelection;
				if (PrimarySelectionChanged != null) {
					PrimarySelectionChanged(this, EventArgs.Empty);
				}
			}
			
			if (SelectionChanged != null) {
				SelectionChanged(this, new DesignItemCollectionEventArgs(componentsToNotifyOfSelectionChange));
			}
		}
	}
}
