// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Contains global WPF styles used in SharpDevelop.
	/// </summary>
	public static class GlobalStyles
	{
		static Style FindResource(ResourceKey key)
		{
			// don't crash if controls using GlobalStyles are instanciated in unit test mode
			if (Application.Current == null)
				return null;
			else
				return (Style)Application.Current.FindResource(key);
		}
		
		public static Style WindowStyle {
			get { return FindResource(windowStyleKey); }
		}
		
		static readonly ResourceKey windowStyleKey = new ComponentResourceKey(typeof(GlobalStyles), "WindowStyle");
		
		public static ResourceKey WindowStyleKey {
			get { return windowStyleKey; }
		}
		
		public static Style DialogWindowStyle {
			get { return FindResource(dialogWindowStyleKey); }
		}
		
		static readonly ResourceKey dialogWindowStyleKey = new ComponentResourceKey(typeof(GlobalStyles), "DialogWindowStyle");
		
		public static ResourceKey DialogWindowStyleKey {
			get { return dialogWindowStyleKey; }
		}
		
		public static Style ButtonStyle {
			get { return FindResource(buttonStyleKey); }
		}
		
		static readonly ResourceKey buttonStyleKey = new ComponentResourceKey(typeof(GlobalStyles), "ButtonStyle");
		
		public static ResourceKey ButtonStyleKey {
			get { return buttonStyleKey; }
		}
		
		public static Style WordWrapCheckBoxStyle {
			get { return FindResource(wordWrapCheckBoxStyleKey); }
		}
		
		static readonly ResourceKey wordWrapCheckBoxStyleKey = new ComponentResourceKey(typeof(GlobalStyles), "WordWrapCheckBoxStyle");
		
		public static ResourceKey WordWrapCheckBoxStyleKey {
			get { return wordWrapCheckBoxStyleKey; }
		}
		
		static readonly ResourceKey flowDirectionKey = new ComponentResourceKey(typeof(GlobalStyles), "FlowDirectionKey");
		
		public static ResourceKey FlowDirectionKey {
			get { return flowDirectionKey; }
		}
		
		static readonly ResourceKey listViewItemFocusHighlightStyleKey = new ComponentResourceKey(typeof(GlobalStyles), "ListViewItemFocusHighlightStyle");
		
		public static ResourceKey ListViewItemFocusHighlightStyleKey {
			get { return listViewItemFocusHighlightStyleKey; }
		}
		
		public static Style ListViewItemFocusHighlightStyle {
			get { return FindResource(listViewItemFocusHighlightStyleKey); }
		}
		
		static readonly ResourceKey listBoxItemFocusHighlightStyleKey = new ComponentResourceKey(typeof(GlobalStyles), "ListBoxItemFocusHighlightStyle");
		
		public static ResourceKey ListBoxItemFocusHighlightStyleKey {
			get { return listBoxItemFocusHighlightStyleKey; }
		}
		
		public static Style ListBoxItemFocusHighlightStyle {
			get { return FindResource(listBoxItemFocusHighlightStyleKey); }
		}
	}
}
