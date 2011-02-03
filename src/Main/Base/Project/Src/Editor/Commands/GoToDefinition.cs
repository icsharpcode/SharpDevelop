// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using Mono.Cecil;

namespace ICSharpCode.SharpDevelop.Editor.Commands
{
	public class GoToDefinition : SymbolUnderCaretMenuCommand
	{
		protected override void RunImpl(ITextEditor editor, int offset, ResolveResult symbol)
		{
			if (symbol == null)
				return;
			FilePosition pos = symbol.GetDefinitionPosition();
			if (pos.IsEmpty) {
				new GoToDecompiledDefinition().Run(symbol);
			} else {
				try {
					if (pos.Position.IsEmpty)
						FileService.OpenFile(pos.FileName);
					else
						FileService.JumpToFilePosition(pos.FileName, pos.Line, pos.Column);
				} catch (Exception ex) {
					MessageService.ShowException(ex, "Error jumping to '" + pos.FileName + "'.");
				}
			}
		}
	}
	
	public class GoToDecompiledDefinition : AbstractMenuCommand
	{
		/// <summary>
		/// Don't use this method; throws NotImplementedException.
		/// </summary>
		public override void Run()
		{
			throw new NotImplementedException();
		}
		
		public void Run(ResolveResult symbol)
		{
			if (ProjectService.CurrentProject == null)
				return;
			string filePath = null;
			if (symbol is MemberResolveResult) {
				var s = (MemberResolveResult)symbol;
				DecompilerService.ReadMetadata(s.ResolvedMember.DeclaringType, out filePath);
				if (string.IsNullOrEmpty(filePath))
					return;
				
				// jump to definition
				var info = ParserService.ParseFile(filePath);
				
				if (info == null)
					return;
				
				int line = 0, col = 0;
				foreach(var c in info.CompilationUnit.Classes) {
					if (s.ResolvedMember.EntityType == EntityType.Event) {
						foreach (var ev in c.Events) {
							if (s.ResolvedMember.FullyQualifiedName == ev.FullyQualifiedName &&
							    s.ResolvedMember.ReturnType.FullyQualifiedName == ev.ReturnType.FullyQualifiedName) {
								col = ev.Region.BeginColumn;
								line = ev.Region.BeginLine;
							}
						}
						
						if (col > 0 || line > 0)
							break;
					}
					
					if (s.ResolvedMember.EntityType == EntityType.Method) {
						
						foreach(var m1 in s.ResolvedMember.DeclaringType.Methods) {
							if (s.ResolvedMember.ReturnType.FullyQualifiedName != m1.ReturnType.FullyQualifiedName ||
							    s.ResolvedMember.FullyQualifiedName != m1.FullyQualifiedName) continue;
							
							foreach (var m in c.Methods) {
								if (m1.FullyQualifiedName == m.FullyQualifiedName &&
								    m1.ReturnType.FullyQualifiedName == m1.ReturnType.FullyQualifiedName &&
								    m1.Parameters.Count == m.Parameters.Count) {
									
									col = m.Region.BeginColumn;
									line = m.Region.BeginLine;
									break;
								}
							}
							
							if (col > 0 || line > 0)
								break;
						}
						
						if (col > 0 || line > 0)
							break;
					}
					
					if (s.ResolvedMember.EntityType == EntityType.Property) {
						foreach (var p in c.Properties) {
							if (s.ResolvedMember.FullyQualifiedName == p.FullyQualifiedName &&
							    s.ResolvedMember.ReturnType.FullyQualifiedName == p.ReturnType.FullyQualifiedName) {
								col = p.Region.BeginColumn;
								line = p.Region.BeginLine;
								break;
							}
						}
						
						if (col > 0 || line > 0)
							break;
					}
				}
				
				if (col > 0 || line > 0)
					FileService.JumpToFilePosition(filePath, line, col);
				else
					FileService.OpenFile(filePath);
			}
			
			if (symbol is TypeResolveResult) {
				var s = (TypeResolveResult)symbol;
				DecompilerService.ReadMetadata(s.ResolvedClass, out filePath);
				
				if (!string.IsNullOrEmpty(filePath))
					FileService.OpenFile(filePath);
			}
		}
	}
}
