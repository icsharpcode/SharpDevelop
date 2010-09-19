// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Threading;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.Svn.Commands;
using SharpSvn.Diff;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Description of SvnDiffProvider.
	/// </summary>
	public class SvnDiffProvider : IDiffProvider
	{
		public Stream GetDiff(string fileName, ITextBuffer modifiedBuffer)
		{
			if (!RegisterEventsCommand.CanBeVersionControlledFile(fileName))
				return null;
			
			SvnFileDiff diff;
			string tempFile = Path.GetTempFileName();
			MemoryStream stream = new MemoryStream();

			File.WriteAllText(tempFile, modifiedBuffer.Text);
			SvnFileDiff.TryCreate(fileName, tempFile, new SvnFileDiffArgs(), out diff);
			diff.WriteDifferences(stream, new SvnDiffWriteDifferencesArgs());
			File.Delete(tempFile);
			
			

			return stream;
		}
	}
	
	public class SvnBaseVersionProvider : IDocumentBaseVersionProvider
	{
		public Stream OpenBaseVersion(string fileName)
		{
			throw new NotImplementedException();
		}
	}
}
