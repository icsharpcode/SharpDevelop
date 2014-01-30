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
using CSharpBinding.Refactoring;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using System;
using System.Linq;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;
using NUnit.Framework;
using Rhino.Mocks;

namespace CSharpBinding.Tests
{
	[TestFixture]
	public class CSharpCodeGeneratorTests
	{
		string program = @"using System;
using System.Reflection;

[assembly: AssemblyTitle(""CSharpBinding.Tests"")]

class MySimpleAttribute : Attribute {}

class TargetClass
{
	void TargetA()
	{
	}
	
	public int TargetB { get; set; }
	
	public int TargetC_ {
		get {
			return 0;
		}
	}
}

interface TargetInterface
{
}
";
		
		MockTextEditor textEditor;
		IProject project;
		CSharpCodeGenerator gen;
		
		static readonly IUnresolvedAssembly Corlib = new CecilLoader().LoadAssemblyFile(typeof(object).Assembly.Location);
		
		[SetUp]
		public void SetUp()
		{
			SD.InitializeForUnitTests();
			textEditor = new MockTextEditor();
			textEditor.Document.Text = program;
			var parseInfo = textEditor.CreateParseInformation();
			this.project = MockRepository.GenerateStrictMock<IProject>();
			var pc = new CSharpProjectContent().AddOrUpdateFiles(parseInfo.UnresolvedFile);
			pc = pc.AddAssemblyReferences(new[] { Corlib });
			var compilation = pc.CreateCompilation();
			SD.Services.AddService(typeof(IParserService), MockRepository.GenerateStrictMock<IParserService>());
			
			SD.ParserService.Stub(p => p.GetCachedParseInformation(textEditor.FileName)).Return(parseInfo);
			SD.ParserService.Stub(p => p.GetCompilation(project)).Return(compilation);
			SD.ParserService.Stub(p => p.GetCompilationForFile(textEditor.FileName)).Return(compilation);
			SD.ParserService.Stub(p => p.Parse(textEditor.FileName, textEditor.Document)).WhenCalled(
				i => {
					var syntaxTree = new CSharpParser().Parse(textEditor.Document, textEditor.FileName);
					i.ReturnValue = new CSharpFullParseInformation(syntaxTree.ToTypeSystem(), null, syntaxTree);
				}).Return(parseInfo); // fake Return to make it work
			SD.Services.AddService(typeof(IFileService), MockRepository.GenerateStrictMock<IFileService>());
			IViewContent view = MockRepository.GenerateStrictMock<IViewContent>();
			view.Stub(v => v.GetService(typeof(ITextEditor))).Return(textEditor);
			SD.FileService.Stub(f => f.OpenFile(textEditor.FileName, false)).Return(view);
			gen = new CSharpCodeGenerator();
		}
		
		[TearDown]
		public void TearDown()
		{
			SD.TearDownForUnitTests();
		}
		
