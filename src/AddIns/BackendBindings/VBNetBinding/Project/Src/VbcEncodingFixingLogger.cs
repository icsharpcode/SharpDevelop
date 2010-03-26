// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Framework;
using System.IO;

namespace VBNetBinding
{
	/// <summary>
	/// Fixes SD2-995 : Special characters not correctly encoded for languages others than English
	/// </summary>
	public sealed class VbcEncodingFixingLogger : IMSBuildLoggerFilter
	{
		public IMSBuildChainedLoggerFilter CreateFilter(MSBuildEngine engine, IMSBuildChainedLoggerFilter nextFilter)
		{
			return new VbcLoggerImpl(engine, nextFilter);
		}
		
		sealed class VbcLoggerImpl : IMSBuildChainedLoggerFilter
		{
			readonly MSBuildEngine engineWorker;
			readonly IMSBuildChainedLoggerFilter nextFilter;
			
			public VbcLoggerImpl(MSBuildEngine engineWorker, IMSBuildChainedLoggerFilter nextFilter)
			{
				this.engineWorker = engineWorker;
				this.nextFilter = nextFilter;
			}
			
			static string FixEncoding(string text)
			{
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
