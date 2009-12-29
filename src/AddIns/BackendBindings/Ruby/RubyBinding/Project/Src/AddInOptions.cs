// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Holds the options for the RubyBinding AddIn
	/// </summary>
	public class AddInOptions
	{
		/// <summary>
		/// The name of the options as read from the PropertyService.
		/// </summary>
		public static readonly string AddInOptionsName = "RubyBinding.Options";
		
		/// <summary>
		/// The default Ruby console filename.
		/// </summary>
		public static readonly string DefaultRubyFileName = "ir.exe";
		
		#region Property names
		public static readonly string RubyFileNameProperty = "RubyFileName";
		#endregion
		
		Properties properties;
		
		public AddInOptions()
			: this(PropertyService.Get(AddInOptionsName, new Properties()))
		{
		}
		
		/// <summary>
		/// Creates the addin options class which will use
		/// the options from the properties class specified.
		/// </summary>
		public AddInOptions(Properties properties)
		{
			this.properties = properties;
		}
		
		/// <summary>
		/// Gets or sets the Ruby console filename.
		/// </summary>
		public string RubyFileName {
			get {
				return properties.Get<string>(RubyFileNameProperty, GetDefaultRubyFileName());
			}
			set {
				if (String.IsNullOrEmpty(value)) {
					properties.Set(RubyFileNameProperty, GetDefaultRubyFileName());
				} else {
					properties.Set(RubyFileNameProperty, value);
				}
			}
		}
		
		/// <summary>
		/// Gets the path to the specified addin.
		/// </summary>
		/// <param name="addIn">The addin name: "${addin:ICSharpCode.RubyBinding}"</param>
		protected virtual string GetAddInPath(string addIn)
		{
			return StringParser.Parse(addIn);
		}
		
		/// <summary>
		/// Returns the full path to ir.exe which is installed in the
		/// Ruby addin folder.
		/// </summary>
		string GetDefaultRubyFileName()
		{
			string path = GetAddInPath("${addinpath:ICSharpCode.RubyBinding}");
			return Path.Combine(path, DefaultRubyFileName);
		}
	}
}
