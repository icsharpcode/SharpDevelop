// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace UnitTesting.Tests.Utils
{
	public class MockStatusBarService : IStatusBarService
	{
		public void SetCaretPosition(int x, int y, int charOffset)
		{
			throw new NotImplementedException();
		}
		
		public void SetMessage(string message, bool highlighted, IImage icon)
		{
			throw new NotImplementedException();
		}
		
		public IProgressMonitor CreateProgressMonitor(System.Threading.CancellationToken cancellationToken)
		{
			return new DummyProgressMonitor();
		}
		
		public void AddProgress(ProgressCollector progress)
		{
			throw new NotImplementedException();
		}
	}
}
