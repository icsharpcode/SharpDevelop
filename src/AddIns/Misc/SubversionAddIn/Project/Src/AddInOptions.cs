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
		public static string DefaultLogMessage {
			get {
				return properties.Get("DefaultLogMessage",
				                      "# All lines starting with a # will be ignored" + Environment.NewLine +
				                      "# This template can be modified by using the 'Tools->IDE Options->Source Control->Subversion' panel");
			}
			set {
				properties.Set("DefaultLogMessage", value);
			}
		}
		
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
		
		public static bool AutomaticallyReloadProject {
			get {
				return properties.Get("AutomaticallyReloadProject", true);
			}
			set {
				properties.Set("AutomaticallyReloadProject", value);
			}
		}
		#endregion
	}
}
