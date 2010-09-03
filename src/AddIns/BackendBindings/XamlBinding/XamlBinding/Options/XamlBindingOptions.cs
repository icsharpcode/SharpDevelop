// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public static bool UseExtensionCompletion {
			get { return Properties.Get("UseExtensionCompletion", true); }
			set { Properties.Set("UseExtensionCompletion", value); }
		}
		
		public static bool SwitchToCodeViewAfterInsertion {
			get { return Properties.Get("SwitchToCodeViewAfterInsertion", true); }
			set { Properties.Set("SwitchToCodeViewAfterInsertion", value); }
		}
		
		public static bool UseAdvancedHighlighting {
			get { return Properties.Get("UseAdvancedHighlighting", true); }
			set { Properties.Set("UseAdvancedHighlighting", value); }
		}
		
		public static string EventHandlerNamePattern {
			get { return Properties.Get("EventHandlerNamePattern", "%Object%_%Event%"); }
			set { Properties.Set("EventHandlerNamePattern", value); }
		}
		
		public static Color PropertyForegroundColor {
			get { return Properties.Get("PropertyForeground", Colors.CadetBlue); }
			set { Properties.Set("PropertyForeground", value); }
		}
		
		public static Color PropertyBackgroundColor {
			get { return Properties.Get("PropertyBackground", Colors.Transparent); }
			set { Properties.Set("PropertyBackground", value); }
		}
		
		public static Color EventForegroundColor {
			get { return Properties.Get("EventForeground", Colors.Green); }
			set { Properties.Set("EventForeground", value); }
		}

		public static Color EventBackgroundColor {
			get { return Properties.Get("EventBackground", Colors.Transparent); }
			set { Properties.Set("EventBackground", value); }
		}
		
		public static Color NamespaceDeclarationForegroundColor {
			get { return Properties.Get("NamespaceDeclarationForeground", Colors.Orange); }
			set { Properties.Set("NamespaceDeclarationForeground", value); }
		}
		
		public static Color NamespaceDeclarationBackgroundColor { 
			get { return Properties.Get("NamespaceDeclarationBackground", Colors.Transparent); }
			set { Properties.Set("NamespaceDeclarationBackground", value); }
		}
		
		public static Color IgnoredForegroundColor {
			get { return Properties.Get("IgnoredForeground", Colors.LightGray); }
			set { Properties.Set("IgnoredForeground", value); }
		}
		
		public static Color IgnoredBackgroundColor {
			get { return Properties.Get("IgnoredBackground", Colors.Transparent); }
			set { Properties.Set("IgnoredBackground", value); }
		}
	}
}
