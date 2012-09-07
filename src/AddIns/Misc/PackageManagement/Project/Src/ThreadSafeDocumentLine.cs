// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor;
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
		
		public int Offset {
			get {
				if (WorkbenchSingleton.InvokeRequired) {
					return WorkbenchSingleton.SafeThreadFunction(() => Offset);
				}
				return line.Offset;
			}
		}
		
		public int Length {
			get {
				if (WorkbenchSingleton.InvokeRequired) {
					return WorkbenchSingleton.SafeThreadFunction(() => Length);
				}
				return line.Length;
			}
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
		
		public string Text {
			get {
				if (WorkbenchSingleton.InvokeRequired) {
					return WorkbenchSingleton.SafeThreadFunction(() => Text);
				}
				return line.Text;
			}
		}
	}
}
