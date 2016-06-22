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
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.TypeScriptBinding.Hosting;

namespace ICSharpCode.TypeScriptBinding
{
	public class TypeScriptUnresolvedFile : IUnresolvedFile
	{
		List<IUnresolvedTypeDefinition> typeDefinitions = new List<IUnresolvedTypeDefinition>();
		List<Error> errors = new List<Error>();
		
		public TypeScriptUnresolvedFile(FileName fileName)
		{
			FileName = fileName;
		}
		
		public string FileName { get; private set; }
		
		public DateTime? LastWriteTime { get; set; }
		
		public IList<IUnresolvedTypeDefinition> TopLevelTypeDefinitions {
			get { return typeDefinitions; }
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
			return null;
		}
		
		public IUnresolvedTypeDefinition GetInnermostTypeDefinition(TextLocation location)
		{
			return null;
		}
		
		public IUnresolvedMember GetMember(TextLocation location)
		{
			return null;
		}
		
		public void AddNavigation(NavigationBarItem[] navigation, ITextSource textSource)
		{
			IDocument document = new TextDocument(textSource);
			AddNavigationInfo(navigation, document);
		}
		
		public void AddNavigationInfo(NavigationBarItem[] navigation, IDocument document)
		{
			foreach (NavigationBarItem item in navigation) {
				switch (item.kind) {
					case "class":
						AddClass(item, document);
						break;
					case "interface":
						AddInterface(item, document);
						break;
					case "module":
						AddModule(item, document);
						break;
				}
			}
		}
		
		TypeScriptUnresolvedTypeDefinition AddClass(NavigationBarItem item, IDocument document)
		{
			var defaultClass = new TypeScriptUnresolvedTypeDefinition(item.text) {
				UnresolvedFile = this
			};
			defaultClass.BodyRegion = item.ToRegionStartingFromOpeningCurlyBrace(document);
			defaultClass.Region = defaultClass.BodyRegion;
			
			TypeScriptUnresolvedTypeDefinition parentClass = FindParentClass(defaultClass);
			if (parentClass != null) {
				defaultClass.Namespace = parentClass.FullName;
				parentClass.NestedTypes.Add(defaultClass);
			} else {
				typeDefinitions.Add(defaultClass);
			}
			AddMethods(defaultClass, item.childItems, document);
			return defaultClass;
		}
		
		void AddMethods(TypeScriptUnresolvedTypeDefinition parent, NavigationBarItem[] childItems, IDocument document)
		{
			foreach (NavigationBarItem item in childItems) {
				switch (item.kind) {
					case "method":
					case "constructor":
						AddMethod(parent, item, document);
						break;
				}
			}
		}
		
		void AddInterface(NavigationBarItem item, IDocument document)
		{
			TypeScriptUnresolvedTypeDefinition c = AddClass(item, document);
			c.Kind = TypeKind.Interface;
		}
		
		void AddModule(NavigationBarItem item, IDocument document)
		{
			if (IsGlobalModule(item)) {
				return;
			}
			
			TypeScriptUnresolvedTypeDefinition c = AddClass(item, document);
			c.Kind = TypeKind.Module;
		}
		
		static bool IsGlobalModule(NavigationBarItem item)
		{
			return item.text == "<global>";
		}
		
		void AddMethod(TypeScriptUnresolvedTypeDefinition parent, NavigationBarItem item, IDocument document)
		{
			var method = new DefaultUnresolvedMethod(parent, item.text);
			UpdateMethodRegions(method, item, document);
			parent.Members.Add(method);
		}
		
		void UpdateMethodRegions(DefaultUnresolvedMethod method, NavigationBarItem item, IDocument document)
		{
			DomRegion region = item.ToRegionStartingFromOpeningCurlyBrace(document);
			method.Region = new DomRegion(
				region.BeginLine,
				region.BeginColumn,
				region.BeginLine,
				region.BeginColumn);
			method.BodyRegion = region;
		}
		
		TypeScriptUnresolvedTypeDefinition FindParentClass(TypeScriptUnresolvedTypeDefinition c)
		{
			return typeDefinitions
				.OfType<TypeScriptUnresolvedTypeDefinition>()
				.FirstOrDefault(parent => IsInside(c.BodyRegion, parent.BodyRegion));
		}
		
		bool IsInside(DomRegion childBodyRegion, DomRegion parentBodyRegion)
		{
			return (childBodyRegion.BeginLine >= parentBodyRegion.BeginLine) &&
				(childBodyRegion.EndLine <= parentBodyRegion.EndLine);
		}
	}
}
