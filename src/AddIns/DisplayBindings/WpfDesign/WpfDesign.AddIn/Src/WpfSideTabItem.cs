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
using System.Drawing;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// Description of WpfSideTabItem.
	/// </summary>
	public class WpfSideTabItem : SharpDevelopSideTabItem
	{
		public WpfSideTabItem(Type componentType) : base(componentType.Name, new CreateComponentTool(componentType))
		{
			CanBeRenamed = false;
//			this.Icon = tag.Bitmap;
		}
		
		///<summary>create a default tabitem : a pointer icon with an empty toolboxitem</summary>
		public WpfSideTabItem() : base("Pointer")
		{
			CanBeRenamed = false;
			CanBeDeleted = false;
			Bitmap pointerBitmap = new Bitmap(IconService.GetBitmap("Icons.16x16.FormsDesigner.PointerIcon"), 16, 16);
			this.Icon = pointerBitmap;
			this.Tag  = null;
		}
	}
}
