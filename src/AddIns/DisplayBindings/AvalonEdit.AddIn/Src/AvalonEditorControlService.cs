// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.AvalonEdit.AddIn
{
	/// <summary>
	/// Implementation of IEditorControlService, allows other addins to create editors or access the options without
	/// requiring a reference to AvalonEdit.AddIn.
	/// </summary>
	public class AvalonEditorControlService : IEditorControlService
	{
		public ITextEditorOptions GlobalOptions {
			get { return CodeEditorOptions.Instance; }
		}
		
		public ITextEditor CreateEditor(out object control)
		{
			SharpDevelopTextEditor editor = new SharpDevelopTextEditor();
			control = editor;
			return new CodeCompletionEditorAdapter(editor);
		}
		
		public IHighlighter CreateHighlighter(IDocument document)
		{
			if (document.FileName == null)
				return new MultiHighlighter(document);
			var def = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(document.FileName));
			if (def == null)
				return new MultiHighlighter(document);
			List<IHighlighter> highlighters = new List<IHighlighter>();
			var textDocument = document as TextDocument;
			var readOnlyDocument = document as ReadOnlyDocument;
			if (textDocument != null) {
				highlighters.Add(new DocumentHighlighter(textDocument, def));
			} else if (readOnlyDocument != null) {
				highlighters.Add(new DocumentHighlighter(readOnlyDocument, def));
			}
			// add additional highlighters
			highlighters.AddRange(SD.AddInTree.BuildItems<IHighlighter>(HighlighterDoozer.AddInPath, document, false));
			var multiHighlighter = new MultiHighlighter(document, highlighters.ToArray());
			return new CustomizingHighlighter(multiHighlighter, CustomizedHighlightingColor.FetchCustomizations(def.Name));
		}
	}

	public class HighlighterDoozer : IDoozer
	{
		internal const string AddInPath = "/SharpDevelop/ViewContent/AvalonEdit/Highlighters";
		
		public bool HandleConditions {
			get { return false; }
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			if (!(args.Parameter is IDocument))
				throw new ArgumentException("Caller must be IDocument!");
			Codon codon = args.Codon;
			if (!codon.Properties["extensions"].Split(';').Contains(Path.GetExtension(((IDocument)args.Parameter).FileName)))
				return null;
			return Activator.CreateInstance(codon.AddIn.FindType(codon.Properties["class"]), args.Parameter);
		}
	}

}
