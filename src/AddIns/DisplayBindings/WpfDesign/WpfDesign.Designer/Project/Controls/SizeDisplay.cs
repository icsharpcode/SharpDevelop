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
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// Display height of the element.
	/// </summary>
	class HeightDisplay : Control
	{
		static HeightDisplay()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(HeightDisplay), new FrameworkPropertyMetadata(typeof(HeightDisplay)));
		}
	}

	/// <summary>
	/// Display width of the element.
	/// </summary>
	class WidthDisplay : Control
	{
		static WidthDisplay()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(WidthDisplay), new FrameworkPropertyMetadata(typeof(WidthDisplay)));
		}
	}
}
