// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.WpfDesign.Extensions
{
	/// <summary>
	/// Applies an extension to the selected components.
	/// </summary>
	public class SelectionExtensionServer : DefaultExtensionServer
	{
		/// <summary>
		/// Is called after the extension server is initialized and the Context property has been set.
		/// </summary>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			Services.Selection.SelectionChanged += OnSelectionChanged;
		}
		
		void OnSelectionChanged(object sender, DesignItemCollectionEventArgs e)
		{
			ReapplyExtensions(e.Items);
		}
		
		/// <summary>
		/// Gets if the item is selected.
		/// </summary>
		public override bool ShouldApplyExtensions(DesignItem extendedItem)
		{
			return Services.Selection.IsComponentSelected(extendedItem);
		}
	}
	
	/// <summary>
	/// Applies an extension to the selected components, but not to the primary selection.
	/// </summary>
	public class SecondarySelectionExtensionServer : SelectionExtensionServer
	{
		/// <summary>
		/// Gets if the item is in the secondary selection.
		/// </summary>
		public override bool ShouldApplyExtensions(DesignItem extendedItem)
		{
			return base.ShouldApplyExtensions(extendedItem) && Services.Selection.PrimarySelection != extendedItem;
		}
	}
	
	/// <summary>
	/// Applies an extension to the primary selection.
	/// </summary>
	public class PrimarySelectionExtensionServer : DefaultExtensionServer
	{
		DesignItem oldPrimarySelection;
		
		/// <summary>
		/// Is called after the extension server is initialized and the Context property has been set.
		/// </summary>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
		}
		
		void OnPrimarySelectionChanged(object sender, EventArgs e)
		{
			DesignItem newPrimarySelection = this.Services.Selection.PrimarySelection;
			if (oldPrimarySelection != newPrimarySelection) {
				if (oldPrimarySelection == null) {
					ReapplyExtensions(new DesignItem[] { newPrimarySelection });
				} else if (newPrimarySelection == null) {
					ReapplyExtensions(new DesignItem[] { oldPrimarySelection });
				} else {
					ReapplyExtensions(new DesignItem[] { oldPrimarySelection, newPrimarySelection });
				}
				oldPrimarySelection = newPrimarySelection;
			}
		}
		
		/// <summary>
		/// Gets if the item is the primary selection.
		/// </summary>
		public override bool ShouldApplyExtensions(DesignItem extendedItem)
		{
			return Services.Selection.PrimarySelection == extendedItem;
		}
	}
	
	/// <summary>
	/// Applies an extension to the parent of the primary selection.
	/// </summary>
	public class PrimarySelectionParentExtensionServer : DefaultExtensionServer
	{
		/// <summary>
		/// Is called after the extension server is initialized and the Context property has been set.
		/// </summary>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
		}
		
		DesignItem primarySelection;
		DesignItem primarySelectionParent;
		
		void OnPrimarySelectionChanged(object sender, EventArgs e)
		{
			DesignItem newPrimarySelection = this.Services.Selection.PrimarySelection;
			if (primarySelection != newPrimarySelection) {
				if (primarySelection != null) {
					primarySelection.ParentChanged -= OnParentChanged;
				}
				if (newPrimarySelection != null) {
					newPrimarySelection.ParentChanged += OnParentChanged;
				}
				primarySelection = newPrimarySelection;
				OnParentChanged(sender, e);
			}
		}
		
		void OnParentChanged(object sender, EventArgs e)
		{
			DesignItem newPrimarySelectionParent = primarySelection != null ? primarySelection.Parent : null;
			
			if (primarySelectionParent != newPrimarySelectionParent) {
				DesignItem oldPrimarySelectionParent = primarySelectionParent;
				primarySelectionParent = newPrimarySelectionParent;
				ReapplyExtensions(new DesignItem[] { oldPrimarySelectionParent, newPrimarySelectionParent });
			}
		}
		
		/// <summary>
		/// Gets if the item is the primary selection.
		/// </summary>
		public override bool ShouldApplyExtensions(DesignItem extendedItem)
		{
			return primarySelectionParent == extendedItem;
		}
	}
}
