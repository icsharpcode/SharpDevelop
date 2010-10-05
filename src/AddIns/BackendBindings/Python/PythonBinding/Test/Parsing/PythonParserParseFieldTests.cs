// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Parsing
{
	[TestFixture]
	public class PythonParserParseFieldTests
	{
		IClass myClass;
		
		void ParseCode(string code)
		{	
			ParseInformation parseInfo = PythonParserHelper.CreateParseInfo(code);
			myClass = parseInfo.CompilationUnit.Classes[0];
		}
		
		[Test]
		public void Parse_ClassHasOneFieldCalledCount_ReturnsParseInfoWithClassWithFieldCalledCount()
		{
			string code =
				"class MyClass:\r\n" +
				"    def __init__(self):\r\n" +
        		"        self._count = 0\r\n" +
 				"\r\n";
			
			ParseCode(code);
			IField field = myClass.Fields[0];
			string name = field.Name;			
			string expectedName = "_count";
			
			Assert.AreEqual(expectedName, name);
		}
		
		[Test]
		public void Parse_ClassFieldInitialisedTwice_ReturnsParseInfoWithClassWithOnlyOneField()
		{
			string code =
				"class MyClass:\r\n" +
				"    def __init__(self):\r\n" +
        		"        self._count = 0\r\n" +
				"        self._count = 3\r\n" +
 				"\r\n";
			
			ParseCode(code);
			int howManyFields = myClass.Fields.Count;
			int expectedNumberOfFields = 1;
			
			Assert.AreEqual(expectedNumberOfFields, howManyFields);
		}
	}
}
