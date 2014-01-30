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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using ICSharpCode.WpfDesign.Adorners;
using ICSharpCode.WpfDesign.Extensions;
using ICSharpCode.WpfDesign.Designer.Controls;
namespace ICSharpCode.WpfDesign.Designer.Extensions
{
	/// <summary>
	/// Display Height/Width on the primary selection
	/// </summary>
	[ExtensionFor(typeof(UIElement))]
	class SizeDisplayExtension : PrimarySelectionAdornerProvider
	{
		HeightDisplay _heightDisplay;
		WidthDisplay _widthDisplay;
		
		public HeightDisplay HeightDisplay{
			get { return _heightDisplay; }
		}
		
		public WidthDisplay WidthDisplay{
			get { return _widthDisplay; }
		}
		
		protected override void OnInitialized()
		{
			base.OnInitialized();
			if (this.ExtendedItem != null)
			{
				RelativePlacement placementHeight = new RelativePlacement(HorizontalAlignment.Right, VerticalAlignment.Stretch);
				placementHeight.XOffset = 10;
				_heightDisplay = new HeightDisplay();
				_heightDisplay.DataContext = this.ExtendedItem.Component;

				RelativePlacement placementWidth = new RelativePlacement(HorizontalAlignment.Stretch, VerticalAlignment.Bottom);
				placementWidth.YOffset = 10;
				_widthDisplay = new WidthDisplay();
				_widthDisplay.DataContext = this.ExtendedItem.Component;

				this.AddAdorners(placementHeight, _heightDisplay);
				this.AddAdorners(placementWidth, _widthDisplay);
				_heightDisplay.Visibility=Visibility.Hidden;
				_widthDisplay.Visibility=Visibility.Hidden;
			}
		}
	}
}
