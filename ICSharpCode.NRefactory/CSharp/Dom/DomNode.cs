// 
// AstNode.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.NRefactory.CSharp
{
	public abstract class DomNode
	{
		#region Null
		public static readonly DomNode Null = new NullAstNode ();
		sealed class NullAstNode : DomNode
		{
			public override NodeType NodeType {
				get {
					return NodeType.Unknown;
				}
			}
			
			public override bool IsNull {
				get {
					return true;
				}
			}
			
			public override S AcceptVisitor<T, S> (DomVisitor<T, S> visitor, T data)
			{
				return default (S);
			}
		}
		#endregion
		
		DomNode parent;
		DomNode prevSibling;
		DomNode nextSibling;
		DomNode firstChild;
		DomNode lastChild;
		int role;
		
		public abstract NodeType NodeType {
			get;
		}
		
		public virtual bool IsNull {
			get {
				return false;
			}
		}
		
		public virtual DomLocation StartLocation {
			get {
				var child = firstChild;
				if (child == null)
					return DomLocation.Empty;
				return child.StartLocation;
			}
		}
		
		public virtual DomLocation EndLocation {
			get {
				var child = lastChild;
				if (child == null)
					return DomLocation.Empty;
				return child.EndLocation;
			}
		}
		
		public DomNode Parent {
			get { return parent; }
		}
		
		public int Role {
			get { return role; }
		}
		
		public DomNode NextSibling {
			get { return nextSibling; }
		}
		
		public DomNode PrevSibling {
			get { return prevSibling; }
		}
		
		public DomNode FirstChild {
			get { return firstChild; }
		}
		
		public DomNode LastChild {
			get { return lastChild; }
		}
		
		public IEnumerable<DomNode> Children {
			get {
				var cur = firstChild;
				while (cur != null) {
					yield return cur;
					cur = cur.nextSibling;
				}
			}
		}
		
		public DomNode GetChildByRole (int role)
		{
			var cur = firstChild;
			while (cur != null) {
				if (cur.role == role)
					return cur;
				cur = cur.nextSibling;
			}
			return null;
		}
		
		public IEnumerable<DomNode> GetChildrenByRole (int role)
		{
			var cur = firstChild;
			while (cur != null) {
				if (cur.role == role)
					yield return cur;
				cur = cur.nextSibling;
			}
		}
		
		public void AddChild (DomNode child, int role)
		{
			if (child == null)
				return;
			if (child.parent != null)
				throw new ArgumentException ("Node is already used in another tree.", "child");
			child.parent = this;
			child.role = role;
			if (firstChild == null) {
				lastChild = firstChild = child;
			} else {
				lastChild.nextSibling = child;
				child.prevSibling = lastChild;
				lastChild = child;
			}
		}
		
		public void InsertChildBefore (DomNode nextSibling, DomNode child, int role)
		{
			if (nextSibling == null) {
				AddChild (child, role);
				return;
			}
			
			if (child == null)
				return;
			if (child.parent != null)
				throw new ArgumentException ("Node is already used in another tree.", "child");
			if (nextSibling.parent != this)
				throw new ArgumentException ("NextSibling is not a child of this node.", "nextSibling");
			
			child.parent = this;
			child.role = role;
			child.nextSibling = nextSibling;
			child.prevSibling = nextSibling.prevSibling;
			
			if (nextSibling.prevSibling != null) {
				Debug.Assert(nextSibling.prevSibling.nextSibling == nextSibling);
				nextSibling.prevSibling.nextSibling = child;
			} else {
				Debug.Assert(firstChild == nextSibling);
				firstChild = child;
			}
			nextSibling.prevSibling = child;
		}
		
		/// <summary>
		/// Removes this node from its parent.
		/// </summary>
		public void Remove()
		{
			if (parent != null) {
				if (prevSibling != null) {
					Debug.Assert(prevSibling.nextSibling == this);
					prevSibling.nextSibling = nextSibling;
				} else {
					Debug.Assert(parent.firstChild == this);
					parent.firstChild = nextSibling;
				}
				if (nextSibling != null) {
					Debug.Assert(nextSibling.prevSibling == this);
					nextSibling.prevSibling = prevSibling;
				} else {
					Debug.Assert(parent.lastChild == this);
					parent.lastChild = prevSibling;
				}
				parent = null;
				prevSibling = null;
				nextSibling = null;
			}
		}
		
		/// <summary>
		/// Replaces this node with the new node.
		/// </summary>
		public void Replace(DomNode newNode)
		{
			if (newNode == null) {
				Remove();
				return;
			}
			if (newNode.parent != null)
				throw new ArgumentException ("Node is already used in another tree.", "newNode");
			newNode.parent = parent;
			newNode.role = role;
			newNode.prevSibling = prevSibling;
			newNode.nextSibling = nextSibling;
			if (parent != null) {
				if (prevSibling != null) {
					Debug.Assert(prevSibling.nextSibling == this);
					prevSibling.nextSibling = newNode;
				} else {
					Debug.Assert(parent.firstChild == this);
					parent.firstChild = newNode;
				}
				if (nextSibling != null) {
					Debug.Assert(nextSibling.prevSibling == this);
					nextSibling.prevSibling = newNode;
				} else {
					Debug.Assert(parent.lastChild == this);
					parent.lastChild = newNode;
				}
				parent = null;
				prevSibling = null;
				nextSibling = null;
			}
		}
		
		public abstract S AcceptVisitor<T, S> (DomVisitor<T, S> visitor, T data);
		
		public static class Roles
		{
			// some pre defined constants for common roles
			public const int Identifier    = 1;
			public const int Keyword   = 2;
			public const int Parameter  = 3;
			public const int Attribute = 4;
			public const int ReturnType = 5;
			public const int Modifier  = 6;
			public const int Body       = 7;
			public const int Initializer = 8;
			public const int Condition = 9;
			public const int EmbeddedStatement = 10;
			public const int Iterator = 11;
			public const int Expression = 12;
			public const int Statement = 13;
			public const int TargetExpression = 14;
			public const int Member = 15;
			
			// some pre defined constants for most used punctuation
			
			public const int LPar = 50; // (
			public const int RPar = 51; // )
			
			public const int LBrace = 52; // {
			public const int RBrace = 53; // }
			
			public const int LBracket = 54; // [
			public const int RBracket = 55; // ]
			
			public const int LChevron = 56; // <
			public const int RChevron = 57; // >
			
			public const int Dot = 58; // ,
			public const int Comma = 59; // ,
			public const int Colon = 60; // :
			public const int Semicolon = 61; // ;
			public const int QuestionMark = 62; // ?
			
			public const int Assign = 63; // =
			
			public const int TypeParameter = 64;
			public const int Constraint = 65;
			public const int TypeArgument = 66;
			
			public const int Comment = 67;
		}
	}
}
