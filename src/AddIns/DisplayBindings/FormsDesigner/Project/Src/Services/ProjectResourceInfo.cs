// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.IO;
using System.Resources;

using ICSharpCode.SharpDevelop;

namespace ICSharpCode.FormsDesigner.Services
{
	/// <summary>
	/// Describes a project resource reference.
	/// </summary>
	public sealed class ProjectResourceInfo
	{
		readonly string resourceFile;
		readonly string resourceKey;
		readonly object originalValue;
		
		/// <summary>
		/// Gets the full file name of the resource file that contains the resource.
		/// </summary>
		public string ResourceFile {
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
		public ProjectResourceInfo(string resourceFile, string resourceKey)
		{
			if (resourceFile == null)
				throw new ArgumentNullException("resourceFile");
			if (resourceKey == null)
				throw new ArgumentNullException("resourceKey");
			this.resourceFile = resourceFile;
			this.resourceKey = resourceKey;
			
			if (File.Exists(resourceFile)) {
				
				OpenedFile openedFile = FileService.GetOpenedFile(resourceFile);
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
