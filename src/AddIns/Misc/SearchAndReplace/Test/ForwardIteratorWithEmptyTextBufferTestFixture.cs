// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using SearchAndReplace.Tests.Utils;
using ICSharpCode.TextEditor.Document;
using SearchAndReplace;
using NUnit.Framework;

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
			StringTextBufferStrategy textBufferStrategy = new StringTextBufferStrategy();
			doc.TextBufferStrategy = textBufferStrategy;
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
