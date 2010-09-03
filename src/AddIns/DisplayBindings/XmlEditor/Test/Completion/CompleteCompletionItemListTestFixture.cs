// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.XmlEditor;
using NUnit.Framework;

namespace XmlEditor.Tests.Completion
{
	[TestFixture]
	public class CompleteCompletionItemListTestFixture : ICompletionItem
	{
		CompletionContext contextPassedToCompleteMethod;
		
		[Test]
		public void ListCompleteMethodCallsCompletionItemCompleteMethod()
		{
			CompletionContext context = new CompletionContext();
			XmlCompletionItemCollection completionItems = new XmlCompletionItemCollection();
			completionItems.Complete(context, this);
			
			Assert.AreSame(contextPassedToCompleteMethod, context);
		}
		
		void ICompletionItem.Complete(CompletionContext context)
		{
			contextPassedToCompleteMethod = context;
		}		
		
		string ICompletionItem.Text {
			get { return String.Empty; }
		}
		
		string ICompletionItem.Description {
			get { return String.Empty; }
		}
		
		ICSharpCode.SharpDevelop.IImage ICompletionItem.Image {
			get { return null; }
		}
		
		double ICompletionItem.Priority {
			get { return 0; }
		}
	}
}
