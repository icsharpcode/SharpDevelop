// CSharp Editor Example with Code Completion
// Copyright (c) 2006, Daniel Grunwald
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
// 
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
// 
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
// 
// - Neither the name of the ICSharpCode nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

using Dom = ICSharpCode.SharpDevelop.Dom;
using NRefactoryResolver = ICSharpCode.SharpDevelop.Dom.NRefactoryResolver.NRefactoryResolver;

namespace CSharpEditor
{
	class CodeCompletionProvider : ICompletionDataProvider
	{
		MainForm mainForm;
		
		public CodeCompletionProvider(MainForm mainForm)
		{
			this.mainForm = mainForm;
		}
		
		public ImageList ImageList {
			get {
				return mainForm.imageList1;
			}
		}
		
		public string PreSelection {
			get {
				return null;
			}
		}
		
		public int DefaultIndex {
			get {
				return -1;
			}
		}
		
		public CompletionDataProviderKeyResult ProcessKey(char key)
		{
			if (char.IsLetterOrDigit(key) || key == '_') {
				return CompletionDataProviderKeyResult.NormalKey;
			} else {
				// key triggers insertion of selected items
				return CompletionDataProviderKeyResult.InsertionKey;
			}
		}
		
		/// <summary>
		/// Called when entry should be inserted. Forward to the insertion action of the completion data.
		/// </summary>
		public bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
		{
			textArea.Caret.Position = textArea.Document.OffsetToPosition(insertionOffset);
			return data.InsertAction(textArea, key);
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			// We can return code-completion items like this:
			
			//return new ICompletionData[] {
			//	new DefaultCompletionData("Text", "Description", 1)
			//};
			
			NRefactoryResolver resolver = new NRefactoryResolver(mainForm.myProjectContent, mainForm.myProjectContent.Language);
			Dom.ResolveResult rr = resolver.Resolve(FindExpression(textArea),
			                                        textArea.Caret.Line,
			                                        textArea.Caret.Column,
			                                        fileName,
			                                        textArea.MotherTextEditorControl.Text);
			List<ICompletionData> resultList = new List<ICompletionData>();
			if (rr != null) {
				ArrayList completionData = rr.GetCompletionData(mainForm.myProjectContent);
				if (completionData != null) {
					AddCompletionData(resultList, completionData);
				}
			}
			return resultList.ToArray();
		}
		
		/// <summary>
		/// Find the expression the cursor is at.
		/// Also determines the context (using statement, "new"-expression etc.) the
		/// cursor is at.
		/// </summary>
		Dom.ExpressionResult FindExpression(TextArea textArea)
		{
			Dom.VBNet.VBExpressionFinder finder;
			finder = new Dom.VBNet.VBExpressionFinder();
			return finder.FindExpression(textArea.Document.TextContent, textArea.Caret.Offset);
		}
		
		void AddCompletionData(List<ICompletionData> resultList, ArrayList completionData)
		{
			// Add the completion data as returned by SharpDevelop.Dom to the
			// list for the text editor
			foreach (object obj in completionData) {
				if (obj is string) {
					// namespace names are returned as string
					resultList.Add(new DefaultCompletionData((string)obj, "namespace " + obj, 5));
				} else if (obj is Dom.IClass) {
					Dom.IClass c = (Dom.IClass)obj;
					if (c.ClassType == Dom.ClassType.Enum) {
						resultList.Add(new DefaultCompletionData(c.Name,
						                                         GetDescription(c),
						                                         4));
					} else { // missing: struct, delegate etc.
						resultList.Add(new DefaultCompletionData(c.Name,
						                                         GetDescription(c),
						                                         0));
					}
				} else if (obj is Dom.IMember) {
					Dom.IMember m = (Dom.IMember)obj;
					if (m is Dom.IMethod && ((m as Dom.IMethod).IsConstructor)) {
						// Skip constructors
						continue;
					}
					// TODO: Group results by name and add "(x Overloads)" to the
					// description if there are multiple results with the same name.
					resultList.Add(new DefaultCompletionData(m.Name,
					                                         GetDescription(m),
					                                         GetMemberImageIndex(m)));
				} else {
					// Current ICSharpCode.SharpDevelop.Dom should never return anything else
					throw new NotSupportedException();
				}
			}
		}
		
		/// <summary>
		/// Converts a member to text.
		/// Returns the declaration of the member as C# code, e.g.
		/// "public void MemberName(string parameter)"
		/// </summary>
		string GetDescription(Dom.IDecoration entity)
		{
			return GetCSharpText(entity) + Environment.NewLine + entity.Documentation;
		}
		
		string GetCSharpText(Dom.IDecoration entity)
		{
			if (entity is Dom.IMethod)
				return Dom.CSharp.CSharpAmbience.Instance.Convert(entity as Dom.IMethod);
			if (entity is Dom.IProperty)
				return Dom.CSharp.CSharpAmbience.Instance.Convert(entity as Dom.IProperty);
			if (entity is Dom.IEvent)
				return Dom.CSharp.CSharpAmbience.Instance.Convert(entity as Dom.IEvent);
			if (entity is Dom.IField)
				return Dom.CSharp.CSharpAmbience.Instance.Convert(entity as Dom.IField);
			if (entity is Dom.IClass)
				return Dom.CSharp.CSharpAmbience.Instance.Convert(entity as Dom.IClass);
			// unknown entity:
			return entity.ToString();
		}
		
		int GetMemberImageIndex(Dom.IMember member)
		{
			// Missing: different icons for private/public member
			if (member is Dom.IMethod)
				return 1;
			if (member is Dom.IProperty)
				return 2;
			if (member is Dom.IField)
				return 3;
			if (member is Dom.IEvent)
				return 6;
			return 3;
		}
	}
}
