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

using ICSharpCode.WpfDesign.UIExtensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// A thumb where the look can depend on the IsPrimarySelection property.
	/// </summary>
	public class DesignerThumb : Thumb
	{
		/// <summary>
		/// Dependency property for <see cref="IsPrimarySelection"/>.
		/// </summary>
		public static readonly DependencyProperty IsPrimarySelectionProperty
			= DependencyProperty.Register("IsPrimarySelection", typeof(bool), typeof(DesignerThumb));
		
		/// <summary>
		/// Dependency property for <see cref="IsPrimarySelection"/>.
		/// </summary>
		public static readonly DependencyProperty ThumbVisibleProperty
			= DependencyProperty.Register("ThumbVisible", typeof(bool), typeof(DesignerThumb), new FrameworkPropertyMetadata(true));

		/// <summary>
		/// Dependency property for <see cref="OperationMenu"/>.
		/// </summary>
		public static readonly DependencyProperty OperationMenuProperty =
			DependencyProperty.Register("OperationMenu", typeof(Control[]), typeof(DesignerThumb), new PropertyMetadata(null));

		internal PlacementAlignment Alignment;
		
		static DesignerThumb()
		{
			//This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
			//This style is defined in themes\generic.xaml
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerThumb), new FrameworkPropertyMetadata(typeof(DesignerThumb)));
		}

		public void ReDraw()
		{
			var parent = this.TryFindParent<FrameworkElement>();
			if (parent != null)
				parent.InvalidateArrange();
		}

		/// <summary>
		/// Gets/Sets if the resize thumb is attached to the primary selection.
		/// </summary>
		public bool IsPrimarySelection {
			get { return (bool)GetValue(IsPrimarySelectionProperty); }
			set { SetValue(IsPrimarySelectionProperty, value); }
		}
		
		/// <summary>
		/// Gets/Sets if the resize thumb is visible.
		/// </summary>
		public bool ThumbVisible {
			get { return (bool)GetValue(ThumbVisibleProperty); }
			set { SetValue(ThumbVisibleProperty, value); }
		}

		/// <summary>
		/// Gets/Sets the OperationMenu.
		/// </summary>
		public Control[] OperationMenu
		{
			get { return (Control[])GetValue(OperationMenuProperty); }
			set { SetValue(OperationMenuProperty, value); }
		}
	}
}
