// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.Search;
using System;
using NUnit.Framework;
using SearchAndReplace;
using SearchAndReplace.Tests.Utils;

namespace SearchAndReplace.Tests
{
	/// <summary>
	/// The fix for SD2-857 highlighted another bug (SD2-1312) in the 
	/// ForwardTextIterator where it does not handle the case where
	/// the ITextBufferStrategy has a length of zero.
	/// </summary>
	[TestFixture]
	public class ForwardIteratorWithEmptyTextBufferTestFixture
	{
		ForwardTextIterator forwardTextIterator;
		
		[SetUp]
		public void SetUp()
		{
			// Create the document to be iterated through.
			MockDocument doc = new MockDocument();
			ProvidedDocumentInformation docInfo = new ProvidedDocumentInformation(doc, 
				@"C:\Temp\test.txt", 
				0);
			
			// Create the forward iterator.
			forwardTextIterator = new ForwardTextIterator(docInfo);
		}
		
		[Test]
		public void CannotMoveAhead()
		{
			Assert.IsFalse(forwardTextIterator.MoveAhead(1));
		}
	}
}
