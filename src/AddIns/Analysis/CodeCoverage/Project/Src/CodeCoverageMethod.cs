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
using System.Xml.Linq;
using ICSharpCode.Core;

namespace ICSharpCode.CodeCoverage
{
	public class CodeCoverageMethod
	{
		readonly string name = String.Empty;
		readonly string className = String.Empty;
		readonly string fullClassName = String.Empty;
		readonly string classNamespace = String.Empty;
		readonly List<CodeCoverageSequencePoint> sequencePoints = new List<CodeCoverageSequencePoint>();
		
		public CodeCoverageMethod(string name, string className)
		{
			this.name = name;
			this.fullClassName = className;
			
			int index = fullClassName.LastIndexOf('.');
			if (index > 0) {
				this.classNamespace = fullClassName.Substring(0, index);
				this.className = fullClassName.Substring(index + 1);
			} else {
				this.className = fullClassName;
			}
		}
		
		public CodeCoverageMethod(string className, XElement reader)
		    : this (className, reader, null) {}
		public CodeCoverageMethod(string className, XElement reader, CodeCoverageResults parent)
			: this(className, new CodeCoverageMethodElement(reader, parent))
		{
		}
		
		public CodeCoverageMethod(string className, CodeCoverageMethodElement element)
			: this(element.MethodName, className)
		{
			IsProperty = element.IsProperty && IsPropertyMethodName();
			IsGetter = element.IsGetter;
			IsSetter = element.IsSetter;

			this.IsVisited = element.IsVisited;
			this.BranchCoverage = element.BranchCoverage;
			this.SequencePointsCount = element.SequencePointsCount;
			this.sequencePoints = element.SequencePoints;
			this.FileID = element.FileID;
		}
		
		/// <summary>
		/// Returns true if the method is a getter or setter method for a property.
		/// </summary>
		public bool IsProperty { get; private set; }
		public bool IsGetter { get; private set; }
		public bool IsSetter { get; private set; }
		
		public bool IsVisited { get; private set; }
		public decimal BranchCoverage { get; private set; }
		public int SequencePointsCount { get; private set; }
		public string FileID { get; private set; }

		bool IsPropertyMethodName()
		{
			return name.Contains("get_") || name.Contains("set_");
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
			return index > 0 ? ns.Substring(0, index) : ns;
		}
		
		public List<CodeCoverageSequencePoint> SequencePoints {
			get { return sequencePoints; }
		}
		
		public int VisitedSequencePointsCount {
			get {
				int count = 0;
				foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
					if (sequencePoint.VisitCount != 0) {
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
		
		public int GetVisitedCodeLength()
		{
			int total = 0;
			foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
				if (sequencePoint.FileID == this.FileID && sequencePoint.VisitCount != 0) {
					total += sequencePoint.Length;
				}
			}
			return total;
		}
		
		public int GetUnvisitedCodeLength()
		{
			int total = 0;
			foreach (CodeCoverageSequencePoint sequencePoint in sequencePoints) {
				if (sequencePoint.FileID == this.FileID && sequencePoint.VisitCount == 0) {
					total += sequencePoint.Length;
				}
			}
			return total;
		}
		
		public List<CodeCoverageSequencePoint> GetSequencePoints(string fileName)
		{
			var matchedSequencePoints = new List<CodeCoverageSequencePoint>();
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
			return prefix.Length > 0 ? String.Concat(prefix, ".", name) : name;
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
			var items = new List<string>();
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
			var matchedMethods = new List<CodeCoverageMethod>();
			namespaceStartsWith += ".";
			foreach (CodeCoverageMethod method in methods) {
				if ((method.ClassNamespace + ".").StartsWith(namespaceStartsWith, StringComparison.Ordinal)) {
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
			var matchedMethods = new List<CodeCoverageMethod>();
			foreach (CodeCoverageMethod method in methods) {
				if (method.ClassName == className && method.ClassNamespace == ns) {
					matchedMethods.Add(method);
				}
			}
			return matchedMethods;
		}
		
		public static List<string> GetClassNames(List<CodeCoverageMethod> methods, string ns)
		{
			var names = new List<string>();
			foreach (CodeCoverageMethod method in methods) {
				if (method.ClassNamespace == ns && !names.Contains(method.ClassName)) {
					names.Add(method.ClassName);
				}
			}
			return names;
		}
	}
}
