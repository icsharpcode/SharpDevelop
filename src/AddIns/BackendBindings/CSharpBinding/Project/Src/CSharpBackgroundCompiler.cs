// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Util;
using Ast = ICSharpCode.NRefactory.Ast;
using Compiler = System.CodeDom.Compiler;
using Util = Microsoft.Build.Utilities;

namespace CSharpBinding
{
	/// <summary>
	/// Description of CSharpBackgroundCompiler.
	/// </summary>
	public class CSharpBackgroundCompiler
	{
		static WorkerThread worker;
		static Thread thread;
		static IAsyncResult currentWork;
		static bool init;
		
		public static void Init()
		{
			if (init)
				return;
			
			init = true;
			worker = new WorkerThread();
			
			thread = new Thread(
				delegate() {
					LoggingService.Info("start background compiler");
					worker.RunLoop();
				}
			);
			
			thread.IsBackground = true;
			thread.Name = "CSBackgroundCompiler";
			
			ParserService.ParserUpdateStepFinished += delegate {
				if (WorkbenchSingleton.Workbench.ActiveViewContent == null)
					return;
				
				if (ParserService.LoadSolutionProjectsThreadRunning)
					return;
				
				ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
				
				if (provider == null)
					return;
				
				ParseInformation parseInfo = ParserService.GetExistingParseInformation(provider.TextEditor.FileName);
				
				if (parseInfo == null)
					return;
				
				string fileName = provider.TextEditor.FileName;
				string fileContent = provider.TextEditor.Document.Text;
				IProjectContent pc = parseInfo.CompilationUnit.ProjectContent;
				
				if (currentWork == null)
					thread.Start();
				
				if (currentWork == null || currentWork.IsCompleted)
					currentWork = worker.Enqueue(() => RunCompile(fileName, fileContent, pc));
			};
		}
		
		static void RunCompile(string openedFileName, string openedFileContent, IProjectContent pc)
		{

			var codeProvider = pc.Language.CodeDomProvider;
			
			if (codeProvider == null)
				return;
			
			string fileContent1 = CreateLineMarker(1, openedFileName) + "\n" + openedFileContent;
			
			StringWriter writer = new StringWriter();
			Stopwatch watch = new Stopwatch();
			watch.Start();
			WriteClasses(pc, writer, openedFileName);
			watch.Stop();
			Console.WriteLine("WriteClasses for {0} took {1}ms", (pc.Project as IProject).Name, watch.ElapsedMilliseconds);
			watch.Restart();
			Compiler.CompilerResults cr = codeProvider.CompileAssemblyFromSource(GetParameters(pc.Project as CompilableProject), fileContent1, writer.ToString());
			watch.Stop();
			writer.Close();
			var errors = cr.Errors.OfType<Compiler.CompilerError>().ToArray();
			Console.WriteLine("CompileFile for {0} took {1}ms", (pc.Project as IProject).Name, watch.ElapsedMilliseconds);

			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate() {
					List<Task> tasks = new List<Task>();
					
					foreach (Compiler.CompilerError e in errors) {
						tasks.Add(new Task(string.IsNullOrEmpty(e.FileName) ? null : new FileName(e.FileName), e.ErrorText + " (" + e.ErrorNumber + ")", e.Column, e.Line, e.IsWarning ? TaskType.Warning : TaskType.Error));
					}
					
					TaskService.ClearExceptCommentTasks();
					
					TaskService.AddRange(tasks);
				}
			);
		}
		
		static Compiler.CompilerParameters GetParameters(CompilableProject project)
		{
			Compiler.CompilerParameters p = new Compiler.CompilerParameters();
			
			var items = project.GetItemsOfType(ItemType.Reference)
				.OfType<ReferenceProjectItem>()
				.Select(item => item.FileName)
				.Concat(
					project.GetItemsOfType(ItemType.ProjectReference)
					.OfType<ProjectReferenceProjectItem>()
					.Select(item => item.ReferencedProject.OutputAssemblyFullPath)
				);
			
			
			TargetFramework tf = project.CurrentTargetFramework;

			if (tf == null)
				throw new InvalidOperationException();
			
			p.CompilerOptions = "/nostdlib /unsafe";
			
			p.GenerateInMemory = true;
			p.ReferencedAssemblies.AddRange(items.ToArray());
			string mscorlib = GetMscorlibPath(tf);
			if (mscorlib != null)
				p.ReferencedAssemblies.Add(mscorlib);
			if (IsGreaterThan(Util.TargetDotNetFrameworkVersion.Version35, VersionForString(tf.Name)) &&
			    !p.ReferencedAssemblies.OfType<string>().Any(s => s.Contains("System.Core.dll")))
				p.ReferencedAssemblies.Add("System.Core.dll");
			
			return p;
		}
		
		static string CreateLineMarker(int line, string filename)
		{
			return string.Format("#line {0} \"{1}\"", line, filename.Replace("\\", "\\\\"));
		}
		
		static string GetMscorlibPath(TargetFramework tf)
		{
			while (tf != null) {
				string path = Path.Combine(Util.ToolLocationHelper.GetPathToDotNetFramework(VersionForString(tf.Name)), "mscorlib.dll");
				if (File.Exists(path))
					return path;
				
				tf = tf.BasedOn;
			}
			
			return null;
		}
		
