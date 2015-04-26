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
	/// Applies an extension to the primary selection, but only when multiple Items are selected!
	/// </summary>
	public class PrimarySelectionButOnlyWhenMultipleSelectedExtensionServer : PrimarySelectionExtensionServer
	{
		/// <summary>
		/// Is called after the extension server is initialized and the Context property has been set.
		/// </summary>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.Services.Selection.SelectionChanged += OnSelectionChanged;
		}
		
		void OnSelectionChanged(object sender, EventArgs e)
		{
			ReapplyExtensions(this.Services.Selection.SelectedItems);
		}
		
		/// <inheritdoc/>
		public override bool ShouldBeReApplied()
		{
			return true;
		}
		
		/// <summary>
		/// Gets if the item is in the secondary selection.
		/// </summary>
		public override bool ShouldApplyExtensions(DesignItem extendedItem)
		{
			return Services.Selection.PrimarySelection == extendedItem && Services.Selection.SelectionCount > 1;
		}
	}
	
	/// <summary>
	/// Applies an extension only when multiple Items are selected!
	/// </summary>
	public class MultipleSelectedExtensionServer : DefaultExtensionServer
	{
		/// <summary>
		/// Is called after the extension server is initialized and the Context property has been set.
		/// </summary>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.Services.Selection.SelectionChanged += OnSelectionChanged;
		}
		
		void OnSelectionChanged(object sender, EventArgs e)
		{
			ReapplyExtensions(this.Services.Selection.SelectedItems);
		}
		
		/// <summary>
		/// Gets if the item is in the secondary selection.
		/// </summary>
		public override bool ShouldApplyExtensions(DesignItem extendedItem)
		{
			return Services.Selection.SelectionCount > 1;
		}
	}
	
	/// <summary>
	/// Applies an extension to the primary selection if Only One Item is Selected.
	/// </summary>
	public class OnlyOneItemSelectedExtensionServer : PrimarySelectionExtensionServer
	{
		/// <summary>
		/// Is called after the extension server is initialized and the Context property has been set.
		/// </summary>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.Services.Selection.SelectionChanged += OnSelectionChanged;
		}
		
		void OnSelectionChanged(object sender, EventArgs e)
		{
			if (this.Services.Selection.SelectedItems.Count > 1)
				ReapplyExtensions(this.Services.Selection.SelectedItems);
		}
		
		/// <summary>
		/// Gets if the item is the primary selection.
		/// </summary>
		public override bool ShouldApplyExtensions(DesignItem extendedItem)
		{
			return Services.Selection.PrimarySelection == extendedItem && Services.Selection.SelectionCount < 2;
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
