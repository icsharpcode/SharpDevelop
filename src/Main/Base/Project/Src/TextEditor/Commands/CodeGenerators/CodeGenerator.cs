// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public abstract class OldCodeGeneratorBase
	{
		ArrayList content = new ArrayList();
		protected int       numOps  = 0;
		protected IAmbience csa;
		protected IAmbience vba;
		protected IClass    currentClass = null;
		protected TextArea editActionHandler;
		
		public OldCodeGeneratorBase(IClass currentClass)
		{
			this.currentClass = currentClass;
			try {
				csa = (IAmbience)AddInTree.BuildItem("/SharpDevelop/Workbench/Ambiences/C#", this);
				csa.ConversionFlags = ConversionFlags.ShowAccessibility | ConversionFlags.ShowModifiers | ConversionFlags.QualifiedNamesOnlyForReturnTypes | ConversionFlags.ShowReturnType | ConversionFlags.ShowParameterNames;
			} catch (TreePathNotFoundException) {
				LoggingService.Warn("CSharpAmbience not found -- is the C# backend binding loaded???");
			}
			
			try {
				vba = (IAmbience)AddInTree.BuildItem("/SharpDevelop/Workbench/Ambiences/VBNet", this);
				vba.ConversionFlags = ConversionFlags.ShowAccessibility | ConversionFlags.ShowModifiers | ConversionFlags.QualifiedNamesOnlyForReturnTypes | ConversionFlags.ShowReturnType | ConversionFlags.ShowParameterNames;
			} catch (TreePathNotFoundException) {
				LoggingService.Warn("VBNet ambience not found -- is the VB.NET backend binding loaded???");
			}
		}
		
		public abstract string CategoryName {
			get;
		}
		
		public abstract string Hint {
			get;
		}
		
		public abstract int ImageIndex {
			get;
		}
		
		public virtual bool IsActive {
			get {
				return content.Count > 0;
			}
		}
		
		public ArrayList Content {
			get {
				return content;
			}
		}
		
		public static bool BlankLinesBetweenMembers {
			get {
				return AmbienceService.CodeGenerationProperties.Get("BlankLinesBetweenMembers", true);
			}
		}
		
		public static bool StartCodeBlockInSameLine {
			get {
				return AmbienceService.CodeGenerationProperties.Get("StartBlockOnSameLine", true);
			}
		}
		
		public void GenerateCode(TextArea editActionHandler, IList items)
		{
			numOps = 0;
			this.editActionHandler = editActionHandler;
			editActionHandler.BeginUpdate();
			
			
			bool save1         = editActionHandler.TextEditorProperties.AutoInsertCurlyBracket;
			IndentStyle save2  = editActionHandler.TextEditorProperties.IndentStyle;
			bool save3         = PropertyService.Get("VBBinding.TextEditor.EnableEndConstructs", true);
			PropertyService.Set("VBBinding.TextEditor.EnableEndConstructs", false);
			editActionHandler.TextEditorProperties.AutoInsertCurlyBracket = false;
			editActionHandler.TextEditorProperties.IndentStyle            = IndentStyle.Smart;
			
			string extension = Path.GetExtension(editActionHandler.MotherTextEditorControl.FileName).ToLower();
			StartGeneration(items, extension);
			
			if (numOps > 0) {
				editActionHandler.Document.UndoStack.UndoLast(numOps);
			}
			// restore old property settings
			editActionHandler.TextEditorProperties.AutoInsertCurlyBracket = save1;
			editActionHandler.TextEditorProperties.IndentStyle            = save2;
			PropertyService.Set("VBBinding.TextEditor.EnableEndConstructs", save3);
			editActionHandler.EndUpdate();
			
			editActionHandler.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			editActionHandler.Document.CommitUpdate();
		}
		
		protected abstract void StartGeneration(IList items, string fileExtension);
		
		protected void Indent()
		{
			
			Properties p = ((Properties)PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties()));
			
			bool tabsToSpaces = p.Get("TabsToSpaces", false);
			
			int  tabSize      = p.Get("TabIndent", 4);
			int  indentSize   = p.Get("IndentationSize", 4);
			
			if (tabsToSpaces) {
				editActionHandler.InsertString(new String(' ', indentSize));
			} else {
				editActionHandler.InsertString(new String('\t', indentSize / tabSize));
				int trailingSpaces = indentSize % tabSize;
				if (trailingSpaces > 0) {
					editActionHandler.InsertString(new String(' ', trailingSpaces));
					++numOps;
				}
			}
			++numOps;
		}
		
		protected void Return()
		{
			IndentLine();
			new Return().Execute(editActionHandler);
			++numOps;
		}
		
		protected void IndentLine()
		{
			editActionHandler.Document.FormattingStrategy.IndentLine(editActionHandler, editActionHandler.Caret.Line);
		}
	}
}
