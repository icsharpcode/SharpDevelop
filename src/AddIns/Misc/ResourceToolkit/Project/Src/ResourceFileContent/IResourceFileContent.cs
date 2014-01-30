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
using System.Globalization;

namespace Hornung.ResourceToolkit.ResourceFileContent
{
	/// <summary>
	/// Describes the content of a resource file.
	/// </summary>
	public interface IResourceFileContent
	{
		
		/// <summary>
		/// Gets the file name of the resource file this instance represents.
		/// If this instance represents multiple resource files, it's the name
		/// of the resource file which new entries are added to.
		/// </summary>
		string FileName {
			get;
		}
		
		/// <summary>
		/// Gets the culture of the resource file this instance represents.
		/// </summary>
		CultureInfo Culture {
			get;
		}
		
		/// <summary>
		/// Gets an iterator that can be used to iterate over all key/value pairs in this resource.
		/// </summary>
		IEnumerable<KeyValuePair<string, object>> Data {
			get;
		}
		
		/// <summary>
		/// Determines if the resource file this instance represents contains the specified key.
		/// </summary>
		bool ContainsKey(string key);
		
		/// <summary>
		/// Tries to get the value of the resource with the specified key.
		/// </summary>
		/// <returns><c>true</c>, if the key exists, otherwise <c>false</c>.</returns>
		bool TryGetValue(string key, out object value);
		
		/// <summary>
		/// Adds a new key to the resource file.
		/// </summary>
		/// <exception cref="ArgumentException">A key with the same name already exists.</exception>
		void Add(string key, object value);
		
		/// <summary>
		/// Modify the value of an existing entry.
		/// </summary>
		/// <exception cref="ArgumentException">The specified key does not exist.</exception>
		void SetValue(string key, object value);
		
		/// <summary>
		/// Renames a resource key.
		/// </summary>
		/// <param name="oldName">The old name of the resource key to rename.</param>
		/// <param name="newName">The new name of the resource key.</param>
		/// <exception cref="ArgumentException">The specified key does not exist.</exception>
		void RenameKey(string oldName, string newName);
		
		/// <summary>
		/// Removes the specified resource key permanently.
		/// </summary>
		/// <param name="key">The resource key to remove.</param>
		/// <exception cref="ArgumentException">The specified key does not exist.</exception>
		void RemoveKey(string key);
		
	}
}
