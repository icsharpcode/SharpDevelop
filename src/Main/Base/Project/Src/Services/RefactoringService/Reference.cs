// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// A reference to a class or class member.
	/// </summary>
	public class Reference
	{
		string fileName;
		int offset, length;
		string expression;
		ResolveResult resolveResult;
		
		public Reference(string fileName, int offset, int length, string expression, ResolveResult resolveResult)
		{
			this.fileName = fileName;
			this.offset = offset;
			this.length = length;
			this.expression = expression;
			this.resolveResult = resolveResult;
		}
		
		public string FileName {
			get {
				return fileName;
			}
		}
		
		public int Offset {
			get {
				return offset;
			}
		}
		
		public int Length {
			get {
				return length;
			}
		}
		
		public string Expression {
			get {
				return expression;
			}
		}
		
		public ResolveResult ResolveResult {
			get {
				return resolveResult;
			}
		}
	}
}
