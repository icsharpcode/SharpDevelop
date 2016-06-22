// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	public class EmptyStringCodeCompletionTests
	{
		MockMemberProvider memberProvider;
		ScriptingConsoleCompletionDataProvider completionProvider;
		
		[SetUp]
		public void Init()
		{
			memberProvider = new MockMemberProvider();
			completionProvider = new ScriptingConsoleCompletionDataProvider(memberProvider);
		}
		
		/// <summary>
		/// If the user presses the dot character without having any text in the command line then
		/// a SyntaxException occurs if the code calls IronPython's CommandLine.GetMemberNames. So this
		/// tests ensures that if the string is empty then this method is not called.
		/// </summary>
		[Test]
		public void NoCompletionItemsGeneratedForEmptyString()
		{
			memberProvider.SetMemberNames(new string[] {"a"});
			memberProvider.SetGlobals(new string[] {"a"});
			
			Assert.AreEqual(0, completionProvider.GenerateCompletionData(">>> ").Length);
		}
		
		/// <summary>
		/// Checks that the GenerateCompletionData method catches any exceptions thrown by the
		/// IMemberProvider implementation. This can occur when an invalid name is passed to 
		/// IronPython's CommandLine.GetMemberNames or GetGlobals. For example, an UnboundNameException is
		/// thrown if an unknown name is used.
		/// </summary>
		[Test]
		public void NoCompletionItemsGeneratedWhenExceptionThrown()
		{
			memberProvider.ExceptionToThrow = new ApplicationException("Should not be thrown");
			
			Assert.AreEqual(0, completionProvider.GenerateCompletionData(">>> a").Length);
		}
		
		[Test]
		public void UnderscoresPassedToGetMemberNames()
		{
			completionProvider.GenerateCompletionData(">>> __builtins__");
			Assert.AreEqual("__builtins__", memberProvider.GetMemberNamesParameter);
		}
	}
}
