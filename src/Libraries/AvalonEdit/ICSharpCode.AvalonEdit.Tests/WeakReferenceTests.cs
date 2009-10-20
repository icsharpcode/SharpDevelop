// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using NUnit.Framework;

namespace ICSharpCode.AvalonEdit
{
	[TestFixture]
	public class WeakReferenceTests
	{
		[Test]
		public void GCCallbackTest()
		{
			bool collectedTextView = false;
			TextView textView = new TextViewWithGCCallback(delegate { collectedTextView = true; });
			textView = null;
			GarbageCollect();
			Assert.IsTrue(collectedTextView);
		}
		
		[Test]
		public void DocumentDoesNotHoldReferenceToTextView()
		{
			bool collectedTextView = false;
			TextDocument textDocument = new TextDocument();
			Assert.AreEqual(0, textDocument.LineTrackers.Count);
			
			TextView textView = new TextViewWithGCCallback(delegate { collectedTextView = true; });
			textView.Document = textDocument;
			Assert.AreEqual(1, textDocument.LineTrackers.Count);
			textView = null;
			
			GarbageCollect();
			Assert.IsTrue(collectedTextView);
			// document cannot immediately clear the line tracker
			Assert.AreEqual(1, textDocument.LineTrackers.Count);
			
			// but it should clear it on the next change
			textDocument.Insert(0, "a");
			Assert.AreEqual(0, textDocument.LineTrackers.Count);
		}
		
		[Test]
		[Ignore(".NET 4.0 Beta 2 seems to cause trouble here. With the same AvalonEdit binary, " +
		        "this test fails depending on whether it runs in NUnit on .NET 3.5 or .NET 4.0.")]
		public void DocumentDoesNotHoldReferenceToTextArea()
		{
			bool collectedTextArea = false;
			TextDocument textDocument = new TextDocument();
			
			TextArea textArea = new TextAreaWithGCCallback(delegate { collectedTextArea = true; });
			textArea.Document = textDocument;
			textArea = null;
			
			GarbageCollect();
			Assert.IsTrue(collectedTextArea);
			GC.KeepAlive(textDocument);
		}
		
		[Test]
		public void DocumentDoesNotHoldReferenceToTextEditor()
		{
			bool collectedTextEditor = false;
			TextDocument textDocument = new TextDocument();
			
			TextEditor textEditor = new TextEditorWithGCCallback(delegate { collectedTextEditor = true; });
			textEditor.Document = textDocument;
			textEditor = null;
			
			GarbageCollect();
			Assert.IsTrue(collectedTextEditor);
			GC.KeepAlive(textDocument);
		}
		
		[Test]
		public void DocumentDoesNotHoldReferenceToLineMargin()
		{
			bool collectedTextView = false;
			TextDocument textDocument = new TextDocument();
			
			DocumentDoesNotHoldReferenceToLineMargin_CreateMargin(textDocument, delegate { collectedTextView = true; });
			
			GarbageCollect();
			Assert.IsTrue(collectedTextView);
			GC.KeepAlive(textDocument);
		}
		
		// using a method to ensure the local variables can be garbage collected after the method returns
		void DocumentDoesNotHoldReferenceToLineMargin_CreateMargin(TextDocument textDocument, Action finalizeAction)
		{
			TextView textView = new TextViewWithGCCallback(finalizeAction) {
				Document = textDocument
			};
			LineNumberMargin margin = new LineNumberMargin() {
				TextView = textView
			};
		}
		
		static void GarbageCollect()
		{
			GC.WaitForPendingFinalizers();
			GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
			GC.WaitForPendingFinalizers();
		}
		
		sealed class TextViewWithGCCallback : TextView
		{
			Action onFinalize;
			
			public TextViewWithGCCallback(Action onFinalize)
			{
				this.onFinalize = onFinalize;
			}
			
			~TextViewWithGCCallback()
			{
				onFinalize();
			}
		}
		
		sealed class TextAreaWithGCCallback : TextArea
		{
			Action onFinalize;
			
			public TextAreaWithGCCallback(Action onFinalize)
			{
				this.onFinalize = onFinalize;
			}
			
			~TextAreaWithGCCallback()
			{
				onFinalize();
			}
		}
		
		sealed class TextEditorWithGCCallback : TextEditor
		{
			Action onFinalize;
			
			public TextEditorWithGCCallback(Action onFinalize)
			{
				this.onFinalize = onFinalize;
			}
			
			~TextEditorWithGCCallback()
			{
				onFinalize();
			}
		}
	}
}
