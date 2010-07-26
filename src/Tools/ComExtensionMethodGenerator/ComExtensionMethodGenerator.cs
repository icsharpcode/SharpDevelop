// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser.CSharp;
using ICSharpCode.NRefactory.PrettyPrinter;
using System.Reflection;

namespace ComExtensionMethodGenerator
{
	public class Program
	{
		static string header = 
				"// <file>" + "\r\n" + 
				"//     <copyright see=\"prj:///doc/copyright.txt\"/>" + "\r\n" + 
				"//     <license see=\"prj:///doc/license.txt\"/>" + "\r\n" + 
				"//     <owner name=\"David Srbecký\" email=\"dsrbecky@gmail.com\"/>" + "\r\n" + 
				"//     <version>$Revision$</version>" + "\r\n" + 
				"// </file>" + "\r\n" + "\r\n";
		
		static string path = Path.Combine(Assembly.GetExecutingAssembly().Location, @"..\..\..\..\..\..\src\AddIns\Debugger\Debugger.Core\Interop\");
		
		public static void Main(string[] args)
		{
			foreach(string name in new string[] { "CorDebug", "CorSym", "CorPublish" })  {
				new ComExtensionMethodGenerator() {
					InputFile = path + name + ".cs",
					OutputFile = path + name + "ExtensionMethods.generated.cs",
					Header = header,
					Namespace = "Debugger.Interop." + name,
					TypeName = name + "ExtensionMethods",
					MethodPrefix = "__",
				}.Generate();
			}
		}
	}
	
	public class ComExtensionMethodGenerator
	{
		public string InputFile;
		public string OutputFile;
		public string Header;
		public string Namespace;
		public string TypeName = "ExtensionMethods";
		public string ThisParameterName = "instance";
		public bool ProcessOutParameter = true;
		public string ProcessOutParameterMethodName = "ProcessOutParameter";
		public string[] ProcessOutParameterIgnores = new string[] { "System.Int32", "System.UInt32", "System.Int64", "System.UInt64", "System.Boolean", "Guid" };
		public bool ConvertOutParameterToReturn = true;
		public string ReturnValueName = "returnValue";
		public string MethodPrefix = string.Empty;
		
