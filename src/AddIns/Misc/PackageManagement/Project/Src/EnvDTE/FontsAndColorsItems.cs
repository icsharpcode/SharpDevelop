// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
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
			return Colors.Find(c => c.Name == CustomizableHighlightingColorizer.DefaultTextAndBackground);
		}
		
		CustomizedHighlightingColor AddPlainTextHighlightingColorToCustomColors()
		{
			var color = new CustomizedHighlightingColor() {
				Name = CustomizableHighlightingColorizer.DefaultTextAndBackground
			};
			Colors.Add(color);
			return color;
		}
		
		List<CustomizedHighlightingColor> Colors {
			get {
				if (colors == null) {
					colors = highlightingRules.LoadColors();
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
