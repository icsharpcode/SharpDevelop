// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Inference engine for common base type / common super type.
	/// This is not used in the C# resolver, as C# does not have this kind of powerful type inference.
	/// The logic used by C# is implemented in <see cref="CSharp.Resolver.TypeInference.GetBestCommonType"/>.
	/// 
	/// This inference engine is intended for use in Refactorings.
	/// </summary>
	public sealed class CommonTypeInference
	{
		// The algorithm used is loosely based an extended version of the corresponding Java algorithm.
		// The Java specifiction calls this 'lub' (least upper bound), and is defined only in one direction
		// (for the other direction, Java uses intersection types).
		// 
		// An improved algorithm for Java is presented in:
		//    Daniel Smith and Robert Cartwright. Java Type Inference Is Broken: Can We
		//    Fix It? In OOPSLA ’08: Proceedings of the 23rd ACM SIGPLAN conference
		//    on Object-oriented programming systems languages and applications pages 505–524,
		//    New York, NY, USA, 2008.
		// 
		// The algorithm used here is losely based on that, although of course there are major differences:
		// C# does not have any equivalent to Java's 'recursive types'
		// (e.g. Comparable<? extends Comparable<? extends ...>>, nested infinitely),
		// so a large part of the problematic cases disappear.
		// 
		// However, C# also does not have any kind of intersection or union types.
		// This means we have to find an approximation to such types, for which there might
		// not be a unique solution.
		
		// We use the term Union(T,S) to denote a type X for which T <: X and S <: X.
		// X is a common base type of T and S.
		// (if we had union types, the union "T|S" would be the most specific possible type X).
		
		// We use the term Intersect(T,S) to denote a type X for which X <: T and X <: S.
		// X is a common subtype of T and S.
		// (if we had intersection types, the intersection "T|S" would be the least specific possible type X).
		
		// Some examples to show the possible common base types:
		// Union(List<string>, List<object>) = {
		//    IEnumerable<Union(string, object)> = IEnumerable<object>,
		//    IList,
		//    ICollection,
		//    IEnumerable,
		//    object,
		// }
		// Removing entries that are uninteresting as they as less specific than other existing entries, the result
		// of Union(List<string>, List<object>) is { IEnumerable<object>, IList }.
		
		// The number of options can be extremely high, especially when dealing with common subtypes.
		// Intersect(IDisposable, ICloneable) will return all classes that implement both interfaces.
		// In fact, for certain kinds of class declarations, there will be an infinite number of options.
		// For this reason, this algorithm supports aborting the operation, either after a specific number of options
		// has been found; or using a CancellationToken (e.g. when user clicks Cancel, or simply when too much time has expired).
		
		readonly ITypeResolveContext context;
		readonly IConversions conversions;
		
		/// <summary>
		/// Creates a new CommonTypeInference instance.
		/// </summary>
		/// <param name="context">The type resolve context to use.</param>
		/// <param name="conversions">The language-specified conversion rules to use.</param>
		public CommonTypeInference(ITypeResolveContext context, IConversions conversions)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (conversions == null)
				throw new ArgumentNullException("conversions");
			this.context = context;
			this.conversions = conversions;
		}
		
		public IEnumerable<IType> CommonBaseTypes(IList<IType> inputTypes, bool useOnlyReferenceConversion = false)
		{
			if (inputTypes == null)
				throw new ArgumentNullException("inputTypes");
			if (inputTypes.Count == 0)
				return EmptyList<IType>.Instance;
			
			// First test whether there is a type in the input that all other input types are convertible to
			IType potentialCommonBaseType = inputTypes[0];
			for (int i = 1; i < inputTypes.Count; i++) {
				if (useOnlyReferenceConversion) {
					if (conversions.ImplicitReferenceConversion(inputTypes[i], potentialCommonBaseType)) {
						// OK, continue
					} else if (conversions.ImplicitReferenceConversion(potentialCommonBaseType, inputTypes[i])) {
						potentialCommonBaseType = inputTypes[i];
					} else {
						potentialCommonBaseType = null;
						break;
					}
				} else {
					if (conversions.ImplicitConversion(inputTypes[i], potentialCommonBaseType)) {
						// OK, continue
					} else if (conversions.ImplicitConversion(potentialCommonBaseType, inputTypes[i])) {
						potentialCommonBaseType = inputTypes[i];
					} else {
						potentialCommonBaseType = null;
						break;
					}
				}
			}
			if (potentialCommonBaseType != null)
				return new[] { potentialCommonBaseType };
			
			// If we're supposed to only use reference conversions, but there is a non-reference type left in the input,
			// we can give up.
			if (useOnlyReferenceConversion && inputTypes.Any(t => t.IsReferenceType != true))
				return EmptyList<IType>.Instance;
			
			// Debug output: input values
			Debug.WriteLine("CommonBaseTypes input = {");
			Debug.Indent();
			foreach (IType type in inputTypes)
				Debug.WriteLine(type);
			Debug.Unindent();
			Debug.WriteLine("}");
			
			Dictionary<ITypeDefinition, TP[]> dict = new Dictionary<ITypeDefinition, TP[]>();
			HashSet<IType> potentialTypes = new HashSet<IType>();
			// Retrieve the initial candidates from the first bound
			// generic types go to dict, non-generic types directly go to potentialTypes
			foreach (IType baseType in inputTypes[0].GetAllBaseTypes(context)) {
				ParameterizedType pt = baseType as ParameterizedType;
				if (pt != null) {
					TP[] tp = new TP[pt.TypeParameterCount];
					for (int i = 0; i < tp.Length; i++) {
						tp[i] = new TP(pt.GetDefinition().TypeParameters[i]);
						tp[i].Bounds.Add(pt.TypeArguments[i]);
					}
					dict[pt.GetDefinition()] = tp;
				} else {
					potentialTypes.Add(baseType);
				}
			}
			// Now retrieve candidates for all other bounds, and intersect the different sets of candidates.
			for (int i = 1; i < inputTypes.Count; i++) {
				IEnumerable<IType> baseTypesForThisBound = inputTypes[i].GetAllBaseTypes(context);
				HashSet<ITypeDefinition> genericTypeDefsForThisLowerBound = new HashSet<ITypeDefinition>();
				foreach (IType baseType in baseTypesForThisBound) {
					ParameterizedType pt = baseType as ParameterizedType;
					if (pt != null) {
						TP[] tp;
						if (dict.TryGetValue(pt.GetDefinition(), out tp)) {
							genericTypeDefsForThisLowerBound.Add(pt.GetDefinition());
							for (int j = 0; j < tp.Length; j++) {
								tp[j].Bounds.Add(pt.TypeArguments[j]);
							}
						}
					}
				}
				potentialTypes.IntersectWith(baseTypesForThisBound);
				foreach (ITypeDefinition def in dict.Keys.ToArray()) {
					if (!genericTypeDefsForThisLowerBound.Contains(def))
						dict.Remove(def);
				}
			}
			
			// Now figure out the generic types, and add them to potential types if possible.
			foreach (var pair in dict) {
				Debug.WriteLine("CommonBaseTypes: " + pair.Key);
				Debug.Indent();
				IType[][] typeArguments = new IType[pair.Value.Length][];
				bool error = false;
				for (int i = 0; i < pair.Value.Length; i++) {
					var tp = pair.Value[i];
					Debug.WriteLine("Fixing " + tp);
					Debug.Indent();
					switch (tp.Variance) {
						case VarianceModifier.Covariant:
							typeArguments[i] = CommonBaseTypes(tp.Bounds.ToArray(), true).ToArray();
							break;
						case VarianceModifier.Contravariant:
							typeArguments[i] = CommonSubTypes(tp.Bounds.ToArray(), true).ToArray();
							break;
						default: // Invariant
							if (tp.Bounds.Count == 1)
								typeArguments[i] = new IType[] { tp.Bounds.Single() };
							break;
					}
					Debug.Unindent();
					if (typeArguments[i] == null || typeArguments[i].Length == 0) {
						Debug.WriteLine("  -> error");
						error = true;
						break;
					} else {
						Debug.WriteLine("  -> " + string.Join(",", typeArguments[i].AsEnumerable()));
					}
				}
				if (!error) {
					foreach (IType[] ta in AllCombinations(typeArguments)) {
						IType result = new ParameterizedType(pair.Key, ta);
						Debug.WriteLine("Result: " + result);
						potentialTypes.Add(result);
					}
				}
				Debug.Unindent();
			}
			
			// Debug output: list candidates found so far:
			Debug.WriteLine("CommonBaseTypes candidates = {");
			Debug.Indent();
			foreach (IType type in potentialTypes)
				Debug.WriteLine(type);
			Debug.Unindent();
			Debug.WriteLine("}");
			
			// Remove redundant types
			foreach (IType type in potentialTypes.ToArray()) {
				bool isRedundant = false;
				foreach (IType otherType in potentialTypes) {
					if (type != otherType && conversions.ImplicitReferenceConversion(otherType, type)) {
						isRedundant = true;
						break;
					}
				}
				if (isRedundant)
					potentialTypes.Remove(type);
			}
			
			return potentialTypes;
		}
		
		/// <summary>
		/// Performs the combinatorial explosion.
		/// </summary>
		IEnumerable<IType[]> AllCombinations(IType[][] typeArguments)
		{
			int[] index = new int[typeArguments.Length];
			index[typeArguments.Length - 1] = -1;
			while (true) {
				int i;
				for (i = index.Length - 1; i >= 0; i--) {
					if (++index[i] == typeArguments[i].Length)
						index[i] = 0;
					else
						break;
				}
				if (i < 0)
					break;
				IType[] r = new IType[typeArguments.Length];
				for (i = 0; i < r.Length; i++) {
					r[i] = typeArguments[i][index[i]];
				}
				yield return r;
			}
		}
		
		public IEnumerable<IType> CommonSubTypes(IList<IType> inputTypes, bool useOnlyReferenceConversion = false)
		{
			if (inputTypes == null)
				throw new ArgumentNullException("inputTypes");
			if (inputTypes.Count == 0)
				return EmptyList<IType>.Instance;
			
			// First test whether there is a type in the input that can be converted to all other input types
			IType potentialCommonSubType = inputTypes[0];
			for (int i = 1; i < inputTypes.Count; i++) {
				if (useOnlyReferenceConversion) {
					if (conversions.ImplicitReferenceConversion(potentialCommonSubType, inputTypes[i])) {
						// OK, continue
					} else if (conversions.ImplicitReferenceConversion(inputTypes[i], potentialCommonSubType)) {
						potentialCommonSubType = inputTypes[i];
					} else {
						potentialCommonSubType = null;
						break;
					}
				} else {
					if (conversions.ImplicitConversion(potentialCommonSubType, inputTypes[i])) {
						// OK, continue
					} else if (conversions.ImplicitConversion(inputTypes[i], potentialCommonSubType)) {
						potentialCommonSubType = inputTypes[i];
					} else {
						potentialCommonSubType = null;
						break;
					}
				}
			}
			if (potentialCommonSubType != null)
				return new[] { potentialCommonSubType };
			
			// Now we're left with the open-ended quest to find a type that derives from all input types.
			
			
			return new IType[0];
		}
		
		sealed class TP
		{
			public readonly VarianceModifier Variance;
			
			public TP(ITypeParameter tp)
			{
				this.Variance = tp.Variance;
			}
			
			public readonly HashSet<IType> Bounds = new HashSet<IType>();
			
			#if DEBUG
			public override string ToString()
			{
				StringBuilder b = new StringBuilder();
				b.Append('(');
				if (this.Variance == VarianceModifier.Covariant) {
					bool first = true;
					foreach (IType type in Bounds) {
						if (first) first = false; else b.Append(" | ");
						b.Append(type);
					}
					b.Append(" <: ");
				}
				b.Append("TP");
				if (this.Variance == VarianceModifier.Contravariant) {
					b.Append(" <: ");
					bool first = true;
					foreach (IType type in Bounds) {
						if (first) first = false; else b.Append(" & ");
						b.Append(type);
					}
				}  else if (this.Variance == VarianceModifier.Invariant) {
					foreach (IType type in Bounds) {
						b.Append(" = ");
						b.Append(type);
					}
				}
				b.Append(')');
				return b.ToString();
			}
			#endif
		}
	}
}
