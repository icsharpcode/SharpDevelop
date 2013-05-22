//
// InitializerPath.cs
//
// Author:
//       Simon Lindgren <simon.n.lindgren@gmail.com>
//
// Copyright (c) 2012 Simon Lindgren
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
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.TypeSystem;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	class AccessPath
	{
		public AccessPath(IVariable target)
		{
			VariableRoot = target;
			MemberPath = new List<IMember>();
		}

		public static AccessPath FromResolveResult(ResolveResult resolveResult)
		{
			var memberPath = new List<IMember>();
			var currentResolveResult = resolveResult;
			do {
				if (currentResolveResult is MemberResolveResult) {
					var memberResolveResult = (MemberResolveResult)currentResolveResult;
					memberPath.Add(memberResolveResult.Member);
					currentResolveResult = memberResolveResult.TargetResult;
				} else if (currentResolveResult is LocalResolveResult) {
					// This is the root variable
					var localResolveResult = (LocalResolveResult)currentResolveResult;
					memberPath.Reverse();
					return new AccessPath(localResolveResult.Variable) {
						MemberPath = memberPath
					};
				} else if (currentResolveResult is ThisResolveResult) {
					break;
				} else {
					// Unsupported path
					return null;
				}
			} while (currentResolveResult != null);

			memberPath.Reverse();
			return new AccessPath(null) {
				MemberPath = memberPath
			};
		}

		public AccessPath GetParentPath()
		{
			if (MemberPath.Count < 1)
				throw new InvalidOperationException("Cannot get the parent path of a path that does not contain any members.");
			return new AccessPath(VariableRoot) {
				MemberPath = MemberPath.Take(MemberPath.Count - 1).ToList()
			};
		}

		public bool IsSubPath(AccessPath other)
		{
			if (!object.Equals(other.VariableRoot, VariableRoot))
				return false;
			if (MemberPath.Count <= other.MemberPath.Count)
				return false;
			for (int i = 0; i < other.MemberPath.Count; i++) {
				if (MemberPath [i] != other.MemberPath [i])
					return false;
			}
			return true;
		}

		public IVariable VariableRoot { get; set; }

		public int PartCount {
			get {
				return MemberPath.Count + (VariableRoot == null ? 0 : 1);
			}
		}

		public string RootName {
			get {
				if (VariableRoot != null)
					return VariableRoot.Name;
				return MemberPath.First().Name;
			}
		}

		public IList<IMember> MemberPath { get; private set; }

		public override bool Equals(object obj)
		{
			if (obj == null || obj.GetType() != typeof(AccessPath))
				return false;

			var other = (AccessPath)obj;

			if (!object.Equals(VariableRoot, other.VariableRoot))
				return false;

			if (MemberPath.Count != other.MemberPath.Count)
				return false;

			for (int i = 0; i < MemberPath.Count; i++) {
				if (!object.Equals(MemberPath [i], other.MemberPath [i]))
					return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hash = VariableRoot != null ? VariableRoot.GetHashCode() : 37;
			foreach (var member in MemberPath)
				hash ^= 31 * member.GetHashCode();
			return hash;
		}

		public static bool operator==(AccessPath left, AccessPath right)
		{
			return object.Equals(left, right);
		}

		public static bool operator!=(AccessPath left, AccessPath right)
		{
			return !object.Equals(left, right);
		}

		public override string ToString()
		{
			string memberPathString = string.Join(".", MemberPath.Select<IMember, string>(member => member.Name));
			if (VariableRoot == null)
				return string.Format("[AccessPath: {0}]", memberPathString);
			return string.Format("[AccessPath: {0}.{1}]", VariableRoot.Name, memberPathString);
		}
	}

}

