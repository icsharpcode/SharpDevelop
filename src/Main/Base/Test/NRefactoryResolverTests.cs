using System;
using System.IO;
using NUnit.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace DefaultNamespace.Tests
{
	[TestFixture]
	public class NRefactoryResolverTests
	{
		ICompilationUnit Parse(string fileName, string fileContent)
		{
			ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(ICSharpCode.NRefactory.Parser.SupportedLanguages.CSharp, new StringReader(fileContent));
			p.Parse();
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor();
			visitor.Visit(p.CompilationUnit, null);
			visitor.Cu.FileName = fileName;
			visitor.Cu.ErrorsDuringCompile = p.Errors.count > 0;
			visitor.Cu.Tag = p.CompilationUnit;
			
			return visitor.Cu;
		}
		
		ICompilationUnit ParseVB(string fileName, string fileContent)
		{
			ICSharpCode.NRefactory.Parser.IParser p = ICSharpCode.NRefactory.Parser.ParserFactory.CreateParser(ICSharpCode.NRefactory.Parser.SupportedLanguages.VBNet, new StringReader(fileContent));
			p.Parse();
			NRefactoryASTConvertVisitor visitor = new NRefactoryASTConvertVisitor();
			visitor.Visit(p.CompilationUnit, null);
			visitor.Cu.FileName = fileName;
			visitor.Cu.ErrorsDuringCompile = p.Errors.count > 0;
			visitor.Cu.Tag = p.CompilationUnit;
			
			return visitor.Cu;
		}
		
		DefaultParserService ParserService;
			
		[TestFixtureSetUp]
		public void Init()
		{
			ParserService = new DefaultParserService();
			ICSharpCode.Core.ServiceManager.Services.AddService(ParserService);
			
			foreach (Type type in typeof(System.String).Assembly.GetTypes()) {
				ParserService.AddClassToNamespaceList(new ReflectionClass(type, null));
			}
		}
//		
//		public static void Main(string[] args)
//		{
//			NRefactoryResolverTests test = new NRefactoryResolverTests();
//			test.Init();
//			test.OuterclassPrivateFieldResolveTest();
//				
//		}
		
		// Issue SD-291
		[Test]
		public void VBNetMultipleVariableDeclarationsTest()
		{
			string program = @"Class X
	Shared Sub Main
		Dim a, b As String
		
	End Sub
End Class
";
			ParserService.AddCompilationUnit(ParseVB("a.vb", program), 
			                                 "a.vb",
			                                 false);
			
			NRefactoryResolver resolover = new NRefactoryResolver(ICSharpCode.NRefactory.Parser.SupportedLanguages.VBNet);
			ResolveResult result = resolover.Resolve(ParserService,
			                                         "a",
			                                         4, 24,
			                                         "a.vb");
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Members.Count > 0);
			
		}
		
		// Issue SD-258
		[Test]
		public void VBNetForeachLoopVariableTest()
		{
			string program = @"Class Test
	Shared Sub Main()
		For Each c As String In MyColl
			
		Next 
	End Sub 
End Class
";
			ParserService.AddCompilationUnit(ParseVB("a.vb", program), 
			                                 "a.vb",
			                                 false);
			
			NRefactoryResolver resolover = new NRefactoryResolver(ICSharpCode.NRefactory.Parser.SupportedLanguages.VBNet);
			ResolveResult result = resolover.Resolve(ParserService,
			                                         "c",
			                                         4, 24,
			                                         "a.vb");
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Members.Count > 0);
			
		}
		
		
		
		// Issue SD-265
		[Test]
		public void VBNetStaticMembersonObjectTest()
		{
			string program = @"Class X
	Sub Z()
		Dim a As String
		
	End Sub
End Class";
			ParserService.AddCompilationUnit(ParseVB("a.vb", program), 
			                                 "a.vb",
			                                 false);
			
			NRefactoryResolver resolover = new NRefactoryResolver(ICSharpCode.NRefactory.Parser.SupportedLanguages.VBNet);
			ResolveResult result = resolover.Resolve(ParserService,
			                                         "a",
			                                         4, 24,
			                                         "a.vb");
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Members.Count > 0);
			
			IField field = null;
			foreach (object o in result.Members) {
				if (o is IField) {
					field = o as IField;
					break;
				}
			}
			Assert.IsNotNull(field);
			Assert.AreEqual(field.Name, "Empty");
		}
		
		// Issue SD-217
		[Test]
		public void VBNetLocalArrayLookupTest()
		{
			string program = @"Module Main
	Sub Main()
		Dim t As String()
		
	End Sub
End Module";
			ParserService.AddCompilationUnit(ParseVB("a.vb", program), 
			                                 "a.vb",
			                                 false);
			
			NRefactoryResolver resolover = new NRefactoryResolver(ICSharpCode.NRefactory.Parser.SupportedLanguages.VBNet);
			ResolveResult result = resolover.Resolve(ParserService,
			                                         "t(0)",
			                                         4, 24,
			                                         "a.vb");
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Members.Count > 0);
		}

		
		[Test]
		public void OuterclassPrivateFieldResolveTest()
		{
			string program = @"class A
{
	int myField;
	class B
	{
		void MyMethod(A a)
		{
		
		}
	}
}
";
			ParserService.AddCompilationUnit(Parse("a.cs", program), 
			                                 "a.cs",
			                                 false);
			
			NRefactoryResolver resolover = new NRefactoryResolver(ICSharpCode.NRefactory.Parser.SupportedLanguages.CSharp);
			ResolveResult result = resolover.Resolve(ParserService,
			                                         "a",
			                                         8, 24,
			                                         "a.cs");
			Assert.IsNotNull(result);
			Assert.IsTrue(result.Members.Count > 0);
			IField field = null;
			foreach (object o in result.Members) {
				if (o is IField) {
					field = o as IField;
					break;
				}
			}
			Assert.IsNotNull(field);
			Assert.AreEqual(field.Name, "myField");
		}
	}
}