		public string Generate()
		{
			// Parse the source code
			IParser parser = ParserFactory.CreateParser(InputFile);
			parser.Parse();
			
			bool hasWarnings = false;
			
			// Prepare the output
			CompilationUnit generated = new CompilationUnit();
			if (Header != null)
				generated.AddChild(new IdentifierExpression(Header));
			foreach(UsingDeclaration usingDec in parser.CompilationUnit.Children.OfType<UsingDeclaration>()) {
				generated.AddChild(usingDec);
			}
			if (generated.Children.OfType<UsingDeclaration>().Count() > 0)
				generated.AddChild(new IdentifierExpression("\r\n"));
			TypeDeclaration extensionMethodsType = new TypeDeclaration(Modifiers.Public | Modifiers.Partial | Modifiers.Static, new List<AttributeSection>()) { Name = TypeName };
			generated.AddChild(
				Namespace == null ?
					(INode)extensionMethodsType :
					new NamespaceDeclaration(Namespace) { Children = extensionMethodsType.ToList<INode>() }
			);
			
//			// Add the ProcessOutParameter method
//			if (ProcessOutParameter) {
//				extensionMethodsType.AddChild(
//					new MethodDeclaration() {
//						Modifier = Modifiers.Static,
//						TypeReference = new TypeReference("void", true),
//						Name = ProcessOutParameterMethodName,
//						Parameters =
//							new ParameterDeclarationExpression(
//								new TypeReference("object", true),
//								"parameter"
//							).ToList(),
//						Body = new BlockStatement()
//					}
//				);
//				extensionMethodsType.AddChild(new IdentifierExpression("\t\t\r\n"));
//			}
				
			// Add the extesion methods
			foreach(NamespaceDeclaration ns in parser.CompilationUnit.Children.OfType<NamespaceDeclaration>()) {
				foreach(TypeDeclaration type in ns.Children.OfType<TypeDeclaration>()) {
					foreach(MethodDeclaration method in type.Children.OfType<MethodDeclaration>()) {
						MethodDeclaration extensionMethod = new MethodDeclaration();
						// Signature
						extensionMethod.Modifier = Modifiers.Public | Modifiers.Static;
						extensionMethod.TypeReference = method.TypeReference;
						extensionMethod.IsExtensionMethod = true;
						if (string.IsNullOrEmpty(MethodPrefix)) {
							extensionMethod.Name = method.Name;
						} else if (method.Name.StartsWith(MethodPrefix)) {
							extensionMethod.Name = method.Name.Substring(MethodPrefix.Length);
						} else {
							extensionMethod.Name = method.Name;
							Console.WriteLine("Warning: {0}.{1} is missing prefix {2}.", type.Name, method.Name, MethodPrefix);
							hasWarnings = true;
						}
						// HACK: GetType is used by System.Object
						if (extensionMethod.Name == "GetType")
							extensionMethod.Name = "GetTheType";
						// Parameters
						extensionMethod.Parameters.Add(new ParameterDeclarationExpression(new TypeReference(type.Name), ThisParameterName));
						foreach(ParameterDeclarationExpression param in method.Parameters) {
							ParameterDeclarationExpression newParam = new ParameterDeclarationExpression(param.TypeReference, param.ParameterName) { ParamModifier = param.ParamModifier };
							extensionMethod.Parameters.Add(newParam);
						}
						// Invocation
						extensionMethod.Body = new BlockStatement();
						InvocationExpression invoc = new InvocationExpression(
							new MemberReferenceExpression(new IdentifierExpression(ThisParameterName), method.Name)
						);
						// Generate arguments
						bool hasProcessOuts = false;
						foreach(ParameterDeclarationExpression param in method.Parameters) {
							// Add argument to invocation
							if (param.ParamModifier == ParameterModifiers.Ref) {
								invoc.Arguments.Add(new DirectionExpression(FieldDirection.Ref, new IdentifierExpression(param.ParameterName)));
							} else if (param.ParamModifier == ParameterModifiers.Out) {
								invoc.Arguments.Add(new DirectionExpression(FieldDirection.Out, new IdentifierExpression(param.ParameterName)));
							} else {
								invoc.Arguments.Add(new IdentifierExpression(param.ParameterName));
							}
							// Call ProcessOutParameter
							if (ProcessOutParameter) {
								if (param.ParamModifier == ParameterModifiers.Ref ||
								    param.ParamModifier == ParameterModifiers.Out ||
								    param.TypeReference.IsArrayType)
								{
									if (!ProcessOutParameterIgnores.Contains(param.TypeReference.Type)) {
										extensionMethod.Body.AddChild(
											new ExpressionStatement(
												new InvocationExpression(
													new IdentifierExpression(ProcessOutParameterMethodName),
													new IdentifierExpression(param.ParameterName).ToList<Expression>()
												)
											)
										);
										hasProcessOuts = true;
									}
								}
							}
						}
						// Process return value
						if (method.TypeReference.Type == typeof(void).FullName) {
							extensionMethod.Body.Children.Insert(0, new ExpressionStatement(invoc));
						} else {
							if ((!ProcessOutParameter || ProcessOutParameterIgnores.Contains(method.TypeReference.Type)) && !hasProcessOuts)
							{
								// Short version
								extensionMethod.Body.Children.Insert(0, new ReturnStatement(invoc));
							} else {
								// Declare and get return value
								extensionMethod.Body.Children.Insert(0,
									new LocalVariableDeclaration(
										new VariableDeclaration(ReturnValueName, invoc, method.TypeReference)
									)
								);
								// Call ProcessOutParameter
								if (method.TypeReference.Type != typeof(void).FullName && !ProcessOutParameterIgnores.Contains(method.TypeReference.Type)) {
									extensionMethod.Body.AddChild(
										new ExpressionStatement(
											new InvocationExpression(
												new IdentifierExpression(ProcessOutParameterMethodName),
												new IdentifierExpression(ReturnValueName).ToList<Expression>()
											)
										)
									);
								}
								// Return it
								extensionMethod.Body.AddChild(
									new ReturnStatement(new IdentifierExpression(ReturnValueName))
								);
							}
						}
						// Convert out parameter to return value
						if (ConvertOutParameterToReturn &&
						    method.TypeReference.Type == typeof(void).FullName &&
						    extensionMethod.Parameters.Count > 0 &&
						    extensionMethod.Parameters.Last().ParamModifier == ParameterModifiers.Out &&
						    extensionMethod.Parameters.Where(p => p.ParamModifier == ParameterModifiers.Out).Count() == 1)
						{
							ParameterDeclarationExpression param = extensionMethod.Parameters.Last();
							// Change signature
							extensionMethod.TypeReference = param.TypeReference;
							extensionMethod.Parameters.Remove(param);
							// Define local variable instead
							extensionMethod.Body.Children.Insert(
								0,
								new LocalVariableDeclaration(
									new VariableDeclaration(param.ParameterName, Expression.Null, param.TypeReference)
								)
							);
							// Return it
							extensionMethod.Body.AddChild(
								new ReturnStatement(new IdentifierExpression(param.ParameterName))
							);
						}
						extensionMethodsType.AddChild(extensionMethod);
						extensionMethodsType.AddChild(new IdentifierExpression("\t\t\r\n"));
					}
				}
			}
			
			// Pretty print
			CSharpOutputVisitor csOut = new CSharpOutputVisitor();
			csOut.VisitCompilationUnit(generated, null);
			string output = csOut.Text;
			
			// Save to file
			if (OutputFile != null) {
				File.WriteAllText(OutputFile, output);
			}
			
			if (hasWarnings) {
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey();
			}
			
			return output;
		}
	}
	
	static class ExtensionMethods
	{
		public static List<T> ToList<T>(this T obj)
		{
			List<T> list = new List<T>(1);
			list.Add(obj);
			return list;
		}
	}
}
