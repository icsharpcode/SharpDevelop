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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;

using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	
	/// <summary>
	/// The drag handle displayed for Framework Elements
	/// </summary>
	[ExtensionServer(typeof(PrimarySelectionButOnlyWhenMultipleSelectedExtensionServer))]
	[ExtensionFor(typeof(FrameworkElement))]
	public class TopLeftContainerDragHandleMultipleItems : AdornerProvider
	{
		/// <summary/>
		public TopLeftContainerDragHandleMultipleItems()
		{ }
		
		protected override void OnInitialized()
		{
			base.OnInitialized();
			
			ContainerDragHandle rect = new ContainerDragHandle();
			
			rect.PreviewMouseDown += delegate(object sender, MouseButtonEventArgs e) {
				//Services.Selection.SetSelectedComponents(new DesignItem[] { this.ExtendedItem }, SelectionTypes.Auto);
				new DragMoveMouseGesture(this.ExtendedItem, false).Start(this.ExtendedItem.Services.DesignPanel,e);
				e.Handled=true;
			};
			
			var items = this.ExtendedItem.Services.Selection.SelectedItems;
			
			double minX = 0;
			double minY = 0;
			double maxX = 0;
			double maxY = 0;
			
			foreach (DesignItem di in items) {
				Point relativeLocation = di.View.TranslatePoint(new Point(0, 0), this.ExtendedItem.View);
				
				minX = minX < relativeLocation.X ? minX : relativeLocation.X;
				minY = minY < relativeLocation.Y ? minY : relativeLocation.Y;
				maxX = maxX > relativeLocation.X + ((FrameworkElement)this.ExtendedItem.View).ActualWidth ? maxX : relativeLocation.X + ((FrameworkElement)this.ExtendedItem.View).ActualWidth;
				maxY = maxY > relativeLocation.Y + ((FrameworkElement)this.ExtendedItem.View).ActualHeight ? maxY : relativeLocation.Y + ((FrameworkElement)this.ExtendedItem.View).ActualHeight;
			}
			
			Rectangle rect2 = new Rectangle() {
				Width = (maxX - minX) + 4,
				Height = (maxY - minY) + 4,
				Stroke = Brushes.Black,
				StrokeThickness = 2,
				StrokeDashArray = new DoubleCollection(){ 2, 2 },
			};
			
			RelativePlacement p = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top);
			p.XOffset = minX - 3;
			p.YOffset = minY - 3;
			
			RelativePlacement p2 = new RelativePlacement(HorizontalAlignment.Left, VerticalAlignment.Top);
			p2.XOffset = (minX + rect2.Width) - 2;
			p2.YOffset = (minY + rect2.Height) - 2;
						
			AddAdorner(p, AdornerOrder.Background, rect);
			AddAdorner(p2, AdornerOrder.Background, rect2);
		}
	}	
}
