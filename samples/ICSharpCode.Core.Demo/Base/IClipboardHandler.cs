// Copyright (c) 2005 Daniel Grunwald
// Licensed under the terms of the "BSD License", see doc/license.txt

using System;

namespace Base
{
	public interface IClipboardHandler
	{
		bool CanPaste { get; }
		void Paste();
		
		bool CanCut { get; }
		void Cut();
		
		bool CanCopy { get; }
		void Copy();
		
		bool CanDelete { get; }
		void Delete();
	}
}
