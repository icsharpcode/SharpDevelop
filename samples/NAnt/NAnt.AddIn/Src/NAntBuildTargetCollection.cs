// SharpDevelop samples
// Copyright (c) 2007, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;

namespace ICSharpCode.NAnt
{
	/// <summary>
	///     <para>
	///       A collection that stores <see cref='NAntBuildTarget'/> objects.
	///    </para>
	/// </summary>
	/// <seealso cref='NAntBuildTargetCollection'/>
	[Serializable()]
	public class NAntBuildTargetCollection : CollectionBase {
		
		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='NAntBuildTargetCollection'/>.
		///    </para>
		/// </summary>
		public NAntBuildTargetCollection()
		{
		}
		
		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='NAntBuildTargetCollection'/> based on another <see cref='NAntBuildTargetCollection'/>.
		///    </para>
		/// </summary>
		/// <param name='val'>
		///       A <see cref='NAntBuildTargetCollection'/> from which the contents are copied
		/// </param>
		public NAntBuildTargetCollection(NAntBuildTargetCollection val)
		{
			this.AddRange(val);
		}
		
		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='NAntBuildTargetCollection'/> containing any array of <see cref='NAntBuildTarget'/> objects.
		///    </para>
		/// </summary>
		/// <param name='val'>
		///       A array of <see cref='NAntBuildTarget'/> objects with which to intialize the collection
		/// </param>
		public NAntBuildTargetCollection(NAntBuildTarget[] val)
		{
			this.AddRange(val);
		}
		
		/// <summary>
		/// <para>Represents the entry at the specified index of the <see cref='NAntBuildTarget'/>.</para>
		/// </summary>
		/// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
		/// <value>
		///    <para> The entry at the specified index of the collection.</para>
		/// </value>
		/// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
		public NAntBuildTarget this[int index] {
			get {
				return ((NAntBuildTarget)(List[index]));
			}
			set {
				List[index] = value;
			}
		}
		
		/// <summary>
		///    <para>Adds a <see cref='NAntBuildTarget'/> with the specified value to the 
		///    <see cref='NAntBuildTargetCollection'/> .</para>
		/// </summary>
		/// <param name='val'>The <see cref='NAntBuildTarget'/> to add.</param>
		/// <returns>
		///    <para>The index at which the new element was inserted.</para>
		/// </returns>
		/// <seealso cref='NAntBuildTargetCollection.AddRange'/>
		public int Add(NAntBuildTarget val)
		{
			return List.Add(val);
		}
		
		/// <summary>
		/// <para>Copies the elements of an array to the end of the <see cref='NAntBuildTargetCollection'/>.</para>
		/// </summary>
		/// <param name='val'>
		///    An array of type <see cref='NAntBuildTarget'/> containing the objects to add to the collection.
		/// </param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
		/// <seealso cref='NAntBuildTargetCollection.Add'/>
		public void AddRange(NAntBuildTarget[] val)
		{
			for (int i = 0; i < val.Length; i++) {
				this.Add(val[i]);
			}
		}
		
		/// <summary>
		///     <para>
		///       Adds the contents of another <see cref='NAntBuildTargetCollection'/> to the end of the collection.
		///    </para>
		/// </summary>
		/// <param name='val'>
		///    A <see cref='NAntBuildTargetCollection'/> containing the objects to add to the collection.
		/// </param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
		/// <seealso cref='NAntBuildTargetCollection.Add'/>
		public void AddRange(NAntBuildTargetCollection val)
		{
			for (int i = 0; i < val.Count; i++)
			{
				this.Add(val[i]);
			}
		}
		
		/// <summary>
		/// <para>Gets a value indicating whether the 
		///    <see cref='NAntBuildTargetCollection'/> contains the specified <see cref='NAntBuildTarget'/>.</para>
		/// </summary>
		/// <param name='val'>The <see cref='NAntBuildTarget'/> to locate.</param>
		/// <returns>
		/// <para><see langword='true'/> if the <see cref='NAntBuildTarget'/> is contained in the collection; 
		///   otherwise, <see langword='false'/>.</para>
		/// </returns>
		/// <seealso cref='NAntBuildTargetCollection.IndexOf'/>
		public bool Contains(NAntBuildTarget val)
		{
			return List.Contains(val);
		}
		
