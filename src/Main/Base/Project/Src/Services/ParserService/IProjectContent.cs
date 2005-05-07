using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Core
{
	public interface IProjectContent : IDisposable
	{
		XmlDoc XmlDoc {
			get;
		}
		
		ICollection<IClass> Classes {
			get;
		}
		
		/// <summary>
		/// Gets the properties of the language this project content was written in.
		/// </summary>
		LanguageProperties Language {
			get;
		}
		
		string GetXmlDocumentation(string memberTag);
		
		Hashtable AddClassToNamespaceList(IClass addClass);
		void UpdateCompilationUnit(ICompilationUnit oldUnit, ICompilationUnit parserOutput, string fileName, bool updateCommentTags);
		
		IClass GetClass(string typeName);
		bool NamespaceExists(string name);
		ArrayList GetNamespaceContents(string subNameSpace);
		
		IClass GetClass(string typeName, LanguageProperties language, bool lookInReferences);
		bool NamespaceExists(string name, LanguageProperties language, bool lookInReferences);
		void AddNamespaceContents(ArrayList list, string subNameSpace, LanguageProperties language, bool lookInReferences);
		
		string SearchNamespace(string name, ICompilationUnit unit, int caretLine, int caretColumn);
		IClass SearchType(string name, IClass curType, int caretLine, int caretColumn);
		IClass SearchType(string name, IClass curType, ICompilationUnit unit, int caretLine, int caretColumn);
		
		Position GetPosition(string fullMemberName);
	}
}
