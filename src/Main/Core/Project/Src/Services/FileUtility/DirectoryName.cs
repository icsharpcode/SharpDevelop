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
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Represents a path to a directory.
	/// The equality operator is overloaded to compare for path equality (case insensitive, normalizing paths with '..\')
	/// </summary>
	[TypeConverter(typeof(DirectoryNameConverter))]
	public sealed class DirectoryName : PathName
	{
		public DirectoryName(string path)
			: base(path)
		{
		}
		
		[Obsolete("The input already is a DirectoryName")]
		public DirectoryName(DirectoryName path)
			: base(path)
		{
		}
		
		/// <summary>
		/// Creates a DirectoryName instance from the string.
		/// It is valid to pass null or an empty string to this method (in that case, a null reference will be returned).
		/// </summary>
		public static DirectoryName Create(string DirectoryName)
		{
			if (string.IsNullOrEmpty(DirectoryName))
				return null;
			else
				return new DirectoryName(DirectoryName);
		}
		
		[Obsolete("The input already is a DirectoryName")]
		public static DirectoryName Create(DirectoryName directoryName)
		{
			return directoryName;
		}
		
		/// <summary>
		/// Combines this directory name with a relative path.
		/// </summary>
		public DirectoryName Combine(DirectoryName relativePath)
		{
			if (relativePath == null)
				return null;
			return DirectoryName.Create(Path.Combine(normalizedPath, relativePath));
		}
		
		/// <summary>
		/// Combines this directory name with a relative path.
		/// </summary>
		public FileName Combine(FileName relativePath)
		{
			if (relativePath == null)
				return null;
			return FileName.Create(Path.Combine(normalizedPath, relativePath));
		}
		
		/// <summary>
		/// Combines this directory name with a relative path.
		/// </summary>
		public FileName CombineFile(string relativeFileName)
		{
			if (relativeFileName == null)
				return null;
			return FileName.Create(Path.Combine(normalizedPath, relativeFileName));
		}
		
		/// <summary>
		/// Combines this directory name with a relative path.
		/// </summary>
		public DirectoryName CombineDirectory(string relativeDirectoryName)
		{
			if (relativeDirectoryName == null)
				return null;
			return DirectoryName.Create(Path.Combine(normalizedPath, relativeDirectoryName));
		}
		
		/// <summary>
		/// Converts the specified absolute path into a relative path (relative to <c>this</c>).
		/// </summary>
		public DirectoryName GetRelativePath(DirectoryName path)
		{
			if (path == null)
				return null;
			return DirectoryName.Create(FileUtility.GetRelativePath(normalizedPath, path));
		}
		
		/// <summary>
		/// Converts the specified absolute path into a relative path (relative to <c>this</c>).
		/// </summary>
		public FileName GetRelativePath(FileName path)
		{
			if (path == null)
				return null;
			return FileName.Create(FileUtility.GetRelativePath(normalizedPath, path));
		}
		
		/// <summary>
		/// Gets the directory name as a string, including a trailing backslash.
		/// </summary>
		public string ToStringWithTrailingBackslash()
		{
			if (normalizedPath.EndsWith("\\", StringComparison.Ordinal))
				return normalizedPath; // trailing backslash exists in normalized version for root of drives ("C:\")
			else
				return normalizedPath + "\\";
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return Equals(obj as DirectoryName);
		}
		
		public bool Equals(DirectoryName other)
		{
			if (other != null)
				return string.Equals(normalizedPath, other.normalizedPath, StringComparison.OrdinalIgnoreCase);
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode(normalizedPath);
		}
		
		public static bool operator ==(DirectoryName left, DirectoryName right)
		{
			if (ReferenceEquals(left, right))
				return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
				return false;
			return left.Equals(right);
		}
		
		public static bool operator !=(DirectoryName left, DirectoryName right)
		{
			return !(left == right);
		}
		
		[ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
		public static bool operator ==(DirectoryName left, string right)
		{
			return (string)left == right;
		}
		
		[ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
		public static bool operator !=(DirectoryName left, string right)
		{
			return (string)left != right;
		}
		
		[ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
		public static bool operator ==(string left, DirectoryName right)
		{
			return left == (string)right;
		}
		
		[ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
		public static bool operator !=(string left, DirectoryName right)
		{
			return left != (string)right;
		}
		#endregion
	}
	
	public class DirectoryNameConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(DirectoryName) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string) {
				return DirectoryName.Create((string)value);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture,
		                                 object value, Type destinationType)
		{
			if (destinationType == typeof(string)) {
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
