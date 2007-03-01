// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Provides <see cref="IChildResizeSupport"/> behavior for <see cref="Canvas"/>.
	/// </summary>
	[ExtensionFor(typeof(Canvas), OverrideExtension = typeof(DefaultChildResizeSupport))]
	public sealed class CanvasChildResizeSupport : BehaviorExtension, IChildResizeSupport
	{
		/// <inherits/>
		protected override void OnInitialized()
		{
			base.OnInitialized();
			this.ExtendedItem.AddBehavior(typeof(IChildResizeSupport), this);
		}
		
		/// <inherits/>
		public bool CanResizeChild(DesignItem child)
		{
			return DefaultChildResizeSupport.Instance.CanResizeChild(child);
		}
		
		/// <inherits/>
		public Placement GetPlacement(DesignItem child, double horizontalChange, double verticalChange, HorizontalAlignment horizontal, VerticalAlignment vertical)
		{
			return RootElementResizeSupport.Instance.GetPlacement(child, horizontalChange, verticalChange, horizontal, vertical);
		}
		
		/// <inherits/>
		public void Resize(DesignItem childItem, double horizontalChange, double verticalChange, HorizontalAlignment horizontal, VerticalAlignment vertical)
		{
			RelativePlacement p = (RelativePlacement)GetPlacement(childItem, horizontalChange, verticalChange, horizontal, vertical);
			DefaultChildResizeSupport.Resize(childItem, p);
			
			bool marginIsSet = childItem.Properties[FrameworkElement.MarginProperty].IsSet;
			
			DesignItemProperty left = childItem.Properties.GetAttachedProperty(Canvas.LeftProperty);
			DesignItemProperty top = childItem.Properties.GetAttachedProperty(Canvas.TopProperty);
			
			if (left.IsSet) {
				left.SetValue( p.XOffset + (double)left.ValueOnInstance);
			} else if (p.XOffset != 0 && !marginIsSet) {
				left.SetValue( p.XOffset );
			}
			if (top.IsSet) {
				top.SetValue( p.YOffset + (double)top.ValueOnInstance);
			} else if (p.YOffset != 0 && !marginIsSet) {
				top.SetValue( p.YOffset );
			}
		}
	}
}
