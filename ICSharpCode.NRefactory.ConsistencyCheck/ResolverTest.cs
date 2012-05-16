// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.ConsistencyCheck
{
	/// <summary>
	/// Validates that no compile errors are found in valid code.
	/// </summary>
	public class ResolverTest
	{
		public static void RunTest(CSharpFile file)
		{
			CSharpAstResolver resolver = new CSharpAstResolver(file.Project.Compilation, file.CompilationUnit, file.ParsedFile);
			var navigator = new ValidatingResolveAllNavigator(file.FileName);
			resolver.ApplyNavigator(navigator, CancellationToken.None);
			navigator.Validate(resolver, file.CompilationUnit);
		}
		
		class ValidatingResolveAllNavigator : IResolveVisitorNavigator
		{
			string fileName;
			bool allowErrors;
			
			public ValidatingResolveAllNavigator(string fileName)
			{
				this.fileName = fileName;
				// We allow errors in XAML codebehind because we're currently not adding the XAML-generated
				// members to the type system.
				this.allowErrors = (fileName.Contains(".xaml") || File.Exists(Path.ChangeExtension(fileName, ".xaml")) || fileName.EndsWith("AvalonDockLayout.cs") || fileName.EndsWith("ResourcesFileTreeNode.cs") || fileName.EndsWith("ChangeMarkerMargin.cs"));
			}
			
			Dictionary<AstNode, ResolveResult> resolvedNodes = new Dictionary<AstNode, ResolveResult>();
			HashSet<AstNode> nodesWithConversions = new HashSet<AstNode>();
			
			public ResolveVisitorNavigationMode Scan(AstNode node)
			{
				return ResolveVisitorNavigationMode.Resolve;
			}
			
			public virtual void Resolved(AstNode node, ResolveResult result)
			{
				if (resolvedNodes.ContainsKey(node))
					throw new InvalidOperationException("Duplicate Resolved() call");
				resolvedNodes.Add(node, result);
				if (CSharpAstResolver.IsUnresolvableNode(node))
					throw new InvalidOperationException("Resolved unresolvable node");
				
				if (result.IsError && !allowErrors) {
					Console.WriteLine("Compiler error at " + fileName + ":" + node.StartLocation + ": " + result);
				}
			}
			
			public virtual void ProcessConversion(Expression expression, ResolveResult result, Conversion conversion, IType targetType)
			{
				if (!nodesWithConversions.Add(expression))
					throw new InvalidOperationException("Duplicate ProcessConversion() call");
				if (!conversion.IsValid && !allowErrors) {
					Console.WriteLine("Compiler error at " + fileName + ":" + expression.StartLocation + ": Cannot convert from " + result + " to " + targetType);
				}
			}
			
			public virtual void Validate(CSharpAstResolver resolver, CompilationUnit cu)
			{
				foreach (AstNode node in cu.DescendantsAndSelf.Except(resolvedNodes.Keys)) {
					if (!CSharpAstResolver.IsUnresolvableNode(node)) {
						Console.WriteLine("Forgot to resolve " + node);
					}
				}
				foreach (var pair in resolvedNodes) {
					if (resolver.Resolve(pair.Key) != pair.Value)
						throw new InvalidOperationException("Inconsistent result");
				}
			}
		}
		
		public static void RunTestWithoutParsedFile(CSharpFile file)
		{
			CSharpAstResolver originalResolver = new CSharpAstResolver(file.Project.Compilation, file.CompilationUnit, file.ParsedFile);
			originalResolver.ApplyNavigator(new ValidatingResolveAllNavigator(file.FileName), CancellationToken.None);
			CSharpAstResolver resolver = new CSharpAstResolver(file.Project.Compilation, file.CompilationUnit);
			var navigator = new ComparingResolveAllNavigator(originalResolver);
			resolver.ApplyNavigator(navigator, CancellationToken.None);
			navigator.Validate(resolver, file.CompilationUnit);
		}
		
		sealed class ComparingResolveAllNavigator : ValidatingResolveAllNavigator
		{
			readonly CSharpAstResolver originalResolver;
			
			public ComparingResolveAllNavigator(CSharpAstResolver originalResolver)
				: base(originalResolver.ParsedFile.FileName)
			{
				this.originalResolver = originalResolver;
			}
			
			public override void Resolved(AstNode node, ResolveResult result)
			{
				base.Resolved(node, result);
				ResolveResult originalResult = originalResolver.Resolve(node);
				if (!RandomizedOrderResolverTest.IsEqualResolveResult(originalResult, result)) {
					Console.WriteLine("Compiler error at " + node.GetRegion().FileName + ":" + node.StartLocation + ": Should be " + originalResult + " but was " + result);
				}
			}
		}
	}
}
