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
				nextFilter.HandleError(error);
			}
			
			public void HandleBuildEvent(Microsoft.Build.Framework.BuildEventArgs e)
			{
				nextFilter.HandleBuildEvent(e);
			}
		}
	}
}
