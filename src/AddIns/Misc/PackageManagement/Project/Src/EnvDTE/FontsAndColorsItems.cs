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

using ICSharpCode.AvalonEdit.AddIn;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class FontsAndColorsItems : MarshalByRefObject, global::EnvDTE.FontsAndColorsItems
	{
		static readonly string PlainTextItem = "Plain Text";
		
		ICustomizedHighlightingRules highlightingRules;
		List<CustomizedHighlightingColor> colors;
		
		public FontsAndColorsItems()
			: this(new CustomizedHighlightingRules())
		{
		}
		
		public FontsAndColorsItems(ICustomizedHighlightingRules highlightingRules)
		{
			this.highlightingRules = highlightingRules;
		}
		
		public global::EnvDTE.ColorableItems Item(string name)
		{
			if (IsPlainText(name)) {
				return CreatePlainTextColorableItems();
			}
			return null;
		}
		
		bool IsPlainText(string name)
		{
			return String.Equals(name, PlainTextItem, StringComparison.InvariantCultureIgnoreCase);
		}
		
		ColorableItems CreatePlainTextColorableItems()
		{
			CustomizedHighlightingColor color = FindPlainTextHighlightingColor();
			if (color == null) {
				color = AddPlainTextHighlightingColorToCustomColors();
			}
			return new ColorableItems(PlainTextItem, color, this);
		}
		
		CustomizedHighlightingColor FindPlainTextHighlightingColor()
		{
			return Colors.Find(c => c.Name == CustomizingHighlighter.DefaultTextAndBackground);
		}
		
		CustomizedHighlightingColor AddPlainTextHighlightingColorToCustomColors()
		{
			var color = new CustomizedHighlightingColor() {
				Name = CustomizingHighlighter.DefaultTextAndBackground
			};
			Colors.Add(color);
			return color;
		}
		
		List<CustomizedHighlightingColor> Colors {
			get {
				if (colors == null) {
					colors = new List<CustomizedHighlightingColor>(highlightingRules.LoadColors());
				}
				return colors;
			}
		}
		
		internal void Save()
		{
			highlightingRules.SaveColors(Colors);
		}
	}
}
