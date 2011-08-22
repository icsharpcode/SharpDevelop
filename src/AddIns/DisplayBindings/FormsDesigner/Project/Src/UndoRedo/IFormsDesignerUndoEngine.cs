// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace ICSharpCode.FormsDesigner.UndoRedo
{
	public interface IFormsDesignerUndoEngine
	{
		void Undo();
		void Redo();
		bool EnableUndo { get; }
		bool EnableRedo { get; }
	}
}
