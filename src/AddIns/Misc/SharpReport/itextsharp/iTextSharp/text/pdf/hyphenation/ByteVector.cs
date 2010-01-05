using System;
using System.Collections;

/*
 * $Id: ByteVector.cs,v 1.2 2005/06/18 08:17:05 psoares33 Exp $
 * Copyright (C) 2001 The Apache Software Foundation. All rights reserved.
 * For details on use and redistribution please refer to the
 * LICENSE file included with these sources.
 */

namespace iTextSharp.text.pdf.hyphenation {
	/**
	 * This class implements a simple byte vector with access to the
	 * underlying array.
	 *
	 * @author Carlos Villegas <cav@uniscope.co.jp>
	 */
	public class ByteVector {

		/**
		 * Capacity increment size
		 */
		private static int DEFAULT_BLOCK_SIZE = 2048;
		private int BLOCK_SIZE;

		/**
		 * The encapsulated array
		 */
		private byte[] arr;

		/**
		 * Points to next free item
		 */
		private int n;

		public ByteVector() : this(DEFAULT_BLOCK_SIZE) {}

		public ByteVector(int capacity) {
			if (capacity > 0)
				BLOCK_SIZE = capacity;
			else
				BLOCK_SIZE = DEFAULT_BLOCK_SIZE;
			arr = new byte[BLOCK_SIZE];
			n = 0;
		}

		public ByteVector(byte[] a) {
			BLOCK_SIZE = DEFAULT_BLOCK_SIZE;
			arr = a;
			n = 0;
		}

		public ByteVector(byte[] a, int capacity) {
			if (capacity > 0)
				BLOCK_SIZE = capacity;
			else
				BLOCK_SIZE = DEFAULT_BLOCK_SIZE;
			arr = a;
			n = 0;
		}

		public byte[] Arr {
			get {
				return arr;
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
				return arr.Length;
			}
		}

		public byte this[int index] {
			get {
				return arr[index];
			}

			set {
				arr[index] = value;
			}
		}

		/**
		 * This is to implement memory allocation in the array. Like Malloc().
		 */
		public int Alloc(int size) {
			int index = n;
			int len = arr.Length;
			if (n + size >= len) {
				byte[] aux = new byte[len + BLOCK_SIZE];
				Array.Copy(arr, 0, aux, 0, len);
				arr = aux;
			}
			n += size;
			return index;
		}

		public void TrimToSize() {
			if (n < arr.Length) {
				byte[] aux = new byte[n];
				Array.Copy(arr, 0, aux, 0, n);
				arr = aux;
			}
		}
	}
}
