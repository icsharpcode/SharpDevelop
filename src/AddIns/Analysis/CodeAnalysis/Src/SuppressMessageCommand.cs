// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.CodeAnalysis
{
	public class SuppressMessageCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			TaskView view = (TaskView)Owner;
			foreach (SDTask t in view.SelectedTasks.ToList()) {
				FxCopTaskTag tag = t.Tag as FxCopTaskTag;
				if (tag == null)
					continue;
				throw new NotImplementedException();
				/*CodeGenerator codegen = tag.ProjectContent.Language.CodeGenerator;
				if (codegen == null)
					continue;
				FilePosition p;
				if (tag.MemberName == null)
					p = GetAssemblyAttributeInsertionPosition(tag.ProjectContent);
				else
					p = GetPosition(tag.ProjectContent, tag.TypeName, tag.MemberName);
				if (p.CompilationUnit == null || p.FileName == null || p.Line <= 0)
					continue;
				IViewContent viewContent = FileService.OpenFile(p.FileName);
				ITextEditorProvider provider = viewContent as ITextEditorProvider;
				if (provider == null)
					continue;
				IDocument document = provider.TextEditor.Document;
				if (p.Line >= document.TotalNumberOfLines)
					continue;
				IDocumentLine line = document.GetLine(p.Line);
				StringBuilder indentation = new StringBuilder();
				for (int i = line.Offset; i < document.TextLength; i++) {
					char c = document.GetCharAt(i);
					if (c == ' ' || c == '\t')
						indentation.Append(c);
					else
						break;
				}
				string code = codegen.GenerateCode(CreateSuppressAttribute(p.CompilationUnit, tag), indentation.ToString());
				if (!code.EndsWith("\n")) code += Environment.NewLine;
				document.Insert(line.Offset, code);
				provider.TextEditor.JumpTo(p.Line, p.Column);
				TaskService.Remove(t);
				ParserService.ParseViewContent(viewContent);*/
			}
		}
		
		internal static DomRegion GetPosition(ICompilation compilation, string className, string memberName)
		{
			var typeName = new TopLevelTypeName(className);
			ITypeDefinition type = compilation.MainAssembly.GetTypeDefinition(typeName);
			if (type != null) {
				if (string.IsNullOrEmpty(memberName))
					return type.Region;
				
				IMember m = GetMember(type, memberName);
				if (m != null)
					return m.Region;
				
				return type.Region;
			}
			return DomRegion.Empty;
		}
		
		static IMember GetMember(ITypeDefinition type, string memberName)
		{
			string fullName = type.ReflectionName + "." + memberName;
			return type.GetMembers(m => m.ReflectionName == fullName).FirstOrDefault();
		}
		
		/*
		FilePosition GetAssemblyAttributeInsertionPosition(IProjectContent pc)
		{
			FilePosition best = FilePosition.Empty;
			foreach (IAttribute attrib in pc.GetAssemblyAttributes()) {
				ICompilationUnit cu = attrib.CompilationUnit;
				if (cu != null && !attrib.Region.IsEmpty) {
					var newPos = new FilePosition(cu, attrib.Region.BeginLine, attrib.Region.BeginColumn);
					if (IsBetterAssemblyAttributeInsertionPosition(newPos, best)) {
						best = newPos;
					}
				}
			}
			return best;
		}
		
		bool IsBetterAssemblyAttributeInsertionPosition(FilePosition a, FilePosition b)
		{
			if (b.IsEmpty)
				return true;
			
			bool aIsAssemblyInfo = "AssemblyInfo".Equals(Path.GetFileNameWithoutExtension(a.FileName), StringComparison.OrdinalIgnoreCase);
			bool bIsAssemblyInfo = "AssemblyInfo".Equals(Path.GetFileNameWithoutExtension(b.FileName), StringComparison.OrdinalIgnoreCase);
			if (aIsAssemblyInfo && !bIsAssemblyInfo)
				return true;
			if (!aIsAssemblyInfo && bIsAssemblyInfo)
				return false;
			
			return a.Line > b.Line;
		}*/
		
		const string NamespaceName = "System.Diagnostics.CodeAnalysis";
		const string AttributeName = "SuppressMessage";
		/*
		static Ast.AbstractNode CreateSuppressAttribute(ICompilationUnit cu, FxCopTaskTag tag)
		{
			//System.Diagnostics.CodeAnalysis.SuppressMessageAttribute
			bool importedCodeAnalysis = CheckImports(cu);
			
			// [SuppressMessage("Microsoft.Performance", "CA1801:ReviewUnusedParameters", MessageId:="fileIdentifier"]
			Ast.Attribute a = new Ast.Attribute {
				Name = importedCodeAnalysis ? AttributeName : NamespaceName + "." + AttributeName,
				PositionalArguments = {
					new Ast.PrimitiveExpression(tag.Category, tag.Category),
					new Ast.PrimitiveExpression(tag.CheckID, tag.CheckID)
				}
			};
			if (tag.MessageID != null) {
				a.NamedArguments.Add(new Ast.NamedArgumentExpression("MessageId",
				                                                     new Ast.PrimitiveExpression(tag.MessageID, tag.MessageID)));
			}
			
			return new Ast.AttributeSection {
				AttributeTarget = tag.MemberName == null ? "assembly" : null,
				Attributes = { a }
			};
		}*/
	}
}
