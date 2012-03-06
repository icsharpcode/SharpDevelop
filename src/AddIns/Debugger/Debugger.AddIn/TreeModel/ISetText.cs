// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

namespace Debugger.AddIn.TreeModel
{
	public interface ISetText
	{
		bool CanSetText { get; }
		
		bool SetText(string text);
	}
}
