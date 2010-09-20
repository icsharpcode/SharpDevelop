// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Editor;
using NUnit.Framework;

namespace CSharpBinding.Tests
{
	/// <summary>
	/// Tests code completion after the 'override' keyword
	/// </summary>
	[TestFixture]
	public class OverrideCompletionTests
	{
		string programStart = @"using System;
class BaseClass {
	public abstract void AbstractMethod();
	public virtual void VirtualMethod();
	public void NormalMethod();
	
	public abstract event EventHandler AbstractEvent;
	public virtual event EventHandler VirtualEvent;
	public event EventHandler NormalEvent;
	
	public abstract int AbstractProperty { get; set; }
	public virtual int VirtualProperty { get; set; }
	public int NormalProperty { get; set; }
}
class DerivedClass : BaseClass {
	";
		string programEnd = "\n}";
		
		MockTextEditor textEditor;
		CodeCompletionKeyPressResult keyPressResult;
		
		[SetUp]
		public void SetUp()
		{
			textEditor = new MockTextEditor();
			textEditor.Document.Text = programStart + "override" + programEnd;
			textEditor.Caret.Offset = programStart.Length + "override".Length;
			textEditor.CreateParseInformation();
			CSharpCompletionBinding completion = new CSharpCompletionBinding();
			keyPressResult = completion.HandleKeyPress(textEditor, ' ');
		}
		
		[Test]
		public void CheckKeyPressResult()
		{
			Assert.AreEqual(CodeCompletionKeyPressResult.Completed, keyPressResult);
		}
		
		[Test]
		public void CheckNoPreselection()
		{
			Assert.IsNotNull(textEditor.LastCompletionItemList);
			Assert.AreEqual(0, textEditor.LastCompletionItemList.PreselectionLength);
			Assert.IsNull(textEditor.LastCompletionItemList.SuggestedItem);
		}
		
		[Test]
		public void CheckListOfAvailableMethods()
		{
			var itemNames = textEditor.LastCompletionItemList.Items.Select(i=>i.Text).ToArray();
			Assert.AreEqual(
				new string[] {
					"AbstractEvent", "AbstractMethod()", "AbstractProperty",
					"Equals(obj : Object)", "GetHashCode()", "ToString()",
					"VirtualEvent", "VirtualMethod()", "VirtualProperty"
				}, itemNames);
		}
		
		string Normalize(string text)
		{
			return text.Replace("\t", "  ").Replace("\r", "").Trim();
		}
		
		[Test]
		public void OverrideAbstractMethod()
		{
			CompletionContext context = new CompletionContext();
			context.Editor = textEditor;
			context.StartOffset = textEditor.Caret.Offset;
			context.EndOffset = textEditor.Caret.Offset;
			ICompletionItem item = textEditor.LastCompletionItemList.Items.First(i=>i.Text == "AbstractMethod()");
			textEditor.LastCompletionItemList.Complete(context, item);
			Assert.AreEqual(Normalize(
				programStart + "public override void AbstractMethod()\n" +
				"  {\n" +
				"    throw new NotImplementedException();\n" +
				"  }" + programEnd),
				Normalize(textEditor.Document.Text)
			);
		}
		
		[Test]
		public void OverrideVirtualMethod()
		{
			CompletionContext context = new CompletionContext();
			context.Editor = textEditor;
			context.StartOffset = textEditor.Caret.Offset;
			context.EndOffset = textEditor.Caret.Offset;
			ICompletionItem item = textEditor.LastCompletionItemList.Items.First(i=>i.Text == "VirtualMethod()");
			textEditor.LastCompletionItemList.Complete(context, item);
			Assert.AreEqual(Normalize(
				programStart + "public override void VirtualMethod()\n" +
				"  {\n" +
				"    base.VirtualMethod();\n" +
				"  }" + programEnd),
				Normalize(textEditor.Document.Text)
			);
		}
		
		[Test]
		public void OverrideVirtualProperty()
		{
			CompletionContext context = new CompletionContext();
			context.Editor = textEditor;
			context.StartOffset = textEditor.Caret.Offset;
			context.EndOffset = textEditor.Caret.Offset;
			ICompletionItem item = textEditor.LastCompletionItemList.Items.First(i=>i.Text == "VirtualProperty");
			textEditor.LastCompletionItemList.Complete(context, item);
			Assert.AreEqual(Normalize(
				programStart + "public override int VirtualProperty {\n" +
				"    get { return base.VirtualProperty; }\n" +
				"    set { base.VirtualProperty = value; }\n" +
				"  }" + programEnd),
				Normalize(textEditor.Document.Text)
			);
		}
		
		[Test]
		public void OverrideVirtualEvent()
		{
			CompletionContext context = new CompletionContext();
			context.Editor = textEditor;
			context.StartOffset = textEditor.Caret.Offset;
			context.EndOffset = textEditor.Caret.Offset;
			ICompletionItem item = textEditor.LastCompletionItemList.Items.First(i=>i.Text == "VirtualEvent");
			textEditor.LastCompletionItemList.Complete(context, item);
			Assert.AreEqual(Normalize(
				programStart + "public override event EventHandler VirtualEvent;" + programEnd),
				Normalize(textEditor.Document.Text)
			);
		}
	}
}
