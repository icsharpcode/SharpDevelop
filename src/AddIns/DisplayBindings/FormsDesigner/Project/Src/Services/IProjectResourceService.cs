// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.IO;
using System.Linq;

namespace ICSharpCode.FormsDesigner.Services
{
	public interface IProjectResourceService
	{
		IProjectResourceInfo GetProjectResource(CodePropertyReferenceExpression propRef);
		bool DesignerSupportsProjectResources { get; set; }
		string ProjectResourceKey { get; set; }
	}
	
	public interface IMessageService
	{
		void ShowOutputPad();
		void AppendTextToBuildMessages(string text);
		void ShowException(Exception ex, string message);
		string CodeStatementToString(CodeStatement statement);
	}
	
	public interface IProjectResourceInfo
	{
		string ResourceFile { get; }
		string ResourceKey { get; }
		object OriginalValue { get; }
	}
}
