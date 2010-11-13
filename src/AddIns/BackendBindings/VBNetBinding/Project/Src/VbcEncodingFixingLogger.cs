// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Framework;
using System.IO;

namespace ICSharpCode.VBNetBinding
{
	/// <summary>
	/// Fixes SD2-995 : Special characters not correctly encoded for languages others than English
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vbc")]
	public sealed class VbcEncodingFixingLogger : IMSBuildLoggerFilter
	{
		public IMSBuildChainedLoggerFilter CreateFilter(MSBuildEngine engine, IMSBuildChainedLoggerFilter nextFilter)
		{
			return new VbcLoggerImpl(nextFilter);
		}
		
		sealed class VbcLoggerImpl : IMSBuildChainedLoggerFilter
		{
			readonly IMSBuildChainedLoggerFilter nextFilter;
			
			public VbcLoggerImpl(IMSBuildChainedLoggerFilter nextFilter)
			{
				this.nextFilter = nextFilter;
			}
			
			static string FixEncoding(string text)
			{
				if (text == null)
					return text;
				return Encoding.Default.GetString(ICSharpCode.SharpDevelop.Util.ProcessRunner.OemEncoding.GetBytes(text));
			}
			
			public void HandleError(BuildError error)
			{
				error.ErrorText = FixEncoding(error.ErrorText);
				error.FileName = FixEncoding(error.FileName);
				error.Column = FixColumn(error.FileName, error.Line, error.Column);
				nextFilter.HandleError(error);
			}
			
			public void HandleBuildEvent(Microsoft.Build.Framework.BuildEventArgs e)
			{
				nextFilter.HandleBuildEvent(e);
				if (e is TaskFinishedEventArgs && lastFile != null) {
					lastFile.Close();
					lastFile = null;
				}
			}
			
			string lastFileName, lastLineText;
			StreamReader lastFile;
			int lastLine;
			
			// fixes SD-1746 - VB compiler errors are shown in incorrect column if the line contains tabs
			// (http://bugtracker.sharpdevelop.net/issue/ViewIssue.aspx?id=1746&PROJID=4)
			int FixColumn(string fileName, int line, int column)
			{
				if (!File.Exists(fileName) || line < 1 || column < 1)
					return column;
				
				if (fileName != lastFileName || line < lastLine || lastFile == null) {
					if (lastFile != null)
						lastFile.Close();
					lastFile = new StreamReader(fileName);
					lastFileName = fileName;
					lastLineText = "";
					lastLine = 0;
				}
				
				while (lastLine < line && lastLineText != null) {
					lastLineText = lastFile.ReadLine();
					lastLine++;
				}
				
				if (!string.IsNullOrEmpty(lastLineText)) {
					for (int i = 0; i < column && i < lastLineText.Length; i++) {
						if (lastLineText[i] == '\t')
							column -= 3;
					}
				}
				
				return column;
			}
		}
	}
}
