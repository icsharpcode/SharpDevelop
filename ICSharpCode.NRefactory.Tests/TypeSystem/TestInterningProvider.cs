// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.TypeSystem
{
	//* Not a real unit test
	[TestFixture]
	public class TestInterningProvider : IInterningProvider
	{
		sealed class ReferenceComparer : IEqualityComparer<object>
		{
			public new bool Equals(object a, object b)
			{
				return ReferenceEquals(a, b);
			}
			
			public int GetHashCode(object obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}
		}
		
		sealed class InterningComparer : IEqualityComparer<ISupportsInterning>
		{
			public bool Equals(ISupportsInterning x, ISupportsInterning y)
			{
				return x.EqualsForInterning(y);
			}
			
			public int GetHashCode(ISupportsInterning obj)
			{
				return obj.GetHashCodeForInterning();
			}
		}
		
		sealed class ListComparer : IEqualityComparer<IEnumerable<object>>
		{
			public bool Equals(IEnumerable<object> a, IEnumerable<object> b)
			{
				if (a.GetType() != b.GetType())
					return false;
				return Enumerable.SequenceEqual(a, b, new ReferenceComparer());
			}
			
			public int GetHashCode(IEnumerable<object> obj)
			{
				int hashCode = obj.GetType().GetHashCode();
				unchecked {
					foreach (object o in obj) {
						hashCode *= 27;
						hashCode += RuntimeHelpers.GetHashCode(o);
					}
				}
				return hashCode;
			}
		}
		
		HashSet<object> uniqueObjectsPreIntern = new HashSet<object>(new ReferenceComparer());
		HashSet<object> uniqueObjectsPostIntern = new HashSet<object>(new ReferenceComparer());
		Dictionary<object, object> byValueDict = new Dictionary<object, object>();
		Dictionary<ISupportsInterning, ISupportsInterning> supportsInternDict = new Dictionary<ISupportsInterning, ISupportsInterning>(new InterningComparer());
		Dictionary<IEnumerable<object>, IEnumerable<object>> listDict = new Dictionary<IEnumerable<object>, IEnumerable<object>>(new ListComparer());
		
		public T Intern<T>(T obj) where T : class
		{
			if (obj == null)
				return null;
			uniqueObjectsPreIntern.Add(obj);
			ISupportsInterning s = obj as ISupportsInterning;
			if (s != null) {
				ISupportsInterning output;
				if (supportsInternDict.TryGetValue(s, out output)) {
					obj = (T)output;
				} else {
					s.PrepareForInterning(this);
					if (supportsInternDict.TryGetValue(s, out output))
						obj = (T)output;
					else
						supportsInternDict.Add(s, s);
				}
			} else if (obj is string || obj is IType || obj is bool || obj is int) {
				object output;
				if (byValueDict.TryGetValue(obj, out output))
					obj = (T)output;
				else
					byValueDict.Add(obj, obj);
			}
			uniqueObjectsPostIntern.Add(obj);
			return obj;
		}
		
		public IList<T> InternList<T>(IList<T> list) where T : class
		{
			if (list == null)
				return null;
			uniqueObjectsPreIntern.Add(list);
			for (int i = 0; i < list.Count; i++) {
				T oldItem = list[i];
				T newItem = Intern(oldItem);
				if (oldItem != newItem) {
					if (list.IsReadOnly)
						list = new T[list.Count];
					list[i] = newItem;
				}
			}
			if (!list.IsReadOnly)
				list = new ReadOnlyCollection<T>(list);
			IEnumerable<object> output;
			if (listDict.TryGetValue(list, out output))
				list = (IList<T>)output;
			else
				listDict.Add(list, list);
			uniqueObjectsPostIntern.Add(list);
			return list;
		}
		
		[Test]
		public void PrintStatistics()
		{
			foreach (var c in TreeTraversal.PreOrder(CecilLoaderTests.Mscorlib.GetClasses(), c => c.InnerClasses)) {
				Intern(c.Namespace);
				Intern(c.Name);
				foreach (IMember m in c.Members) {
					Intern(m);
				}
			}
			
			var stats =
				from obj in uniqueObjectsPreIntern
				group 1 by obj.GetType() into g
				join g2 in (from obj in uniqueObjectsPostIntern group 1 by obj.GetType()) on g.Key equals g2.Key
				orderby g.Key.FullName
				select new { Type = g.Key, PreCount = g.Count(), PostCount = g2.Count() };
			foreach (var element in stats) {
				Console.WriteLine(element.Type + ": " + element.PostCount + "/" + element.PreCount);
			}
			Console.WriteLine(stats.Sum(r => r.PostCount * SizeOf(r.Type)) / 1024 + " KB / "
			                  + stats.Sum(r => r.PreCount * SizeOf(r.Type)) / 1024 + " KB");
		}
		
		static int SizeOf(Type t)
		{
			if (t == typeof(string))
				return 16;
			long start = GC.GetTotalMemory(true);
			object o = FormatterServices.GetUninitializedObject(t);
			long stop = GC.GetTotalMemory(true);
			GC.KeepAlive(o);
			return (int)Math.Max(8, stop - start);
		}
	}//*/
}
