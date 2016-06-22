// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Build.Framework;

namespace ICSharpCode.CodeAnalysis
{
	/// <summary>
	/// Not all FxCop messages come with correct line number information,
	/// so this logger fixes the position.
	/// Additionally, it registers the context menu containing the 'suppress message' command.
	/// </summary>
	public class FxCopLogger : IMSBuildLoggerFilter
	{
		public IMSBuildChainedLoggerFilter CreateFilter(IMSBuildLoggerContext context, IMSBuildChainedLoggerFilter nextFilter)
		{
			context.OutputTextLine(StringParser.Parse("${res:ICSharpCode.CodeAnalysis.RunningFxCopOn} " + context.ProjectFileName.GetFileNameWithoutExtension()));
			return new FxCopLoggerImpl(context, nextFilter);
		}
		
		sealed class FxCopLoggerImpl : IMSBuildChainedLoggerFilter
		{
			readonly IMSBuildLoggerContext context;
			readonly IMSBuildChainedLoggerFilter nextChainElement;
			
			public FxCopLoggerImpl(IMSBuildLoggerContext context, IMSBuildChainedLoggerFilter nextChainElement)
			{
				this.context = context;
				this.nextChainElement = nextChainElement;
			}
			
			public void HandleError(BuildError error)
			{
				LoggingService.Debug("FxCopLogger got " + error.ToString());
				
				string[] moreData = (error.Subcategory ?? "").Split('|');
				string checkId = error.ErrorCode;
				error.ErrorCode = (error.ErrorCode != null) ? error.ErrorCode.Split(':')[0] : null;
				if (FileUtility.IsValidPath(error.FileName) &&
				    Path.GetFileName(error.FileName) == "SharpDevelop.CodeAnalysis.targets")
				{
					error.FileName = null;
				}
				var project = context.Project;
				if (project != null) {
					if (error.FileName != null) {
						int pos = error.FileName.IndexOf("positionof#", StringComparison.Ordinal);
						if (pos >= 0) {
							ICompilation compilation = SD.ParserService.GetCompilation(project);
							string memberName = error.FileName.Substring(pos+11);
							DomRegion filePos = GetPosition(compilation, memberName);
							if (!filePos.IsEmpty) {
								error.FileName = filePos.FileName ?? "";
								error.Line = filePos.BeginLine;
								error.Column = filePos.BeginColumn;
							} else {
								error.FileName = null;
							}
						}
					}
					
					if (moreData.Length > 1 && !string.IsNullOrEmpty(moreData[0])) {
						error.Tag = new FxCopTaskTag {
							Project = project,
							TypeName = moreData[0],
							MemberName = moreData[1],
							Category = error.HelpKeyword,
							CheckID = checkId
						};
					} else {
						error.Tag = new FxCopTaskTag {
							Project = project,
							Category = error.HelpKeyword,
							CheckID = checkId
						};
					}
					error.ContextMenuAddInTreeEntry = "/SharpDevelop/Pads/ErrorList/CodeAnalysisTaskContextMenu";
					if (moreData.Length > 2) {
						(error.Tag as FxCopTaskTag).MessageID = moreData[2];
					}
				}
				nextChainElement.HandleError(error);
			}
			
			public void HandleBuildEvent(Microsoft.Build.Framework.BuildEventArgs e)
			{
				nextChainElement.HandleBuildEvent(e);
			}
			
			static DomRegion GetPosition(ICompilation compilation, string memberName)
			{
				// memberName is a special syntax used by our FxCop task:
				// className#memberName
				int pos = memberName.IndexOf('#');
				if (pos <= 0)
					return DomRegion.Empty;
				string className = memberName.Substring(0, pos);
				memberName = memberName.Substring(pos + 1);
				return SuppressMessageCommand.GetPosition(compilation, className, memberName);
			}
		}
	}
	
	public class FxCopTaskTag
	{
		public IProject Project { get; set; }
		public string TypeName { get; set; }
		public string MemberName { get; set; }
		public string Category { get; set; }
		public string CheckID { get; set; }
		public string MessageID { get; set; }
	}
}
