// 
// QualifiedIdentifier.cs
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
using System.Linq;
using System.Text;

namespace ICSharpCode.NRefactory.CSharp
{
	public class QualifiedIdentifier : AbstractNode
	{
		public string QualifiedName {
			get {
				StringBuilder builder = new StringBuilder ();
				int i = 0;
				foreach (Identifier identifier in GetChildrenByRole (Roles.Identifier)) {
					if (i > 0) {
						if (i == 1 && HasDoubleColon)
							builder.Append ("::");
						else
							builder.Append ('.');
					}
					builder.Append (identifier.Name);
					i++;
				}
				return builder.ToString ();
			}
		}
		
		public IEnumerable<Identifier> NameParts {
			get { 
				return GetChildrenByRole (Roles.Identifier).Cast<Identifier>();
			}
		}
		
		public bool HasDoubleColon {
			get { return false; } // TODO
		}
		
		public QualifiedIdentifier ()
		{
		}
	}
}
