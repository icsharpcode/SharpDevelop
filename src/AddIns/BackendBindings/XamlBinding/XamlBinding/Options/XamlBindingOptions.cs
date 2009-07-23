// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;

using ICSharpCode.Core;

namespace ICSharpCode.XamlBinding
{	
	public static class XamlBindingOptions
	{
		static Properties properties;

		static XamlBindingOptions()
		{
			properties = PropertyService.Get("XamlBinding.Options", new Properties());
		}

		static Properties Properties {
			get {
				Debug.Assert(properties != null);
				return properties;
			}
		}
		
		public static IEnumerable<Brush> Colors {
			get {
				foreach (var item in typeof(Brushes).GetProperties())
					yield return item.GetValue(null, null) as Brush;
			}
		}
		
		public static bool UseExtensionCompletion {
			get { return Properties.Get("UseExtensionCompletion", true); }
			set { Properties.Set("UseExtensionCompletion", value); }
		}
		
		public static bool SwitchToCodeViewAfterInsertion {
			get { return Properties.Get("SwitchToCodeViewAfterInsertion", true); }
			set { Properties.Set("SwitchToCodeViewAfterInsertion", value); }
		}
		
		public static string EventHandlerNamePattern {
			get { return Properties.Get("EventHandlerNamePattern", "%Object%_%Event%"); }
			set { Properties.Set("EventHandlerNamePattern", value); }
		}
		
		public static Brush PropertyForegroundBrush {
			get { return Properties.Get("PropertyForeground", Brushes.CadetBlue); }
			set { Properties.Set("PropertyForeground", value); }
		}
		
		public static Brush PropertyBackgroundBrush {
			get { return Properties.Get("PropertyBackground", Brushes.Transparent); }
			set { Properties.Set("PropertyBackground", value); }
		}
		
		public static Brush EventForegroundBrush {
			get { return Properties.Get("EventForeground", Brushes.Green); }
			set { Properties.Set("EventForeground", value); }
		}

		public static Brush EventBackgroundBrush {
			get { return Properties.Get("EventBackground", Brushes.Transparent); }
			set { Properties.Set("EventBackground", value); }
		}
		
		public static Brush NamespaceDeclarationForegroundBrush {
			get { return Properties.Get("NamespaceDeclarationForeground", Brushes.Orange); }
			set { Properties.Set("NamespaceDeclarationForeground", value); }
		}
		
		public static Brush NamespaceDeclarationBackgroundBrush { 
			get { return Properties.Get("NamespaceDeclarationBackground", Brushes.Transparent); }
			set { Properties.Set("NamespaceDeclarationBackground", value); }
		}
		
		public static Brush IgnoredForegroundBrush {
			get { return Properties.Get("IgnoredForeground", Brushes.LightGray); }
			set { Properties.Set("IgnoredForeground", value); }
		}
		
		public static Brush IgnoredBackgroundBrush {
			get { return Properties.Get("IgnoredBackground", Brushes.Transparent); }
			set { Properties.Set("IgnoredBackground", value); }
		}
	}
}
