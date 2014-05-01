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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Extension methods used in SharpDevelop.
	/// </summary>
	public static class SharpDevelopExtensions
	{
		#region RaiseEvent
		/// <summary>
		/// Raises the event.
		/// Does nothing if eventHandler is null.
		/// Because the event handler is passed as parameter, it is only fetched from the event field one time.
		/// This makes
		/// <code>MyEvent.RaiseEvent(x,y);</code>
		/// thread-safe
		/// whereas
		/// <code>if (MyEvent != null) MyEvent(x,y);</code>
		/// would not be safe.
		/// </summary>
		/// <remarks>Using this method is only thread-safe under the Microsoft .NET memory model,
		/// not under the less strict memory model in the CLI specification.</remarks>
		[Obsolete("Use 'event EventHandler MyEvent = delegate{};' instead")]
		public static void RaiseEvent(this EventHandler eventHandler, object sender, EventArgs e)
		{
			if (eventHandler != null) {
				eventHandler(sender, e);
			}
		}
		
		/// <summary>
		/// Raises the event.
		/// Does nothing if eventHandler is null.
		/// Because the event handler is passed as parameter, it is only fetched from the event field one time.
		/// This makes
		/// <code>MyEvent.RaiseEvent(x,y);</code>
		/// thread-safe
		/// whereas
		/// <code>if (MyEvent != null) MyEvent(x,y);</code>
		/// would not be safe.
		/// </summary>
		[Obsolete("Use 'event EventHandler MyEvent = delegate{};' instead")]
		public static void RaiseEvent<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
		{
			if (eventHandler != null) {
				eventHandler(sender, e);
			}
		}
		#endregion
		
		#region Task Extensions
		/// <summary>
		/// If the task throws an exception, notifies the message service.
		/// Call this method on asynchronous tasks if you do not care about the result, but do not want
		/// unhandled exceptions to go unnoticed.
		/// </summary>
		public static void FireAndForget(this Task task)
		{
			task.ContinueWith(
				t => {
					if (t.Exception != null) {
						if (t.Exception.InnerExceptions.Count == 1)
							Core.MessageService.ShowException(t.Exception.InnerExceptions[0]);
						else
							Core.MessageService.ShowException(t.Exception);
					}
				}, TaskContinuationOptions.OnlyOnFaulted);
		}
		#endregion
		
		#region CoerceValue
		/// <summary>
		/// Forces the value to stay between mininum and maximum.
		/// </summary>
		/// <returns>minimum, if value is less than minimum.
		/// Maximum, if value is greater than maximum.
		/// Otherwise, value.</returns>
		public static double CoerceValue(this double value, double minimum, double maximum)
		{
			return Math.Max(Math.Min(value, maximum), minimum);
		}
		
		/// <summary>
		/// Forces the value to stay between mininum and maximum.
		/// </summary>
		/// <returns>minimum, if value is less than minimum.
		/// Maximum, if value is greater than maximum.
		/// Otherwise, value.</returns>
		public static int CoerceValue(this int value, int minimum, int maximum)
		{
			return Math.Max(Math.Min(value, maximum), minimum);
		}
		#endregion
		
		#region Collections
		/// <summary>
		/// Obsolete. Please use a regular foreach loop instead. ForEach() is executed for its side-effects, and side-effects mix poorly with a functional programming style.
		/// </summary>
		//[Obsolete("Please use a regular foreach loop instead. ForEach() is executed for its side-effects, and side-effects mix poorly with a functional programming style.")]
		public static void ForEach<T>(this IEnumerable<T> input, Action<T> action)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			foreach (T element in input) {
				action(element);
			}
		}
		
		/// <summary>
		/// Adds all <paramref name="elements"/> to <paramref name="list"/>.
		/// </summary>
		public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> elements)
		{
			foreach (T o in elements)
				list.Add(o);
		}
		
		public static ReadOnlyCollection<T> AsReadOnly<T>(this IList<T> arr)
		{
			return new ReadOnlyCollection<T>(arr);
		}
		
		[Obsolete("This method seems to be unused now; all uses I've seen have been replaced with IReadOnlyList<T>")]
		public static ReadOnlyCollectionWrapper<T> AsReadOnly<T>(this ICollection<T> arr)
		{
			return new ReadOnlyCollectionWrapper<T>(arr);
		}
		
		public static V GetOrDefault<K,V>(this IReadOnlyDictionary<K, V> dict, K key)
		{
			V ret;
			dict.TryGetValue(key, out ret);
			return ret;
		}
		
		/// <summary>
		/// Searches a sorted list
		/// </summary>
		/// <param name="list">The list to search in</param>
		/// <param name="key">The key to search for</param>
		/// <param name="keySelector">Function that maps list items to their sort key</param>
		/// <param name="keyComparer">Comparer used for the sort</param>
		/// <returns>Returns the index of the element with the specified key.
		/// If no such element is found, this method returns a negative number that is the bitwise complement of the
		/// index where the element could be inserted while maintaining the order.</returns>
		public static int BinarySearch<T, K>(this IList<T> list, K key, Func<T, K> keySelector, IComparer<K> keyComparer = null)
		{
			return BinarySearch(list, 0, list.Count, key, keySelector, keyComparer);
		}
		
		/// <summary>
		/// Searches a sorted list
		/// </summary>
		/// <param name="list">The list to search in</param>
		/// <param name="index">Starting index of the range to search</param>
		/// <param name="length">Length of the range to search</param>
		/// <param name="key">The key to search for</param>
		/// <param name="keySelector">Function that maps list items to their sort key</param>
		/// <param name="keyComparer">Comparer used for the sort</param>
		/// <returns>Returns the index of the element with the specified key.
		/// If no such element is found in the specified range, this method returns a negative number that is the bitwise complement of the
		/// index where the element could be inserted while maintaining the order.</returns>
		public static int BinarySearch<T, K>(this IList<T> list, int index, int length, K key, Func<T, K> keySelector, IComparer<K> keyComparer = null)
		{
			if (keyComparer == null)
				keyComparer = Comparer<K>.Default;
			int low = index;
			int high = index + length - 1;
			while (low <= high) {
				int mid = low + (high - low >> 1);
				int r = keyComparer.Compare(keySelector(list[mid]), key);
				if (r == 0) {
					return mid;
				} else if (r < 0) {
					low = mid + 1;
				} else {
					high = mid - 1;
				}
			}
			return ~low;
		}
		
		/// <summary>
		/// Inserts an item into a sorted list.
		/// </summary>
		public static void OrderedInsert<T>(this IList<T> list, T item, IComparer<T> comparer)
		{
			int pos = BinarySearch(list, item, x => x, comparer);
			if (pos < 0)
				pos = ~pos;
			list.Insert(pos, item);
		}
		
		/// <summary>
		/// Sorts the enumerable using the given comparer.
		/// </summary>
		public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> input, IComparer<T> comparer)
		{
			return Enumerable.OrderBy(input, e => e, comparer);
		}
		
		/// <summary>
		/// Converts a recursive data structure into a flat list.
		/// </summary>
		/// <param name="input">The root elements of the recursive data structure.</param>
		/// <param name="recursion">The function that gets the children of an element.</param>
		/// <returns>Iterator that enumerates the tree structure in preorder.</returns>
		public static IEnumerable<T> Flatten<T>(this IEnumerable<T> input, Func<T, IEnumerable<T>> recursion)
		{
			return ICSharpCode.NRefactory.Utils.TreeTraversal.PreOrder(input, recursion);
		}
		
		
		/// <summary>
		/// Creates an array containing a part of the array (similar to string.Substring).
		/// </summary>
		public static T[] Splice<T>(this T[] array, int startIndex)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			return Splice(array, startIndex, array.Length - startIndex);
		}
		
		/// <summary>
		/// Creates an array containing a part of the array (similar to string.Substring).
		/// </summary>
		public static T[] Splice<T>(this T[] array, int startIndex, int length)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (startIndex < 0 || startIndex > array.Length)
				throw new ArgumentOutOfRangeException("startIndex", startIndex, "Value must be between 0 and " + array.Length);
			if (length < 0 || length > array.Length - startIndex)
				throw new ArgumentOutOfRangeException("length", length, "Value must be between 0 and " + (array.Length - startIndex));
			T[] result = new T[length];
			Array.Copy(array, startIndex, result, 0, length);
			return result;
		}
		
		public static IEnumerable<T> DistinctBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector) where K : IEquatable<K>
		{
			// Don't just use .Distinct(KeyComparer.Create(keySelector)) - that would evaluate the keySelector multiple times.
			var hashSet = new HashSet<K>();
			foreach (var element in source) {
				if (hashSet.Add(keySelector(element))) {
					yield return element;
				}
			}
		}
		
		/// <summary>
		/// Returns the minimum element.
		/// </summary>
		/// <exception cref="InvalidOperationException">The input sequence is empty</exception>
		public static T MinBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector) where K : IComparable<K>
		{
			return source.MinBy(keySelector, Comparer<K>.Default);
		}

		/// <summary>
		/// Returns the minimum element.
		/// </summary>
		/// <exception cref="InvalidOperationException">The input sequence is empty</exception>
		public static T MinBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, IComparer<K> keyComparer)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (keySelector == null)
				throw new ArgumentNullException("selector");
			if (keyComparer == null)
				keyComparer = Comparer<K>.Default;
			using (var enumerator = source.GetEnumerator()) {
				if (!enumerator.MoveNext())
					throw new InvalidOperationException("Sequence contains no elements");
				T minElement = enumerator.Current;
				K minKey = keySelector(minElement);
				while (enumerator.MoveNext()) {
					T element = enumerator.Current;
					K key = keySelector(element);
					if (keyComparer.Compare(key, minKey) < 0) {
						minElement = element;
						minKey = key;
					}
				}
				return minElement;
			}
		}
		
		/// <summary>
		/// Returns the maximum element.
		/// </summary>
		/// <exception cref="InvalidOperationException">The input sequence is empty</exception>
		public static T MaxBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector) where K : IComparable<K>
		{
			return source.MaxBy(keySelector, Comparer<K>.Default);
		}

		/// <summary>
		/// Returns the maximum element.
		/// </summary>
		/// <exception cref="InvalidOperationException">The input sequence is empty</exception>
		public static T MaxBy<T, K>(this IEnumerable<T> source, Func<T, K> keySelector, IComparer<K> keyComparer)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (keySelector == null)
				throw new ArgumentNullException("selector");
			if (keyComparer == null)
				keyComparer = Comparer<K>.Default;
			using (var enumerator = source.GetEnumerator()) {
				if (!enumerator.MoveNext())
					throw new InvalidOperationException("Sequence contains no elements");
				T maxElement = enumerator.Current;
				K maxKey = keySelector(maxElement);
				while (enumerator.MoveNext()) {
					T element = enumerator.Current;
					K key = keySelector(element);
					if (keyComparer.Compare(key, maxKey) > 0) {
						maxElement = element;
						maxKey = key;
					}
				}
				return maxElement;
			}
		}
		
		/// <summary>
		/// Returns the index of the first element for which <paramref name="predicate"/> returns true.
		/// If none of the items in the list fits the <paramref name="predicate"/>, -1 is returned.
		/// </summary>
		public static int FindIndex<T>(this IList<T> list, Func<T, bool> predicate)
		{
			for (int i = 0; i < list.Count; i++) {
				if (predicate(list[i]))
					return i;
			}
			
			return -1;
		}
		
		/// <summary>
		/// Returns the index of the first element for which <paramref name="predicate"/> returns true.
		/// If none of the items in the list fits the <paramref name="predicate"/>, -1 is returned.
		/// </summary>
		public static int FindIndex<T>(this IReadOnlyList<T> list, Func<T, bool> predicate)
		{
			for (int i = 0; i < list.Count; i++) {
				if (predicate(list[i]))
					return i;
			}
			
			return -1;
		}
		
		/// <summary>
		/// Adds item to the list if the item is not null.
		/// </summary>
		public static void AddIfNotNull<T>(this IList<T> list, T itemToAdd) where T : class
		{
			if (itemToAdd != null)
				list.Add(itemToAdd);
		}
		
		public static void RemoveAll<T>(this IList<T> list, Predicate<T> condition)
		{
			if (list == null)
				throw new ArgumentNullException("list");
			int i = 0;
			while (i < list.Count) {
				if (condition(list[i]))
					list.RemoveAt(i);
				else
					i++;
			}
		}
		#endregion
		
		#region NRefactory Type System Extensions
		/// <summary>
		/// Gets the project for which the specified compilation was created.
		/// Returns null if the compilation was not created using the SharpDevelop project system.
		/// </summary>
		public static IProject GetProject(this ICompilation compilation)
		{
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			var snapshot = compilation.SolutionSnapshot as ISolutionSnapshotWithProjectMapping;
			if (snapshot != null)
				return snapshot.GetProject(compilation.MainAssembly);
			else
				return null;
		}
		
		/// <summary>
		/// Gets the project for which the specified assembly was created.
		/// Returns null if the assembly was not created from a project.
		/// </summary>
		public static IProject GetProject(this IAssembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			var snapshot = assembly.Compilation.SolutionSnapshot as ISolutionSnapshotWithProjectMapping;
			if (snapshot == null)
				return null;
			return snapshot.GetProject(assembly);
		}
		
		/// <summary>
		/// Gets the location of the assembly on disk.
		/// </summary>
		public static FileName GetReferenceAssemblyLocation(this IAssembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			return FileName.Create(assembly.UnresolvedAssembly.Location);
		}
		
		/// <summary>
		/// Gets the location of the assembly on disk.
		/// If the specified assembly is a reference assembly, this method the location of the actual runtime assembly instead.
		/// </summary>
		/// <remarks>
		/// May return null if the assembly has no location.
		/// </remarks>
		public static FileName GetRuntimeAssemblyLocation(this IAssembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			IUnresolvedAssembly asm = assembly.UnresolvedAssembly;
			if (!(asm is IProjectContent)) {
				// assembly might be in the GAC
				var location = SD.GlobalAssemblyCache.FindAssemblyInNetGac(new DomAssemblyName(assembly.FullAssemblyName));
				if (location != null)
					return location;
			}
			return FileName.Create(assembly.UnresolvedAssembly.Location);
		}
		
		/// <summary>
		/// Gets whether the specified assembly is located in the Global Assembly Cache.
		/// </summary>
		public static bool IsGacAssembly(this IAssembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			return !(assembly.UnresolvedAssembly is IProjectContent)
				&& SD.GlobalAssemblyCache.FindAssemblyInNetGac(new DomAssemblyName(assembly.FullAssemblyName)) != null;
		}
		
		static readonly string[] dotnetFrameworkPublicKeys = {
			"b77a5c561934e089", // mscorlib, etc.
			"b03f5f7f11d50a3a", // System.Drawing, etc.
			"31bf3856ad364e35" // WPF
		};
		
		/// <summary>
		/// Gets wether the specified assembly is part of the .net Framework.
		/// </summary>
		public static bool IsPartOfDotnetFramework(this IAssembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			if (assembly.UnresolvedAssembly is IProjectContent)
				return false;
			return dotnetFrameworkPublicKeys.Contains(new DomAssemblyName(assembly.FullAssemblyName).PublicKeyToken);
		}
		
		/// <summary>
		/// Gets the ambience for the specified compilation.
		/// Never returns null.
		/// </summary>
		public static IAmbience GetAmbience(this ICompilation compilation)
		{
			IProject p = compilation.GetProject();
			if (p != null)
				return p.GetAmbience();
			else
				return AmbienceService.GetCurrentAmbience();
		}
		
		/// <summary>
		/// Retrieves the model instance for the given entity.
		/// May return null if there is no model for the specified entity.
		/// </summary>
		public static IEntityModel GetModel(this IEntity entity)
		{
			if (entity == null)
				throw new ArgumentNullException("entity");
			
			if (entity is ITypeDefinition)
				return GetModel((ITypeDefinition)entity);
			else if (entity is IMember)
				return GetModel((IMember)entity);
			else
				return null;
		}
		
		/// <summary>
		/// Retrieves the model instance for the given assembly.
		/// May return null if there is no model for the specified assembly.
		/// </summary>
		public static IAssemblyModel GetModel(this IAssembly assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException("assembly");
			
			IProject project = assembly.GetProject();
			if (project != null)
				return project.AssemblyModel;
			
			try {
				return SD.AssemblyParserService.GetAssemblyModel(assembly.GetReferenceAssemblyLocation());
			} catch (Exception) {
				// TODO: use the exact exception types that GetAssemblyModel() throws (+document them)
				// silently ignore errors when loading the assembly
				return null;
			}
		}
		
		/// <summary>
		/// Retrieves the model instance for the given type definition.
		/// May return null if there is no model for the specified type definition.
		/// </summary>
		public static ITypeDefinitionModel GetModel(this ITypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
				throw new ArgumentNullException("typeDefinition");
			
			IAssemblyModel assembly = typeDefinition.ParentAssembly.GetModel();
			if (assembly != null)
				return assembly.TopLevelTypeDefinitions[typeDefinition.FullTypeName];
			else
				return null;
		}
		
		/// <summary>
		/// Retrieves the model instance for the given member.
		/// May return null if there is no model for the specified member.
		/// </summary>
		public static IMemberModel GetModel(this IMember member)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			
			if (member.DeclaringTypeDefinition == null)
				return null;
			
			var snapshot = member.Compilation.SolutionSnapshot as ISolutionSnapshotWithProjectMapping;
			if (snapshot == null)
				return null;
			
			ITypeDefinitionModel typeModel = GetModel(member.DeclaringTypeDefinition);
			if (typeModel == null)
				return null;
			
			foreach (var memberModel in typeModel.Members) {
				if (memberModel.Name == member.Name) {
					if (memberModel.Resolve() == member.MemberDefinition) {
						return memberModel;
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Retrieves the model instance for the given unresolved entity.
		/// </summary>
		/// <param name="entity">The unresolved entity</param>
		/// <param name="project">The project in which the entity is defined.
		/// If this parameter is null, the project will be determined based on the file name.</param>
		/// <returns>The entity model if it could be found; or null otherwise.</returns>
		public static IEntityModel GetModel(this IUnresolvedEntity entity, IProject project = null)
		{
			if (project == null) {
				if (entity.Region.FileName == null)
					return null;
				project = SD.ProjectService.FindProjectContainingFile(FileName.Create(entity.Region.FileName));
				if (project == null)
					return null;
			}
			IUnresolvedTypeDefinition unresolvedTypeDefinition = entity as IUnresolvedTypeDefinition ?? entity.DeclaringTypeDefinition;
			if (unresolvedTypeDefinition == null)
				return null;
			ITypeDefinitionModel typeModel = project.AssemblyModel.TopLevelTypeDefinitions[unresolvedTypeDefinition.FullTypeName];
			if (entity is IUnresolvedTypeDefinition || typeModel == null)
				return typeModel;
			
			ITypeDefinition typeDefinition = typeModel.Resolve();
			IUnresolvedMember unresolvedMember = entity as IUnresolvedMember;
			if (typeDefinition == null || unresolvedMember == null)
				return null;
			
			IMember member = unresolvedMember.Resolve(new SimpleTypeResolveContext(typeDefinition));
			if (member == null)
				return null;
			var snapshot = member.Compilation.SolutionSnapshot as ISolutionSnapshotWithProjectMapping;
			
			foreach (var memberModel in typeModel.Members) {
				if (memberModel.Name == unresolvedMember.Name) {
					if (memberModel.Resolve() == member.MemberDefinition) {
						return memberModel;
					}
				}
			}
			return null;
		}
		#endregion
		
		#region DPI independence
		public static Rect TransformToDevice(this Rect rect, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return Rect.Transform(rect, matrix);
		}
		
		public static Rect TransformFromDevice(this Rect rect, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
			return Rect.Transform(rect, matrix);
		}
		
		public static Size TransformToDevice(this Size size, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
		}
		
		public static Size TransformFromDevice(this Size size, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
			return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
		}
		
		public static Point TransformToDevice(this Point point, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return matrix.Transform(point);
		}
		
		public static Point TransformFromDevice(this Point point, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
			return matrix.Transform(point);
		}
		#endregion
		
		#region String extensions
		/// <summary>
		/// Removes <param name="stringToRemove" /> from the start of this string.
		/// Throws ArgumentException if this string does not start with <param name="stringToRemove" />.
		/// </summary>
		public static string RemoveFromStart(this string s, string stringToRemove)
		{
			if (s == null)
				return null;
			if (string.IsNullOrEmpty(stringToRemove))
				return s;
			if (!s.StartsWith(stringToRemove))
				throw new ArgumentException(string.Format("{0} does not start with {1}", s, stringToRemove));
			return s.Substring(stringToRemove.Length);
		}
		
		/// <summary>
		/// Removes <paramref name="stringToRemove" /> from the end of this string.
		/// Throws ArgumentException if this string does not end with <paramref name="stringToRemove" />.
		/// </summary>
		public static string RemoveFromEnd(this string s, string stringToRemove)
		{
			if (s == null) return null;
			if (string.IsNullOrEmpty(stringToRemove))
				return s;
			if (!s.EndsWith(stringToRemove))
				throw new ArgumentException(string.Format("{0} does not end with {1}", s, stringToRemove));
			return s.Substring(0, s.Length - stringToRemove.Length);
		}
		
		/// <summary>
		/// Trims the string from the first occurence of <paramref name="cutoffStart" /> to the end, including <paramref name="cutoffStart" />.
		/// If the string does not contain <paramref name="cutoffStart" />, just returns the original string.
		/// </summary>
		public static string CutoffEnd(this string s, string cutoffStart)
		{
			if (s == null) return null;
			int pos = s.IndexOf(cutoffStart);
			if (pos != -1) {
				return s.Substring(0, pos);
			} else {
				return s;
			}
		}
		
		/// <summary>
		/// Takes at most <param name="length" /> first characters from string.
		/// String can be null.
		/// </summary>
		public static string TakeStart(this string s, int length)
		{
			if (string.IsNullOrEmpty(s) || length >= s.Length)
				return s;
			return s.Substring(0, length);
		}

		/// <summary>
		/// Takes at most <param name="length" /> first characters from string, and appends '...' if string is longer.
		/// String can be null.
		/// </summary>
		public static string TakeStartEllipsis(this string s, int length)
		{
			if (string.IsNullOrEmpty(s) || length >= s.Length)
				return s;
			return s.Substring(0, length) + "...";
		}

		/// <summary>
		/// Removes any character given in the array from the given string.
		/// </summary>
		public static string RemoveAny(this string s, params char[] chars)
		{
			if (string.IsNullOrEmpty(s))
				return s;
			var b = new StringBuilder(s);
			foreach (char ch in chars) {
				b.Replace(ch.ToString(), "");
			}
			return b.ToString();
		}
		
		public static string Replace(this string original, string pattern, string replacement, StringComparison comparisonType)
		{
			if (original == null)
				throw new ArgumentNullException("original");
			if (pattern == null)
				throw new ArgumentNullException("pattern");
			if (pattern.Length == 0)
				throw new ArgumentException("String cannot be of zero length.", "pattern");
			if (comparisonType != StringComparison.Ordinal && comparisonType != StringComparison.OrdinalIgnoreCase)
				throw new NotSupportedException("Currently only ordinal comparisons are implemented.");
			
			StringBuilder result = new StringBuilder(original.Length);
			int currentPos = 0;
			int nextMatch = original.IndexOf(pattern, comparisonType);
			while (nextMatch >= 0) {
				result.Append(original, currentPos, nextMatch - currentPos);
				// The following line restricts this method to ordinal comparisons:
				// for non-ordinal comparisons, the match length might be different than the pattern length.
				currentPos = nextMatch + pattern.Length;
				result.Append(replacement);
				
				nextMatch = original.IndexOf(pattern, currentPos, comparisonType);
			}
			
			result.Append(original, currentPos, original.Length - currentPos);
			return result.ToString();
		}
		
		public static byte[] GetBytesWithPreamble(this Encoding encoding, string text)
		{
			byte[] encodedText = encoding.GetBytes(text);
			byte[] bom = encoding.GetPreamble();
			if (bom != null && bom.Length > 0) {
				byte[] result = new byte[bom.Length + encodedText.Length];
				bom.CopyTo(result, 0);
				encodedText.CopyTo(result, bom.Length);
				return result;
			} else {
				return encodedText;
			}
		}
		
		public static int IndexOfAny(this string haystack, IEnumerable<string> needles, int startIndex, out int matchLength)
		{
			if (haystack == null)
				throw new ArgumentNullException("haystack");
			if (needles == null)
				throw new ArgumentNullException("needles");
			int index = -1;
			matchLength = 0;
			foreach (var needle in needles) {
				int i = haystack.IndexOf(needle, startIndex, StringComparison.Ordinal);
				if (i != -1 && (index == -1 || index > i)) {
					index = i;
					matchLength = needle.Length;
				}
			}
			return index;
		}
		
		public static bool ContainsAny(this string haystack, IEnumerable<string> needles, int startIndex, out string match)
		{
			if (haystack == null)
				throw new ArgumentNullException("haystack");
			if (needles == null)
				throw new ArgumentNullException("needles");
			int index = -1;
			match = null;
			foreach (var needle in needles) {
				int i = haystack.IndexOf(needle, startIndex, StringComparison.Ordinal);
				if (i != -1 && (index == -1 || index > i)) {
					index = i;
					match = needle;
				}
			}
			return index > -1;
		}
		
		/// <summary>
		/// Retrieves a hash code for the specified string that is stable across
		/// multiple runs of SharpDevelop and .NET upgrades.
		/// 
		/// Use this method instead of the normal <c>string.GetHashCode</c> if the hash code
		/// is persisted to disk.
		/// </summary>
		public static int GetStableHashCode(this string text)
		{
			unchecked {
				int h = 0;
				foreach (char c in text) {
					h = (h << 5) - h + c;
				}
				return h;
			}
		}
		
		public static async Task CopyToAsync(this TextReader reader, TextWriter writer)
		{
			char[] buffer = new char[2048];
			int read;
			while ((read = await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0) {
				writer.Write(buffer, 0, read);
			}
		}
		#endregion
		
		#region Service Provider Extensions
		/// <summary>
		/// Retrieves the service of type <c>T</c> from the provider.
		/// If the service cannot be found, this method returns <c>null</c>.
		/// </summary>
		public static T GetService<T>(this IServiceProvider provider) where T : class
		{
			return (T)provider.GetService(typeof(T));
		}
		
		/// <summary>
		/// Retrieves the service of type <c>T</c> from the provider.
		/// If the service cannot be found, a <see cref="ServiceNotFoundException"/> will be thrown.
		/// </summary>
		public static T GetRequiredService<T>(this IServiceProvider provider) where T : class
		{
			return (T)GetRequiredService(provider, typeof(T));
		}
		
		/// <summary>
		/// Retrieves the service of type <paramref name="serviceType"/> from the provider.
		/// If the service cannot be found, a <see cref="ServiceNotFoundException"/> will be thrown.
		/// </summary>
		public static object GetRequiredService(this IServiceProvider provider, Type serviceType)
		{
			object service = provider.GetService(serviceType);
			if (service == null)
				throw new ServiceNotFoundException(serviceType);
			return service;
		}
		#endregion
		
		#region Service Extensions
		public static ICompilation GetCompilationForCurrentProject(this IParserService svc)
		{
			if (svc == null)
				throw new ArgumentNullException("svc");
			IProject project = SD.ProjectService.CurrentProject;
			if (project == null) return null;
			return SD.ParserService.GetCompilation(project);
		}
		#endregion
		
		#region Resource Service Extensions
		/// <summary>
		/// Gets an <see cref="IImage"/> from a resource.
		/// </summary>
		/// <exception cref="ResourceNotFoundException">The resource with the specified name does not exist</exception>
		public static IImage GetImage(this IResourceService resourceService, string resourceName)
		{
			if (resourceService == null)
				throw new ArgumentNullException("resourceService");
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");
			return new ResourceServiceImage(resourceService, resourceName);
		}
		
		/// <summary>
		/// Gets an image source from a resource.
		/// </summary>
		/// <exception cref="ResourceNotFoundException">The resource with the specified name does not exist</exception>
		public static ImageSource GetImageSource(this IResourceService resourceService, string resourceName)
		{
			if (resourceService == null)
				throw new ArgumentNullException("resourceService");
			if (resourceName == null)
				throw new ArgumentNullException("resourceName");
			return PresentationResourceService.GetBitmapSource(resourceName);
		}
		
		/// <summary>
		/// Creates a new image for the image source.
		/// </summary>
		public static Image CreateImage(this IImage image)
		{
			if (image == null)
				throw new ArgumentNullException("image");
			return new Image { Source = image.ImageSource };
		}
		#endregion
		
		#region XML extensions
		public static XElement FormatXml(this XElement element, int indentationLevel)
		{
			StringWriter sw = new StringWriter();
			using (XmlTextWriter xmlW = new XmlTextWriter(sw)) {
				if (SD.EditorControlService.GlobalOptions.ConvertTabsToSpaces) {
					xmlW.IndentChar = ' ';
					xmlW.Indentation = SD.EditorControlService.GlobalOptions.IndentationSize;
				} else {
					xmlW.Indentation = 1;
					xmlW.IndentChar = '\t';
				}
				xmlW.Formatting = Formatting.Indented;
				element.WriteTo(xmlW);
			}
			string xmlText = sw.ToString();
			xmlText = xmlText.Replace(sw.NewLine, sw.NewLine + GetIndentation(indentationLevel));
			return XElement.Parse(xmlText, LoadOptions.PreserveWhitespace);
		}
		
		static string GetIndentation(int level)
		{
			StringBuilder indentation = new StringBuilder();
			for (int i = 0; i < level; i++) {
				indentation.Append(SD.EditorControlService.GlobalOptions.IndentationString);
			}
			return indentation.ToString();
		}
		
		public static XElement AddWithIndentation(this XElement element, XElement newContent)
		{
			int indentationLevel = 0;
			XElement tmp = element;
			while (tmp != null) {
				tmp = tmp.Parent;
				indentationLevel++;
			}
			if (!element.Nodes().Any()) {
				element.Add(new XText(Environment.NewLine + GetIndentation(indentationLevel - 1)));
			}
			XText whitespace = element.Nodes().Last() as XText;
			if (whitespace != null && string.IsNullOrWhiteSpace(whitespace.Value)) {
				whitespace.AddBeforeSelf(new XText(Environment.NewLine + GetIndentation(indentationLevel)));
				whitespace.AddBeforeSelf(newContent = FormatXml(newContent, indentationLevel));
			} else {
				element.Add(new XText(Environment.NewLine + GetIndentation(indentationLevel)));
				element.Add(newContent = FormatXml(newContent, indentationLevel));
			}
			return newContent;
		}
		
		public static XElement AddFirstWithIndentation(this XElement element, XElement newContent)
		{
			int indentationLevel = 0;
			StringBuilder indentation = new StringBuilder();
			XElement tmp = element;
			while (tmp != null) {
				tmp = tmp.Parent;
				indentationLevel++;
				indentation.Append(SD.EditorControlService.GlobalOptions.IndentationString);
			}
			if (!element.Nodes().Any()) {
				element.Add(new XText(Environment.NewLine + GetIndentation(indentationLevel - 1)));
			}
			element.AddFirst(newContent = FormatXml(newContent, indentationLevel));
			element.AddFirst(new XText(Environment.NewLine + indentation.ToString()));
			return newContent;
		}
		#endregion
		
		#region Compatibility extension methods (to reduce merge conflicts SD4->SD5)
		public static string GetText(this IDocument document, TextLocation startPos, TextLocation endPos)
		{
			int startOffset = document.GetOffset(startPos);
			return document.GetText(startOffset, document.GetOffset(endPos) - startOffset);
		}
		
		/// <summary>
		/// Obsolete. Use GetOffset() instead.
		/// </summary>
		public static int PositionToOffset(this IDocument document, int line, int column)
		{
			return document.GetOffset(line, column);
		}
		
		/// <summary>
		/// Obsolete. Use GetLineByNumber() instead.
		/// </summary>
		public static IDocumentLine GetLine(this IDocument document, int lineNumber)
		{
			return document.GetLineByNumber(lineNumber);
		}
		
		/// <summary>
		/// Obsolete. Use GetLineByOffset() instead.
		/// </summary>
		public static IDocumentLine GetLineForOffset(this IDocument document, int offset)
		{
			return document.GetLineByOffset(offset);
		}
		#endregion
		
		#region IProject extensions
		/// <summary>
		/// Checks if the project's output is a .NET executable that can be run in a 32-bit process.
		/// </summary>
		public static bool IsPlatformTarget32BitOrAnyCPU(this IProject project)
		{
			MSBuildBasedProject msbuildProject = project as MSBuildBasedProject;
			if (msbuildProject != null) {
				string platformTarget = msbuildProject.GetEvaluatedProperty("PlatformTarget");
				return string.IsNullOrEmpty(platformTarget) || String.Equals(platformTarget, "x86", StringComparison.OrdinalIgnoreCase)
					|| String.Equals(platformTarget, "AnyCPU", StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
		#endregion
	}
}
