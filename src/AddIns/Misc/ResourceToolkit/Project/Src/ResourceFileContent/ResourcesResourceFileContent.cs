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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;

using ICSharpCode.Core;

namespace Hornung.ResourceToolkit.ResourceFileContent
{
	/// <summary>
	/// Describes the content of a .resources resource file.
	/// </summary>
	public class ResourcesResourceFileContent : IResourceFileContent
	{
		
		readonly string fileName;
		readonly CultureInfo culture;
		
		DateTime lastWriteTimeUtc;
		Dictionary<string, object> data;
		
		/// <summary>
		/// Gets the file name of the resource file this instance represents.
		/// </summary>
		public string FileName {
			get {
				return this.fileName;
			}
		}
		
		/// <summary>
		/// Gets the culture of the resource file this instance represents.
		/// </summary>
		public CultureInfo Culture {
			get {
				return this.culture;
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourcesFileContent" /> class.
		/// </summary>
		/// <param name="fileName">The file name of the resource file this instance represents.</param>
		public ResourcesResourceFileContent(string fileName)
		{
			this.fileName = fileName;
			// Determine culture from file name
			string cultureExt = Path.GetExtension(Path.GetFileNameWithoutExtension(fileName));
			if (!String.IsNullOrEmpty(cultureExt)) {
				try {
					this.culture = CultureInfo.GetCultureInfo(cultureExt.Substring(1));	// need to remove leading dot from cultureExt
				} catch (ArgumentException) {
					this.culture = CultureInfo.InvariantCulture;
				}
			} else {
				this.culture = CultureInfo.InvariantCulture;
			}
			
			#if DEBUG
			LoggingService.Debug("ResourceToolkit: Created ResourceFileContent, file '"+fileName+"', culture name: '"+this.Culture.Name+"'");
			#endif
		}
		
		/// <summary>
		/// Synchronises the cache with the content of the actual file on disk.
		/// </summary>
		protected void EnsureLoaded()
		{
			if (this.data == null || File.GetLastWriteTimeUtc(this.FileName) != this.lastWriteTimeUtc) {
				this.InitializeContent();
				this.LoadContent();
				this.lastWriteTimeUtc = File.GetLastWriteTimeUtc(this.FileName);
			}
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes all instance fields in preparation for loading of the file content.
		/// </summary>
		protected virtual void InitializeContent()
		{
			this.data = new Dictionary<string, object>();
		}
		
		/// <summary>
		/// Loads the content of the specified <see cref="IResourceReader" /> into the cache.
		/// </summary>
		/// <param name="reader">The <see cref="IResourceReader" /> to be used to read the resource content.</param>
		protected virtual void LoadContent(IResourceReader reader)
		{
			IDictionaryEnumerator en = reader.GetEnumerator();
			while (en.MoveNext()) {
				this.data.Add((string)en.Key, en.Value);
			}
		}
		
		/// <summary>
		/// Loads the content of the file into the cache.
		/// </summary>
		protected virtual void LoadContent()
		{
			using(IResourceReader reader = this.GetResourceReader()) {
				if (reader != null) {
					this.LoadContent(reader);
					reader.Close();
				}
			}
		}
		
		/// <summary>
		/// Gets a resource reader for the resource file represented by this instance.
		/// </summary>
		/// <returns>A resource reader for the resource file represented by this instance.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		protected virtual IResourceReader GetResourceReader()
		{
			return new ResourceReader(this.FileName);
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Save changes done to the resource content to disk.
		/// </summary>
		protected void Save()
		{
			this.SaveContent();
			this.lastWriteTimeUtc = File.GetLastWriteTimeUtc(this.FileName);
		}
		
		/// <summary>
		/// Save changes done to the resource content to disk.
		/// </summary>
		protected virtual void SaveContent()
		{
			using(IResourceWriter writer = this.GetResourceWriter()) {
				if (writer != null) {
					this.SaveContent(writer);
					writer.Close();
				}
			}
		}
		
		/// <summary>
		/// Save changes done to the resource content to disk.
		/// </summary>
		/// <param name="writer">The <see cref="IResourceWriter" /> to be used to save the resource content.</param>
		protected virtual void SaveContent(IResourceWriter writer)
		{
			foreach (KeyValuePair<string, object> entry in this.data) {
				writer.AddResource(entry.Key, entry.Value);
			}
		}
		
		/// <summary>
		/// Gets a resource writer for the resource file represented by this instance.
		/// </summary>
		/// <returns>A resource writer for the resource file represented by this instance.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		protected virtual IResourceWriter GetResourceWriter()
		{
			return new ResourceWriter(this.FileName);
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Gets an iterator that can be used to iterate over all key/value pairs in this resource.
		/// </summary>
		public IEnumerable<KeyValuePair<string, object>> Data {
			get {
				this.EnsureLoaded();
				return this.data;
			}
		}
		
		/// <summary>
		/// Determines if the resource file this instance represents contains the specified key.
		/// </summary>
		public bool ContainsKey(string key)
		{
			this.EnsureLoaded();
			return this.data.ContainsKey(key);
		}
		
		/// <summary>
		/// Tries to get the value of the resource with the specified key.
		/// </summary>
		/// <returns><c>true</c>, if the key exists, otherwise <c>false</c>.</returns>
		public bool TryGetValue(string key, out object value)
		{
			this.EnsureLoaded();
			return this.data.TryGetValue(key, out value);
		}
		
		/// <summary>
		/// Adds a new key to the resource file.
		/// </summary>
		/// <exception cref="ArgumentException">A key with the same name already exists.</exception>
		public void Add(string key, object value)
		{
			this.EnsureLoaded();
			if (this.data.ContainsKey(key)) {
				throw new ArgumentException("A key with the name '"+key+"' already exists.", "key");
			}
			this.data.Add(key, value);
			this.Save();
		}
		
		/// <summary>
		/// Modify the value of an existing entry.
		/// </summary>
		/// <exception cref="ArgumentException">The specified key does not exist.</exception>
		public void SetValue(string key, object value)
		{
			this.EnsureLoaded();
			if (!this.data.ContainsKey(key)) {
				throw new ArgumentException("The key '"+key+"' does not exist.", "key");
			}
			this.data[key] = value;
			this.Save();
		}
		
		/// <summary>
		/// Renames a resource key.
		/// </summary>
		/// <param name="oldName">The old name of the resource key to rename.</param>
		/// <param name="newName">The new name of the resource key.</param>
		/// <exception cref="ArgumentException">The specified key does not exist or the new key does already exist.</exception>
		public void RenameKey(string oldName, string newName)
		{
			this.EnsureLoaded();
			if (!this.data.ContainsKey(oldName)) {
				throw new ArgumentException("The key '"+oldName+"' does not exist.", "oldName");
			}
			if (this.data.ContainsKey(newName)) {
				throw new ArgumentException("The key '"+newName+"' already exists.", "newName");
			}
			this.data.Add(newName, this.data[oldName]);
			this.data.Remove(oldName);
			this.Save();
		}
		
		/// <summary>
		/// Removes the specified resource key permanently.
		/// </summary>
		/// <param name="key">The resource key to remove.</param>
		/// <exception cref="ArgumentException">The specified key does not exist.</exception>
		public void RemoveKey(string key)
		{
			this.EnsureLoaded();
			if (!this.data.ContainsKey(key)) {
				throw new ArgumentException("The key '"+key+"' does not exist.", "key");
			}
			this.data.Remove(key);
			this.Save();
		}
		
	}
}
