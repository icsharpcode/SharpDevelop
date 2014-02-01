// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class ThreadSafeDocumentLine : IDocumentLine
	{
		IDocumentLine line;
		
		public ThreadSafeDocumentLine(IDocumentLine line)
		{
			this.line = line;
		}
		
		T InvokeIfRequired<T>(Func<T> callback)
		{
			return SD.MainThread.InvokeIfRequired(callback);
		}
		
		public int Offset {
			get { return InvokeIfRequired(() => line.Offset); }
		}
		
		public int Length {
			get { return InvokeIfRequired(() => line.Length); }
		}
		
		public int EndOffset {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int TotalLength {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int DelimiterLength {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int LineNumber {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IDocumentLine PreviousLine {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IDocumentLine NextLine {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsDeleted {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
