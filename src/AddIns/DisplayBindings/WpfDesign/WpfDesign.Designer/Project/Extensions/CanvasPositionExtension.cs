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
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Designer.Controls;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	[ExtensionFor(typeof(FrameworkElement))]
	[ExtensionServer(typeof(PrimarySelectionExtensionServer))]
	public class CanvasPositionExtension : AdornerProvider
	{
		private MarginHandle[] _handles;
		private MarginHandle _leftHandle, _topHandle, _rightHandle, _bottomHandle;
		private Canvas _canvas;

		protected override void OnInitialized()
		{
			base.OnInitialized();
			if (this.ExtendedItem.Parent != null)
			{
				if (this.ExtendedItem.Parent.ComponentType == typeof(Canvas))
				{
					FrameworkElement extendedControl = (FrameworkElement)this.ExtendedItem.Component;
					AdornerPanel adornerPanel = new AdornerPanel();

					// If the Element is rotated/skewed in the grid, then margin handles do not appear
					if (extendedControl.LayoutTransform.Value == Matrix.Identity && extendedControl.RenderTransform.Value == Matrix.Identity)
					{
						_canvas = this.ExtendedItem.Parent.View as Canvas;
						_handles = new[]
						{
							_leftHandle = new CanvasPositionHandle(ExtendedItem, adornerPanel, HandleOrientation.Left),
							_topHandle = new CanvasPositionHandle(ExtendedItem, adornerPanel, HandleOrientation.Top),
							_rightHandle = new CanvasPositionHandle(ExtendedItem, adornerPanel, HandleOrientation.Right),
							_bottomHandle = new CanvasPositionHandle(ExtendedItem, adornerPanel, HandleOrientation.Bottom),
						};
					}

					if (adornerPanel != null)
						this.Adorners.Add(adornerPanel);
				}
			}
		}

		public void HideHandles()
		{
			if (_handles != null)
			{
				foreach (var handle in _handles)
				{
					handle.ShouldBeVisible = false;
					handle.Visibility = Visibility.Hidden;
				}
			}
		}

		public void ShowHandles()
		{
			if (_handles != null)
			{
				foreach (var handle in _handles)
				{
					handle.ShouldBeVisible = true;
					handle.Visibility = Visibility.Visible;
					handle.DecideVisiblity(handle.HandleLength);
				}
			}
		}
	}
}
