// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>


using System;
using ICSharpCode.NRefactory.Parser;
using NUnit.Framework;

namespace VBParserExperiment
{
	[TestFixture]
	public class TokenTests
	{
		[Test]
		public void TestMethod()
		{
			Assert.DoesNotThrow(
				() => {
					string text = new Token(71, 1, 1).ToString();
					Console.WriteLine(text);
				}
			);
		}
	}
}
