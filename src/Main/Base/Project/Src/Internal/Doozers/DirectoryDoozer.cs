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
using System.Collections;
using System.IO;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Creates path names using a relative to the folder containing the addin file.
	/// </summary>
	/// <attribute name="path" use="required">
	/// Path relative to the directory which contains the .addin file defining the codon.
	/// </attribute>
	/// <usage>Where directory paths to a folder inside the addin directory are expected, e.g.
	/// /SharpDevelop/BackendBindings/Templates</usage>
	/// <returns>
	/// A string containing the full path name.
	/// </returns>
	public class DirectoryDoozer : IDoozer
	{
		public bool HandleConditions { get { return false; } }
		
		public object BuildItem(BuildItemArgs args)
		{
			return Path.Combine(Path.GetDirectoryName(args.AddIn.FileName), args.Codon["path"]);
		}
	}
}
