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
