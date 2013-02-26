// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Represents a directory path or Directoryname.
	/// The equality operator is overloaded to compare for path equality (case insensitive, normalizing paths with '..\')
	/// </summary>
	[TypeConverter(typeof(DirectoryNameConverter))]
	public sealed class DirectoryName : IEquatable<DirectoryName>
	{
		readonly string normalizedDirectoryName;
		
		public DirectoryName(string directoryName)
		{
			if (directoryName == null)
				throw new ArgumentNullException("DirectoryName");
			if (directoryName.Length == 0)
				throw new ArgumentException("The empty string is not a valid DirectoryName");
			this.normalizedDirectoryName = FileUtility.NormalizePath(directoryName);
		}
		
		[Obsolete("The input already is a DirectoryName")]
		public DirectoryName(DirectoryName directoryName)
		{
			this.normalizedDirectoryName = directoryName.normalizedDirectoryName;
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
		
		public DirectoryName GetParentDirectory()
		{
			return DirectoryName.Create(Path.GetDirectoryName(normalizedDirectoryName));
		}
		
		public static implicit operator string(DirectoryName directoryName)
		{
			if (directoryName != null)
				return directoryName.normalizedDirectoryName;
			else
				return null;
		}
		
		public string ToStringWithTrailingBackslash()
		{
			if (normalizedDirectoryName.EndsWith("\\", StringComparison.Ordinal))
				return normalizedDirectoryName; // trailing backslash exists in normalized version for root of drives ("C:\")
			else
				return normalizedDirectoryName + "\\";
		}
		
		public override string ToString()
		{
			return normalizedDirectoryName;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return Equals(obj as DirectoryName);
		}
		
		public bool Equals(DirectoryName other)
		{
			if (other != null)
				return string.Equals(normalizedDirectoryName, other.normalizedDirectoryName, StringComparison.OrdinalIgnoreCase);
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode(normalizedDirectoryName);
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
