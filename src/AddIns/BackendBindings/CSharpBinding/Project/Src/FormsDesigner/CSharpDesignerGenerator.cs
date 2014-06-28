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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.CSharp;
using CSharpBinding.FormattingStrategy;
using CSharpBinding.Parser;
using CSharpBinding.Refactoring;

namespace CSharpBinding.FormsDesigner
{
	public class CSharpDesignerGenerator
	{
		readonly CSharpFullParseInformation primaryParseInfo;
		readonly ICSharpDesignerLoaderContext context;
		readonly ICompilation compilation;
		readonly IUnresolvedTypeDefinition primaryPart;
		readonly ITypeDefinition formClass;
		readonly IMethod initializeComponents;
		
		public CSharpDesignerGenerator(ICSharpDesignerLoaderContext context)
		{
			this.context = context;
			this.primaryParseInfo = context.GetPrimaryFileParseInformation();
			this.compilation = context.GetCompilation();
			
			// Find designer class
			formClass = FormsDesignerSecondaryDisplayBinding.GetDesignableClass(primaryParseInfo.UnresolvedFile, compilation, out primaryPart);
			initializeComponents = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(formClass);
			if (initializeComponents == null)
				throw new FormsDesignerLoadException("Could not find InitializeComponents");
		}
		
		public void MergeFormChanges(CodeCompileUnit codeUnit)
		{
			var codeNamespace = codeUnit.Namespaces.Cast<CodeNamespace>().Single();
			var codeClass = codeNamespace.Types.Cast<CodeTypeDeclaration>().Single();
			var codeMethod = codeClass.Members.OfType<CodeMemberMethod>().Single(m => m.Name == "InitializeComponent");
			var codeFields = codeClass.Members.OfType<CodeMemberField>().ToList();
			RemoveUnsupportedCode(codeClass, codeMethod);
			
			SaveInitializeComponents(codeMethod);
			MergeFields(codeFields);
			ApplyScripts();
		}
		
		#region RemoveUnsupportedCode
		/// <summary>
		/// This method solves all problems that are caused if the designer generates
		/// code that is not supported in a previous version of .NET. (3.5 and below)
		/// Currently it fixes:
		///  - remove calls to ISupportInitialize.BeginInit/EndInit, if the interface is not implemented by the type in the target framework.
		/// </summary>
		/// <remarks>When adding new workarounds make sure that the code does not remove too much code!</remarks>
		void RemoveUnsupportedCode(CodeTypeDeclaration codeClass, CodeMemberMethod initializeComponent)
		{
			if (compilation.GetProject() is MSBuildBasedProject) {
				MSBuildBasedProject p = (MSBuildBasedProject)compilation.GetProject();
				string v = (p.GetEvaluatedProperty("TargetFrameworkVersion") ?? "").Trim('v');
				Version version;
				if (!Version.TryParse(v, out version) || version.Major >= 4)
					return;
			}
			
			List<CodeStatement> stmtsToRemove = new List<CodeStatement>();
			var iSupportInitializeInterface = compilation.FindType(typeof(ISupportInitialize)).GetDefinition();
			
			if (iSupportInitializeInterface == null)
				return;
			
			foreach (var stmt in initializeComponent.Statements.OfType<CodeExpressionStatement>().Where(ces => ces.Expression is CodeMethodInvokeExpression)) {
				CodeMethodInvokeExpression invocation = (CodeMethodInvokeExpression)stmt.Expression;
				CodeCastExpression expr = invocation.Method.TargetObject as CodeCastExpression;
				if (expr != null) {
					if (expr.TargetType.BaseType != "System.ComponentModel.ISupportInitialize")
						continue;
					var fieldType = GetTypeOfControl(expr.Expression, initializeComponent, codeClass).GetDefinition();
					if (fieldType == null)
						continue;
					if (!fieldType.IsDerivedFrom(iSupportInitializeInterface))
						stmtsToRemove.Add(stmt);
				}
			}
			
			foreach (var stmt in stmtsToRemove) {
				initializeComponent.Statements.Remove(stmt);
			}
		}
		
