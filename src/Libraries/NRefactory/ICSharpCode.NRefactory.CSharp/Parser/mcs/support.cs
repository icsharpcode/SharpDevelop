//
// support.cs: Support routines to work around the fact that System.Reflection.Emit
// can not introspect types that are being constructed
//
// Author:
//   Miguel de Icaza (miguel@ximian.com)
//   Marek Safar (marek.safar@gmail.com)
//
// Copyright 2001 Ximian, Inc (http://www.ximian.com)
// Copyright 2003-2009 Novell, Inc
// Copyright 2011 Xamarin Inc
//

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Mono.CSharp {

	sealed class ReferenceEquality<T> : IEqualityComparer<T> where T : class
	{
		public static readonly IEqualityComparer<T> Default = new ReferenceEquality<T> ();

		private ReferenceEquality ()
		{
		}

		public bool Equals (T x, T y)
		{
			return ReferenceEquals (x, y);
		}

		public int GetHashCode (T obj)
		{
			return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode (obj);
		}
	}
#if !NET_4_0 && !MONODROID
	public class Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
	{
		public Tuple (T1 item1, T2 item2)
		{
			Item1 = item1;
			Item2 = item2;
		}

		public T1 Item1 { get; private set; }
		public T2 Item2 { get; private set; }

		public override int GetHashCode ()
		{
			return ((object)Item1 ?? 0) .GetHashCode () ^ ((object)Item2 ?? 0).GetHashCode ();
		}

		#region IEquatable<Tuple<T1,T2>> Members

		public bool Equals (Tuple<T1, T2> other)
		{
			return EqualityComparer<T1>.Default.Equals (Item1, other.Item1) &&
				EqualityComparer<T2>.Default.Equals (Item2, other.Item2);
		}

		#endregion
	}

	public class Tuple<T1, T2, T3> : IEquatable<Tuple<T1, T2, T3>>
	{
		public Tuple (T1 item1, T2 item2, T3 item3)
		{
			Item1 = item1;
			Item2 = item2;
			Item3 = item3;
		}

		public T1 Item1 { get; private set; }
		public T2 Item2 { get; private set; }
		public T3 Item3 { get; private set; }

		public override int GetHashCode ()
		{
			return Item1.GetHashCode () ^ Item2.GetHashCode () ^ Item3.GetHashCode ();
		}

		#region IEquatable<Tuple<T1,T2>> Members

		public bool Equals (Tuple<T1, T2, T3> other)
		{
			return EqualityComparer<T1>.Default.Equals (Item1, other.Item1) &&
				EqualityComparer<T2>.Default.Equals (Item2, other.Item2) &&
				EqualityComparer<T3>.Default.Equals (Item3, other.Item3);
		}

		#endregion
	}

	static class Tuple
	{
		public static Tuple<T1, T2> Create<T1, T2> (T1 item1, T2 item2)
		{
			return new Tuple<T1, T2> (item1, item2);
		}

		public static Tuple<T1, T2, T3> Create<T1, T2, T3> (T1 item1, T2 item2, T3 item3)
		{
			return new Tuple<T1, T2, T3> (item1, item2, item3);
		}
	}
#endif

	static class ArrayComparer
	{
		public static bool IsEqual<T> (T[] array1, T[] array2)
		{
			if (array1 == null || array2 == null)
				return array1 == array2;

			var eq = EqualityComparer<T>.Default;

			for (int i = 0; i < array1.Length; ++i) {
				if (!eq.Equals (array1[i], array2[i])) {
					return false;
				}
			}

			return true;
		}
	}
	public class UnixUtils {
		[System.Runtime.InteropServices.DllImport ("libc", EntryPoint="isatty")]
		extern static int _isatty (int fd);
			
		public static bool isatty (int fd)
		{
			try {
				return _isatty (fd) == 1;
			} catch {
				return false;
			}
		}
	}

	/// <summary>
	///   An exception used to terminate the compiler resolution phase and provide completions
	/// </summary>
	/// <remarks>
	///   This is thrown when we want to return the completions or
	///   terminate the completion process by AST nodes used in
	///   the completion process.
	/// </remarks>
	public class CompletionResult : Exception {
		string [] result;
		string base_text;
		
		public CompletionResult (string base_text, string [] res)
		{
			if (base_text == null)
				throw new ArgumentNullException ("base_text");
			this.base_text = base_text;

			result = res;
			Array.Sort (result);
		}

		public string [] Result {
			get {
				return result;
			}
		}

		public string BaseText {
			get {
				return base_text;
			}
		}
	}
}
