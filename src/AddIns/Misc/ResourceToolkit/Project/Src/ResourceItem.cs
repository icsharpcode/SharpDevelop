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

namespace Hornung.ResourceToolkit
{
	/// <summary>
	/// Describes a resource item by file name and key.
	/// </summary>
	public class ResourceItem
	{
		readonly string fileName;
		readonly string key;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceItem"/> class.
		/// </summary>
		/// <param name="fileName">The name of the resource file.</param>
		/// <param name="key">The resource key.</param>
		/// <exception cref="ArgumentNullException">The <paramref name="fileName"/> parameter is <c>null</c>.</exception>
		public ResourceItem(string fileName, string key)
		{
			if (fileName == null) {
				throw new ArgumentNullException("fileName");
			}
			this.fileName = fileName;
			this.key = key;
		}
		
		/// <summary>
		/// Gets the name of the resource file this resource item is contained in.
		/// </summary>
		public string FileName {
			get {
				return fileName;
			}
		}
		
		/// <summary>
		/// Gets the resource key of this resource item.
		/// </summary>
		public string Key {
			get {
				return key;
			}
		}
	}
}
