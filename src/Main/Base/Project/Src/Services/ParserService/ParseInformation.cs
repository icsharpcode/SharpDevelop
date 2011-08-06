// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Represents the result of a parser operation.
	/// Note: the parser service only stores the IParsedFile, not any of the extra information.
	/// The extra information is only provided to listeners of the ParseInformationUpdated event.
	/// Those listeners may then decide to store the extra information (e.g. the TaskService stores TagComments).
	/// </summary>
	public class ParseInformation : AbstractAnnotatable
	{
		readonly IParsedFile parsedFile;
		readonly List<TagComment> tagComments = new List<TagComment>();
		
		public ParseInformation(IParsedFile parsedFile)
		{
			if (parsedFile == null)
				throw new ArgumentNullException("parsedFile");
			this.parsedFile = parsedFile;
		}
		
		public IParsedFile ParsedFile {
			get { return parsedFile; }
		}
		
		public FileName FileName {
			get { return FileName.Create(parsedFile.FileName); }
		}
		
		public IProjectContent ProjectContent {
			get { return parsedFile.ProjectContent; }
		}
		
		public IEnumerable<TagComment> TagComments {
			get { return tagComments; }
		}
	}
}
