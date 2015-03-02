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
using System.ComponentModel;
using System.Windows;

namespace ICSharpCode.WpfDesign.Designer.themes
{
	public class VersionedAssemblyResourceDictionary : ResourceDictionary, ISupportInitialize
	{
		private static readonly string _uriStart;

		private static readonly int _subLength;

		static VersionedAssemblyResourceDictionary()
		{
			var nm = typeof(VersionedAssemblyResourceDictionary).Assembly.GetName();
			_uriStart = string.Format( @"/{0};v{1};component/", nm.Name, nm.Version);
			
			_subLength = "ICSharpCode.WpfDesign.Designer.".Length;
		}

		public string RelativePath {get;set;}

		void ISupportInitialize.EndInit()
		{
			this.Source = new Uri(_uriStart + this.RelativePath, UriKind.Relative);
			base.EndInit();
		}
		
		public static string GetXamlNameForType(Type t)
		{
			return _uriStart + t.FullName.Substring(_subLength).Replace(".","/").ToLower() + ".xaml";
		}
	}
}
