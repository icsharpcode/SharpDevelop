// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
using NUnit.Framework;

namespace PythonBinding.Tests.Console
{
	[TestFixture]
	public class EmptyStringCodeCompletionTestFixture
	{
		MockMemberProvider memberProvider;
		PythonConsoleCompletionDataProvider completionProvider;
		
		[SetUp]
		public void Init()
		{
			memberProvider = new MockMemberProvider();
			completionProvider = new PythonConsoleCompletionDataProvider(memberProvider);
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
		public void ImageIndexIsMethod()
		{
			memberProvider.SetMemberNames(new string[] {"a"});
			
			ICompletionData[] items = completionProvider.GenerateCompletionData(">>> a");
			Assert.AreEqual(ClassBrowserIconService.MethodIndex, items[0].ImageIndex);
		}
		
		[Test]
		public void UnderscoresPassedToGetMemberNames()
		{
			completionProvider.GenerateCompletionData(">>> __builtins__");
			Assert.AreEqual("__builtins__", memberProvider.GetMemberNamesParameter);
		}
	}
}
