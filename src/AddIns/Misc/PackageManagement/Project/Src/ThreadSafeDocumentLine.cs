// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
