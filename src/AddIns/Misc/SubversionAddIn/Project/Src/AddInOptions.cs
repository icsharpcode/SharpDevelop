using System;
using System.Diagnostics;
using ICSharpCode.Core;

namespace ICSharpCode.Svn
{
	public class AddInOptions
	{
		public static readonly string OptionsProperty = "ICSharpCode.Svn.Options";
		
		static Properties properties;
		
		static AddInOptions()
		{
			
			properties = (Properties)PropertyService.Get(OptionsProperty, new Properties());
		}
		
		static Properties Properties {
			get {
				Debug.Assert(properties != null);
				return properties;
			}
		}
		
		#region Properties
		public static string DefaultLogMessage {
			get {
				return Properties.Get("DefaultLogMessage",
				                      "# All lines starting with a # will be ignored" + Environment.NewLine +
				                      "# This template can be modified by using the 'Tools->IDE Options->Source Control->Subversion' panel");
			}
			set {
				Properties.Set("DefaultLogMessage", value);
			}
		}
		
		public static bool AutomaticallyAddFiles {
			get {
				return Properties.Get("AutomaticallyAddFiles", true);
			}
			set {
				Properties.Set("AutomaticallyAddFiles", value);
			}
		}
		
		public static bool AutomaticallyDeleteFiles {
			get {
				return Properties.Get("AutomaticallyDeleteFiles", false);
			}
			set {
				Properties.Set("AutomaticallyDeleteFiles", value);
			}
		}
		
		public static bool AutomaticallyReloadProject {
			get {
				return Properties.Get("AutomaticallyReloadProject", true);
			}
			set {
				Properties.Set("AutomaticallyReloadProject", value);
			}
		}
		#endregion
	}
}
