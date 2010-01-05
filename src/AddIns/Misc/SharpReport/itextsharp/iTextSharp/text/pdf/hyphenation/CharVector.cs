using System;

/*
 * $Id: CharVector.cs,v 1.2 2005/06/18 08:17:05 psoares33 Exp $
 * Copyright (C) 2001 The Apache Software Foundation. All rights reserved.
 * For details on use and redistribution please refer to the
 * LICENSE file included with these sources.
 */

namespace iTextSharp.text.pdf.hyphenation {
	/**
	 * This class implements a simple char vector with access to the
	 * underlying array.
	 *
	 * @author Carlos Villegas <cav@uniscope.co.jp>
	 */
	public class CharVector : ICloneable {

		/**
		 * Capacity increment size
		 */
		private static int DEFAULT_BLOCK_SIZE = 2048;
		private int BLOCK_SIZE;

		/**
		 * The encapsulated array
		 */
		private char[] array;

		/**
		 * Points to next free item
		 */
		private int n;

		public CharVector() : this(DEFAULT_BLOCK_SIZE) {}

		public CharVector(int capacity) {
			if (capacity > 0)
				BLOCK_SIZE = capacity;
			else
				BLOCK_SIZE = DEFAULT_BLOCK_SIZE;
			array = new char[BLOCK_SIZE];
			n = 0;
		}

		public CharVector(char[] a) {
			BLOCK_SIZE = DEFAULT_BLOCK_SIZE;
			array = a;
			n = a.Length;
		}

		public CharVector(char[] a, int capacity) {
			if (capacity > 0)
				BLOCK_SIZE = capacity;
			else
				BLOCK_SIZE = DEFAULT_BLOCK_SIZE;
			array = a;
			n = a.Length;
		}

		/**
		 * Reset Vector but don't resize or clear elements
		 */
		public void Clear() {
			n = 0;
		}

		public Object Clone() {
			CharVector cv = new CharVector((char[])array.Clone(), BLOCK_SIZE);
			cv.n = this.n;
			return cv;
		}

		public char[] Arr {
			get {
				return array;
			}
		}

		/**
		 * return number of items in array
		 */
		public int Length {
			get {
				return n;
			}
		}

		/**
		 * returns current capacity of array
		 */
		public int Capacity {
			get {
				return array.Length;
			}
		}

		public char this[int index] {
			get {
				return array[index];
			}

			set {
				array[index] = value;
			}
		}

		public int Alloc(int size) {
			int index = n;
			int len = array.Length;
			if (n + size >= len) {
				char[] aux = new char[len + BLOCK_SIZE];
				Array.Copy(array, 0, aux, 0, len);
				array = aux;
			}
			n += size;
			return index;
		}

		public void TrimToSize() {
			if (n < array.Length) {
				char[] aux = new char[n];
				Array.Copy(array, 0, aux, 0, n);
				array = aux;
			}
		}
	}
}
