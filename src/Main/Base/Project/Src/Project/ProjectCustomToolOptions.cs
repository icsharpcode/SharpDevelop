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
using System.Linq;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ProjectCustomToolOptions
	{
		Properties properties;
		
		public ProjectCustomToolOptions(IProject project)
		{
			properties = project.Preferences.NestedProperties("customTool");
		}
		
		public bool RunCustomToolOnBuild {
			get { return properties.Get("runOnBuild", false); }
			set { properties.Set("runOnBuild", value); }
		}
		
		public string FileNames {
			get { return properties.Get("fileNames", String.Empty); }
			set { properties.Set("fileNames", value); }
		}
		
		public IList<string> SplitFileNames()
		{
			return
				FileNames
				.Replace("\r\n", ";")
				.Split(';', ',')
				.Select(fileName => fileName.Trim())
				.Where(fileName => !String.IsNullOrEmpty(fileName))
				.ToList();
		}
	}
}
