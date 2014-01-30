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
using System.Windows.Media;

using ICSharpCode.Core;

namespace ICSharpCode.XamlBinding
{	
	public static class XamlBindingOptions
	{
		static Properties properties;

		static XamlBindingOptions()
		{
			properties = PropertyService.NestedProperties("XamlBinding.Options");
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
