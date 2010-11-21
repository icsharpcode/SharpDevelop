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
		// Note: I'm pretty sure this is wrong in some rare cases, and incomplete in others;
		// but it should be good enough 99% of the time.
		
		// We use the term CommonBaseTypes(T,S) to denote the set { X | T <: X and S <: X }.
		// (if we had union types, the union "T|S" would be the most specific possible type X).
		
		// We use the term CommonSubTypes(T,S) to denote the set { X | X <: T and X <: S }.
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
						tp[i] = new TP(pt.GetDefinition().TypeParameters[i], false);
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
				IType[][] typeArguments = Fix(pair.Value);
				if (typeArguments != null) {
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
		
		IType[][] Fix(TP[] tps)
		{
			IType[][] typeArguments = new IType[tps.Length][];
			bool error = false;
			for (int i = 0; i < tps.Length; i++) {
				var tp = tps[i];
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
			return error ? null : typeArguments;
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
			
			ITypeDefinition[] inputTypeDefinitions = new ITypeDefinition[inputTypes.Count];
			for (int i = 0; i < inputTypeDefinitions.Length; i++) {
				inputTypeDefinitions[i] = inputTypes[i].GetDefinition();
				if (inputTypeDefinitions[i] == null) {
					// if there's any array or pointer type, we cannot find a common subtype
					return EmptyList<IType>.Instance;
				}
			}
			
			// Debug output: input values
			Debug.WriteLine("CommonSubTypes input = {");
			Debug.Indent();
			foreach (IType type in inputTypes)
				Debug.WriteLine(type);
			Debug.Unindent();
			Debug.WriteLine("}");
			
			// Now we're left with the open-ended quest to find a type that derives from all input types.
			List<ITypeDefinition> candidateTypeDefs = new List<ITypeDefinition>();
			foreach (ITypeDefinition d in context.GetAllClasses()) {
				bool ok = true;
				// first check whether the type is derived from all input types
				foreach (ITypeDefinition inputTypeDef in inputTypeDefinitions) {
					if (!d.IsDerivedFrom(inputTypeDef, context)) {
						ok = false;
						break;
					}
				}
				if (!ok)
					continue;
				// then check that the type isn't redundant (derives from existing candidate)
				foreach (ITypeDefinition oldCandidate in candidateTypeDefs) {
					if (d.IsDerivedFrom(oldCandidate, context)) {
						ok = false;
						break;
					}
				}
				if (!ok)
					continue;
				// remove all existing candidates that are made redundant by the new type
				candidateTypeDefs.RemoveAll(oldCandidate => oldCandidate.IsDerivedFrom(d, context));
				candidateTypeDefs.Add(d); // add new candidate
			}
			
			HashSet<IType> potentialTypes = new HashSet<IType>();
			foreach (var candidate in candidateTypeDefs) {
				if (candidate.TypeParameterCount == 0) {
					potentialTypes.Add(candidate);
					continue;
				}
				Debug.WriteLine("  Considering " + candidate);
				TP[] tp = new TP[candidate.TypeParameterCount];
				for (int i = 0; i < tp.Length; i++) {
					tp[i] = new TP(candidate.TypeParameters[i], true);
				}
				// self-parameterize the candidate
				IType parameterizedCandidate = new ParameterizedType(candidate, candidate.TypeParameters);
				foreach (var candidateBase in parameterizedCandidate.GetAllBaseTypes(context).OfType<ParameterizedType>()) {
					for (int i = 0; i < inputTypeDefinitions.Length; i++) {
						if (candidateBase.GetDefinition() == inputTypeDefinitions[i]) {
							ParameterizedType pt = inputTypes[i] as ParameterizedType;
							if (pt != null && pt.TypeParameterCount == candidateBase.TypeParameterCount) {
								// HACK: only handle the trivial case
								// what actually needs to be done here is very much like C# type inference,
								// so I should probably restructure the code to reuse C#'s MakeLowerBoundInference etc.
								for (int j = 0; j < Math.Min(pt.TypeParameterCount, candidate.TypeParameterCount); j++) {
									if (candidateBase.TypeArguments[j] == candidate.TypeParameters[j]) {
										tp[j].Bounds.Add(pt.TypeArguments[j]);
									}
								}
							}
						}
					}
				}
				Debug.Indent();
				var typeArguments = Fix(tp);
				if (typeArguments != null) {
					foreach (IType[] ta in AllCombinations(typeArguments)) {
						IType result = new ParameterizedType(candidate, ta);
						Debug.WriteLine("Result: " + result);
						potentialTypes.Add(result);
					}
				}
				Debug.Unindent();
			}
			
			return potentialTypes;
		}
		
		sealed class TP
		{
			public readonly VarianceModifier Variance;
			
			public TP(ITypeParameter tp, bool negative)
			{
				this.Variance = tp.Variance;
				if (negative) {
					switch (tp.Variance) {
						case VarianceModifier.Covariant:
							this.Variance = VarianceModifier.Contravariant;
							break;
						case VarianceModifier.Contravariant:
							this.Variance = VarianceModifier.Covariant;
							break;
					}
				}
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
