// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.VBNetBinding.Tests
{
	[TestFixture]
	public class LanguageUtilsTests
	{
		[Test]
		public void TrimPreprocessorDirectives1()
		{
			string input = "			#Region Test";
			string expectedOutput = "			";
			
			TestTrimPreprocessorDirectives(input, expectedOutput);
		}
		
		[Test]
		public void TrimPreprocessorDirectives2()
		{
			string input = "Dim a# = 12.0";
			string expectedOutput = "Dim a# = 12.0";
			
			TestTrimPreprocessorDirectives(input, expectedOutput);
		}
		
		[Test]
		public void TrimPreprocessorDirectives3()
		{
			string input = "Dim a = 12.0#";
			string expectedOutput = "Dim a = 12.0#";
			
			TestTrimPreprocessorDirectives(input, expectedOutput);
		}
		
		[Test]
		public void TrimPreprocessorDirectives4()
		{
			string input = "		#10/10/2010#";
			string expectedOutput = "		#10/10/2010#";
			
			TestTrimPreprocessorDirectives(input, expectedOutput);
		}
		
		[Test]
		public void TrimPreprocessorDirectives5()
		{
			string input = "Dim a = 12.0#  ";
			string expectedOutput = "Dim a = 12.0#  ";
			
			TestTrimPreprocessorDirectives(input, expectedOutput);
		}
		
		[Test]
		public void TrimPreprocessorDirectives6()
		{
			string input = "		#10/10/2010#  ";
			string expectedOutput = "		#10/10/2010#  ";
			
			TestTrimPreprocessorDirectives(input, expectedOutput);
		}
		
		void TestTrimPreprocessorDirectives(string input, string expectedOutput)
		{
			string output = LanguageUtils.TrimPreprocessorDirectives(input);
			Assert.AreEqual(expectedOutput, output);
		}
	}
}
