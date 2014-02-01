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
using System.Xml;

using ICSharpCode.XmlEditor;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Common base class for the WixDirectoryElement and WixDirectoryRefElement
	/// classes.
	/// </summary>
	public abstract class WixDirectoryElementBase : WixElementBase
	{
		public WixDirectoryElementBase(string localName, WixDocument document)
			: base(localName, document)
		{
		}
		
		/// <summary>
		/// Gets any child directory elements.
		/// </summary>
		public WixDirectoryElement[] GetDirectories()
		{
			List<WixDirectoryElement> directories = new List<WixDirectoryElement>();
			foreach (XmlNode childNode in ChildNodes) {
				WixDirectoryElement childElement = childNode as WixDirectoryElement;
				if (childElement != null) {
					directories.Add(childElement);
				}
			}
			return directories.ToArray();
		}
		
		/// <summary>
		/// Gets any child component elements.
		/// </summary>
		public WixComponentElement[] GetComponents()
		{
			List<WixComponentElement> components = new List<WixComponentElement>();
			foreach (XmlNode childNode in ChildNodes) {
				WixComponentElement childElement = childNode as WixComponentElement;
				if (childElement != null) {
					components.Add(childElement);
				}
			}
			return components.ToArray();
		}

		/// <summary>
		/// Adds a new directory with the specified name and generates a unique id 
		/// for the directory.
		/// </summary>
		public WixDirectoryElement AddDirectory(string name)
		{
			WixDirectoryElement directoryElement = new WixDirectoryElement(OwnerWixDocument);
			directoryElement.Id = GenerateUniqueChildDirectoryId(name);
			directoryElement.DirectoryName = name;
			AppendChild(directoryElement);
			return directoryElement;
		}
		
		string GenerateUniqueChildDirectoryId(string childDirectoryId)
		{
			childDirectoryId = WixFileElement.GenerateId(childDirectoryId);
			if (!OwnerWixDocument.DirectoryIdExists(childDirectoryId)) {
				return childDirectoryId;
			}
			return GenerateUniqueChildDirectoryIdUsingParentDirectoryId(childDirectoryId);
		}
		
		string GenerateUniqueChildDirectoryIdUsingParentDirectoryId(string childDirectoryId)
		{
			return Id + childDirectoryId;
		}
	}
}
