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
	/// Note: the parser service only stores the IParsedFile, not any of the extra information.
	/// The extra information is only provided to listeners of the ParseInformationUpdated event.
	/// Those listeners may then decide to store the extra information (e.g. the TaskService stores TagComments).
	/// </summary>
	public class ParseInformation : AbstractAnnotatable, IFreezable
	{
		readonly IParsedFile parsedFile;
		IList<TagComment> tagComments = new List<TagComment>();
		readonly bool isFullParseInformation;
		bool isFrozen;
		
		public ParseInformation(IParsedFile parsedFile, bool isFullParseInformation)
		{
			if (parsedFile == null)
				throw new ArgumentNullException("parsedFile");
			this.parsedFile = parsedFile;
			this.isFullParseInformation = isFullParseInformation;
		}
		
		public bool IsFrozen {
			get { return isFrozen; }
		}
		
		public void Freeze()
		{
			if (!isFrozen) {
				
				isFrozen = true;
			}
		}
		
		/// <summary>
		/// Gets whether this parse information contains 'extra' data.
		/// True = extra data is provided (e.g. folding information).
		/// False = Only ParsedFile and TagComments are provided.
		/// </summary>
		public bool IsFullParseInformation {
			get { return isFullParseInformation; }
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
		
		public IList<TagComment> TagComments {
			get { return tagComments; }
		}
	}
}
