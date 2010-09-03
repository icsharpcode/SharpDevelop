// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public sealed class ParserDescriptor
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

		public string[] Supportedextensions { get; private set; }

		public bool CanParse(string fileName)
		{
			string fileExtension = Path.GetExtension(fileName);
			foreach (string ext in Supportedextensions) {
				if (string.Equals(fileExtension, ext, StringComparison.OrdinalIgnoreCase)) {
					return true;
				}
			}
			return false;
		}

		public ParserDescriptor(Codon codon)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			this.codon = codon;
			this.Language = codon.Id;
			this.Supportedextensions = codon.Properties["supportedextensions"].Split(';');
		}
		
		public ParserDescriptor(Type parserType, string language, string[] supportedExtensions)
		{
			if (parserType == null)
				throw new ArgumentNullException("parserType");
			if (language == null)
				throw new ArgumentNullException("language");
			if (supportedExtensions == null)
				throw new ArgumentNullException("supportedExtensions");
			this.parserType = parserType;
			this.Language = language;
			this.Supportedextensions = supportedExtensions;
		}
	}
}
