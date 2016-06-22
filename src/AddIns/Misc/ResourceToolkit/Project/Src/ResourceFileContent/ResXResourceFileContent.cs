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
using System.Resources;

namespace Hornung.ResourceToolkit.ResourceFileContent
{
	/// <summary>
	/// Describes the content of a .resx resource file.
	/// </summary>
	public class ResXResourceFileContent : ResourcesResourceFileContent
	{
		
		Dictionary<string, object> metadata;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ResXResourceFileContent" /> class.
		/// </summary>
		/// <param name="fileName">The file name of the resource file this instance represents.</param>
		public ResXResourceFileContent(string fileName) : base(fileName)
		{
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes all instance fields in preparation for loading of the file content.
		/// </summary>
		protected override void InitializeContent()
		{
			base.InitializeContent();
			this.metadata = new Dictionary<string, object>();
		}
		
		/// <summary>
		/// Loads the content of the specified <see cref="IResourceReader" /> into the cache.
		/// </summary>
		/// <param name="reader">The <see cref="IResourceReader" /> to be used to read the resource content.</param>
		protected override void LoadContent(IResourceReader reader)
		{
			base.LoadContent(reader);
			IDictionaryEnumerator en = ((ResXResourceReader)reader).GetMetadataEnumerator();
			while (en.MoveNext()) {
				this.metadata.Add((string)en.Key, en.Value);
			}
		}
		
		/// <summary>
		/// Gets a resx resource reader for the resource file represented by this instance.
		/// </summary>
		/// <returns>A resx resource reader for the resource file represented by this instance.</returns>
		protected override IResourceReader GetResourceReader()
		{
			return new ResXResourceReader(this.FileName);
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Save changes done to the resource content to disk.
		/// </summary>
		/// <param name="writer">The <see cref="IResourceWriter" /> to be used to save the resource content.</param>
		protected override void SaveContent(IResourceWriter writer)
		{
			base.SaveContent(writer);
			ResXResourceWriter w = (ResXResourceWriter)writer;
			foreach (KeyValuePair<string, object> entry in this.metadata) {
				w.AddMetadata(entry.Key, entry.Value);
			}
		}
		
		/// <summary>
		/// Gets a resx resource writer for the resource file represented by this instance.
		/// </summary>
		/// <returns>A resx resource writer for the resource file represented by this instance.</returns>
		protected override IResourceWriter GetResourceWriter()
		{
			return new ResXResourceWriter(this.FileName);
		}
		
	}
}
