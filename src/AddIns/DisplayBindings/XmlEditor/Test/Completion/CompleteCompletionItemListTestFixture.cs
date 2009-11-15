// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
