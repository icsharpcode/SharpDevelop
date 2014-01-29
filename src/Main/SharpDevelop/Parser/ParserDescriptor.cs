// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text.RegularExpressions;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Parser
{
	sealed class ParserDescriptor
	{
		Codon codon;
		Type parserType;
		
		public IParser CreateParser()
		{
			if (codon != null)
				return (IParser)codon.AddIn.CreateObject(codon.Properties["class"]);
			else
				return (IParser)Activator.CreateInstance(parserType);
		}

		public string Language { get; private set; }

		public Regex SupportedFilenamePattern { get; private set; }

		public bool CanParse(FileName fileName)
		{
			return SupportedFilenamePattern.IsMatch(fileName);
		}

		public ParserDescriptor(Codon codon)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			this.codon = codon;
			this.Language = codon.Id;
			this.SupportedFilenamePattern = new Regex(codon.Properties["supportedfilenamepattern"], RegexOptions.IgnoreCase);
		}
		
		public ParserDescriptor(Type parserType, string language, Regex supportedFilenamePattern)
		{
			if (parserType == null)
				throw new ArgumentNullException("parserType");
			if (language == null)
				throw new ArgumentNullException("language");
			if (supportedFilenamePattern == null)
				throw new ArgumentNullException("supportedFilenamePattern");
			this.parserType = parserType;
			this.Language = language;
			this.SupportedFilenamePattern = supportedFilenamePattern;
		}
	}
}
