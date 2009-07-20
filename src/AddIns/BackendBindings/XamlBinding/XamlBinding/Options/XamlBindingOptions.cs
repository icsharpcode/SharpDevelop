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
			get { return properties.Get("UseExtensionCompletion", true); }
			set { properties.Set("UseExtensionCompletion", value); }
		}
		
		public static bool SwitchToCodeViewAfterInsertion {
			get { return properties.Get("SwitchToCodeViewAfterInsertion", true); }
			set { properties.Set("SwitchToCodeViewAfterInsertion", value); }
		}
		
		public static string EventHandlerNamePattern {
			get { return properties.Get("EventHandlerNamePattern", "%Object%_%Event%"); }
			set { properties.Set("EventHandlerNamePattern", value); }
		}
		
		public static Brush PropertyForegroundBrush {
			get { return properties.Get("PropertyForeground", Brushes.CadetBlue); }
			set { properties.Set("PropertyForeground", value); }
		}
		
		public static Brush PropertyBackgroundBrush {
			get { return properties.Get("PropertyBackground", Brushes.Transparent); }
			set { properties.Set("PropertyBackground", value); }
		}
		
		public static Brush EventForegroundBrush {
			get { return properties.Get("EventForeground", Brushes.Green); }
			set { properties.Set("EventForeground", value); }
		}

		public static Brush EventBackgroundBrush {
			get { return properties.Get("EventBackground", Brushes.Transparent); }
			set { properties.Set("EventBackground", value); }
		}
		
		public static Brush NamespaceDeclarationForegroundBrush {
			get { return properties.Get("NamespaceDeclarationForeground", Brushes.Orange); }
			set { properties.Set("NamespaceDeclarationForeground", value); }
		}
		
		public static Brush NamespaceDeclarationBackgroundBrush { 
			get { return properties.Get("NamespaceDeclarationBackground", Brushes.Transparent); }
			set { properties.Set("NamespaceDeclarationBackground", value); }
		}
		
		public static Brush IgnoredForegroundBrush {
			get { return properties.Get("IgnoredForeground", Brushes.LightGray); }
			set { properties.Set("IgnoredForeground", value); }
		}
		
		public static Brush IgnoredBackgroundBrush {
			get { return properties.Get("IgnoredBackground", Brushes.Transparent); }
			set { properties.Set("IgnoredBackground", value); }
		}
	}
}