		/// <summary>
		/// <para>Copies the <see cref='NAntBuildTargetCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
		///    specified index.</para>
		/// </summary>
		/// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='NAntBuildTargetCollection'/> .</para></param>
		/// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
		/// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='NAntBuildTargetCollection'/> is greater than the available space between <paramref name='arrayIndex'/> and the end of <paramref name='array'/>.</para></exception>
		/// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
		/// <exception cref='System.ArgumentOutOfRangeException'><paramref name='arrayIndex'/> is less than <paramref name='array'/>'s lowbound. </exception>
		/// <seealso cref='System.Array'/>
		public void CopyTo(NAntBuildTarget[] array, int index)
		{
			List.CopyTo(array, index);
		}
		
		/// <summary>
		///    <para>Returns the index of a <see cref='NAntBuildTarget'/> in 
		///       the <see cref='NAntBuildTargetCollection'/> .</para>
		/// </summary>
		/// <param name='val'>The <see cref='NAntBuildTarget'/> to locate.</param>
		/// <returns>
		/// <para>The index of the <see cref='NAntBuildTarget'/> of <paramref name='val'/> in the 
		/// <see cref='NAntBuildTargetCollection'/>, if found; otherwise, -1.</para>
		/// </returns>
		/// <seealso cref='NAntBuildTargetCollection.Contains'/>
		public int IndexOf(NAntBuildTarget val)
		{
			return List.IndexOf(val);
		}
		
		/// <summary>
		/// <para>Inserts a <see cref='NAntBuildTarget'/> into the <see cref='NAntBuildTargetCollection'/> at the specified index.</para>
		/// </summary>
		/// <param name='index'>The zero-based index where <paramref name='val'/> should be inserted.</param>
		/// <param name='val'>The <see cref='NAntBuildTarget'/> to insert.</param>
		/// <returns><para>None.</para></returns>
		/// <seealso cref='NAntBuildTargetCollection.Add'/>
		public void Insert(int index, NAntBuildTarget val)
		{
			List.Insert(index, val);
		}
		
		/// <summary>
		///    <para>Returns an enumerator that can iterate through 
		///       the <see cref='NAntBuildTargetCollection'/> .</para>
		/// </summary>
		/// <returns><para>None.</para></returns>
		/// <seealso cref='System.Collections.IEnumerator'/>
		public new NAntBuildTargetEnumerator GetEnumerator()
		{
			return new NAntBuildTargetEnumerator(this);
		}
		
		/// <summary>
		///    <para> Removes a specific <see cref='NAntBuildTarget'/> from the 
		///    <see cref='NAntBuildTargetCollection'/> .</para>
		/// </summary>
		/// <param name='val'>The <see cref='NAntBuildTarget'/> to remove from the <see cref='NAntBuildTargetCollection'/> .</param>
		/// <returns><para>None.</para></returns>
		/// <exception cref='System.ArgumentException'><paramref name='val'/> is not found in the Collection. </exception>
		public void Remove(NAntBuildTarget val)
		{
			List.Remove(val);
		}
		
		public class NAntBuildTargetEnumerator : IEnumerator
		{
			IEnumerator baseEnumerator;
			IEnumerable temp;
			
			public NAntBuildTargetEnumerator(NAntBuildTargetCollection mappings)
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = temp.GetEnumerator();
			}
			
			public NAntBuildTarget Current {
				get {
					return ((NAntBuildTarget)(baseEnumerator.Current));
				}
			}
			
			object IEnumerator.Current {
				get {
					return baseEnumerator.Current;
				}
			}
			
			public bool MoveNext()
			{
				return baseEnumerator.MoveNext();
			}
			
			bool IEnumerator.MoveNext()
			{
				return baseEnumerator.MoveNext();
			}
			
			public void Reset()
			{
				baseEnumerator.Reset();
			}
			
			void IEnumerator.Reset()
			{
				baseEnumerator.Reset();
			}
		}
	}
}
