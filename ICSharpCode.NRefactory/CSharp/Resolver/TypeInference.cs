// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Implements C# 4.0 Type Inference (§7.5.2).
	/// </summary>
	public class TypeInference
	{
		readonly ITypeResolveContext context;
		
		public TypeInference(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
		}
		
		public IType[] InferTypeArguments(IList<ITypeParameter> typeParameters, IList<ResolveResult> arguments, IList<IType> parameterTypes, out bool success)
		{
			this.typeParameters = new TP[typeParameters.Count];
			for (int i = 0; i < this.typeParameters.Length; i++) {
				if (i != typeParameters[i].Index)
					throw new ArgumentException("Type parameter has wrong index");
				this.typeParameters[i] = new TP(typeParameters[i]);
			}
			this.parameterTypes = new IType[Math.Min(arguments.Count, parameterTypes.Count)];
			this.arguments = new ResolveResult[this.parameterTypes.Length];
			for (int i = 0; i < this.parameterTypes.Length; i++) {
				if (arguments[i] == null || parameterTypes[i] == null)
					throw new ArgumentNullException();
				this.arguments[i] = arguments[i];
				this.parameterTypes[i] = parameterTypes[i];
			}
			PhaseOne();
			success = PhaseTwo();
			return this.typeParameters.Select(tp => tp.FixedTo ?? SharedTypes.UnknownType).ToArray();
		}
		
		TP[] typeParameters;
		IType[] parameterTypes;
		ResolveResult[] arguments;
		
		sealed class TP
		{
			public readonly HashSet<IType> LowerBounds = new HashSet<IType>();
			public readonly HashSet<IType> UpperBounds = new HashSet<IType>();
			public readonly ITypeParameter TypeParameter;
			public IType FixedTo;
			
			public bool Fixed { // TODO: rename to IsFixed
				get { return FixedTo != null; }
			}
			
			public bool HasBounds {
				get { return LowerBounds.Count > 0 || UpperBounds.Count > 0; }
			}
			
			public TP(ITypeParameter typeParameter)
			{
				if (typeParameter == null)
					throw new ArgumentNullException("typeParameter");
				this.TypeParameter = typeParameter;
			}
			
			public override string ToString()
			{
				return TypeParameter.Name;
			}
		}
		
		sealed class OccursInVisitor : TypeVisitor
		{
			readonly TP[] tp;
			public readonly bool[] Occurs;
			
			public OccursInVisitor(TypeInference typeInference)
			{
				this.tp = typeInference.typeParameters;
				this.Occurs = new bool[tp.Length];
			}
			
			public override IType VisitTypeParameter(ITypeParameter type)
			{
				int index = type.Index;
				if (index < tp.Length && tp[index].TypeParameter == type)
					Occurs[index] = true;
				return base.VisitTypeParameter(type);
			}
		}
		
		void PhaseOne()
		{
			// C# 4.0 spec: §7.5.2.1 The first phase
			Log("Phase One");
			for (int i = 0; i < arguments.Length; i++) {
				ResolveResult Ei = arguments[i];
				IType Ti = parameterTypes[i];
				// TODO: what if Ei is an anonymous function?
				IType U = Ei.Type;
				if (U != SharedTypes.UnknownType) {
					if (Ti is ByReferenceType) {
						MakeExactInference(Ei.Type, Ti);
					} else {
						MakeLowerBoundInference(Ei.Type, Ti);
					}
				}
			}
		}
		
		bool PhaseTwo()
		{
			// C# 4.0 spec: §7.5.2.2 The second phase
			Log("Phase Two");
			// All unfixed type variables Xi which do not depend on any Xj are fixed.
			List<TP> typeParametersToFix = new List<TP>();
			foreach (TP Xi in typeParameters) {
				if (Xi.Fixed == false) {
					if (!typeParameters.Any((TP Xj) => DependsOn(Xi, Xj))) {
						typeParametersToFix.Add(Xi);
					}
				}
			}
			// If no such type variables exist, all unfixed type variables Xi are fixed for which all of the following hold:
			if (typeParametersToFix.Count == 0) {
				foreach (TP Xi in typeParameters) {
					// Xi has a non­empty set of bounds
					if (!Xi.Fixed && Xi.HasBounds) {
						// There is at least one type variable Xj that depends on Xi
						if (typeParameters.Any((TP Xj) => DependsOn(Xj, Xi))) {
							typeParametersToFix.Add(Xi);
						}
					}
				}
			}
			// now fix 'em
			bool errorDuringFix = false;
			foreach (TP tp in typeParametersToFix) {
				if (!Fix(tp))
					errorDuringFix = true;
			}
			if (errorDuringFix)
				return false;
			bool unfixedTypeVariablesExist = typeParameters.Any((TP X) => X.Fixed == false);
			if (typeParametersToFix.Count == 0 && unfixedTypeVariablesExist) {
				// If no such type variables exist and there are still unfixed type variables, type inference fails.
				return false;
			} else if (!unfixedTypeVariablesExist) {
				// Otherwise, if no further unfixed type variables exist, type inference succeeds.
				return true;
			} else {
				// Otherwise, for all arguments ei with corresponding parameter type Ti
				for (int i = 0; i < arguments.Length; i++) {
					ResolveResult Ei = arguments[i];
					IType Ti = parameterTypes[i];
					// where the output types (§7.4.2.4) contain unfixed type variables Xj
					// but the input types (§7.4.2.3) do not
					if (OutputTypeContainsUnfixed(Ei, Ti) && !InputTypesContainsUnfixed(Ei, Ti)) {
						// an output type inference (§7.4.2.6) is made for ei with type Ti.
						Log("MakeOutputTypeInference for #" + i);
						MakeOutputTypeInference(Ei, Ti);
					}
				}
				// Then the second phase is repeated.
				return PhaseTwo();
			}
		}
		
		#region Input Types / Output Types (§7.5.2.3 + §7.5.2.4)
		static readonly IType[] emptyTypeArray = new IType[0];
		
		IType[] InputTypes(ResolveResult e, IType t)
		{
			// C# 4.0 spec: §7.5.2.3 Input types
			/* TODO
			AnonymousMethodReturnType amrt = e as AnonymousMethodReturnType;
			if (amrt != null && amrt.HasImplicitlyTypedParameters || e is MethodGroupReturnType) {
				IMethod m = GetDelegateOrExpressionTreeSignature(t, amrt != null && amrt.CanBeConvertedToExpressionTree);
				if (m != null) {
					return m.Parameters.Select(p => p.ReturnType);
				}
			}*/
			return emptyTypeArray;
		}
		
		
		IType[] OutputTypes(ResolveResult e, IType t)
		{
			// C# 4.0 spec: §7.5.2.4 Input types
			/*
			AnonymousMethodReturnType amrt = e as AnonymousMethodReturnType;
			if (amrt != null || e is MethodGroupReturnType) {
				IMethod m = GetDelegateOrExpressionTreeSignature(T, amrt != null && amrt.CanBeConvertedToExpressionTree);
				if (m != null) {
					return new[] { m.ReturnType };
				}
			}
			 */
			return emptyTypeArray;
		}
		
		bool InputTypesContainsUnfixed(ResolveResult argument, IType parameterType)
		{
			return AnyTypeContainsUnfixedParameter(InputTypes(argument, parameterType));
		}
		
		bool OutputTypeContainsUnfixed(ResolveResult argument, IType parameterType)
		{
			return AnyTypeContainsUnfixedParameter(OutputTypes(argument, parameterType));
		}
		
		bool AnyTypeContainsUnfixedParameter(IEnumerable<IType> types)
		{
			OccursInVisitor o = new OccursInVisitor(this);
			foreach (var type in types) {
				type.AcceptVisitor(o);
			}
			for (int i = 0; i < typeParameters.Length; i++) {
				if (!typeParameters[i].Fixed && o.Occurs[i])
					return true;
			}
			return false;
		}
		#endregion
		
		#region DependsOn (§7.5.2.5)
		// C# 4.0 spec: §7.5.2.5 Dependance
		bool[,] dependencyMatrix;
		
		void CalculateDependencyMatrix()
		{
			int n = typeParameters.Length;
			dependencyMatrix = new bool[n, n];
			for (int k = 0; k < arguments.Length; k++) {
				OccursInVisitor input = new OccursInVisitor(this);
				OccursInVisitor output = new OccursInVisitor(this);
				foreach (var type in InputTypes(arguments[k], parameterTypes[k])) {
					type.AcceptVisitor(input);
				}
				foreach (var type in OutputTypes(arguments[k], parameterTypes[k])) {
					type.AcceptVisitor(output);
				}
				for (int i = 0; i < n; i++) {
					for (int j = 0; j < n; j++) {
						dependencyMatrix[i, j] |= input.Occurs[j] && output.Occurs[i];
					}
				}
			}
			// calculate transitive closure using Warshall's algorithm:
			for (int i = 0; i < n; i++) {
				for (int j = 0; j < n; j++) {
					if (dependencyMatrix[i, j]) {
						for (int k = 0; k < n; k++) {
							if (dependencyMatrix[j, k])
								dependencyMatrix[i, k] = true;
						}
					}
				}
			}
		}
		
		bool DependsOn(TP x, TP y)
		{
			if (dependencyMatrix == null)
				CalculateDependencyMatrix();
			// x depends on y
			return dependencyMatrix[x.TypeParameter.Index, y.TypeParameter.Index];
		}
		#endregion
		
		#region MakeOutputTypeInference (§7.5.2.6)
		void MakeOutputTypeInference(ResolveResult e, IType t)
		{
			// If E is an anonymous function with inferred return type  U (§7.5.2.12) and T is a delegate type or expression
			// tree type with return type Tb, then a lower-bound inference (§7.5.2.9) is made from U to Tb.
			/* TODO AnonymousMethodReturnType amrt = e as AnonymousMethodReturnType;
			if (amrt != null) {
				IMethod m = GetDelegateOrExpressionTreeSignature(T, amrt.CanBeConvertedToExpressionTree);
				if (m != null) {
					IReturnType inferredReturnType;
					if (amrt.HasParameterList && amrt.MethodParameters.Count == m.Parameters.Count) {
						var inferredParameterTypes = m.Parameters.Select(p => SubstituteFixedTypes(p.ReturnType)).ToArray();
						inferredReturnType = amrt.ResolveReturnType(inferredParameterTypes);
					} else {
						inferredReturnType = amrt.ResolveReturnType();
					}
					
					MakeLowerBoundInference(inferredReturnType, m.ReturnType);
					return;
				}
			}*/
			// Otherwise, if E is a method group and T is a delegate type or expression tree type
			// with parameter types T1…Tk and return type Tb, and overload resolution
			// of E with the types T1…Tk yields a single method with return type U, then a lower­-bound
			// inference is made from U to Tb.
			MethodGroupResolveResult mgrr = e as MethodGroupResolveResult;
			if (mgrr != null) {
				throw new NotImplementedException();
			}
			// Otherwise, if E is an expression with type U, then a lower-bound inference is made from U to T.
			if (e.Type != SharedTypes.UnknownType) {
				MakeLowerBoundInference(e.Type, t);
			}
		}
		#endregion
		
		void MakeExplicitParameterTypeInference(ResolveResult e, IType t)
		{
			// C# 4.0 spec: §7.5.2.7 Explicit parameter type inferences
			throw new NotImplementedException();
			/*AnonymousMethodReturnType amrt = e as AnonymousMethodReturnType;
			if (amrt != null && amrt.HasParameterList) {
				IMethod m = GetDelegateOrExpressionTreeSignature(T, amrt.CanBeConvertedToExpressionTree);
				if (m != null && amrt.MethodParameters.Count == m.Parameters.Count) {
					for (int i = 0; i < amrt.MethodParameters.Count; i++) {
						MakeExactInference(amrt.MethodParameters[i].ReturnType, m.Parameters[i].ReturnType);
					}
				}
			}*/
		}
		
		#region MakeExactInference
		/// <summary>
		/// Make exact inference from U to V.
		/// C# 4.0 spec: 7.5.2.8 Exact inferences
		/// </summary>
		void MakeExactInference(IType U, IType V)
		{
			Log(" MakeExactInference from " + U + " to " + V);
			
			// If V is one of the unfixed Xi then U is added to the set of bounds for Xi.
			TP tp = GetTPForType(V);
			if (tp != null && tp.Fixed == false) {
				Log(" Add exact bound '" + U + "' to " + tp);
				tp.LowerBounds.Add(U);
				tp.UpperBounds.Add(U);
				return;
			}
			// Handle by reference types:
			ByReferenceType brU = U as ByReferenceType;
			ByReferenceType brV = V as ByReferenceType;
			if (brU != null && brV != null) {
				MakeExactInference(brU.ElementType, brV.ElementType);
				return;
			}
			// Handle array types:
			ArrayType arrU = U as ArrayType;
			ArrayType arrV = V as ArrayType;
			if (arrU != null && arrV != null && arrU.Dimensions == arrV.Dimensions) {
				MakeExactInference(arrU.ElementType, arrV.ElementType);
				return;
			}
			// Handle parameterized type:
			ParameterizedType pU = U as ParameterizedType;
			ParameterizedType pV = V as ParameterizedType;
			if (pU != null && pV != null
			    && object.Equals(pU.GetDefinition(), pV.GetDefinition())
			    && pU.TypeParameterCount == pV.TypeParameterCount)
			{
				for (int i = 0; i < pU.TypeParameterCount; i++) {
					MakeExactInference(pU.TypeArguments[i], pV.TypeArguments[i]);
				}
			}
		}
		
		TP GetTPForType(IType v)
		{
			ITypeParameter p = v as ITypeParameter;
			if (p != null) {
				int index = p.Index;
				if (index < typeParameters.Length && typeParameters[index].TypeParameter == p)
					return typeParameters[index];
			}
			return null;
		}
		#endregion
		
		#region MakeLowerBoundInference
		/// <summary>
		/// Make lower bound inference from U to V.
		/// C# 4.0 spec: §7.5.2.9 Lower-bound inferences
		/// </summary>
		void MakeLowerBoundInference(IType U, IType V)
		{
			Log(" MakeLowerBoundInference from " + U + " to " + V);
			
			// If V is one of the unfixed Xi then U is added to the set of bounds for Xi.
			TP tp = GetTPForType(V);
			if (tp != null && tp.Fixed == false) {
				Log("  Add lower bound '" + U + "' to " + tp);
				tp.LowerBounds.Add(U);
				return;
			}
			
			// Handle array types:
			ArrayType arrU = U as ArrayType;
			ArrayType arrV = V as ArrayType;
			ParameterizedType pV = V as ParameterizedType;
			if (arrU != null && (arrV != null && arrU.Dimensions == arrV.Dimensions
			                     || IsIEnumerableCollectionOrList(pV) && arrU.Dimensions == 1))
			{
				MakeLowerBoundInference(arrU.ElementType, arrV.ElementType);
				return;
			}
			// Handle parameterized types:
			if (pV != null) {
				ParameterizedType uniqueBaseType = null;
				foreach (IType baseU in U.GetAllBaseTypes(context)) {
					ParameterizedType pU = baseU as ParameterizedType;
					if (pU != null && object.Equals(pU.GetDefinition(), pV.GetDefinition()) && pU.TypeParameterCount == pV.TypeParameterCount) {
						if (uniqueBaseType == null)
							uniqueBaseType = pU;
						else
							return; // cannot make an inference because it's not unique
					}
				}
				if (uniqueBaseType != null) {
					for (int i = 0; i < uniqueBaseType.TypeParameterCount; i++) {
						IType Ui = uniqueBaseType.TypeArguments[i];
						IType Vi = pV.TypeArguments[i];
						if (Ui.IsReferenceType == true) {
							// look for variance
							ITypeParameter Xi = pV.GetDefinition().TypeParameters[i];
							switch (Xi.Variance) {
								case VarianceModifier.Covariant:
									MakeLowerBoundInference(Ui, Vi);
									break;
								case VarianceModifier.Contravariant:
									MakeUpperBoundInference(Ui, Vi);
									break;
								default: // invariant
									MakeExactInference(Ui, Vi);
									break;
							}
						} else {
							// not known to be a reference type
							MakeExactInference(Ui, Vi);
						}
					}
				}
			}
		}
		
		static bool IsIEnumerableCollectionOrList(ParameterizedType rt)
		{
			if (rt == null || rt.TypeParameterCount != 1)
				return false;
			switch (rt.GetDefinition().FullName) {
				case "System.Collections.Generic.IList":
				case "System.Collections.Generic.ICollection":
				case "System.Collections.Generic.IEnumerable":
					return true;
				default:
					return false;
			}
		}
		#endregion
		
		#region MakeUpperBoundInference
		/// <summary>
		/// Make upper bound inference from U to V.
		/// C# 4.0 spec: §7.5.2.10 Upper-bound inferences
		/// </summary>
		void MakeUpperBoundInference(IType U, IType V)
		{
			Log(" MakeUpperBoundInference from " + U + " to " + V);
			
			// If V is one of the unfixed Xi then U is added to the set of bounds for Xi.
			TP tp = GetTPForType(V);
			if (tp != null && tp.Fixed == false) {
				Log("  Add upper bound '" + U + "' to " + tp);
				tp.UpperBounds.Add(U);
				return;
			}
			
			// Handle array types:
			ArrayType arrU = U as ArrayType;
			ArrayType arrV = V as ArrayType;
			ParameterizedType pU = U as ParameterizedType;
			if (arrV != null && (arrU != null && arrU.Dimensions == arrV.Dimensions
			                     || IsIEnumerableCollectionOrList(pU) && arrV.Dimensions == 1))
			{
				MakeUpperBoundInference(arrU.ElementType, arrV.ElementType);
				return;
			}
			// Handle parameterized types:
			if (pU != null) {
				ParameterizedType uniqueBaseType = null;
				foreach (IType baseV in V.GetAllBaseTypes(context)) {
					ParameterizedType pV = baseV as ParameterizedType;
					if (pV != null && object.Equals(pU.GetDefinition(), pV.GetDefinition()) && pU.TypeParameterCount == pV.TypeParameterCount) {
						if (uniqueBaseType == null)
							uniqueBaseType = pV;
						else
							return; // cannot make an inference because it's not unique
					}
				}
				if (uniqueBaseType != null) {
					for (int i = 0; i < uniqueBaseType.TypeParameterCount; i++) {
						IType Ui = pU.TypeArguments[i];
						IType Vi = uniqueBaseType.TypeArguments[i];
						if (Vi.IsReferenceType == true) {
							// look for variance
							ITypeParameter Xi = pU.GetDefinition().TypeParameters[i];
							switch (Xi.Variance) {
								case VarianceModifier.Covariant:
									MakeUpperBoundInference(Ui, Vi);
									break;
								case VarianceModifier.Contravariant:
									MakeLowerBoundInference(Ui, Vi);
									break;
								default: // invariant
									MakeExactInference(Ui, Vi);
									break;
							}
						} else {
							// not known to be a reference type
							MakeExactInference(Ui, Vi);
						}
					}
				}
			}
		}
		#endregion
		
		#region Fixing
		bool Fix(TP tp)
		{
			Log("Trying to fix " + tp);
			Debug.Assert(!tp.Fixed);
			Log("  Lower bounds: ", tp.LowerBounds);
			Log("  Upper bounds: ", tp.UpperBounds);
			Conversions conversions = new Conversions(context);
			IEnumerable<IType> candidates = tp.LowerBounds.Union(tp.UpperBounds);
			// keep only the candidates that are within all bounds
			candidates = candidates.Where(c => tp.LowerBounds.All(b => conversions.ImplicitConversion(b, c)));
			candidates = candidates.Where(c => tp.UpperBounds.All(b => conversions.ImplicitConversion(c, b)));
			candidates = candidates.ToList(); // evaluate the query only once
			Log("  Candidates: ", candidates);
			IType solution = null;
			foreach (var c in candidates) {
				if (candidates.All(o => conversions.ImplicitConversion(c, o))) {
					if (solution == null)
						solution = c;
					else
						return false; // solution is not unique
				}
			}
			if (solution != null) {
				Log("  " + tp + " was fixed to " + solution);
				tp.FixedTo = solution;
				return true;
			} else {
				return false;
			}
		}
		#endregion
		
		#region Finding the best common type of a set of expresssions
		/// <summary>
		/// Gets the best common type (C# 4.0 spec: §7.5.2.14) of a set of expressions.
		/// </summary>
		public IType GetBestCommonType(IList<ResolveResult> expressions, out bool success)
		{
			this.typeParameters = new TP[1];
			typeParameters[0] = new TP(DummyTypeParameter.Instance);
			this.arguments = expressions.ToArray();
			this.parameterTypes = new IType[expressions.Count];
			for (int i = 0; i < parameterTypes.Length; i++) {
				parameterTypes[i] = DummyTypeParameter.Instance;
			}
			for (int i = 0; i < arguments.Length; i++) {
				MakeOutputTypeInference(arguments[i], DummyTypeParameter.Instance);
			}
			success = Fix(typeParameters[0]);
			return typeParameters[0].FixedTo ?? SharedTypes.UnknownType;
		}
		
		sealed class DummyTypeParameter : AbstractType, ITypeParameter
		{
			public static readonly DummyTypeParameter Instance = new DummyTypeParameter();
			
			public override string Name {
				get { return "X"; }
			}
			
			public override bool? IsReferenceType {
				get { return null; }
			}
			
			public override int GetHashCode()
			{
				return 0;
			}
			
			public override bool Equals(IType other)
			{
				return this == other;
			}
			
			int ITypeParameter.Index {
				get { return 0; }
			}
			
			IList<IAttribute> ITypeParameter.Attributes {
				get { return EmptyList<IAttribute>.Instance; }
			}
			
			IEntity ITypeParameter.Parent {
				get { throw new NotSupportedException(); }
			}
			
			IMethod ITypeParameter.ParentMethod {
				get { throw new NotSupportedException(); }
			}
			
			ITypeDefinition ITypeParameter.ParentClass {
				get { throw new NotSupportedException(); }
			}
			
			IList<ITypeReference> ITypeParameter.Constraints {
				get { return EmptyList<ITypeReference>.Instance; }
			}
			
			bool ITypeParameter.HasDefaultConstructorConstraint {
				get { return false; }
			}
			
			bool ITypeParameter.HasReferenceTypeConstraint {
				get { return false; }
			}
			
			bool ITypeParameter.HasValueTypeConstraint {
				get { return false; }
			}
			
			VarianceModifier ITypeParameter.Variance {
				get { return VarianceModifier.Invariant; }
			}
			
			IType ITypeParameter.BoundTo {
				get { return null; }
			}
			
			ITypeParameter ITypeParameter.UnboundTypeParameter {
				get { return null; }
			}
			
			bool IFreezable.IsFrozen {
				get { return true; }
			}
			
			void IFreezable.Freeze()
			{
			}
		}
		#endregion
		
		[Conditional("DEBUG")]
		static void Log(string text)
		{
			Debug.WriteLine(text);
		}
		
		[Conditional("DEBUG")]
		static void Log<T>(string text, IEnumerable<T> lines)
		{
			#if DEBUG
			T[] arr = lines.ToArray();
			if (arr.Length == 0) {
				Log(text + "<empty collection>");
			} else {
				Log(text + (arr[0] != null ? arr[0].ToString() : "<null>"));
				for (int i = 1; i < arr.Length; i++) {
					Log(new string(' ', text.Length) + (arr[i] != null ? arr[i].ToString() : "<null>"));
				}
			}
			#endif
		}
	}
}
