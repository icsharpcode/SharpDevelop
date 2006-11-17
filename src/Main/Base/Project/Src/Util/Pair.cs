// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// A pair of two values that implements Equals and GetHashCode using Equals and GetHashCode
	/// of both elements. It also overrides operator == and operator !=.
	/// Pair member values may not be null!
	/// </summary>
	public struct Pair<A, B> : IEquatable<Pair<A, B>> where A : IEquatable<A> where B : IEquatable<B>
	{
		public readonly A First;
		public readonly B Second;
		
		public Pair(A first, B second)
		{
			if (first == null)
				throw new ArgumentNullException("first");
			if (second == null)
				throw new ArgumentNullException("second");
			this.First = first;
			this.Second = second;
		}
		
		public override bool Equals(object obj)
		{
			if (obj is Pair<A, B>)
				return Equals((Pair<A, B>)obj); // use Equals method below
			else
				return false;
		}
		
		public bool Equals(Pair<A, B> other)
		{
			// add comparisions for all members here
			return First.Equals(other.First) && Second.Equals(other.Second);
		}
		
		public override int GetHashCode()
		{
			// combine the hash codes of all members here (e.g. with XOR operator ^)
			return unchecked ( 27 * First.GetHashCode() + Second.GetHashCode() );
		}
		
		public static bool operator ==(Pair<A, B> lhs, Pair<A, B> rhs)
		{
			return lhs.Equals(rhs);
		}
		
		public static bool operator !=(Pair<A, B> lhs, Pair<A, B> rhs)
		{
			return !(lhs.Equals(rhs)); // use operator == and negate result
		}
	}
}
