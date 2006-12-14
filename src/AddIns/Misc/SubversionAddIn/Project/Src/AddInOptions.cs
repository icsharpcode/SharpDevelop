// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.Svn
{
	public class AddInOptions
	{
		public static readonly string OptionsProperty = "ICSharpCode.Svn.Options";
		
		static Properties properties;
		
		static AddInOptions()
		{
			properties = PropertyService.Get(OptionsProperty, new Properties());
		}
		
		#region Properties
		public static bool AutomaticallyAddFiles {
			get {
				return properties.Get("AutomaticallyAddFiles", true);
			}
			set {
				properties.Set("AutomaticallyAddFiles", value);
			}
		}
		
		public static bool AutomaticallyDeleteFiles {
			get {
				return properties.Get("AutomaticallyDeleteFiles", true);
			}
			set {
				properties.Set("AutomaticallyDeleteFiles", value);
			}
		}
		
		public static bool AutomaticallyRenameFiles {
			get {
				return properties.Get("AutomaticallyRenameFiles", true);
			}
			set {
				properties.Set("AutomaticallyRenameFiles", value);
			}
		}
		
		public static bool AutomaticallyReloadProject {
			get {
				return properties.Get("AutomaticallyReloadProject", true);
			}
			set {
				properties.Set("AutomaticallyReloadProject", value);
			}
		}
		
		public static bool UseHistoryDisplayBinding {
			get {
				return properties.Get("UseHistoryDisplayBinding", true);
			}
			set {
				properties.Set("UseHistoryDisplayBinding", value);
			}
		}
		#endregion
	}
}
