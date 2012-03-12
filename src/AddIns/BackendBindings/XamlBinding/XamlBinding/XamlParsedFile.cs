// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Xml;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.XamlBinding
{
	public sealed class XamlParsedFile : IParsedFile
	{
		FileName fileName;
		AXmlDocument document;
		List<Error> errors;
		ITypeDefinition xamlTypeDefinition;
		
		XamlParsedFile(FileName fileName, AXmlDocument document)
		{
			this.fileName = fileName;
			this.document = document;
			this.errors = new List<Error>();
		}
		
		public static XamlParsedFile Create(FileName fileName, string fileContent, AXmlDocument document)
		{
			XamlParsedFile file = new XamlParsedFile(fileName, document);
			
			file.errors.AddRange(document.SyntaxErrors.Select(err => new Error(ErrorType.Error, err.Description)));
			
			file.lastWriteTime = DateTime.UtcNow;
			return file;
		}
		
		public string FileName {
			get { return fileName; }
		}
		
		DateTime lastWriteTime = DateTime.UtcNow;
		
		public DateTime LastWriteTime {
			get { return lastWriteTime; }
		}
		
		public IList<IUnresolvedTypeDefinition> TopLevelTypeDefinitions {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IList<IUnresolvedAttribute> AssemblyAttributes {
			get { return EmptyList<IUnresolvedAttribute>.Instance; }
		}
		
		public IList<IUnresolvedAttribute> ModuleAttributes {
			get { return EmptyList<IUnresolvedAttribute>.Instance; }
		}
		
		public IList<Error> Errors {
			get { return errors; }
		}
		
		public IUnresolvedTypeDefinition GetTopLevelTypeDefinition(TextLocation location)
		{
			throw new NotImplementedException();
		}
		
		public IUnresolvedTypeDefinition GetInnermostTypeDefinition(TextLocation location)
		{
			throw new NotImplementedException();
		}
		
		public IUnresolvedMember GetMember(TextLocation location)
		{
			throw new NotImplementedException();
		}
		
		public ITypeResolveContext GetTypeResolveContext(ICompilation compilation, TextLocation loc)
		{
			throw new NotImplementedException();
		}
	}
}
