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
			Services.ExtensionManager.ReapplyExtensions(e.Items, this);
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
					this.Services.ExtensionManager.ReapplyExtensions(new DesignItem[] { newPrimarySelection }, this);
				} else if (newPrimarySelection == null) {
					this.Services.ExtensionManager.ReapplyExtensions(new DesignItem[] { oldPrimarySelection }, this);
				} else {
					this.Services.ExtensionManager.ReapplyExtensions(new DesignItem[] { oldPrimarySelection, newPrimarySelection }, this);
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
}
