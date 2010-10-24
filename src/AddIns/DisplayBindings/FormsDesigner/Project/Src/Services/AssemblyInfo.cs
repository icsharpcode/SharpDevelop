// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.FormsDesigner.Services
{
	public struct AssemblyInfo : IEquatable<AssemblyInfo>
	{
		public static readonly AssemblyInfo Empty = new AssemblyInfo("", false);
		
		public readonly string FullNameOrPath;
		public readonly bool IsInGac;
		
		public AssemblyInfo(string fullNameOrPath, bool isInGac)
		{
			this.FullNameOrPath = fullNameOrPath;
			this.IsInGac = isInGac;
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			return (obj is AssemblyInfo) && Equals((AssemblyInfo)obj);
		}
		
		public bool Equals(AssemblyInfo other)
		{
			return this.FullNameOrPath == other.FullNameOrPath && this.IsInGac == other.IsInGac;
		}
		
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (FullNameOrPath != null)
					hashCode += 1000000007 * FullNameOrPath.GetHashCode();
				hashCode += 1000000009 * IsInGac.GetHashCode();
			}
			return hashCode;
		}
		
		public static bool operator ==(AssemblyInfo lhs, AssemblyInfo rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(AssemblyInfo lhs, AssemblyInfo rhs)
		{
			return !(lhs == rhs);
		}
		#endregion

	}
}
