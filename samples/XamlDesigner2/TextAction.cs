using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.TextEditor.Undo;
using SharpDevelop.XamlDesigner.Dom.UndoSystem;

namespace SharpDevelop.Samples.XamlDesigner
{
	class TextAction : UndoAction
	{
		public TextAction(IUndoableOperation op)
		{
			this.op = op;
		}

		IUndoableOperation op;

		public override void Redo()
		{
			op.Redo();
		}

		public override void Undo()
		{
			op.Undo();
		}
	}
}
