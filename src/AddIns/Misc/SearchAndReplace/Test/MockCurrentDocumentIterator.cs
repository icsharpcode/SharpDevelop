// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
