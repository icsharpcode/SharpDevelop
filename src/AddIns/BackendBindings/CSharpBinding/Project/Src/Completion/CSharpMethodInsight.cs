// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Completion;
using ICSharpCode.NRefactory.CSharp.Completion;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace CSharpBinding.Completion
{
	sealed class CSharpMethodInsight : IParameterDataProvider
	{
		readonly int startOffset;
		internal readonly IReadOnlyList<CSharpInsightItem> items;
		readonly ITextEditor editor;
		IInsightWindow window;
		CSharpInsightItem initiallySelectedItem;
		
		public CSharpMethodInsight(ITextEditor editor, int startOffset, IEnumerable<CSharpInsightItem> items)
		{
			Debug.Assert(editor != null);
			Debug.Assert(items != null);
			this.editor = editor;
			this.startOffset = startOffset;
			this.items = items.ToList();
		}
		
		public void Show()
		{
			window = editor.ShowInsightWindow(items);
			window.StartOffset = startOffset;
			// closing the window at the end of the parameter list is handled by the CaretPositionChanged event
			window.EndOffset = editor.Document.TextLength;
			if (initiallySelectedItem != null)
				window.SelectedItem = initiallySelectedItem;
			window.CaretPositionChanged += window_CaretPositionChanged;
		}
		
		void window_CaretPositionChanged(object sender, EventArgs e)
		{
			var completionContext = CSharpCompletionContext.Get(editor);
			if (completionContext == null) {
				window.Close();
				return;
			}
			var completionFactory = new CSharpCompletionDataFactory(completionContext, new CSharpResolver(completionContext.TypeResolveContextAtCaret));
			var pce = new CSharpParameterCompletionEngine(
				editor.Document,
				completionContext.CompletionContextProvider,
				completionFactory,
				completionContext.ProjectContent,
				completionContext.TypeResolveContextAtCaret
			);
			UpdateHighlightedParameter(pce);
		}
		
		public void UpdateHighlightedParameter(CSharpParameterCompletionEngine pce)
		{
			int parameterIndex = pce.GetCurrentParameterIndex(window != null ? window.StartOffset : startOffset, editor.Caret.Offset);
			if (parameterIndex < 0 && window != null) {
				window.Close();
			} else {
				if (window == null || parameterIndex > ((CSharpInsightItem)window.SelectedItem).Method.Parameters.Count) {
					var newItem = items.FirstOrDefault(i => parameterIndex <= i.Method.Parameters.Count);
					if (newItem != null) {
						if (window != null)
							window.SelectedItem = newItem;
						else
							initiallySelectedItem = newItem;
					}
				}
				if (parameterIndex > 0)
					parameterIndex--; // NR returns 1-based parameter index
				foreach (var item in items)
					item.HighlightParameter(parameterIndex);
			}
		}
		
		#region IParameterDataProvider implementation
		int IParameterDataProvider.Count {
			get { return items.Count; }
		}
		
		int IParameterDataProvider.StartOffset {
			get { return startOffset; }
		}
		
		string IParameterDataProvider.GetHeading(int overload, string[] parameterDescription, int currentParameter)
		{
			throw new NotImplementedException();
		}
		
		string IParameterDataProvider.GetDescription(int overload, int currentParameter)
		{
			throw new NotImplementedException();
		}
		
		string IParameterDataProvider.GetParameterDescription(int overload, int paramIndex)
		{
			throw new NotImplementedException();
		}
		
		string IParameterDataProvider.GetParameterName(int overload, int currentParameter)
		{
			throw new NotImplementedException();
		}
		
		int IParameterDataProvider.GetParameterCount(int overload)
		{
			throw new NotImplementedException();
		}
		
		bool IParameterDataProvider.AllowParameterList(int overload)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
