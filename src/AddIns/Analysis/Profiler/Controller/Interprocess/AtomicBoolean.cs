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
			return Value.GetHashCode();
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
			return Value == other.Value;
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