		[Test]
		public void AddSimpleAttributeToClass()
		{
			var compilation = SD.ParserService.GetCompilationForFile(textEditor.FileName);
			var entity = FindEntity<ITypeDefinition>("TargetClass");
			var attribute = compilation.FindType(new FullTypeName("MySimpleAttribute"));
			gen.AddAttribute(entity, new DefaultAttribute(attribute));
			
			Assert.AreEqual(@"using System;
using System.Reflection;

[assembly: AssemblyTitle(""CSharpBinding.Tests"")]

class MySimpleAttribute : Attribute {}

[MySimple]
class TargetClass
{
	void TargetA()
	{
	}
	
	public int TargetB { get; set; }
	
	public int TargetC_ {
		get {
			return 0;
		}
	}
}

interface TargetInterface
{
}
", textEditor.Document.Text);
		}
		
		[Test]
		public void AddSimpleAttributeToMethod()
		{
			var compilation = SD.ParserService.GetCompilationForFile(textEditor.FileName);
			var entity = FindEntity<IMethod>("TargetA");
			var attribute = compilation.FindType(new FullTypeName("MySimpleAttribute"));
			gen.AddAttribute(entity, new DefaultAttribute(attribute));
			
			Assert.AreEqual(@"using System;
using System.Reflection;

[assembly: AssemblyTitle(""CSharpBinding.Tests"")]

class MySimpleAttribute : Attribute {}

class TargetClass
{
	[MySimple]
	void TargetA()
	{
	}
	
	public int TargetB { get; set; }
	
	public int TargetC_ {
		get {
			return 0;
		}
	}
}

interface TargetInterface
{
}
", textEditor.Document.Text);
		}
		
		[Test]
		public void AddSimpleAttributeToInterface()
		{
			var compilation = SD.ParserService.GetCompilationForFile(textEditor.FileName);
			var entity = FindEntity<ITypeDefinition>("TargetInterface");
			var attribute = compilation.FindType(new FullTypeName("MySimpleAttribute"));
			gen.AddAttribute(entity, new DefaultAttribute(attribute));
			
			Assert.AreEqual(@"using System;
using System.Reflection;

[assembly: AssemblyTitle(""CSharpBinding.Tests"")]

class MySimpleAttribute : Attribute {}

class TargetClass
{
	void TargetA()
	{
	}
	
	public int TargetB { get; set; }
	
	public int TargetC_ {
		get {
			return 0;
		}
	}
}

[MySimple]
interface TargetInterface
{
}
", textEditor.Document.Text);
		}
		
		[Test]
		public void AddSimpleAttributeToProperty()
		{
			var compilation = SD.ParserService.GetCompilationForFile(textEditor.FileName);
			var entity = FindEntity<IProperty>("TargetB");
			var attribute = compilation.FindType(new FullTypeName("MySimpleAttribute"));
			gen.AddAttribute(entity, new DefaultAttribute(attribute));
			
			Assert.AreEqual(@"using System;
using System.Reflection;

[assembly: AssemblyTitle(""CSharpBinding.Tests"")]

class MySimpleAttribute : Attribute {}

class TargetClass
{
	void TargetA()
	{
	}
	
	[MySimple]
	public int TargetB { get; set; }
	
	public int TargetC_ {
		get {
			return 0;
		}
	}
}

interface TargetInterface
{
}
", textEditor.Document.Text);
		}
		
		[Test]
		public void AddSimpleAttributeToPropertyGetter()
		{
			var compilation = SD.ParserService.GetCompilationForFile(textEditor.FileName);
			var entity = FindEntity<IProperty>("TargetC_");
			Assert.IsTrue(entity.CanGet);
			var attribute = compilation.FindType(new FullTypeName("MySimpleAttribute"));
			gen.AddAttribute(entity.Getter, new DefaultAttribute(attribute));
			
			Assert.AreEqual(@"using System;
using System.Reflection;

[assembly: AssemblyTitle(""CSharpBinding.Tests"")]

class MySimpleAttribute : Attribute {}

class TargetClass
{
	void TargetA()
	{
	}
	
	public int TargetB { get; set; }
	
	public int TargetC_ {
		[MySimple]
		get {
			return 0;
		}
	}
}

interface TargetInterface
{
}
", textEditor.Document.Text);
		}
		
		[Test]
		[Ignore("The C# parser provides empty location info for AttributeSections.")]
		public void AddSimpleAssemblyAttribute()
		{
			var compilation = SD.ParserService.GetCompilationForFile(textEditor.FileName);
			var attribute = compilation.FindType(new FullTypeName("MySimpleAttribute"));
			gen.AddAssemblyAttribute(project, new DefaultAttribute(attribute));
			
			Assert.AreEqual(@"using System;
using System.Reflection;

[assembly: MySimple]
[assembly: AssemblyTitle(""CSharpBinding.Tests"")]

class MySimpleAttribute : Attribute {}

[MySimple]
class TargetClass
{
	void TargetA()
	{
	}
	
	public int TargetB { get; set; }
	
	public int TargetC_ {
		get {
			return 0;
		}
	}
}

interface TargetInterface
{
}
", textEditor.Document.Text);
		}
		
		
		T FindEntity<T>(string targetClass) where T : IEntity
		{
			var compilation = SD.ParserService.GetCompilationForFile(textEditor.FileName);
			var parseInfo = SD.ParserService.Parse(textEditor.FileName, textEditor.Document);
			
			int i = textEditor.Document.IndexOf(targetClass, 0, textEditor.Document.TextLength, StringComparison.Ordinal);
			Assert.Greater(i, -1);
			TextLocation location = textEditor.Document.GetLocation(i);
			var member = parseInfo.UnresolvedFile.GetMember(location);
			var type = parseInfo.UnresolvedFile.GetInnermostTypeDefinition(location);
			
			var context = new SimpleTypeResolveContext(compilation.MainAssembly);
			var rt = type.Resolve(context).GetDefinition();
			
			if (member != null) {
				return (T)member.CreateResolved(context.WithCurrentTypeDefinition(rt));
			}
			
			return (T)rt;
		}
	}
}
