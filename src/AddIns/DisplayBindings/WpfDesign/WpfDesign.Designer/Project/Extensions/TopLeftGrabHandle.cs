// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// The resize thumb at the top left edge of a component.
	/// </summary>
	[ExtensionFor(typeof(FrameworkElement))]
	public sealed class TopLeftResizeThumb : PrimarySelectionAdornerProvider
	{
		AdornerPanel adornerPanel;
		
		/// <summary></summary>
		public TopLeftResizeThumb()
		{
			adornerPanel = new AdornerPanel();
			adornerPanel.Order = AdornerOrder.Foreground;
			
			ResizeThumb resizeThumb;
			
			resizeThumb = new ResizeThumb();
			AdornerPanel.SetPlacement(resizeThumb, new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top));
			adornerPanel.Children.Add(resizeThumb);
			
			resizeThumb = new ResizeThumb();
			AdornerPanel.SetPlacement(resizeThumb, new RelativePlacement(HorizontalAlignment.Right, VerticalAlignment.Top));
			adornerPanel.Children.Add(resizeThumb);
			
			resizeThumb = new ResizeThumb();
			AdornerPanel.SetPlacement(resizeThumb, new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Bottom));
			adornerPanel.Children.Add(resizeThumb);
			
			resizeThumb = new ResizeThumb();
			AdornerPanel.SetPlacement(resizeThumb, new RelativePlacement(HorizontalAlignment.Right, VerticalAlignment.Bottom));
			adornerPanel.Children.Add(resizeThumb);
		}
		
		/// <summary/>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.ExtendedItem.PropertyChanged += OnPropertyChanged;
			this.Services.Selection.PrimarySelectionChanged += OnPrimarySelectionChanged;
			UpdateAdornerVisibility();
			OnPrimarySelectionChanged(null, null);
		}
		
		/// <summary/>
		protected override void OnRemove()
		{
			this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
			this.Services.Selection.PrimarySelectionChanged -= OnPrimarySelectionChanged;
			base.OnRemove();
		}
		
		void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Width" || e.PropertyName == "Height") {
				UpdateAdornerVisibility();
			}
		}
		
		void OnPrimarySelectionChanged(object sender, EventArgs e)
		{
			bool isPrimarySelection = this.Services.Selection.PrimarySelection == this.ExtendedItem;
			foreach (ResizeThumb g in adornerPanel.Children) {
				g.IsPrimarySelection = isPrimarySelection;
			}
		}
		
		void UpdateAdornerVisibility()
		{
			FrameworkElement element = (FrameworkElement)this.ExtendedItem.Component;
			if (double.IsNaN(element.Width) && double.IsNaN(element.Height)) {
				if (this.Adorners.Count == 1) {
					this.Adorners.Clear();
				}
			} else {
				if (this.Adorners.Count == 0) {
					this.Adorners.Add(adornerPanel);
				}
			}
		}
	}
}
