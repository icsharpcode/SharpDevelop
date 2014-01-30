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
using ICSharpCode.Core;

namespace ICSharpCode.Svn
{
	public class AddInOptions
	{
		public static readonly string OptionsProperty = "ICSharpCode.Svn.Options";
		
		static Properties properties;
		
		static AddInOptions()
		{
			properties = PropertyService.NestedProperties(OptionsProperty);
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
