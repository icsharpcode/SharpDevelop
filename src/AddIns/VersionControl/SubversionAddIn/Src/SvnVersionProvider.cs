// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.Svn.Commands;
using SharpSvn;

namespace ICSharpCode.Svn
{
	public class SvnVersionProvider : IDocumentVersionProvider
	{
		public Stream OpenBaseVersion(string fileName)
		{
			if (!SvnClientWrapper.IsInSourceControl(fileName))
				return null;
			
			using (SvnClientWrapper client = new SvnClientWrapper())
				return client.OpenBaseVersion(fileName);
		}
		
		public IDisposable WatchBaseVersionChanges(string fileName, EventHandler callback)
		{
			return null;
		}
	}
}
