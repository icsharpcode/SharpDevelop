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

using System;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using CSharpBinding.Parser;

namespace CSharpBinding.Refactoring
{
	[ContextAction("Rename file to match type name", Description = "Renames the file so that it matches the type name.")]
	public class RenameFileToMatchTypeNameContextAction : ContextAction
	{
		public override async Task<bool> IsAvailableAsync(EditorRefactoringContext context, System.Threading.CancellationToken cancellationToken)
		{
			SyntaxTree st = await context.GetSyntaxTreeAsync().ConfigureAwait(false);
			Identifier identifier = (Identifier) st.GetNodeAt(context.CaretLocation, node => node.Role == Roles.Identifier);
			if (identifier == null)
				return false;
			if (identifier.Name.Equals(Path.GetFileNameWithoutExtension(context.FileName), StringComparison.OrdinalIgnoreCase))
				return false;
			if (identifier.Parent.Parent is TypeDeclaration)
				return false;
			ParseInformation info = await context.GetParseInformationAsync().ConfigureAwait(false);
			if (info.UnresolvedFile.TopLevelTypeDefinitions.Count > 1)
				return false;
			return identifier.Parent is TypeDeclaration || identifier.Parent is DelegateDeclaration || identifier.Parent is EnumMemberDeclaration;
		}

		public override void Execute(EditorRefactoringContext context)
		{
			CSharpFullParseInformation parseInformation = context.GetParseInformation() as CSharpFullParseInformation;
			if (parseInformation != null) {
				SyntaxTree st = parseInformation.SyntaxTree;
				Identifier identifier = (Identifier) st.GetNodeAt(context.CaretLocation, node => node.Role == Roles.Identifier);
				if (identifier == null)
					return;
				
				ICompilation compilation = context.GetCompilation();
				IProject project = compilation.GetProject();
				RenameFile(project, context.FileName, Path.Combine(Path.GetDirectoryName(context.FileName), MakeValidFileName(identifier.Name)));
				if (project != null) {
					project.Save();
				}
			}
		}

		public override string DisplayName
		{
			get {
				return "Rename file to match type name";
			}
		}
		
		public override string GetDisplayName(EditorRefactoringContext context)
		{
			CSharpFullParseInformation parseInformation = context.GetParseInformation() as CSharpFullParseInformation;
			if (parseInformation != null) {
				SyntaxTree st = parseInformation.SyntaxTree;
				Identifier identifier = (Identifier) st.GetNodeAt(context.CaretLocation, node => node.Role == Roles.Identifier);
				if (identifier == null)
					return DisplayName;
				
				return StringParser.Parse("${res:SharpDevelop.Refactoring.RenameFileTo}",
				                          new StringTagPair("FileName", MakeValidFileName(identifier.Name)));
			}
			
			return DisplayName;
		}
		
		string MakeValidFileName(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (name.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
				return name.RemoveAny(Path.GetInvalidFileNameChars()) + ".cs";
			return name + ".cs";
		}
		
		/// <summary>
		/// Renames file as well as files it is dependent upon.
		/// </summary>
		static void RenameFile(IProject p, string oldFileName, string newFileName)
		{
			FileService.RenameFile(oldFileName, newFileName, false);
			if (p != null) {
				string oldPrefix = Path.GetFileNameWithoutExtension(oldFileName) + ".";
				string newPrefix = Path.GetFileNameWithoutExtension(newFileName) + ".";
				foreach (ProjectItem item in p.Items) {
					FileProjectItem fileItem = item as FileProjectItem;
					if (fileItem == null)
						continue;
					string dependentUpon = fileItem.DependentUpon;
					if (string.IsNullOrEmpty(dependentUpon))
						continue;
					string directory = Path.GetDirectoryName(fileItem.FileName);
					dependentUpon = Path.Combine(directory, dependentUpon);
					if (FileUtility.IsEqualFileName(dependentUpon, oldFileName)) {
						fileItem.DependentUpon = FileUtility.GetRelativePath(directory, newFileName);
						string fileName = Path.GetFileName(fileItem.FileName);
						if (fileName.StartsWith(oldPrefix)) {
							RenameFile(p, fileItem.FileName, Path.Combine(directory, newPrefix + fileName.Substring(oldPrefix.Length)));
						}
					}
				}
			}
		}
	}
}