		/// <summary>
		/// Tries to find the type of the expression.
		/// </summary>
		IType GetTypeOfControl(CodeExpression expression, CodeMemberMethod initializeComponentMethod, CodeTypeDeclaration formTypeDeclaration)
		{
			if (expression is CodeVariableReferenceExpression) {
				string name = ((CodeVariableReferenceExpression)expression).VariableName;
				var decl = initializeComponentMethod.Statements.OfType<CodeVariableDeclarationStatement>().Single(v => v.Name == name);
				return ReflectionHelper.ParseReflectionName(decl.Type.BaseType).Resolve(compilation);
			}
			if (expression is CodeFieldReferenceExpression && ((CodeFieldReferenceExpression)expression).TargetObject is CodeThisReferenceExpression) {
				string name = ((CodeFieldReferenceExpression)expression).FieldName;
				var decl = formTypeDeclaration.Members.OfType<CodeMemberField>().FirstOrDefault(f => name == f.Name);
				if (decl != null)
					return ReflectionHelper.ParseReflectionName(decl.Type.BaseType).Resolve(compilation);
				var field = formClass.GetFields(f => f.Name == name).LastOrDefault();
				if (field == null)
					return SpecialType.UnknownType;
				return field.Type;
			}
			return SpecialType.UnknownType;
		}
		#endregion
		
		#region Script management
		Dictionary<FileName, DocumentScript> scripts = new Dictionary<FileName, DocumentScript>();
		
		DocumentScript GetScript(string fileName)
		{
			DocumentScript script;
			var fileNameObj = FileName.Create(fileName);
			if (scripts.TryGetValue(fileNameObj, out script))
				return script;
			
			IDocument document = context.GetDocument(fileNameObj);
			var ctx = SDRefactoringContext.Create(fileNameObj, document);
			var formattingOptions = CSharpFormattingPolicies.Instance.GetProjectOptions(compilation.GetProject());
			script = new DocumentScript(document, formattingOptions.OptionsContainer.GetEffectiveOptions(), new TextEditorOptions());
			scripts.Add(fileNameObj, script);
			return script;
		}
		
		void ApplyScripts()
		{
			foreach (var pair in scripts) {
				var script = pair.Value;
				IDocument newDocument = script.CurrentDocument;
				script.Dispose();
				SD.ParserService.ParseFileAsync(pair.Key, newDocument).FireAndForget();
				Debug.Assert(FileName.Create(newDocument.FileName) == pair.Key);
			}
			scripts.Clear();
		}
		#endregion
		
		#region SaveInitializeComponents
		void SaveInitializeComponents(CodeMemberMethod codeMethod)
		{
			var bodyRegion = initializeComponents.BodyRegion;
			DocumentScript script = GetScript(bodyRegion.FileName);
			
			string newline = DocumentUtilities.GetLineTerminator(script.OriginalDocument, bodyRegion.BeginLine);
			string indentation = DocumentUtilities.GetIndentation(script.OriginalDocument, bodyRegion.BeginLine);
			string code = "{" + newline + GenerateInitializeComponents(codeMethod, indentation, newline) + newline + indentation + "}";
			
			int startOffset = script.GetCurrentOffset(bodyRegion.Begin);
			int endOffset = script.GetCurrentOffset(bodyRegion.End);
			script.Replace(startOffset, endOffset - startOffset, code);
		}
		
		string GenerateInitializeComponents(CodeMemberMethod codeMethod, string indentation, string newline)
		{
			var writer = new StringWriter();
			writer.NewLine = newline;
			var options = new CodeGeneratorOptions();
			options.IndentString = SD.EditorControlService.GlobalOptions.IndentationString;
			var codeProvider = new CSharpCodeProvider();
			foreach (CodeStatement statement in codeMethod.Statements) {
				writer.Write(indentation);
				// indentation isn't generated when calling GenerateCodeFromStatement
				writer.Write(options.IndentString);
				try {
					codeProvider.GenerateCodeFromStatement(statement, writer, options);
				} catch (Exception e) {
					writer.WriteLine("// TODO: Error while generating statement : " + e.Message);
					SD.Log.Error(e);
				}
			}
			
			return writer.ToString();
		}
		#endregion
		
		#region MergeFields
		void MergeFields(List<CodeMemberField> codeFields)
		{
			// apply changes the designer made to field declarations
			// first loop looks for added and changed fields
			foreach (CodeMemberField newField in codeFields) {
				IField oldField = formClass.Fields.FirstOrDefault(f => f.Name == newField.Name);
				if (oldField == null) {
					CreateField(newField);
				} else if (FieldChanged(oldField, newField)) {
					UpdateField(oldField, newField);
				}
			}
			
			// second loop looks for removed fields
			foreach (IField field in formClass.Fields) {
				if (!codeFields.Any(f => f.Name == field.Name)) {
					RemoveField(field);
				}
			}
		}
		
