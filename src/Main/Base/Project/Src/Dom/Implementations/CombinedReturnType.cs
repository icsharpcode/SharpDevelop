// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Combines multiple return types.
	/// </summary>
	public class CombinedReturnType : IReturnType
	{
		IList<IReturnType> baseTypes;
		
		string fullName;
		string name;
		string @namespace;
		string dotnetName;
		
		public CombinedReturnType(IList<IReturnType> baseTypes, string fullName, string name, string @namespace, string dotnetName)
		{
			this.baseTypes = baseTypes;
			this.fullName = fullName;
			this.name = name;
			this.@namespace = @namespace;
			this.dotnetName = dotnetName;
		}
		
		public IList<IReturnType> BaseTypes {
			get {
				return baseTypes;
			}
		}
		
		List<T> Combine<T>(Converter<IReturnType, List<T>> conv) where T : IMember
		{
			int count = baseTypes.Count;
			if (count == 0)
				return null;
			List<T> list = null;
			foreach (IReturnType baseType in baseTypes) {
				List<T> newList = conv(baseType);
				if (newList == null)
					continue;
				if (list == null) {
					list = newList;
				} else {
					foreach (T element in newList) {
						bool found = false;
						foreach (T t in list) {
							if (t.CompareTo(element) == 0) {
								found = true;
								break;
							}
						}
						if (!found) {
							list.Add(element);
						}
					}
				}
			}
			return list;
		}
		
		public List<IMethod> GetMethods()
		{
			return Combine<IMethod>(delegate(IReturnType type) { return type.GetMethods(); });
		}
		
		public List<IProperty> GetProperties()
		{
			return Combine<IProperty>(delegate(IReturnType type) { return type.GetProperties(); });
		}
		
		public List<IField> GetFields()
		{
			return Combine<IField>(delegate(IReturnType type) { return type.GetFields(); });
		}
		
		public List<IEvent> GetEvents()
		{
			return Combine<IEvent>(delegate(IReturnType type) { return type.GetEvents(); });
		}
		
		public List<IIndexer> GetIndexers()
		{
			return Combine<IIndexer>(delegate(IReturnType type) { return type.GetIndexers(); });
		}
		
		public string FullyQualifiedName {
			get {
				return fullName;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string Namespace {
			get {
				return @namespace;
			}
		}
		
		public string DotNetName {
			get {
				return dotnetName;
			}
		}
		
		public int ArrayDimensions {
			get {
				return 0;
			}
		}
		
		public bool IsDefaultReturnType {
			get {
				return false;
			}
		}
		
		public IClass GetUnderlyingClass()
		{
			return null;
		}
	}
}
