// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Represents a directory path or filename.
	/// The equality operator is overloaded to compare for path equality (case insensitive, normalizing paths with '..\')
	/// </summary>
	public sealed class FileName : IEquatable<FileName>
	{
		readonly string normalizedFileName;
		
		public FileName(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			this.normalizedFileName = FileUtility.NormalizePath(fileName);
		}
		
		public static FileName Create(string fileName)
		{
			if (fileName != null)
				return new FileName(fileName);
			else
				return null;
		}
		
		public static implicit operator string(FileName fileName)
		{
			if (fileName != null)
				return fileName.normalizedFileName;
			else
				return null;
		}
		
		public override string ToString()
		{
			return normalizedFileName;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return Equals(obj as FileName);
		}
		
		public bool Equals(FileName other)
		{
			if (other != null)
				return string.Equals(normalizedFileName, other.normalizedFileName, StringComparison.OrdinalIgnoreCase);
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode(normalizedFileName);
		}
		
		public static bool operator ==(FileName left, FileName right)
		{
			if (ReferenceEquals(left, right))
				return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
				return false;
			return left.Equals(right);
		}
		
		public static bool operator !=(FileName left, FileName right)
		{
			return !(left == right);
		}
		
		[ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
		public static bool operator ==(FileName left, string right)
		{
			return (string)left == right;
		}
		
		[ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
		public static bool operator !=(FileName left, string right)
		{
			return (string)left != right;
		}
		
		[ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
		public static bool operator ==(string left, FileName right)
		{
			return left == (string)right;
		}
		
		[ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
		public static bool operator !=(string left, FileName right)
		{
			return left != (string)right;
		}
		#endregion
	}
}
