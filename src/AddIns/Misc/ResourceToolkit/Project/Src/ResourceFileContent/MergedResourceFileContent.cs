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
	/// Makes multiple <see cref="IResourceFileContent" /> implementations accessible
	/// through a single one. All contents are merged into a single dictionary.
	/// When adding new entries, they are added to the master content.
	/// </summary>
	public class MergedResourceFileContent : IMultiResourceFileContent
	{
		
		IResourceFileContent masterContent;
		IResourceFileContent[] otherContents;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="MergedResourceFileContent" /> class.
		/// </summary>
		/// <param name="masterContent">The master resource file content.</param>
		/// <param name="otherContents">Additional resource file contents.</param>
		/// <exception cref="ArgumentException">The cultures of the specified resource file contents do not match.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String)")]
		public MergedResourceFileContent(IResourceFileContent masterContent, IResourceFileContent[] otherContents)
		{
			this.masterContent = masterContent;
			this.otherContents = otherContents;
			
			// Ensure that all contents are for the same culture
			foreach (IResourceFileContent c in this.otherContents) {
				if (!c.Culture.Equals(this.masterContent.Culture)) {
					throw new ArgumentException("The cultures of the specified resource file contents do not match.");
				}
			}
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Gets the file name of the master resource file.
		/// </summary>
		public string FileName {
			get {
				return this.masterContent.FileName;
			}
		}
		
		/// <summary>
		/// Gets the culture of the resource files this instance represents.
		/// </summary>
		public CultureInfo Culture {
			get {
				return this.masterContent.Culture;
			}
		}
		
		/// <summary>
		/// Gets an iterator that can be used to iterate over all key/value pairs in all resource files this instance represents.
		/// </summary>
		public IEnumerable<KeyValuePair<string, object>> Data {
			get {
				foreach (KeyValuePair<string, object> entry in this.masterContent.Data) {
					yield return entry;
				}
				foreach (IResourceFileContent c in this.otherContents) {
					foreach (KeyValuePair<string, object> entry in c.Data) {
						yield return entry;
					}
				}
			}
		}
		
		/// <summary>
		/// Determines if any of the resource files this instance represents contains the specified key.
		/// </summary>
		public bool ContainsKey(string key)
		{
			if (this.masterContent.ContainsKey(key)) {
				return true;
			}
			foreach (IResourceFileContent c in this.otherContents) {
				if (c.ContainsKey(key)) {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Tries to get the value of the resource with the specified key.
		/// </summary>
		/// <returns><c>true</c>, if the key exists, otherwise <c>false</c>.</returns>
		public bool TryGetValue(string key, out object value)
		{
			if (this.masterContent.TryGetValue(key, out value)) {
				return true;
			}
			foreach (IResourceFileContent c in this.otherContents) {
				if (c.TryGetValue(key, out value)) {
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Adds a new key to the master resource file.
		/// </summary>
		/// <exception cref="ArgumentException">A key with the same name already exists.</exception>
		public void Add(string key, object value)
		{
			this.masterContent.Add(key, value);
		}
		
		/// <summary>
		/// Modify the value of an existing entry.
		/// </summary>
		/// <exception cref="ArgumentException">The specified key does not exist.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String,System.String)")]
		public void SetValue(string key, object value)
		{
			if (this.masterContent.ContainsKey(key)) {
				this.masterContent.SetValue(key, value);
				return;
			} else {
				foreach (IResourceFileContent c in this.otherContents) {
					if (c.ContainsKey(key)) {
						c.SetValue(key, value);
						return;
					}
				}
			}
			throw new ArgumentException("The key '"+key+"' does not exist.", "key");
		}
		
		/// <summary>
		/// Renames a resource key.
		/// </summary>
		/// <param name="oldName">The old name of the resource key to rename.</param>
		/// <param name="newName">The new name of the resource key.</param>
		/// <exception cref="ArgumentException">The specified key does not exist or the new key does already exist.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String,System.String)")]
		public void RenameKey(string oldName, string newName)
		{
			if (this.masterContent.ContainsKey(oldName)) {
				this.masterContent.RenameKey(oldName, newName);
				return;
			} else {
				foreach (IResourceFileContent c in this.otherContents) {
					if (c.ContainsKey(oldName)) {
						c.RenameKey(oldName, newName);
						return;
					}
				}
			}
			throw new ArgumentException("The key '"+oldName+"' does not exist.", "oldName");
		}
		
		/// <summary>
		/// Removes the specified resource key permanently.
		/// </summary>
		/// <param name="key">The resource key to remove.</param>
		/// <exception cref="ArgumentException">The specified key does not exist.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentException.#ctor(System.String,System.String)")]
		public void RemoveKey(string key)
		{
			if (this.masterContent.ContainsKey(key)) {
				this.masterContent.RemoveKey(key);
				return;
			} else {
				foreach (IResourceFileContent c in this.otherContents) {
					if (c.ContainsKey(key)) {
						c.RemoveKey(key);
						return;
					}
				}
			}
			throw new ArgumentException("The key '"+key+"' does not exist.", "key");
		}
		
		/// <summary>
		/// Gets the file name of the resource file the specified key is in.
		/// </summary>
		/// <returns>The name of the resource file the specified key is in, or <c>null</c> if the key cannot be found in any resource file this instance represents.</returns>
		public string GetFileNameForKey(string key)
		{
			string fileName;
			IMultiResourceFileContent mrfc;
			if ((mrfc = (this.masterContent as IMultiResourceFileContent)) != null) {
				if ((fileName = mrfc.GetFileNameForKey(key)) != null) {
					return fileName;
				}
			} else if (this.masterContent.ContainsKey(key)) {
				return this.masterContent.FileName;
			}
			foreach (IResourceFileContent c in this.otherContents) {
				if ((mrfc = (c as IMultiResourceFileContent)) != null) {
					if ((fileName = mrfc.GetFileNameForKey(key)) != null) {
						return fileName;
					}
				} else if (c.ContainsKey(key)) {
					return c.FileName;
				}
			}
			return null;
		}
		
	}
}
