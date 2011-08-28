// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public interface IProjectChangeWatcher : IDisposable
	{
		void Enable();
		void Disable();
		void Rename(string newFileName);
	}
	
	public sealed class MockProjectChangeWatcher : IProjectChangeWatcher
	{
		public void Enable()
		{
		}
		
		public void Disable()
		{
		}
		
		public void Rename(string newFileName)
		{
		}
		
		public void Dispose()
		{
		}
	}
}
