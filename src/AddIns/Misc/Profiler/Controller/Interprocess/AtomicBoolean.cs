// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Threading;

namespace ICSharpCode.Profiler.Interprocess
{
	/// <summary>
	/// A boolean that starts 'false' and can be atomically set/reset.
	/// </summary>
	public struct AtomicBoolean : IEquatable<AtomicBoolean>
	{
		int val;
		
		/// <summary>
		/// Gets/Sets the value.
		/// </summary>
		public bool Value {
			get {
				return Thread.VolatileRead(ref val) != 0;
			}
			set {
				Thread.VolatileWrite(ref val, value ? 1 : 0);
			}
		}
		
		/// <summary>
		/// Sets the value to true.
		/// </summary>
		/// <returns>True if the value was successfully set from false to true,
		/// false if the value already was true.</returns>
		public bool Set()
		{
			return Interlocked.Exchange(ref val, 1) == 0;
		}
		
		/// <summary>
		/// Sets the value to false.
		/// </summary>
		/// <returns>True if the value was successfully set from true to false,
		/// false if the value already was false.</returns>
		public bool Reset()
		{
			return Interlocked.Exchange(ref val, 0) != 0;
		}
		
		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}
		
		/// <inheritdoc/>
		public override bool Equals(object obj)
		{
			return (obj is AtomicBoolean) && Equals((AtomicBoolean)obj);
		}
		
		/// <summary>
		/// Tests the boolean for equality.
		/// </summary>
		public bool Equals(AtomicBoolean other)
		{
			return this.Value == other.Value;
		}
		
		/// <summary>
		/// Tests the boolean for equality.
		/// </summary>
		public static bool operator ==(AtomicBoolean left, AtomicBoolean right)
		{
			return left.Value == right.Value;
		}
		
		/// <summary>
		/// Tests the boolean for inequality.
		/// </summary>
		public static bool operator !=(AtomicBoolean left, AtomicBoolean right)
		{
			return left.Value != right.Value;
		}
	}
}
