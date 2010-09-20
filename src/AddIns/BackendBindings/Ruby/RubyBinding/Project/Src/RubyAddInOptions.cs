// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.RubyBinding
{
	public class RubyAddInOptions
	{
		/// <summary>
		/// The name of the options as read from the PropertyService.
		/// </summary>
		public static readonly string AddInOptionsName = "RubyBinding.Options";
		
		/// <summary>
		/// The default ruby console filename.
		/// </summary>
		public static readonly string DefaultRubyFileName = "ir.exe";
		
		#region Property names
		public static readonly string RubyFileNameProperty = "RubyFileName";
		public static readonly string RubyLibraryPathProperty = "RubyLibraryPath";
		#endregion
		
		Properties properties;
		
		public RubyAddInOptions()
			: this(PropertyService.Get(AddInOptionsName, new Properties()))
		{
		}
		
		/// <summary>
		/// Creates the addin options class which will use
		/// the options from the properties class specified.
		/// </summary>
		public RubyAddInOptions(Properties properties)
		{
			this.properties = properties;
		}
		
		public string RubyLibraryPath {
			get { return properties.Get<string>(RubyLibraryPathProperty, String.Empty); }
			set { properties.Set(RubyLibraryPathProperty, value.Trim()); }
		}
		
		public bool HasRubyLibraryPath {
			get { return !String.IsNullOrEmpty(RubyLibraryPath); }
		}

		/// <summary>
		/// Gets or sets the ruby console filename.
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
