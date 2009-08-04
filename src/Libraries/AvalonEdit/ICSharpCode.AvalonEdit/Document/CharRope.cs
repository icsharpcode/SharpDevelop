/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 03.08.2009
 * Time: 20:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.Text;

namespace ICSharpCode.AvalonEdit.Document
{
	/// <summary>
	/// Poor man's template specialization: extension methods for Rope&lt;char&gt;.
	/// </summary>
	public static class CharRope
	{
		/// <summary>
		/// Creates a new rope from the specified text.
		/// </summary>
		public static Rope<char> Create(string text)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			return new Rope<char>(InitFromString(text));
		}
		
		/// <summary>
		/// Retrieves the text for a portion of the rope.
		/// Runs in O(lg N + M), where M=<paramref name="length"/>.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">offset or length is outside the valid range.</exception>
		/// <remarks>
		/// This method counts as a read access and may be called concurrently to other read accesses.
		/// </remarks>
		public static string ToString(this Rope<char> rope, int startIndex, int length)
		{
			if (rope == null)
				throw new ArgumentNullException("rope");
			StringBuilder b = new StringBuilder(length);
			rope.WriteTo(b, startIndex, length);
			return b.ToString();
		}
		
		/// <summary>
		/// Retrieves the text for a portion of the rope and writes it to the specified string builder.
		/// Runs in O(lg N + M), where M=<paramref name="length"/>.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">offset or length is outside the valid range.</exception>
		/// <remarks>
		/// This method counts as a read access and may be called concurrently to other read accesses.
		/// </remarks>
		public static void WriteTo(this Rope<char> rope, StringBuilder output, int startIndex, int length)
		{
			if (rope == null)
				throw new ArgumentNullException("rope");
			if (output == null)
				throw new ArgumentNullException("output");
			rope.VerifyRange(startIndex, length);
			rope.root.WriteTo(startIndex, output, length);
		}
		
		/// <summary>
		/// Appends text to this rope.
		/// Runs in O(lg N + M).
		/// </summary>
		/// <exception cref="ArgumentNullException">newElements is null.</exception>
		public static void AddText(this Rope<char> rope, string text)
		{
			InsertText(rope, rope.Length, text);
		}
		
		/// <summary>
		/// Inserts text into this rope.
		/// Runs in O(lg N + M).
		/// </summary>
		/// <exception cref="ArgumentNullException">newElements is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">index or length is outside the valid range.</exception>
		public static void InsertText(this Rope<char> rope, int index, string text)
		{
			if (rope == null)
				throw new ArgumentNullException("rope");
			rope.InsertRange(index, text.ToCharArray(), 0, text.Length);
			/*if (index < 0 || index > rope.Length) {
				throw new ArgumentOutOfRangeException("index", index, "0 <= index <= " + rope.Length.ToString(CultureInfo.InvariantCulture));
			}
			if (text == null)
				throw new ArgumentNullException("text");
			if (text.Length == 0)
				return;
			rope.root = rope.root.Insert(index, text);
			rope.OnChanged();*/
		}
		
		internal static RopeNode<char> InitFromString(string text)
		{
			char[] arr = text.ToCharArray();
			return RopeNode<char>.CreateFromArray(arr, 0, arr.Length);
			/*
			if (text.Length == 0) {
				return RopeNode<char>.emptyRopeNode;
			}
			int nodeCount = (text.Length + RopeNode<char>.NodeSize - 1) / RopeNode<char>.NodeSize;
			RopeNode<char> node = RopeNode<char>.CreateNodes(nodeCount);
			// TODO: store data
			return node;
			 */
		}
		
		internal static void WriteTo(this RopeNode<char> node, int index, StringBuilder output, int count)
		{
			if (node.height == 0) {
				// leaf node: append data
				output.Append(node.contents, index, count);
			} else {
				// concat node: do recursive calls
				if (index + count <= node.left.length) {
					node.left.WriteTo(index, output, count);
				} else if (index >= node.left.length) {
					node.right.WriteTo(index - node.left.length, output, count);
				} else {
					int amountInLeft = node.left.length - index;
					node.left.WriteTo(index, output, amountInLeft);
					node.right.WriteTo(0, output, count - amountInLeft);
				}
			}
		}
		
		/*
		internal static RopeNode<char> Insert(this RopeNode<char> node, int offset, string newText)
		{
			if (node.height == 0) {
				if (node.length + newText.Length < RopeNode<char>.NodeSize) {
					RopeNode<char> result = node.CloneIfShared();
					int lengthAfterOffset = node.length - offset;
					char[] arr = result.contents;
					for (int i = lengthAfterOffset; i >= 0; i--) {
						arr[i + offset + newText.Length] = arr[i + offset];
					}
					newText.CopyTo(0, arr, offset, newText.Length);
					result.length += newText.Length;
					return result;
				} else {
					// TODO: implement this more efficiently
					return node.Insert(offset, InitFromString(newText));
				}
			} else {
				RopeNode<char> result = node.CloneIfShared();
				if (offset < result.left.length) {
					result.left = result.left.Insert(offset, newText);
				} else {
					result.right = result.right.Insert(offset - result.left.length, newText);
				}
				result.length += newText.Length;
				result.Rebalance();
				return result;
			}
		}*/
	}
}
