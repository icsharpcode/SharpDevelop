// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using ICSharpCode.AvalonEdit.AddIn.Options;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
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
			var def = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(document.FileName));
			var doc = document as TextDocument;
			if (def == null || doc == null)
				return null;
			var baseHighlighter = new DocumentHighlighter(doc, def);
			var additionalHighlighters = AddInTree.BuildItems<IHighlighter>(HighlighterDoozer.AddInPath, doc, false);
			var multiHighlighter = new MultiHighlighter(document, new[] { baseHighlighter }.Concat(additionalHighlighters).ToArray());
			return new CustomizableHighlightingColorizer.CustomizingHighlighter(
				document, CustomizedHighlightingColor.FetchCustomizations(def.Name),
				def, multiHighlighter);
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
			if (!(args.Caller is IDocument))
				throw new ArgumentException("Caller must be IDocument!");
			Codon codon = args.Codon;
			if (!codon.Properties["extensions"].Split(';').Contains(Path.GetExtension(((IDocument)args.Caller).FileName)))
				return null;
			return Activator.CreateInstance(codon.AddIn.FindType(codon.Properties["class"]), args.Caller);
		}
	}

}
