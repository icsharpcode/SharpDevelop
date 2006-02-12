// Copyright (c) 2005 Daniel Grunwald
// Licensed under the terms of the "BSD License", see doc/license.txt

using System;

namespace Base
{
	public interface IUndoHandler
	{
		bool CanUndo { get; }
		void Undo();
		bool CanRedo { get; }
		void Redo();
	}
}
