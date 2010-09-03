// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.Search;
using System;
using SearchAndReplace;

namespace SearchAndReplace.Tests.Utils
{
	public class MockCurrentDocumentIterator : IDocumentIterator
	{
		ProvidedDocumentInformation current;
		string currentFileName = String.Empty;
		bool moved;
		
		public MockCurrentDocumentIterator()
		{
		}
		
		public ProvidedDocumentInformation Current {
			get {
				return current;
			}
			set {
				current = value;
			}
		}
		
		public string CurrentFileName {
			get {
				return currentFileName;
			}
			set {
				currentFileName = value;
			}
		}
		
		public bool MoveForward()
		{
			if (moved) {
				return false;
			} else {
				moved = true;
			}
			return true;
		}
		
		public bool MoveBackward()
		{
			return false;
		}
		
		public void Reset()
		{
		}
	}
}
