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

using CSharpBinding.Completion;
using CSharpBinding.Parser;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using NUnit.Framework;
using Rhino.Mocks;

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
	
	public abstract int ReadOnlyAbstractProperty { get; }
	public virtual int VirtualProperty { get; set; }
	public int NormalProperty { get; set; }
	public virtual int ProtectedWriteProperty { get; protected set; }
}
class DerivedClass : BaseClass {
	";
		string programEnd = "\n}";
		
		MockTextEditor textEditor;
		bool keyPressResult;
		
		static readonly IUnresolvedAssembly Corlib = new CecilLoader().LoadAssemblyFile(typeof(object).Assembly.Location);
		
		[SetUp]
		public void SetUp()
		{
			SD.InitializeForUnitTests();
			textEditor = new MockTextEditor();
			textEditor.Document.Text = programStart + "override " + programEnd;
			textEditor.Caret.Offset = programStart.Length + "override ".Length;
			var parseInfo = textEditor.CreateParseInformation();
			var pc = new CSharpProjectContent().AddOrUpdateFiles(parseInfo.UnresolvedFile);
			pc = pc.AddAssemblyReferences(new[] { Corlib });
			var compilation = pc.CreateCompilation();
			SD.Services.AddService(typeof(IParserService), MockRepository.GenerateStrictMock<IParserService>());
			SD.ParserService.Stub(p => p.GetCachedParseInformation(textEditor.FileName)).Return(parseInfo);
			SD.ParserService.Stub(p => p.GetCompilationForFile(textEditor.FileName)).Return(compilation);
			SD.ParserService.Stub(p => p.Parse(textEditor.FileName, textEditor.Document)).WhenCalled(
				i => {
					var syntaxTree = new CSharpParser().Parse(textEditor.Document, textEditor.FileName);
					i.ReturnValue = new CSharpFullParseInformation(syntaxTree.ToTypeSystem(), null, syntaxTree);
				});
			CSharpCompletionBinding completion = new CSharpCompletionBinding();
			keyPressResult = completion.HandleKeyPressed(textEditor, ' ');
		}
		
		[TearDown]
		public void TearDown()
		{
			SD.TearDownForUnitTests();
		}
		
		[Test]
		public void CheckKeyPressResult()
		{
			Assert.IsTrue(keyPressResult);
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
					"AbstractEvent", "AbstractMethod()",
					"Equals(object obj)", "GetHashCode()", "ProtectedWriteProperty",
					"ReadOnlyAbstractProperty", "ToString()",
					"VirtualEvent", "VirtualMethod()", "VirtualProperty",
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
		public void OverrideReadOnlyAbstractProperty()
		{
			CompletionContext context = new CompletionContext();
			context.Editor = textEditor;
			context.StartOffset = textEditor.Caret.Offset;
			context.EndOffset = textEditor.Caret.Offset;
			ICompletionItem item = textEditor.LastCompletionItemList.Items.First(i=>i.Text == "ReadOnlyAbstractProperty");
			textEditor.LastCompletionItemList.Complete(context, item);
			Assert.AreEqual(Normalize(
				programStart + "public override int ReadOnlyAbstractProperty {\n" +
				"    get {\n" +
				"      throw new NotImplementedException();\n" +
				"    }\n" +
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
				"    get {\n" +
				"      return base.VirtualProperty;\n" +
				"    }\n" +
				"    set {\n" +
				"      base.VirtualProperty = value;\n" +
				"    }\n" +
				"  }" + programEnd),
			                Normalize(textEditor.Document.Text)
			               );
		}
		
		[Test]
		public void OverrideProtectedWriteProperty()
		{
			CompletionContext context = new CompletionContext();
			context.Editor = textEditor;
			context.StartOffset = textEditor.Caret.Offset;
			context.EndOffset = textEditor.Caret.Offset;
			ICompletionItem item = textEditor.LastCompletionItemList.Items.First(i=>i.Text == "ProtectedWriteProperty");
			textEditor.LastCompletionItemList.Complete(context, item);
			Assert.AreEqual(Normalize(
				programStart + "public override int ProtectedWriteProperty {\n" +
				"    get {\n" +
				"      return base.ProtectedWriteProperty;\n" +
				"    }\n" +
				"    protected set {\n" +
				"      base.ProtectedWriteProperty = value;\n" +
				"    }\n" +
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
