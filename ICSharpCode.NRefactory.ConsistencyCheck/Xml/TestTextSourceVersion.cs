// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Editor;

namespace ICSharpCode.NRefactory.ConsistencyCheck.Xml
{
	public class TestTextSourceVersion : ITextSourceVersion
	{
		readonly TestTextSourceVersion baseVersion;
		readonly IEnumerable<TextChangeEventArgs> changesFromBaseVersionToThis;
		
		public TestTextSourceVersion()
		{
		}
		
		public TestTextSourceVersion(TestTextSourceVersion baseVersion, IEnumerable<TextChangeEventArgs> changesFromBaseVersionToThis)
		{
			this.baseVersion = baseVersion;
			this.changesFromBaseVersionToThis = changesFromBaseVersionToThis;
		}
		
		public bool BelongsToSameDocumentAs(ITextSourceVersion other)
		{
			TestTextSourceVersion o = (TestTextSourceVersion)other;
			return o == this || o == baseVersion || o.baseVersion == this;
		}
		
		public int CompareAge(ITextSourceVersion other)
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<TextChangeEventArgs> GetChangesTo(ITextSourceVersion other)
		{
			TestTextSourceVersion o = (TestTextSourceVersion)other;
			if (o.baseVersion == this)
				return o.changesFromBaseVersionToThis;
			else
				throw new NotImplementedException();
		}
		
		public int MoveOffsetTo(ITextSourceVersion other, int oldOffset, AnchorMovementType movement)
		{
			throw new NotImplementedException();
		}
	}
}
