// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>


using System;
using System.IO;
using ICSharpCode.NRefactory.Parser.VBNet.Experimental;
using NUnit.Framework;

namespace VBParserExperiment
{
	[TestFixture]
	public class LATextReaderTests
	{
		[Test]
		public void TestPeek()
		{
			LATextReader reader = new LATextReader(new StringReader("abcd"));
			
			CheckPeek(reader, 0, 'a');
			CheckPeek(reader, 2, 'c');
			CheckPeek(reader, 3, 'd');
			CheckPeek(reader, 1, 'b');
			CheckPeek(reader, 0, 'a');
			Assert.AreEqual((int)'a', reader.Read());
			CheckPeek(reader, 1, 'c');
			CheckPeek(reader, 2, 'd');
			CheckPeek(reader, 0, 'b');
		}
		
		void CheckPeek(LATextReader reader, int num1, char char2)
		{
			Assert.AreEqual((int)char2, reader.Peek(num1));
		}
	}
}
