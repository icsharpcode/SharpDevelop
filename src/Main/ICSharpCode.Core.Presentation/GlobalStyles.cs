// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// Contains global WPF styles used in SharpDevelop.
	/// </summary>
	public static class GlobalStyles
	{
		public static Style WindowStyle {
			get {
				return (Style)Application.Current.FindResource(WindowStyleKey);
			}
		}
		
		static readonly ResourceKey windowStyleKey = new ComponentResourceKey(typeof(GlobalStyles), "WindowStyle");
		
		public static ResourceKey WindowStyleKey {
			get { return windowStyleKey; }
		}
		
		public static Style DialogWindowStyle {
			get {
				return (Style)Application.Current.FindResource(DialogWindowStyleKey);
			}
		}
		
		static readonly ResourceKey dialogWindowStyleKey = new ComponentResourceKey(typeof(GlobalStyles), "DialogWindowStyle");
		
		public static ResourceKey DialogWindowStyleKey {
			get { return dialogWindowStyleKey; }
		}
		
		public static Style ButtonStyle {
			get {
				return (Style)Application.Current.FindResource(ButtonStyleKey);
			}
		}
		
		static readonly ResourceKey buttonStyleKey = new ComponentResourceKey(typeof(GlobalStyles), "ButtonStyle");
		
		public static ResourceKey ButtonStyleKey {
			get { return buttonStyleKey; }
		}
		
		public static Style WordWrapCheckBoxStyle {
			get {
				return (Style)Application.Current.FindResource(WordWrapCheckBoxStyleKey);
			}
		}
		
		static readonly ResourceKey wordWrapCheckBoxStyleKey = new ComponentResourceKey(typeof(GlobalStyles), "WordWrapCheckBoxStyle");
		
		public static ResourceKey WordWrapCheckBoxStyleKey {
			get { return wordWrapCheckBoxStyleKey; }
		}
	}
}
