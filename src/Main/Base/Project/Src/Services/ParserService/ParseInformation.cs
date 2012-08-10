// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Represents the result of a parser operation.
	/// Note: the parser service only stores the IUnresolvedFile, not any of the extra information.
	/// The extra information is only provided to listeners of the ParseInformationUpdated event.
	/// Those listeners may then decide to store the extra information (e.g. the TaskService stores TagComments).
	/// </summary>
	public class ParseInformation
	{
		readonly IUnresolvedFile unresolvedFile;
		IList<TagComment> tagComments = new List<TagComment>();
		readonly bool isFullParseInformation;
		
		public ParseInformation(IUnresolvedFile unresolvedFile, bool isFullParseInformation)
		{
			if (unresolvedFile == null)
				throw new ArgumentNullException("unresolvedFile");
			this.unresolvedFile = unresolvedFile;
			this.isFullParseInformation = isFullParseInformation;
		}
		
		/// <summary>
		/// Gets whether this parse information contains 'extra' data.
		/// True = extra data is provided (e.g. folding information).
		/// False = Only UnresolvedFile and TagComments are provided.
		/// </summary>
		public bool IsFullParseInformation {
			get { return isFullParseInformation; }
		}
		
		public IUnresolvedFile UnresolvedFile {
			get { return unresolvedFile; }
		}
		
		public FileName FileName {
			get { return FileName.Create(unresolvedFile.FileName); }
		}
		
		public IList<TagComment> TagComments {
			get { return tagComments; }
		}
	}
}
