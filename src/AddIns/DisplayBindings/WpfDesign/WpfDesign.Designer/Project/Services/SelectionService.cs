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
		HashSet<object> _selectedComponents = new HashSet<object>();
		object _primarySelection;
		
		public bool IsComponentSelected(object component)
		{
			return _selectedComponents.Contains(component);
		}
		
		public ICollection<object> SelectedComponents {
			get { return _selectedComponents.Clone(); }
		}
		
		public object PrimarySelection
		{
			get { return _primarySelection; }
		}
		
		public int SelectionCount
		{
			get { return _selectedComponents.Count; }
		}
		
		public event EventHandler SelectionChanging;
		public event EventHandler<ComponentCollectionEventArgs> SelectionChanged;
		public event EventHandler PrimarySelectionChanging;
		public event EventHandler PrimarySelectionChanged;
		
		public void SetSelectedComponents(ICollection<object> components)
		{
			SetSelectedComponents(components, SelectionTypes.Auto);
		}
		
		public void SetSelectedComponents(ICollection<object> components, SelectionTypes selectionType)
		{
			if (components == null)
				components = new object[0];
			
			if (SelectionChanging != null)
				SelectionChanging(this, EventArgs.Empty);
			
			object newPrimarySelection = _primarySelection;
			
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
				foreach (object obj in components) {
					newPrimarySelection = obj;
					break;
				}
				
				selectionType &= ~SelectionTypes.Primary;
				// if selectionType was only Primary, keep current selection; but if new primary selection
				// is not yet selected, replace existing selection with new
				if (selectionType == 0 && IsComponentSelected(newPrimarySelection) == false) {
					selectionType = SelectionTypes.Replace;
				}
			}
			
			HashSet<object> componentsToNotifyOfSelectionChange = new HashSet<object>();
			switch (selectionType) {
				case SelectionTypes.Add:
					// add to selection and notify if required
					foreach (object obj in components) {
						if (_selectedComponents.Add(obj))
							componentsToNotifyOfSelectionChange.Add(obj);
					}
					break;
				case SelectionTypes.Remove:
					// remove from selection and notify if required
					foreach (object obj in components) {
						if (_selectedComponents.Remove(obj))
							componentsToNotifyOfSelectionChange.Add(obj);
					}
					break;
				case SelectionTypes.Replace:
					// notify all old components:
					componentsToNotifyOfSelectionChange.AddRange(_selectedComponents);
					// set _selectedCompontents to new components
					_selectedComponents.Clear();
					foreach (object obj in components) {
						_selectedComponents.Add(obj);
						// notify the new components
						componentsToNotifyOfSelectionChange.Add(obj);
					}
					break;
				case SelectionTypes.Toggle:
					// toggle selection and notify
					foreach (object obj in components) {
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
				foreach (object obj in _selectedComponents) {
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
			
			// Notify the components that changed selection state:
			/*
			foreach (object obj in componentsToNotifyOfSelectionChange) {
				DesignSite objSite = DesignSite.GetSite(obj as DependencyObject);
				if (objSite != null)
					objSite.Notify(this, null);
			}
			*/
			
			if (SelectionChanged != null) {
				SelectionChanged(this, new ComponentCollectionEventArgs(componentsToNotifyOfSelectionChange));
			}
		}
	}
}
