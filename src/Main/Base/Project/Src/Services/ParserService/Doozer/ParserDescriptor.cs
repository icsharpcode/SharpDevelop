// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public sealed class ParserDescriptor
	{
		readonly Codon _codon;
		readonly Type _parserType;

		public string Language { get; private set; }
		public List<string> SupportedExtensions { get; private set; }

		public IParser CreateParser()
		{
			if (_codon != null)
				return (IParser)_codon.AddIn.CreateObject(_codon.Properties["class"]);
			return (IParser)Activator.CreateInstance(_parserType);
		}

		public bool CanParse(string fileName)
		{
			string fileExtension = Path.GetExtension(fileName);
			if (SupportedExtensions.Contains(fileExtension))
			{
				// Current file doesn't belong to any project
				if (ProjectService.OpenSolution == null)
					return true;

				// Current file belongs to some project. 
				// Let's ask this project whiether given file by current parser
				foreach (var project in ProjectService.OpenSolution.Projects)
				{
					if (project.IsFileInProject(fileName))
						return project.CanParse(Language, fileExtension);
				}
				return true;
			}
			return false;
		}

		public ParserDescriptor(Codon codon)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			_codon = codon;
			Language = codon.Id;
			SupportedExtensions = new List<string>(codon.Properties["supportedextensions"].Split(';'));
		}
		
		public ParserDescriptor(Type parserType, string language, string[] supportedExtensions)
		{
			if (parserType == null)
				throw new ArgumentNullException("parserType");
			if (language == null)
				throw new ArgumentNullException("language");
			if (supportedExtensions == null)
				throw new ArgumentNullException("supportedExtensions");
			_parserType = parserType;
			Language = language;
			SupportedExtensions = new List<string>(supportedExtensions);
		}
	}
}
