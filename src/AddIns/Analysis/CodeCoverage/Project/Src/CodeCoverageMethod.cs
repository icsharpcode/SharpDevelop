// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Reflection;
using ICSharpCode.Core;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageMethod
	{
		string name = String.Empty;
		string className = String.Empty;
		string fullClassName = String.Empty;
		string classNamespace = String.Empty;
		MethodAttributes methodAttributes;
		List<CodeCoverageSequencePoint> sequencePoints = new List<CodeCoverageSequencePoint>();
		
		public CodeCoverageMethod(string name, string className) : this(name, className, MethodAttributes.Public)
		{
		}
		
		public CodeCoverageMethod(string name, string className, MethodAttributes methodAttributes)
		{
			this.name = name;
			this.fullClassName = className;
			this.methodAttributes = methodAttributes;
			
			int index = fullClassName.LastIndexOf('.');
			if (index > 0) {
				this.classNamespace = fullClassName.Substring(0, index);
				this.className = fullClassName.Substring(index + 1);
			} else {
				this.className = fullClassName;
			}
		}
		
		/// <summary>
		/// Determines whether this method has been excluded.
		/// </summary>
		/// <remarks>
		/// A method is considered excluded if all of its
		/// sequence points have been marked as excluded.
		/// </remarks>
		public bool IsExcluded {
			get {
				foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
					if (!sequencePoint.IsExcluded) {
						return false;
					}
				}
				return true;
			}
		}
		
		/// <summary>
		/// Returns true if the method is a getter or setter method for a property.
		/// </summary>
		public bool IsProperty {
			get { 
				return (methodAttributes & MethodAttributes.SpecialName) == MethodAttributes.SpecialName && IsPropertyMethodName(name);
			}
		}
		
		public string Name {
			get { return name; }
		}
		
		public string ClassName {
			get { return className; }
		}
		
		/// <summary>
		/// Returns the full class name including the namespace prefix.
		/// </summary>
		public string FullClassName {
			get { return fullClassName; }
		}
		
		public string ClassNamespace {
			get { return classNamespace; }
		}
		
		public string RootNamespace {
			get { return GetRootNamespace(classNamespace); }
		}
		
		public static string GetRootNamespace(string ns)
		{
			int index = ns.IndexOf('.');
			if (index > 0) {
				return ns.Substring(0, index);
			}
			return ns;
		}
		
		public List<CodeCoverageSequencePoint> SequencePoints {
			get { return sequencePoints; }
		}
		
		public int VisitedSequencePointsCount {
			get {
				int count = 0;
				foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
					if (sequencePoint.VisitCount > 0) {
						count++;
					}
				}
				return count;
			}
		}
		
		public int NotVisitedSequencePointsCount {
			get {
				int count = 0;
				foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
					if (sequencePoint.VisitCount == 0) {
						count++;
					}
				}
				return count;
			}
		}
		
		public List<CodeCoverageSequencePoint> GetSequencePoints(string fileName)
		{
			List<CodeCoverageSequencePoint> matchedSequencePoints = new List<CodeCoverageSequencePoint>();
			foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
				if (FileUtility.IsEqualFileName(fileName, sequencePoint.Document)) {
					matchedSequencePoints.Add(sequencePoint);
				}
			}
			return matchedSequencePoints;
		}
		
		/// <summary>
		/// Gets the next namespace level given the parent namespace.
		/// </summary>
		public static string GetChildNamespace(string fullNamespace, string parentNamespace)
		{
			string end = fullNamespace.Substring(parentNamespace.Length + 1);
			return GetRootNamespace(end);
		}
		
		/// <summary>
		/// Adds the child namespace to the namespace prefix.
		/// </summary>
		public static string GetFullNamespace(string prefix, string name)
		{
			if (prefix.Length > 0) {
				return String.Concat(prefix, ".", name);
			}
			return name;
		}
		
		/// <summary>
		/// Gets all child namespaces that starts with the specified string.
		/// </summary>
		/// <remarks>
		/// If the starts with string is 'ICSharpCode' and there is a code coverage
		/// method with a namespace of 'ICSharpCode.XmlEditor.Tests', then this
		/// method will return 'XmlEditor' as one of its strings.
		/// </remarks>
		public static List<string> GetChildNamespaces(List<CodeCoverageMethod> methods, string parentNamespace) {
			List<string> items = new List<string>();
			foreach (CodeCoverageMethod method in methods) {
				string classNamespace = method.ClassNamespace;
				string dottedParentNamespace = parentNamespace + ".";
				if (classNamespace.Length > parentNamespace.Length && classNamespace.StartsWith(dottedParentNamespace)) {
					string ns = CodeCoverageMethod.GetChildNamespace(method.ClassNamespace, parentNamespace);
					if (!items.Contains(ns)) {
						items.Add(ns);
					}
				}
			}
			return items;
		}
		
		/// <summary>
		/// Gets all methods whose namespaces starts with the specified string.
		/// </summary>
		public static List<CodeCoverageMethod> GetAllMethods(List<CodeCoverageMethod> methods, string namespaceStartsWith)
		{
			List<CodeCoverageMethod> matchedMethods = new List<CodeCoverageMethod>();
			foreach (CodeCoverageMethod method in methods) {
				if (method.ClassNamespace.StartsWith(namespaceStartsWith)) {
					matchedMethods.Add(method);
				}
			}
			return matchedMethods;
		}
		
		/// <summary>
		/// Gets only those methods for the specified class.
		/// </summary>
		public static List<CodeCoverageMethod> GetMethods(List<CodeCoverageMethod> methods, string ns, string className)
		{
			List<CodeCoverageMethod> matchedMethods = new List<CodeCoverageMethod>();
			foreach (CodeCoverageMethod method in methods) {
				if (method.ClassName == className && method.ClassNamespace == ns) {
					matchedMethods.Add(method);
				}
			}
			return matchedMethods;
		}
		
		public static List<string> GetClassNames(List<CodeCoverageMethod> methods, string ns)
		{
			List<string> names = new List<string>();
			foreach (CodeCoverageMethod method in methods) {
				if (method.ClassNamespace == ns && !names.Contains(method.ClassName)) {
					names.Add(method.ClassName);
				}
			}
			return names;
		}
		
		static bool IsPropertyMethodName(string name)
		{
			return name.StartsWith("get_") || name.StartsWith("set_");
		}
	}
}
