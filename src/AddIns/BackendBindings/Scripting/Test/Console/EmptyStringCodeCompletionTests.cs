// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