		static Util.TargetDotNetFrameworkVersion VersionForString(string name)
		{
			switch (name) {
				case "v1.1":
					return Util.TargetDotNetFrameworkVersion.Version11;
				case "v2.0":
					return Util.TargetDotNetFrameworkVersion.Version20;
				case "v3.0":
					return Util.TargetDotNetFrameworkVersion.Version30;
				case "v3.5":
					return Util.TargetDotNetFrameworkVersion.Version35;
				case "v4.0":
					return Util.TargetDotNetFrameworkVersion.Version40;
			}
			
			return Util.TargetDotNetFrameworkVersion.VersionLatest;
		}
		
		static void WriteClasses(IProjectContent content, TextWriter writer, string excludeFilename)
		{
			writer.WriteLine("#pragma warning disable");
			writer.WriteLine("using System;");
			
			foreach (IClass c in content.Classes) {
				CompoundClass cc = c as CompoundClass;
				if (cc != null) {
					foreach (IClass part in cc.Parts)
						WriteClass(part, cc, content.Project as IProject, writer, excludeFilename);
				} else {
					WriteClass(c, null, content.Project as IProject, writer, excludeFilename);
				}
			}
			
			writer.Write("\n#pragma warning restore\n");
		}
		
		static string[] allowedAttributes = {
			"System.ObsoleteAttribute",
			"System.Diagnostics.ConditionalAttribute",
			"System.CLSCompliantAttribute",
			"System.Runtime.CompilerServices.ExtensionAttribute" // required for extension methods
		};
		
		static void WriteClass(IClass c, IClass compound, IProject project, TextWriter writer, string excludeFilename)
		{
			if (FileUtility.IsEqualFileName(excludeFilename, c.CompilationUnit.FileName))
				return;
			if (project != null) {
				FileProjectItem file = project.FindFile(c.CompilationUnit.FileName);
				if (file == null || file.BuildAction != "Compile")
					return;
			}
			Ast.AttributedNode type = CodeGenerator.ConvertClass(c, null);
			
			CleanType(type);
			
			if (compound != null)
				type.Modifier = CodeGenerator.ConvertModifier(compound.Modifiers, null);
			
			CSharpOutputVisitor output = new CSharpOutputVisitor();
			type.AcceptVisitor(output, null);
			// TODO : add fine grained mappings of files/lines
			writer.WriteLine(CreateLineMarker(c.Region.BeginLine - 1, c.CompilationUnit.FileName));
			if (!string.IsNullOrEmpty(c.Namespace)) {
				writer.WriteLine("namespace " + c.Namespace + "{");
				writer.Write(output.Text);
				writer.WriteLine("}");
			} else {
				writer.Write(output.Text);
			}
		}

		static void CleanType(Ast.AttributedNode type)
		{
			foreach (Ast.AttributeSection section in type.Attributes) {
				section.Attributes.RemoveAll(a => !allowedAttributes.Contains(a.Name));
			}
			type.Attributes.RemoveAll(s => !s.Attributes.Any());
			foreach (Ast.MethodDeclaration node in type.Children.OfType<Ast.MethodDeclaration>()) {
				if (node.Body != null) {
					if (IsExternAllowed(type, node))
						node.Modifier |= Ast.Modifiers.Extern;
					if (!NeedsBody(type, node))
						node.Body = null;
				}
			}
			foreach (Ast.ConstructorDeclaration node in type.Children.OfType<Ast.ConstructorDeclaration>()) {
				if (IsExternAllowed(type, node))
					node.Modifier |= Ast.Modifiers.Extern;
				if (!NeedsBody(type, node))
					node.Body = null;
			}
			foreach (Ast.DestructorDeclaration node in type.Children.OfType<Ast.DestructorDeclaration>()) {
				if (IsExternAllowed(type, node))
					node.Modifier |= Ast.Modifiers.Extern;
				if (!NeedsBody(type, node))
					node.Body = null;
			}
			foreach (Ast.PropertyDeclaration node in type.Children.OfType<Ast.PropertyDeclaration>()) {
				if (IsExternAllowed(type, node))
					node.Modifier |= Ast.Modifiers.Extern;
				if (node.HasGetRegion && !NeedsBody(type, node))
					node.GetRegion.Block = null;
				if (node.HasSetRegion && !NeedsBody(type, node))
					node.SetRegion.Block = null;
			}
			
			foreach (Ast.AttributedNode node in type.Children.OfType<Ast.AttributedNode>())
				CleanType(node);
		}
		
		static bool IsExternAllowed(Ast.AttributedNode type, Ast.AttributedNode member)
		{
			if (type is Ast.TypeDeclaration) {
				var decl = type as Ast.TypeDeclaration;
				if (decl.Type == Ast.ClassType.Interface || (decl.Type == Ast.ClassType.Struct && member is Ast.PropertyDeclaration))
					return false;
			}
			
			if (member.Modifier.HasFlag(Ast.Modifiers.Abstract))
				return false;
			
			return true;
		}
		
		static bool NeedsBody(Ast.AttributedNode type, Ast.AttributedNode member)
		{
			if (type is Ast.TypeDeclaration) {
				var decl = type as Ast.TypeDeclaration;
				if (decl.Type == Ast.ClassType.Interface)
					return false;
			}
			
			if (member.Modifier.HasFlag(Ast.Modifiers.Abstract) || member.Modifier.HasFlag(Ast.Modifiers.Extern))
				return false;
			
			return true;
		}
		
		static bool IsGreaterThan(Microsoft.Build.Utilities.TargetDotNetFrameworkVersion version, Microsoft.Build.Utilities.TargetDotNetFrameworkVersion otherVersion)
		{
			return (int)otherVersion > (int)version;
		}
	}
}
