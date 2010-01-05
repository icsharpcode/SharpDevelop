using System;
using System.Collections;

using Org.BouncyCastle.Utilities.Collections;

namespace Org.BouncyCastle.Asn1
{
    abstract public class Asn1Set
        : Asn1Object, IEnumerable
    {
        private readonly ArrayList _set;

		/**
         * return an ASN1Set from the given object.
         *
         * @param obj the object we want converted.
         * @exception ArgumentException if the object cannot be converted.
         */
        public static Asn1Set GetInstance(
            object obj)
        {
            if (obj == null || obj is Asn1Set)
            {
                return (Asn1Set)obj;
            }

			throw new ArgumentException("Unknown object in GetInstance: " + obj.GetType().FullName, "obj");
        }

        /**
         * Return an ASN1 set from a tagged object. There is a special
         * case here, if an object appears to have been explicitly tagged on
         * reading but we were expecting it to be implicitly tagged in the
         * normal course of events it indicates that we lost the surrounding
         * set - so we need to add it back (this will happen if the tagged
         * object is a sequence that contains other sequences). If you are
         * dealing with implicitly tagged sets you really <b>should</b>
         * be using this method.
         *
         * @param obj the tagged object.
         * @param explicitly true if the object is meant to be explicitly tagged
         *          false otherwise.
         * @exception ArgumentException if the tagged object cannot
         *          be converted.
         */
        public static Asn1Set GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
			Asn1Object inner = obj.GetObject();

			if (explicitly)
            {
                if (!obj.IsExplicit())
                    throw new ArgumentException("object implicit - explicit expected.");

				return (Asn1Set) inner;
            }

			//
            // constructed object which appears to be explicitly tagged
            // and it's really implicit means we have to add the
            // surrounding sequence.
            //
            if (obj.IsExplicit())
            {
                return new DerSet(inner);
            }

			if (inner is Asn1Set)
            {
                return (Asn1Set) inner;
            }

            //
            // in this case the parser returns a sequence, convert it
            // into a set.
            //
			if (inner is Asn1Sequence)
            {
				Asn1EncodableVector v = new Asn1EncodableVector();
				Asn1Sequence s = (Asn1Sequence) inner;

				foreach (Asn1Encodable ae in s)
				{
                    v.Add(ae);
                }

				// TODO Should be able to construct set directly from sequence?
				return new DerSet(v, false);
            }

			throw new ArgumentException("Unknown object in GetInstance: " + obj.GetType().FullName, "obj");
		}

		protected internal Asn1Set(
			int capacity)
        {
			_set = new ArrayList(capacity);
        }

		public virtual IEnumerator GetEnumerator()
		{
			return _set.GetEnumerator();
		}

		[Obsolete("Use GetEnumerator() instead")]
        public IEnumerator GetObjects()
        {
            return GetEnumerator();
        }

		/**
         * return the object at the set position indicated by index.
         *
         * @param index the set number (starting at zero) of the object
         * @return the object at the set position indicated by index.
         */
		public virtual Asn1Encodable this[int index]
		{
			get { return (Asn1Encodable) _set[index]; }
		}

		[Obsolete("Use 'object[index]' syntax instead")]
		public Asn1Encodable GetObjectAt(
            int index)
        {
             return this[index];
        }

		[Obsolete("Use 'Count' property instead")]
		public int Size
        {
			get { return Count; }
        }

		public virtual int Count
		{
			get { return _set.Count; }
		}

		private class Asn1SetParserImpl
			: Asn1SetParser
		{
			private readonly Asn1Set outer;
			private readonly int max;
			private int index;

			public Asn1SetParserImpl(
				Asn1Set outer)
			{
				this.outer = outer;
				this.max = outer.Count;
			}

			public IAsn1Convertible ReadObject()
			{
				if (index == max)
					return null;

				Asn1Encodable obj = outer[index++];
				if (obj is Asn1Sequence)
					return ((Asn1Sequence)obj).Parser;

				if (obj is Asn1Set)
					return ((Asn1Set)obj).Parser;

				// NB: Asn1OctetString implements Asn1OctetStringParser directly
//				if (obj is Asn1OctetString)
//					return ((Asn1OctetString)obj).Parser;

				return obj;
			}

			public virtual Asn1Object ToAsn1Object()
			{
				return outer;
			}
		}

		public Asn1SetParser Parser
		{
			get { return new Asn1SetParserImpl(this); }
		}

		protected override int Asn1GetHashCode()
		{
            int hc = Count;

			foreach (object o in this)
			{
				hc *= 17;
				if (o != null)
				{
					hc ^= o.GetHashCode();
				}
            }

			return hc;
        }

		protected override bool Asn1Equals(
			Asn1Object asn1Object)
        {
			Asn1Set other = asn1Object as Asn1Set;

			if (other == null)
				return false;

			if (Count != other.Count)
            {
                return false;
            }

			IEnumerator s1 = GetEnumerator();
            IEnumerator s2 = other.GetEnumerator();

			while (s1.MoveNext() && s2.MoveNext())
			{
//				if (!Platform.Equals(s1.Current, s2.Current))
				Asn1Object o1 = ((Asn1Encodable) s1.Current).ToAsn1Object();

				if (!o1.Equals(s2.Current))
				{
					return false;
				}
			}

			return true;
        }

		/**
         * return true if a &lt;= b (arrays are assumed padded with zeros).
         */
        private bool LessThanOrEqual(
             byte[] a,
             byte[] b)
        {
			int cmpLen = System.Math.Min(a.Length, b.Length);

			for (int i = 0; i < cmpLen; ++i)
			{
				byte l = a[i];
				byte r = b[i];

				if (l != r)
				{
					return r > l ? true : false;
				}
			}

			return a.Length <= b.Length;
        }

		protected internal void Sort()
        {
			if (_set.Count > 1)
			{
				bool swapped = true;
				int lastSwap = _set.Count - 1;

				while (swapped)
				{
					int index = 0;
					int swapIndex = 0;
					byte[] a = ((Asn1Encodable) _set[0]).GetEncoded();

					swapped = false;

					while (index != lastSwap)
					{
						byte[] b = ((Asn1Encodable) _set[index + 1]).GetEncoded();

						if (LessThanOrEqual(a, b))
						{
							a = b;
						}
						else
						{
							object o = _set[index];
							_set[index] = _set[index + 1];
							_set[index + 1] = o;

							swapped = true;
							swapIndex = index;
						}

						index++;
					}

					lastSwap = swapIndex;
				}
			}
        }

		protected internal void AddObject(
			Asn1Encodable obj)
        {
            _set.Add(obj);
        }

		public override string ToString()
		{
			return CollectionUtilities.ToString(_set);
		}
	}
}
