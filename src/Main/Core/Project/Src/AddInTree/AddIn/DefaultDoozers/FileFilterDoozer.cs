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

namespace ICSharpCode.Core
{
	/// <summary>
	/// Creates file filter entries for OpenFileDialogs or SaveFileDialogs.
	/// </summary>
	/// <attribute name="name" use="required">
	/// The name of the file filter entry.
	/// </attribute>
	/// <attribute name="extensions" use="required">
	/// The extensions associated with this file filter entry.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/FileFilter</usage>
	/// <returns>
	/// <see cref="FileFilterDescriptor"/> in the format "name|extensions".
	/// </returns>
	public class FileFilterDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			Codon codon = args.Codon;
			return new FileFilterDescriptor {
				Name = StringParser.Parse(codon.Properties["name"]),
				Extensions = codon.Properties["extensions"],
				MimeType = codon.Properties["mimeType"]
			};
		}
	}
	
	public sealed class FileFilterDescriptor
	{
		public string Name { get; set; }
		public string Extensions { get; set; }
		public string MimeType { get; set; }
		
		/// <summary>
		/// Gets whether this descriptor matches the specified file extension.
		/// </summary>
		/// <param name="extension">File extension starting with '.'</param>
		public bool ContainsExtension(string extension)
		{
			if (string.IsNullOrEmpty(extension))
				return false;
			int index = Extensions.IndexOf("*" + extension, StringComparison.OrdinalIgnoreCase);
			int matchLength = index + extension.Length + 1;
			if (index < 0 || matchLength > Extensions.Length)
				return false;
			return matchLength == Extensions.Length || Extensions[matchLength] == ';';
		}
		
		public override string ToString()
		{
			return Name + "|" + Extensions;
		}
	}
}
