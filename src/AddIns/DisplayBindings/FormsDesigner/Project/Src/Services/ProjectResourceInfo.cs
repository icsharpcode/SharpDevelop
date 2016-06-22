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
using System.Resources;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.FormsDesigner.Services
{
	/// <summary>
	/// Describes a project resource reference.
	/// </summary>
	public sealed class ProjectResourceInfo
	{
		readonly FileName resourceFile;
		readonly string resourceKey;
		readonly object originalValue;
		
		/// <summary>
		/// Gets the full file name of the resource file that contains the resource.
		/// </summary>
		public FileName ResourceFile {
			get { return resourceFile; }
		}
		
		/// <summary>
		/// Gets the resource key.
		/// </summary>
		public string ResourceKey {
			get { return resourceKey; }
		}
		
		/// <summary>
		/// Gets the value of the resource at creation time of this instance.
		/// Can be <c>null</c> if the resource or the file was not found.
		/// </summary>
		public object OriginalValue {
			get { return originalValue; }
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectResourceInfo"/> class
		/// and stores the resource value in the <see cref="OriginalValue"/> property
		/// if the resource file is found and it contains the specified key.
		/// </summary>
		/// <param name="resourceFile">The full name of the resource file that contains the resource.</param>
		/// <param name="resourceKey">The resource key.</param>
		public ProjectResourceInfo(FileName resourceFile, string resourceKey)
		{
			if (resourceFile == null)
				throw new ArgumentNullException("resourceFile");
			if (resourceKey == null)
				throw new ArgumentNullException("resourceKey");
			this.resourceFile = resourceFile;
			this.resourceKey = resourceKey;
			
			if (File.Exists(resourceFile)) {
				
				OpenedFile openedFile = SD.FileService.GetOpenedFile(resourceFile);
				Stream s;
				if (openedFile != null) {
					s = openedFile.OpenRead();
				} else {
					s = new FileStream(resourceFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				}
				
				using(s) {
					using(IResourceReader reader = ResourceStore.CreateResourceReader(s, ResourceStore.GetResourceType(resourceFile))) {
						
						ResXResourceReader resXReader = reader as ResXResourceReader;
						if (resXReader != null) {
							resXReader.BasePath = Path.GetDirectoryName(resourceFile);
						}
						
						foreach (DictionaryEntry entry in reader) {
							if (String.Equals(resourceKey, entry.Key as string, StringComparison.Ordinal)) {
								this.originalValue = entry.Value;
								break;
							}
						}
					}
				}
				
			}
		}
	}
}
