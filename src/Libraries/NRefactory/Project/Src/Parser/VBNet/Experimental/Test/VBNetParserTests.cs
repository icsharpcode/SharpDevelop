// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com" />
//     <version>$Revision$</version>
// </file>


using System;
using NUnit.Framework;

namespace VBParserExperiment
{
	[TestFixture]
	public class VBNetParserTests
	{
		[Test]
		[Ignore]
		// TODO : check results
		public void CommentAfterEnd()
		{
			RunTest(
				@"Class Test
	Sub A()
		Dim a = <Test></Test><!-- a --> a
	End Sub
End Class",
				@""
			);
		}
		
		void RunTest(string code, string result)
		{
			throw new NotImplementedException();
		}
	}
}
