// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
		DesignItem oldPrimarySelectionParent;
		
		/// <summary>
		/// Is called after the extension server is initialized and the Context property has been set.
		/// </summary>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
		}
		
		DesignItem PrimarySelectionParent {
			get {
				DesignItem newPrimarySelection = this.Services.Selection.PrimarySelection;
				if (newPrimarySelection != null) {
					return newPrimarySelection.Parent;
				} else {
					return null;
				}
			}
		}
		
		void OnPrimarySelectionChanged(object sender, EventArgs e)
		{
			DesignItem newPrimarySelectionParent = PrimarySelectionParent;
			
			if (oldPrimarySelectionParent != newPrimarySelectionParent) {
				if (oldPrimarySelectionParent == null) {
					ReapplyExtensions(new DesignItem[] { newPrimarySelectionParent });
				} else if (newPrimarySelectionParent == null) {
					ReapplyExtensions(new DesignItem[] { oldPrimarySelectionParent });
				} else {
					ReapplyExtensions(new DesignItem[] { oldPrimarySelectionParent, newPrimarySelectionParent });
				}
				oldPrimarySelectionParent = newPrimarySelectionParent;
			}
		}
		
		/// <summary>
		/// Gets if the item is the primary selection.
		/// </summary>
		public override bool ShouldApplyExtensions(DesignItem extendedItem)
		{
			return PrimarySelectionParent == extendedItem;
		}
	}
}
