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

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Holds the options for the PythonBinding AddIn
	/// </summary>
	public class PythonAddInOptions
	{
		/// <summary>
		/// The name of the options as read from the PropertyService.
		/// </summary>
		public static readonly string AddInOptionsName = "PythonBinding.Options";
		
		/// <summary>
		/// The default python console filename.
		/// </summary>
		public static readonly string DefaultPythonFileName = "ipy.exe";
		
		#region Property names
		public static readonly string PythonFileNameProperty = "PythonFileName";
		public static readonly string PythonLibraryPathProperty = "PythonLibraryPath";
		#endregion
		
		Properties properties;
		
		public PythonAddInOptions()
			: this(PropertyService.Get(AddInOptionsName, new Properties()))
		{
		}
		
		/// <summary>
		/// Creates the addin options class which will use
		/// the options from the properties class specified.
		/// </summary>
		public PythonAddInOptions(Properties properties)
		{
			this.properties = properties;
		}
		
		public string PythonLibraryPath {
			get { return properties.Get<string>(PythonLibraryPathProperty, String.Empty); }
			set { properties.Set(PythonLibraryPathProperty, value); }
		}
		
		public bool HasPythonLibraryPath {
			get { return !String.IsNullOrEmpty(PythonLibraryPath); }
		}
		
		/// <summary>
		/// Gets or sets the python console filename.
		/// </summary>
		public string PythonFileName {
			get { return properties.Get<string>(PythonFileNameProperty, GetDefaultPythonFileName()); }
			set {
				if (String.IsNullOrEmpty(value)) {
					properties.Set(PythonFileNameProperty, GetDefaultPythonFileName());
				} else {
					properties.Set(PythonFileNameProperty, value);
				}
			}
		}
		
		/// <summary>
		/// Returns the full path to ipyw.exe which is installed in the
		/// Python addin folder.
		/// </summary>
		string GetDefaultPythonFileName()
		{
			string path = GetPythonBindingAddInPath();
			return Path.Combine(path, DefaultPythonFileName);
		}
		
		string GetPythonBindingAddInPath()
		{
			return StringParser.Parse("${addinpath:ICSharpCode.PythonBinding}");
		}
	}
}
