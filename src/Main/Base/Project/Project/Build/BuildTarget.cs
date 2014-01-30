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

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Struct for strongly-typed passing of build targets
	/// - we don't want to use strings everywhere.
	/// Basically this is something like a typedef for C# (without implicit conversions).
	/// </summary>
	public struct BuildTarget : IEquatable<BuildTarget>, IComparable<BuildTarget>
	{
		// Known MSBuild targets:
		public readonly static BuildTarget Build = new BuildTarget("Build");
		public readonly static BuildTarget Rebuild = new BuildTarget("Rebuild");
		public readonly static BuildTarget Clean = new BuildTarget("Clean");
		
		public readonly static BuildTarget ResolveReferences = new BuildTarget("ResolveReferences");
		public readonly static BuildTarget ResolveComReferences = new BuildTarget("ResolveComReferences");
		
		readonly string targetName;
		
		public string TargetName {
			get { return targetName; }
		}
		
		public BuildTarget(string targetName)
		{
			if (targetName == null)
				throw new ArgumentNullException("targetName");
			this.targetName = targetName;
		}
		
		public override string ToString()
		{
			return targetName;
		}
		
		#region Equals and GetHashCode implementation
		// The code in this region is useful if you want to use this structure in collections.
		// If you don't need it, you can just remove the region and the ": IEquatable<BuildTarget>" declaration.
		
		public override bool Equals(object obj)
		{
			if (obj is BuildTarget)
				return Equals((BuildTarget)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(BuildTarget other)
		{
			// add comparisions for all members here
			return this.targetName == other.targetName;
		}
		
		public override int GetHashCode()
		{
			// combine the hash codes of all members here (e.g. with XOR operator ^)
			return targetName.GetHashCode();
		}
		
		public static bool operator ==(BuildTarget lhs, BuildTarget rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(BuildTarget lhs, BuildTarget rhs)
		{
			return !(lhs.Equals(rhs)); // use operator == and negate result
		}
		#endregion
		
		public int CompareTo(BuildTarget other)
		{
			return targetName.CompareTo(other.targetName);
		}
	}
}
