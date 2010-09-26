/*
* Copyright (C) 2008-2009 Johannes E. Schindelin <johannes.schindelin@gmx.de>
*
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or
* without modification, are permitted provided that the following
* conditions are met:
*
* - Redistributions of source code must retain the above copyright
* notice, this list of conditions and the following disclaimer.
*
* - Redistributions in binary form must reproduce the above
* copyright notice, this list of conditions and the following
* disclaimer in the documentation and/or other materials provided
* with the distribution.
*
* - Neither the name of the Eclipse Foundation, Inc. nor the
* names of its contributors may be used to endorse or promote
* products derived from this software without specific prior
* written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
* CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
* INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
* OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
* ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
* CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
* NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
* CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
* STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
* ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
* ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Diff algorithm, based on "An O(ND) Difference Algorithm and its
	/// Variations", by Eugene Myers.
	///
	/// The basic idea is to put the line numbers of text A as columns ("x") and the
	/// lines of text B as rows ("y"). Now you try to find the shortest "edit path"
	/// from the upper left corner to the lower right corner, where you can
	/// always go horizontally or vertically, but diagonally from (x,y) to
	/// (x+1,y+1) only if line x in text A is identical to line y in text B.
	///
	/// Myers' fundamental concept is the "furthest reaching D-path on diagonal k":
	/// a D-path is an edit path starting at the upper left corner and containing
	/// exactly D non-diagonal elements ("differences"). The furthest reaching
	/// D-path on diagonal k is the one that contains the most (diagonal) elements
	/// which ends on diagonal k (where k = y - x).
	///
	/// Example:
	///
	/// H E L L O W O R L D
	/// ____
	/// L \___
	/// O \___
	/// W \________
	///
	/// Since every D-path has exactly D horizontal or vertical elements, it can
	/// only end on the diagonals -D, -D+2, ..., D-2, D.
	///
	/// Since every furthest reaching D-path contains at least one furthest
	/// reaching (D-1)-path (except for D=0), we can construct them recursively.
	///
	/// Since we are really interested in the shortest edit path, we can start
	/// looking for a 0-path, then a 1-path, and so on, until we find a path that
	/// ends in the lower right corner.
	///
	/// To save space, we do not need to store all paths (which has quadratic space
	/// requirements), but generate the D-paths simultaneously from both sides.
	/// When the ends meet, we will have found "the middle" of the path. From the
	/// end points of that diagonal part, we can generate the rest recursively.
	///
	/// This only requires linear space.
	///
	/// The overall (runtime) complexity is
	///
	/// O(N * D^2 + 2 * N/2 * (D/2)^2 + 4 * N/4 * (D/4)^2 + ...)
	/// = O(N * D^2 * 5 / 4) = O(N * D^2),
	///
	/// (With each step, we have to find the middle parts of twice as many regions
	/// as before, but the regions (as well as the D) are halved.)
	///
	/// So the overall runtime complexity stays the same with linear space,
	/// albeit with a larger constant factor.
	/// </summary>
	public class MyersDiff
	{
		/// <summary>
		/// The list of edits found during the last call to <see cref="calculateEdits()"/>
		/// </summary>
		protected List<Edit> edits;
		
		/// <summary>
		/// The first text to be compared. Referred to as "Text A" in the comments
		/// </summary>
		protected ISequence a;
		
		/// <summary>
		/// The second text to be compared. Referred to as "Text B" in the comments
		/// </summary>
		protected ISequence b;
		
		/// <summary>
		/// The only constructor
		/// </summary>
		/// <param name="a">the text A which should be compared</param>
		/// <param name="b">the text B which should be compared</param>
		public MyersDiff(ISequence a, ISequence b)
		{
			this.a = a;
			this.b = b;
			middle = new MiddleEdit(a, b);
			CalculateEdits();
		}
		
		/// <returns>the list of edits found during the last call to {@link #calculateEdits()}</returns>
		public List<Edit> GetEdits()
		{
			return edits;
		}
		
		// TODO: use ThreadLocal for future multi-threaded operations
		MiddleEdit middle;
		
		/// <summary>
		/// Entrypoint into the algorithm this class is all about. This method triggers that the
		/// differences between A and B are calculated in form of a list of edits.
		/// </summary>
		protected void CalculateEdits()
		{
			edits = new List<Edit>();
			
			middle.Initialize(0, a.Size(), 0, b.Size());
			if (middle.beginA >= middle.endA &&
			    middle.beginB >= middle.endB)
				return;
			
			CalculateEdits(middle.beginA, middle.endA,
			               middle.beginB, middle.endB);
		}
		
		/// <summary>
		/// Calculates the differences between a given part of A against another given part of B
		/// </summary>
		/// <param name="beginA">start of the part of A which should be compared (0&lt;=beginA&lt;sizeof(A))</param>
		/// <param name="endA">end of the part of A which should be compared (beginA&lt;=endA&lt;sizeof(A))</param>
		/// <param name="beginB">start of the part of B which should be compared (0&lt;=beginB&lt;sizeof(B))</param>
		/// <param name="endB">end of the part of B which should be compared (beginB&lt;=endB&lt;sizeof(B))</param>
		protected void CalculateEdits(int beginA, int endA,
		                              int beginB, int endB)
		{
			Edit edit = middle.Calculate(beginA, endA, beginB, endB);
			
			if (beginA < edit.BeginA || beginB < edit.BeginB)
			{
				int k = edit.BeginB - edit.BeginA;
				int x = middle.backward.Snake(k, edit.BeginA);
				CalculateEdits(beginA, x, beginB, k + x);
			}
			
			if (edit.EditType != ChangeType.None)
				edits.Add(edit);
			
			
			// after middle
			if (endA > edit.EndA || endB > edit.EndB)
			{
				int k = edit.EndB - edit.EndA;
				int x = middle.forward.Snake(k, edit.EndA);
				CalculateEdits(x, endA, k + x, endB);
			}
		}
		
		/// <summary>
		/// A class to help bisecting the sequences a and b to find minimal
		/// edit paths.
		///
		/// As the arrays are reused for space efficiency, you will need one
		/// instance per thread.
		///
		/// The entry function is the calculate() method.
		/// </summary>
		class MiddleEdit
		{
			private readonly ISequence _a;
			private readonly ISequence _b;
			
			public MiddleEdit(ISequence a, ISequence b)
			{
				_a = a;
				_b = b;
				forward = new ForwardEditPaths(this);
				backward = new BackwardEditPaths(this);
			}
			
			public void Initialize(int beginA, int endA, int beginB, int endB)
			{
				this.beginA = beginA; this.endA = endA;
				this.beginB = beginB; this.endB = endB;
				
				// strip common parts on either end
				int k = beginB - beginA;
				this.beginA = forward.Snake(k, beginA);
				this.beginB = k + this.beginA;
				
				k = endB - endA;
				this.endA = backward.Snake(k, endA);
				this.endB = k + this.endA;
			}
			
			/// <summary>
			/// This function calculates the "middle" Edit of the shortest
			/// edit path between the given subsequences of a and b.
			///
			/// Once a forward path and a backward path meet, we found the
			/// middle part. From the last snake end point on both of them,
			/// we construct the Edit.
			///
			/// It is assumed that there is at least one edit in the range.
			/// </summary>
			// TODO: measure speed impact when this is synchronized
			public Edit Calculate(int beginA, int endA, int beginB, int endB)
			{
				if (beginA == endA || beginB == endB)
					return new Edit(beginA, endA, beginB, endB);
				this.beginA = beginA; this.endA = endA;
				this.beginB = beginB; this.endB = endB;
				
				/*
				 * Following the conventions in Myers' paper, "k" is
				 * the difference between the index into "b" and the
				 * index into "a".
				 */
				int minK = beginB - endA;
				int maxK = endB - beginA;
				
				forward.Initialize(beginB - beginA, beginA, minK, maxK);
				backward.Initialize(endB - endA, endA, minK, maxK);
				
				for (int d = 1; ; d++)
					if (forward.Calculate(d) ||
					    backward.Calculate(d))
				{
					return _edit;
				}
			}
			
			/*
			 * For each d, we need to hold the d-paths for the diagonals
			 * k = -d, -d + 2, ..., d - 2, d. These are stored in the
			 * forward (and backward) array.
			 *
			 * As we allow subsequences, too, this needs some refinement:
			 * the forward paths start on the diagonal forwardK =
			 * beginB - beginA, and backward paths start on the diagonal
			 * backwardK = endB - endA.
			 *
			 * So, we need to hold the forward d-paths for the diagonals
			 * k = forwardK - d, forwardK - d + 2, ..., forwardK + d and
			 * the analogue for the backward d-paths. This means that
			 * we can turn (k, d) into the forward array index using this
			 * formula:
			 *
			 * i = (d + k - forwardK) / 2
			 *
			 * There is a further complication: the edit paths should not
			 * leave the specified subsequences, so k is bounded by
			 * minK = beginB - endA and maxK = endB - beginA. However,
			 * (k - forwardK) _must_ be odd whenever d is odd, and it
			 * _must_ be even when d is even.
			 *
			 * The values in the "forward" and "backward" arrays are
			 * positions ("x") in the sequence a, to get the corresponding
			 * positions ("y") in the sequence b, you have to calculate
			 * the appropriate k and then y:
			 *
			 * k = forwardK - d + i * 2
			 * y = k + x
			 *
			 * (substitute backwardK for forwardK if you want to get the
			 * y position for an entry in the "backward" array.
			 */
			public EditPaths forward;
			public EditPaths backward;
			
			/* Some variables which are shared between methods */
			public int beginA;
			public int endA;
			public int beginB;
			public int endB;
			protected Edit _edit;
			
			internal abstract class EditPaths
			{
				protected readonly MiddleEdit _middleEdit;
				List<int> x = new List<int>();
				List<long> _snake = new List<long>();
				public int beginK;
				public int endK;
				public int middleK;
				int prevBeginK, prevEndK;
				/* if we hit one end early, no need to look further */
				protected int minK, maxK; // TODO: better explanation
				
				protected EditPaths(MiddleEdit middleEdit)
				{
					_middleEdit = middleEdit;
				}
				
				int GetIndex(int d, int k)
				{
					// TODO: remove
					if (((d + k - middleK) % 2) == 1)
						throw new InvalidOperationException("odd: " + d + " + " + k + " - " + middleK);
					return (d + k - middleK) / 2;
				}
				
				public int GetX(int d, int k)
				{
					// TODO: remove
					if (k < beginK || k > endK)
						throw new InvalidOperationException("k " + k + " not in " + beginK + " - " + endK);
					return x[GetIndex(d, k)];
				}
				
				public long GetSnake(int d, int k)
				{
					// TODO: remove
					if (k < beginK || k > endK)
						throw new InvalidOperationException("k " + k + " not in " + beginK + " - " + endK);
					return _snake[GetIndex(d, k)];
				}
				
				private int ForceKIntoRange(int k)
				{
					/* if k is odd, so must be the result */
					if (k < minK)
						return minK + ((k ^ minK) & 1);
					else if (k > maxK)
						return maxK - ((k ^ maxK) & 1);
					return k;
				}
				
				public void Initialize(int k, int x, int minK, int maxK)
				{
					this.minK = minK;
					this.maxK = maxK;
					beginK = endK = middleK = k;
					this.x.Clear();
					this.x.Add(x);
					_snake.Clear();
					_snake.Add(NewSnake(k, x));
				}
				
				public abstract int Snake(int k, int x);
				protected abstract int GetLeft(int x);
				protected abstract int GetRight(int x);
				protected abstract bool IsBetter(int left, int right);
				protected abstract void AdjustMinMaxK(int k, int x);
				protected abstract bool Meets(int d, int k, int x, long snake);
				
				long NewSnake(int k, int x)
				{
					long y = k + x;
					long ret = ((long)x) << 32;
					return ret | y;
				}
				
				int Snake2x(long snake)
				{
					return (int)((ulong)snake >> 32);
				}
				
				int Snake2y(long snake)
				{
					return (int)((ulong)snake >> 32);
				}
				
				protected bool MakeEdit(long snake1, long snake2)
				{
					int x1 = Snake2x(snake1), x2 = Snake2x(snake2);
					int y1 = Snake2y(snake1), y2 = Snake2y(snake2);
					
					/*
					 * Check for incompatible partial edit paths:
					 * when there are ambiguities, we might have
					 * hit incompatible (i.e. non-overlapping)
					 * forward/backward paths.
					 *
					 * In that case, just pretend that we have
					 * an empty edit at the end of one snake; this
					 * will force a decision which path to take
					 * in the next recursion step.
					 */
					if (x1 > x2 || y1 > y2)
					{
						x1 = x2;
						y1 = y2;
					}
					_middleEdit._edit = new Edit(x1, x2, y1, y2);
					return true;
				}
				
				public bool Calculate(int d)
				{
					prevBeginK = beginK;
					prevEndK = endK;
					beginK = ForceKIntoRange(middleK - d);
					endK = ForceKIntoRange(middleK + d);
					// TODO: handle i more efficiently
					// TODO: walk snake(k, getX(d, k)) only once per (d, k)
					// TODO: move end points out of the loop to avoid conditionals inside the loop
					// go backwards so that we can avoid temp vars
					for (int k = endK; k >= beginK; k -= 2)
					{
						int left = -1, right = -1;
						long leftSnake = -1L, rightSnake = -1L;
						// TODO: refactor into its own function
						int i;
						if (k > prevBeginK)
						{
							i = GetIndex(d - 1, k - 1);
							left = x[i];
							int end = Snake(k - 1, left);
							leftSnake = left != end ?
								NewSnake(k - 1, end) :
								_snake[i];
							
							if (Meets(d, k - 1, end, leftSnake))
								return true;
							left = GetLeft(end);
						}
						if (k < prevEndK)
						{
							i = GetIndex(d - 1, k + 1);
							right = x[i];
							int end = Snake(k + 1, right);
							rightSnake = right != end ?
								NewSnake(k + 1, end) :
								_snake[i];
							
							if (Meets(d, k + 1, end, rightSnake))
								return true;
							right = GetRight(end);
						}
						int newX;
						long newSnakeTmp;
						if (k >= prevEndK ||
						    (k > prevBeginK &&
						     IsBetter(left, right)))
						{
							newX = left;
							newSnakeTmp = leftSnake;
						}
						else
						{
							newX = right;
							newSnakeTmp = rightSnake;
						}
						
						if (Meets(d, k, newX, newSnakeTmp))
							return true;
						AdjustMinMaxK(k, newX);
						i = GetIndex(d, k);
						
						x.Set(i, newX);
						_snake.Set(i, newSnakeTmp);
					}
					return false;
				}
			}
			
			class ForwardEditPaths : EditPaths
			{
				public ForwardEditPaths(MiddleEdit middleEdit)
					: base(middleEdit)
				{
				}
				
				public override int Snake(int k, int x)
				{
					for (; x < _middleEdit.endA && k + x < _middleEdit.endB; x++)
						if (!_middleEdit._a.Equals(x, _middleEdit._b, k + x))
							break;
					return x;
				}
				
				protected override int GetLeft(int x)
				{
					return x;
				}
				
				protected override int GetRight(int x)
				{
					return x + 1;
				}
				
				protected override bool IsBetter(int left, int right)
				{
					return left > right;
				}
				
				protected override void AdjustMinMaxK(int k, int x)
				{
					if (x >= _middleEdit.endA || k + x >= _middleEdit.endB)
					{
						if (k > _middleEdit.backward.middleK)
							maxK = k;
						else
							minK = k;
					}
				}
				
				protected override bool Meets(int d, int k, int x, long snake)
				{
					if (k < _middleEdit.backward.beginK || k > _middleEdit.backward.endK)
						return false;
					// TODO: move out of loop
					if (((d - 1 + k - _middleEdit.backward.middleK) % 2) == 1)
						return false;
					if (x < _middleEdit.backward.GetX(d - 1, k))
						return false;
					MakeEdit(snake, _middleEdit.backward.GetSnake(d - 1, k));
					return true;
				}
			}
			
			class BackwardEditPaths : EditPaths
			{
				public BackwardEditPaths(MiddleEdit middleEdit)
					: base(middleEdit)
				{
				}
				
				public override int Snake(int k, int x)
				{
					for (; x > _middleEdit.beginA && k + x > _middleEdit.beginB; x--)
						if (!_middleEdit._a.Equals(x - 1, _middleEdit._b, k + x - 1))
							break;
					return x;
				}
				
				protected override int GetLeft(int x)
				{
					return x - 1;
				}
				
				protected override int GetRight(int x)
				{
					return x;
				}
				
				protected override bool IsBetter(int left, int right)
				{
					return left < right;
				}
				
				protected override void AdjustMinMaxK(int k, int x)
				{
					if (x <= _middleEdit.beginA || k + x <= _middleEdit.beginB)
					{
						if (k > _middleEdit.forward.middleK)
							maxK = k;
						else
							minK = k;
					}
				}
				
				protected override bool Meets(int d, int k, int x, long snake)
				{
					if (k < _middleEdit.forward.beginK || k > _middleEdit.forward.endK)
						return false;
					// TODO: move out of loop
					if (((d + k - _middleEdit.forward.middleK) % 2) == 1)
						return false;
					if (x > _middleEdit.forward.GetX(d, k))
						return false;
					MakeEdit(_middleEdit.forward.GetSnake(d, k), snake);
					return true;
				}
			}
		}
	}
	
	/// <summary>
	/// Arbitrary sequence of elements with fast comparison support.
	/// <para />
	/// A sequence of elements is defined to contain elements in the index range
	/// <code>[0, <seealso cref="size()"/>)</code>, like a standard Java List implementation.
	/// Unlike a List, the members of the sequence are not directly obtainable, but
	/// element equality can be tested if two Sequences are the same implementation.
	/// <para />
	/// An implementation may chose to implement the equals semantic as necessary,
	/// including fuzzy matching rules such as ignoring insignificant sub-elements,
	/// e.g. ignoring whitespace differences in text.
	/// <para />
	/// Implementations of Sequence are primarily intended for use in content
	/// difference detection algorithms, to produce an <seealso cref="EditList"/> of
	/// <seealso cref="Edit"/> instances describing how two Sequence instances differ.
	/// </summary>
	public interface ISequence
	{
		/// <returns>
		/// Total number of items in the sequence.
		/// </returns>
		int Size();

		/// <summary>
		/// Determine if the i-th member is equal to the j-th member.
		/// <para />
		/// Implementations must ensure <code>equals(thisIdx,other,otherIdx)</code>
		/// returns the same as <code>other.equals(otherIdx,this,thisIdx)</code>.
		/// </summary>
		/// <param name="thisIdx">
		/// Index within <code>this</code> sequence; must be in the range
		/// <code>[ 0, this.size() )</code>.
		/// </param>
		/// <param name="other">
		/// Another sequence; must be the same implementation class, that
		/// is <code>this.getClass() == other.getClass()</code>. </param>
		/// <param name="otherIdx">
		/// Index within <code>other</code> sequence; must be in the range
		/// <code>[ 0, other.size() )</code>. </param>
		/// <returns>
		/// true if the elements are equal; false if they are not equal.
		/// </returns>
		bool Equals(int thisIdx, ISequence other, int otherIdx);
	}
	
	/// <summary>
	/// A modified region detected between two versions of roughly the same content.
	/// <para />
	/// Regions should be specified using 0 based notation, so add 1 to the
	/// start and end marks for line numbers in a file.
	/// <para />
	/// An edit where <code>beginA == endA &amp;&amp; beginB &gt; endB</code> is an insert edit,
	/// that is sequence B inserted the elements in region
	/// <code>[beginB, endB)</code> at <code>beginA</code>.
	/// <para />
	/// An edit where <code>beginA &gt; endA &amp;&amp; beginB &gt; endB</code> is a replace edit,
	/// that is sequence B has replaced the range of elements between
	/// <code>[beginA, endA)</code> with those found in <code>[beginB, endB)</code>.
	/// </summary>
	public class Edit
	{
		/// <summary>
		/// Create a new empty edit.
		/// </summary>
		/// <param name="aStart">beginA: start and end of region in sequence A; 0 based.</param>
		/// <param name="bStart">beginB: start and end of region in sequence B; 0 based.</param>
		public Edit(int aStart, int bStart)
			: this(aStart, aStart, bStart, bStart)
		{
		}

		/// <summary>
		/// Create a new empty edit.
		/// </summary>
		/// <param name="aStart">beginA: start and end of region in sequence A; 0 based.</param>
		/// <param name="aEnd">endA: end of region in sequence A; must be >= as.</param>
		/// <param name="bStart">beginB: start and end of region in sequence B; 0 based.</param>
		/// <param name="bEnd">endB: end of region in sequence B; must be >= bs.</param>
		public Edit(int aStart, int aEnd, int bStart, int bEnd)
		{
			BeginA = aStart;
			EndA = aEnd;

			BeginB = bStart;
			EndB = bEnd;
		}

		/// <summary>
		/// Gets the type of this region.
		/// </summary>
		public ChangeType EditType
		{
			get
			{
				if (BeginA == EndA && BeginB < EndB)
					return ChangeType.Added;
				if (BeginA < EndA && BeginB == EndB)
					return ChangeType.Deleted;
				if (BeginA == EndA && BeginB == EndB)
					return ChangeType.None;

				return ChangeType.Modified;
			}
		}

		/// <summary>
		/// Start point in sequence A.
		/// </summary>
		public int BeginA { get; set; }

		/// <summary>
		/// End point in sequence A.
		/// </summary>
		public int EndA { get; private set; }

		/// <summary>
		/// Start point in sequence B.
		/// </summary>
		public int BeginB { get; private set; }

		/// <summary>
		/// End point in sequence B.
		/// </summary>
		public int EndB { get; private set; }

		/// <summary>
		/// Increase <see cref="EndA"/> by 1.
		/// </summary>
		public void ExtendA()
		{
			EndA++;
		}

		/// <summary>
		/// Increase <see cref="EndB"/> by 1.
		/// </summary>
		public void ExtendB()
		{
			EndB++;
		}

		/// <summary>
		/// Swap A and B, so the edit goes the other direction.
		/// </summary>
		public void Swap()
		{
			int sBegin = BeginA;
			int sEnd = EndA;

			BeginA = BeginB;
			EndA = EndB;

			BeginB = sBegin;
			EndB = sEnd;
		}

		public override int GetHashCode()
		{
			return BeginA ^ EndA;
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is
		/// equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the
		/// current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with
		/// the current <see cref="T:System.Object"/>.
		/// </param>
		/// <exception cref="T:System.NullReferenceException">
		/// The <paramref name="obj"/> parameter is null.
		/// </exception>
		/// <filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			Edit e = (obj as Edit);
			if (e != null)
			{
				return BeginA == e.BeginA && EndA == e.EndA && BeginB == e.BeginB && EndB == e.EndB;
			}

			return false;
		}

		public static bool operator ==(Edit left, Edit right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Edit left, Edit right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			ChangeType t = EditType;
			return t + "(" + BeginA + "-" + EndA + "," + BeginB + "-" + EndB + ")";
		}
	}
	
	/// <summary>
	/// A sequence supporting UNIX formatted text in byte[] format.
	/// <para />
	/// Elements of the sequence are the lines of the file, as delimited by the UNIX
	/// newline character ('\n'). The file content is treated as 8 bit binary text,
	/// with no assumptions or requirements on character encoding.
	/// <para />
	/// Note that the first line of the file is element 0, as defined by the Sequence
	/// interface API. Traditionally in a text editor a patch file the first line is
	/// line number 1. Callers may need to subtract 1 prior to invoking methods if
	/// they are converting from "line number" to "element index".
	/// </summary>
	public class RawText : ISequence
	{
		// The file content for this sequence.
		readonly byte[] content;

		/// <summary>
		/// The content of the raw text as byte array.
		/// </summary>
		public byte[] Content {
			get { return content; }
		}

		// Map of line number to starting position within content.
		readonly List<int> lines;

		/// <summary>
		/// Represents starting points of lines in Content. Note: the line indices are 1-based and
		/// are mapped to 0-based positions in the Content byte array. As line indices are based on 1 the result of line 0 is undefined.
		/// </summary>
		public List<int> LineStartIndices {
			get { return lines; }
		}

		// Hash code for each line, for fast equality elimination.
		private readonly List<int> hashes;

		/// <summary>
		/// Create a new sequence from an existing content byte array.
		/// <para />
		/// The entire array (indexes 0 through length-1) is used as the content.
		/// </summary>
		/// <param name="input">
		/// the content array. The array is never modified, so passing
		/// through cached arrays is safe.
		/// </param>
		public RawText(byte[] input)
		{
			content = input;
			lines = RawParseUtils.LineMap(content, 0, content.Length);
			hashes = computeHashes();
		}
		
		public int Size()
		{
			// The line map is always 2 entries larger than the number of lines in
			// the file. Index 0 is padded out/unused. The last index is the total
			// length of the buffer, and acts as a sentinel.
//
			return lines.Count - 2;
		}

		public bool Equals(int thisIdx, ISequence other, int otherIdx)
		{
			return Equals(this, thisIdx + 1, (RawText) other, otherIdx + 1);
		}

		static bool Equals(RawText a, int ai, RawText b, int bi)
		{
			if (a.hashes[ai] != b.hashes[bi])
				return false;

			int a_start = a.lines[ai];
			int b_start = b.lines[bi];
			int a_end = a.lines[ai + 1];
			int b_end = b.lines[bi + 1];

			if (a_end - a_start != b_end - b_start)
				return false;

			while (a_start < a_end) {
				if (a.content[a_start++] != b.content[b_start++])
					return false;
			}
			return true;
		}

		/// <summary>
		/// Write a specific line to the output stream, without its trailing LF.
		/// <para />
		/// The specified line is copied as-is, with no character encoding
		/// translation performed.
		/// <para />
		/// If the specified line ends with an LF ('\n'), the LF is <b>not</b>
		/// copied. It is up to the caller to write the LF, if desired, between
		/// output lines.
		/// </summary>
		/// <param name="out">
		/// Stream to copy the line data onto. </param>
		/// <param name="i">
		/// Index of the line to extract. Note this is 0-based, so line
		/// number 1 is actually index 0. </param>
		/// <exception cref="IOException">
		/// the stream write operation failed.
		/// </exception>
		public void writeLine(Stream @out, int i)
		{
			int start = lines[i + 1];
			int end = lines[i + 2];
			if (content[end - 1] == '\n')
			{
				end--;
			}
			@out.Write(content, start, end - start);
		}

		/// <summary>
		/// Determine if the file ends with a LF ('\n').
		/// </summary>
		/// <returns> true if the last line has an LF; false otherwise. </returns>
		public bool isMissingNewlineAtEnd()
		{
			int end = lines[lines.Count - 1];
			if (end == 0)
				return true;
			return content[end - 1] != '\n';
		}

		private List<int> computeHashes()
		{
			var r = new List<int>(lines.Count);
			r.Add(0);
			for (int lno = 1; lno < lines.Count - 1; lno++)
			{
				int ptr = lines[lno];
				int end = lines[lno + 1];
				r.Add(HashLine(content, ptr, end));
			}
			r.Add(0);
			return r;
		}

		/// <summary>
		/// Compute a hash code for a single line.
		/// </summary>
		/// <param name="raw">The raw file content. </param>
		/// <param name="ptr">
		/// First byte of the content line to hash. </param>
		/// <param name="end">
		/// 1 past the last byte of the content line.
		/// </param>
		/// <returns>
		/// Hash code for the region <code>[ptr, end)</code> of raw.
		/// </returns>
		private static int HashLine(byte[] raw, int ptr, int end)
		{
			int hash = 5381;
			for (; ptr < end; ptr++)
			{
				hash = (hash << 5) ^ (raw[ptr] & 0xff);
			}
			return hash;
		}
	}
	
	static class RawParseUtils
	{
		public static List<int> LineMap(byte[] buf, int ptr, int end)
		{
			// Experimentally derived from multiple source repositories
			// the average number of bytes/line is 36. Its a rough guess
			// to initially size our map close to the target.
			
			List<int> map = new List<int>((end - ptr) / 36);
			map.Add(int.MinValue);
			for (; ptr < end; ptr = buf.IndexOfAny(ptr + 1, 0xA))
				map.Add(ptr);
			map.Add(end);
			return map;
		}
		
		public static int IndexOfAny(this byte[] buf, int start, params byte[] search)
		{
			for (int i = start; i < buf.Length; i++) {
				for (int j = 0; j < search.Length; j++) {
					if (buf[i] == search[j])
						return i;
				}
			}
			
			return buf.Length;
		}
		
		public static void Set<T>(this IList<T> instance, int index, T value)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			
			if (index == instance.Count)
				instance.Add(value);
			else
				instance[index] = value;
		}
	}
}
