// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// A boolean that starts 'false' and can be atomically set/reset.
	/// </summary>
	public struct AtomicBoolean
	{
		int val;
		
		/// <summary>
		/// Gets/Sets the value.
		/// </summary>
		public bool Value {
			get {
				return Volatile.Read(ref val) != 0;
			}
			set {
				Volatile.Write(ref val, value ? 1 : 0);
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
			return (obj is AtomicBoolean) && this.Value == ((AtomicBoolean)obj).Value;
		}
	}
}