		/// <summary>
		/// Compares the SharpDevelop.Dom field declaration oldField to
		/// the CodeDom field declaration newField.
		/// </summary>
		/// <returns>true, if the fields are different in type or modifiers, otherwise false.</returns>
		static bool FieldChanged(IField oldField, CodeMemberField newField)
		{
			// compare types
			if (AreTypesDifferent(oldField.ReturnType, newField.Type)) {
				SD.Log.Debug("FieldChanged (type): "+oldField.Name+", "+oldField.ReturnType.FullName+" -> "+newField.Type.BaseType);
				return true;
			}
			
			// compare accessibility modifiers
			Accessibility oldModifiers = oldField.Accessibility;
			MemberAttributes newModifiers = newField.Attributes & MemberAttributes.AccessMask;
			
			// SharpDevelop.Dom always adds Private modifier, even if not specified
			// CodeDom omits Private modifier if not present (although it is the default)
			if (oldModifiers == Accessibility.Private) {
				if (newModifiers != 0 && newModifiers != MemberAttributes.Private) {
					return true;
				}
			}
			
			Accessibility[] sdModifiers = {Accessibility.Protected, Accessibility.ProtectedAndInternal, Accessibility.Internal, Accessibility.Public};
			MemberAttributes[] cdModifiers = {MemberAttributes.Family, MemberAttributes.FamilyOrAssembly, MemberAttributes.Assembly, MemberAttributes.Public};
			for (int i = 0; i < sdModifiers.Length; i++) {
				if ((oldModifiers  == sdModifiers[i]) ^ (newModifiers  == cdModifiers[i])) {
					return true;
				}
			}
			
			return false;
		}
		
		static bool AreTypesDifferent(IType oldType, CodeTypeReference newType)
		{
			IType oldClass = oldType.GetDefinition();
			if (oldClass == null) {
				// ignore type changes to fields with unresolved type
				return false;
			}
			if (newType == null || newType.BaseType == "System.Void") {
				// field types get replaced with System.Void if the type cannot be resolved
				// (e.g. generic fields in the Boo designer which aren't converted to CodeDom)
				// we'll ignore such type changes (fields should never have the type void)
				return false;
			}
			
			return oldType.GetDefinition().ReflectionName != newType.BaseType;
		}
		
		string GenerateField(CodeMemberField newField)
		{
			StringWriter writer = new StringWriter();
			var provider = new CSharpCodeProvider();
			provider.GenerateCodeFromMember(newField, writer, new CodeGeneratorOptions());
			return writer.ToString().Trim();
		}
		
		void CreateField(CodeMemberField newField)
		{
			// insert new field below InitializeComponents()

			var bodyRegion = initializeComponents.BodyRegion;
			DocumentScript script = GetScript(bodyRegion.FileName);
			string newline = DocumentUtilities.GetLineTerminator(script.OriginalDocument, bodyRegion.BeginLine);
			string indentation = DocumentUtilities.GetIndentation(script.OriginalDocument, bodyRegion.BeginLine);
			
			var insertionLocation = new TextLocation(bodyRegion.EndLine + 1, 1);
			int insertionOffset = script.GetCurrentOffset(insertionLocation);
			string code = indentation + GenerateField(newField) + newline;
			script.InsertText(insertionOffset, code);
		}

		void UpdateField(IField oldField, CodeMemberField newField)
		{
			DomRegion region = oldField.Region;
			DocumentScript script = GetScript(region.FileName);
			
			int offset = script.GetCurrentOffset(region.Begin);
			int endOffset = script.GetCurrentOffset(region.End);
			string code = GenerateField(newField);
			script.Replace(offset, endOffset - offset, code);
		}

		void RemoveField(IField field)
		{
			DomRegion region = field.Region;
			DocumentScript script = GetScript(region.FileName);
			int offset = script.GetCurrentOffset(region.Begin);
			int endOffset = script.GetCurrentOffset(region.End);
			IDocumentLine line = script.CurrentDocument.GetLineByOffset(endOffset);
			if (endOffset == line.EndOffset) {
				endOffset += line.DelimiterLength; // delete the whole line
				// delete indentation in front of the line
				while (offset > 0 && IsTabOrSpace(script.CurrentDocument.GetCharAt(offset - 1)))
					offset--;
			}
			script.RemoveText(offset, endOffset - offset);
		}
		
		static bool IsTabOrSpace(char c)
		{
			return c == '\t' || c == ' ';
		}
		#endregion
	}
}
