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
