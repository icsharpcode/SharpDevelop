// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision: 3731 $</version>
// </file>

using System;
using ICSharpCode.Core;
using System.Diagnostics;

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
	}
}
