// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Holds 16 boolean values.
	/// </summary>
	public struct BitVector16 : IEquatable<BitVector16>
	{
		ushort data;
		
		public bool this[ushort mask] {
			get { return (data & mask) != 0; }
			set {
				if (value)
					data |= mask;
				else
					data &= unchecked((ushort)~mask);
			}
		}
		
		#region Equals and GetHashCode implementation
		public override bool Equals(object obj)
		{
			if (obj is BitVector16)
				return Equals((BitVector16)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(BitVector16 other)
		{
			return this.data == other.data;
		}
		
		public override int GetHashCode()
		{
			return data;
		}
		
		public static bool operator ==(BitVector16 left, BitVector16 right)
		{
			return left.Equals(right);
		}
		
		public static bool operator !=(BitVector16 left, BitVector16 right)
		{
			return !left.Equals(right);
		}
		#endregion
	}
}
