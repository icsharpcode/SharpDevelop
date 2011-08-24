// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			int index = this.Extensions.IndexOf("*" + extension, StringComparison.OrdinalIgnoreCase);
			if (index < 0 || index + extension.Length > this.Extensions.Length)
				return false;
			return index + extension.Length < this.Extensions.Length
				|| this.Extensions[index + extension.Length] == ';';
		}
		
		public override string ToString()
		{
			return Name + "|" + Extensions;
		}
	}
}
