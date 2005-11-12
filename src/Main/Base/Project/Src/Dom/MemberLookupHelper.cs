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
	/// Class with methods to help finding the correct overload for a member.
	/// </summary>
	/// <remarks>
	/// This class tries to do member lookup as close to the C# spec (ECMA-334, § 14.3) as possible.
	/// Other languages might need custom lookup methods.
	/// </remarks>
	public static class MemberLookupHelper
	{
		#region FindOverload
		public static IMethod FindOverload(IList<IMethod> methods, IReturnType[] typeParameters, IReturnType[] arguments)
		{
			if (methods.Count == 0)
				return null;
			bool tmp;
			int[] ranking = RankOverloads(methods, typeParameters, arguments, false, out tmp);
			int bestRanking = -1;
			int best = 0;
			for (int i = 0; i < ranking.Length; i++) {
				if (ranking[i] > bestRanking) {
					bestRanking = ranking[i];
					best = i;
				}
			}
			return methods[best];
		}
		
		public static IProperty FindOverload(IList<IProperty> properties, IReturnType[] arguments)
		{
			if (properties.Count == 0)
				return null;
			bool tmp1; IReturnType[][] tmp2;
			List<IMethodOrProperty> newList = new List<IMethodOrProperty>(properties.Count);
			foreach (IProperty p in properties) newList.Add(p);
			int[] ranking = RankOverloads(newList, arguments, false, out tmp1, out tmp2);
			int bestRanking = -1;
			int best = 0;
			for (int i = 0; i < ranking.Length; i++) {
				if (ranking[i] > bestRanking) {
					bestRanking = ranking[i];
					best = i;
				}
			}
			return properties[best];
		}
		#endregion
		
		#region Rank method overloads
		/// <summary>
		/// Assigns a ranking score to each method in the <paramref name="list"/>.
		/// </summary>
		/// <param name="list">Link with the methods to check.<br/>
		/// <b>Generic methods in the input type are replaced by methods with have the types substituted!</b>
		/// </param>
		/// <param name="typeParameters">List with the type parameters passed to the method.
		/// Can be null (=no type parameters)</param>
		/// <param name="arguments">The types of the arguments passed to the method.
		/// A null return type means any argument type is allowed.</param>
		/// <param name="allowAdditionalArguments">Specifies whether the method can have
		/// more parameters than specified here. Useful for method insight scenarios.</param>
		/// <param name="acceptableMatch">Out parameter that is true when the best ranked
		/// method is acceptable for a method call (no invalid casts)</param>
		/// <returns>Integer array. Each value in the array </returns>
		public static int[] RankOverloads(IList<IMethod> list,
		                                  IReturnType[] typeParameters,
		                                  IReturnType[] arguments,
		                                  bool allowAdditionalArguments,
		                                  out bool acceptableMatch)
		{
			acceptableMatch = false;
			if (list.Count == 0) return new int[] {};
			
			List<IMethodOrProperty> l2 = new List<IMethodOrProperty>(list.Count);
			IReturnType[][] inferredTypeParameters;
			// See ECMA-334, § 14.3
			
			// If type parameters are specified, remove all methods from the list that do not
			// use the specified number of parameters.
			if (typeParameters != null && typeParameters.Length > 0) {
				for (int i = 0; i < list.Count; i++) {
					IMethod m = list[i];
					if (m.TypeParameters.Count == typeParameters.Length) {
						m = (IMethod)m.Clone();
						m.ReturnType = ConstructedReturnType.TranslateType(m.ReturnType, typeParameters, true);
						for (int j = 0; j < m.Parameters.Count; ++j) {
							m.Parameters[j].ReturnType = ConstructedReturnType.TranslateType(m.Parameters[j].ReturnType, typeParameters, true);
						}
						list[i] = m;
						l2.Add(m);
					}
				}
				
				int[] innerRanking = RankOverloads(l2, arguments, allowAdditionalArguments, out acceptableMatch, out inferredTypeParameters);
				int[] ranking = new int[list.Count];
				int innerIndex = 0;
				for (int i = 0; i < ranking.Length; i++) {
					if (list[i].TypeParameters.Count == typeParameters.Length) {
						ranking[i] = innerRanking[innerIndex++];
					} else {
						ranking[i] = 0;
					}
				}
				return ranking;
			} else {
				// Note that when there are no type parameters, methods having type parameters
				// are not removed, since the type inference process might be able to infer the
				// type arguments.
				foreach (IMethod m in list) l2.Add(m);
				
				int[] ranking = RankOverloads(l2, arguments, allowAdditionalArguments, out acceptableMatch, out inferredTypeParameters);
				ApplyInferredTypeParameters(list, inferredTypeParameters);
				return ranking;
			}
		}
		
		static void ApplyInferredTypeParameters(IList<IMethod> list, IReturnType[][] inferredTypeParameters)
		{
			if (inferredTypeParameters == null)
				return;
			for (int i = 0; i < list.Count; i++) {
				IReturnType[] inferred = inferredTypeParameters[i];
				if (inferred != null && inferred.Length > 0) {
					IMethod m = (IMethod)list[i].Clone();
					m.ReturnType = ConstructedReturnType.TranslateType(m.ReturnType, inferred, true);
					for (int j = 0; j < m.Parameters.Count; ++j) {
						m.Parameters[j].ReturnType = ConstructedReturnType.TranslateType(m.Parameters[j].ReturnType, inferred, true);
					}
					list[i] = m;
				}
			}
		}
		#endregion
		
		#region Main ranking algorithm
		/// <summary>
		/// The inner ranking engine. Works on both methods and properties.
		/// For parameter documentation, read the comments on the above method.
		/// </summary>
		public static int[] RankOverloads(IList<IMethodOrProperty> list,
		                                  IReturnType[] arguments,
		                                  bool allowAdditionalArguments,
		                                  out bool acceptableMatch,
		                                  out IReturnType[][] inferredTypeParameters)
		{
			// § 14.4.2 Overload resolution
			acceptableMatch = false;
			inferredTypeParameters = null;
			if (list.Count == 0) return new int[] {};
			
			int[] ranking = new int[list.Count];
			bool[] needToExpand = new bool[list.Count];
			int maxScore = 0;
			int baseScore = 0;
			int score;
			bool expanded;
			for (int i = 0; i < list.Count; i++) {
				if (IsApplicable(list[i].Parameters, arguments, allowAdditionalArguments, out score, out expanded)) {
					acceptableMatch = true;
					score = int.MaxValue;
				} else {
					baseScore = Math.Max(baseScore, score);
				}
				needToExpand[i] = expanded;
				ranking[i] = score;
				maxScore = Math.Max(maxScore, score);
			}
			// all overloads that have maxScore (normally those that are applicable)
			// have to be rescored.
			// The new scala starts with baseScore + 1 to ensure that all applicable members have
			// a higher score than non-applicable members
			
			// The first step is to expand the methods and do type argument substitution
			IReturnType[][] expandedParameters = ExpandParametersAndSubstitute(list, arguments, maxScore, ranking, needToExpand, out inferredTypeParameters);
			
			// find the best function member
			
			score = baseScore + 2;
			int bestIndex = -1;
			for (int i = 0; i < ranking.Length; i++) {
				if (ranking[i] == maxScore) {
					// the best function member is the one that is better than all other function members
					if (bestIndex < 0) {
						ranking[i] = score;
						bestIndex = i;
					} else {
						switch (GetBetterFunctionMember(arguments,
						                                list[i], expandedParameters[i], needToExpand[i],
						                                list[bestIndex], expandedParameters[bestIndex], needToExpand[bestIndex]))
						{
							case 0:
								// neither member is better
								ranking[i] = score;
								break;
							case 1:
								// the new member is better
								ranking[i] = ++score;
								bestIndex = i;
								break;
							case 2:
								// the old member is better
								ranking[i] = score - 1;
								// this is not really correct, normally we would need to
								// compare the member with other members
								break;
						}
					}
				}
			}
			
			return ranking;
		}
		
		static IReturnType[][] ExpandParametersAndSubstitute(IList<IMethodOrProperty> list,
		                                                     IReturnType[] arguments,
		                                                     int maxScore, int[] ranking, bool[] needToExpand,
		                                                     out IReturnType[][] inferredTypeParameters)
		{
			IReturnType[][] expandedParameters = new IReturnType[list.Count][];
			inferredTypeParameters = new IReturnType[list.Count][];
			for (int i = 0; i < ranking.Length; i++) {
				if (ranking[i] == maxScore) {
					IList<IParameter> parameters = list[i].Parameters;
					IReturnType[] typeParameters = (list[i] is IMethod) ? InferTypeArguments((IMethod)list[i], arguments) : null;
					inferredTypeParameters[i] = typeParameters;
					IReturnType paramsType = null;
					expandedParameters[i] = new IReturnType[arguments.Length];
					for (int j = 0; j < arguments.Length; j++) {
						if (j < parameters.Count) {
							IParameter parameter = parameters[j];
							if (parameter.IsParams && needToExpand[i]) {
								if (parameter.ReturnType.ArrayDimensions > 0) {
									paramsType = parameter.ReturnType.ArrayElementType;
									paramsType = ConstructedReturnType.TranslateType(paramsType, typeParameters, true);
								}
								expandedParameters[i][j] = paramsType;
							} else {
								expandedParameters[i][j] = ConstructedReturnType.TranslateType(parameter.ReturnType, typeParameters, true);
							}
						} else {
							expandedParameters[i][j] = paramsType;
						}
					}
				}
			}
			return expandedParameters;
		}
		
		/// <summary>
		/// Gets which function member is better. (§ 14.4.2.2)
		/// </summary>
		/// <param name="arguments">The arguments passed to the function</param>
		/// <param name="m1">The first method</param>
		/// <param name="parameters1">The expanded and substituted parameters of the first method</param>
		/// <param name="m2">The second method</param>
		/// <param name="parameters2">The expanded and substituted parameters of the second method</param>
		/// <returns>0 if neither method is better. 1 if m1 is better. 2 if m2 is better.</returns>
		static int GetBetterFunctionMember(IReturnType[] arguments,
		                                   IMethodOrProperty m1, IReturnType[] parameters1, bool isExpanded1,
		                                   IMethodOrProperty m2, IReturnType[] parameters2, bool isExpanded2)
		{
			int length = Math.Min(Math.Min(parameters1.Length, parameters2.Length), arguments.Length);
			bool foundBetterParamIn1 = false;
			bool foundBetterParamIn2 = false;
			for (int i = 0; i < length; i++) {
				if (arguments[i] == null)
					continue;
				int res = GetBetterConversion(arguments[i], parameters1[i], parameters2[i]);
				if (res == 1) foundBetterParamIn1 = true;
				if (res == 2) foundBetterParamIn2 = true;
			}
			if (foundBetterParamIn1 && !foundBetterParamIn2)
				return 1;
			if (foundBetterParamIn2 && !foundBetterParamIn1)
				return 2;
			if (foundBetterParamIn1 && foundBetterParamIn2)
				return 0; // ambigous
			// If none conversion is better than any other, it is possible that the
			// expanded parameter lists are the same:
			for (int i = 0; i < length; i++) {
				if (!object.Equals(parameters1[i], parameters2[i])) {
					// if expanded parameters are not the same, neither function member is better
					return 0;
				}
			}
			
			// the expanded parameters are the same, apply the tie-breaking rules from the spec:
			
			// if one method is generic and the other non-generic, the non-generic is better
			bool m1IsGeneric = (m1 is IMethod) ? ((IMethod)m1).TypeParameters.Count > 0 : false;
			bool m2IsGeneric = (m2 is IMethod) ? ((IMethod)m2).TypeParameters.Count > 0 : false;
			if (m1IsGeneric && !m2IsGeneric) return 2;
			if (m2IsGeneric && !m1IsGeneric) return 1;
			
			// for params parameters: non-expanded calls are better
			if (isExpanded1 && !isExpanded2) return 2;
			if (isExpanded2 && !isExpanded1) return 1;
			
			// if the number of parameters is different, the one with more parameters is better
			// this occurs when only when both methods are expanded
			if (m1.Parameters.Count > m2.Parameters.Count) return 1;
			if (m2.Parameters.Count > m1.Parameters.Count) return 2;
			
			IReturnType[] m1ParamTypes = new IReturnType[m1.Parameters.Count];
			IReturnType[] m2ParamTypes = new IReturnType[m2.Parameters.Count];
			for (int i = 0; i < m1ParamTypes.Length; i++) {
				m1ParamTypes[i] = m1.Parameters[i].ReturnType;
				m2ParamTypes[i] = m2.Parameters[i].ReturnType;
			}
			return GetMoreSpecific(m1ParamTypes, m2ParamTypes);
		}
		
		/// <summary>
		/// Gets which return type list is more specific.
		/// § 14.4.2.2: types with generic arguments are less specific than types with fixed arguments
		/// </summary>
		/// <returns>0 if both are equally specific, 1 if <paramref name="r"/> is more specific,
		/// 2 if <paramref name="s"/> is more specific.</returns>
		static int GetMoreSpecific(IList<IReturnType> r, IList<IReturnType> s)
		{
			bool foundMoreSpecificParamIn1 = false;
			bool foundMoreSpecificParamIn2 = false;
			int length = Math.Min(r.Count, s.Count);
			for (int i = 0; i < length; i++) {
				int res = GetMoreSpecific(r[i], s[i]);
				if (res == 1) foundMoreSpecificParamIn1 = true;
				if (res == 2) foundMoreSpecificParamIn2 = true;
			}
			if (foundMoreSpecificParamIn1 && !foundMoreSpecificParamIn2)
				return 1;
			if (foundMoreSpecificParamIn2 && !foundMoreSpecificParamIn1)
				return 2;
			return 0;
		}
		
		static int GetMoreSpecific(IReturnType r, IReturnType s)
		{
			if (r == null && s == null) return 0;
			if (r == null) return 2;
			if (s == null) return 1;
			if (r is GenericReturnType && !(s is GenericReturnType))
				return 2;
			if (s is GenericReturnType && !(r is GenericReturnType))
				return 1;
			if (r.ArrayDimensions > 0 && s.ArrayDimensions > 0)
				return GetMoreSpecific(r.ArrayElementType, s.ArrayElementType);
			if (r.TypeArguments != null && s.TypeArguments != null)
				return GetMoreSpecific(r.TypeArguments, s.TypeArguments);
			return 0;
		}
		#endregion
		
		#region Type Argument Inference
		static IReturnType[] InferTypeArguments(IMethod method, IReturnType[] arguments)
		{
			// §25.6.4 Inference of type arguments
			int count = method.TypeParameters.Count;
			if (count == 0) return null;
			IReturnType[] result = new IReturnType[count];
			IList<IParameter> parameters = method.Parameters;
			for (int i = 0; i < arguments.Length; i++) {
				if (i >= parameters.Count)
					break;
				if (!InferTypeArgument(parameters[i].ReturnType, arguments[i], result)) {
					// inferring failed: maybe this is a params parameter that must be expanded?
					if (parameters[i].IsParams && parameters[i].ReturnType.ArrayDimensions == 1) {
						InferTypeArgument(parameters[i].ReturnType.ArrayElementType, arguments[i], result);
					}
				}
			}
			// only return the result array when there something was inferred
			for (int i = 0; i < result.Length; i++) {
				if (result[i] != null) {
					return result;
				}
			}
			return null;
		}
		
		static bool InferTypeArgument(IReturnType expectedArgument, IReturnType passedArgument, IReturnType[] outputArray)
		{
			if (passedArgument == null) return true; // TODO: NullTypeReference
			if (expectedArgument != null && expectedArgument.ArrayDimensions > 0) {
				if (expectedArgument.ArrayDimensions == passedArgument.ArrayDimensions) {
					return InferTypeArgument(expectedArgument.ArrayElementType, passedArgument.ArrayElementType, outputArray);
				} else if (passedArgument.TypeArguments != null) {
					switch (passedArgument.FullyQualifiedName) {
						case "System.Collections.Generic.IList":
						case "System.Collections.Generic.ICollection":
						case "System.Collections.Generic.IEnumerable":
							return InferTypeArgument(expectedArgument.ArrayElementType, passedArgument.TypeArguments[0], outputArray);
					}
				}
				// If P is an array type, and A is not an array type of the same rank,
				// or an instantiation of IList<>, ICollection<>, or IEnumerable<>, then
				// type inference fails for the generic method.
				return false;
			}
			GenericReturnType methodTP = expectedArgument as GenericReturnType;
			if (methodTP != null && methodTP.TypeParameter.Method != null) {
				if (methodTP.TypeParameter.Index < outputArray.Length) {
					outputArray[methodTP.TypeParameter.Index] = passedArgument;
				}
				return true;
			}
			if (expectedArgument.TypeArguments != null) {
				// The spec for this case is quite complex.
				// For our purposes, we can simplify enourmously:
				if (passedArgument.TypeArguments == null) return false;
				int count = Math.Min(expectedArgument.TypeArguments.Count, passedArgument.TypeArguments.Count);
				for (int i = 0; i < count; i++) {
					InferTypeArgument(expectedArgument.TypeArguments[i], passedArgument.TypeArguments[i], outputArray);
				}
			}
			return true;
		}
		#endregion
		
		#region IsApplicable
		static bool IsApplicable(IList<IParameter> parameters,
		                         IReturnType[] arguments,
		                         bool allowAdditionalArguments,
		                         out int score,
		                         out bool expanded)
		{
			// see ECMA-334, § 14.4.2.1
			// TODO: recognize ref/out (needs info about passing mode for arguments, you have to introduce RefReturnType)
			
			expanded = false;
			score = 0;
			if (parameters.Count == 0)
				return arguments.Length == 0;
			if (!allowAdditionalArguments && parameters.Count > arguments.Length + 1)
				return false;
			int lastParameter = parameters.Count - 1;
			
			// check all arguments except the last
			bool ok = true;
			for (int i = 0; i < Math.Min(lastParameter, arguments.Length); i++) {
				if (IsApplicable(arguments[i], parameters[i].ReturnType)) {
					score++;
				} else {
					ok = false;
				}
			}
			if (!ok) {
				return false;
			}
			if (parameters.Count == arguments.Length) {
				// try if method is applicable in normal form by checking last argument
				if (IsApplicable(arguments[lastParameter], parameters[lastParameter].ReturnType)) {
					return true;
				}
			}
			// method is not applicable in normal form, try expanded form:
			// - last parameter must be params array
			if (!parameters[lastParameter].IsParams) {
				return false;
			}
			expanded = true;
			score++;
			
			// - all additional parameters must be applicable to the unpacked array
			IReturnType rt = parameters[lastParameter].ReturnType;
			if (rt == null || rt.ArrayDimensions == 0) {
				return false;
			}
			for (int i = lastParameter; i < arguments.Length; i++) {
				if (IsApplicable(arguments[i], rt.ArrayElementType)) {
					score++;
				} else {
					ok = false;
				}
			}
			return ok;
		}
		
		static bool IsApplicable(IReturnType argument, IReturnType expected)
		{
			if (argument == null) // TODO: Use NullReturnType instead of no return type
				return true; // "null" can be passed for any argument
			if (expected is GenericReturnType) {
				foreach (IReturnType constraint in ((GenericReturnType)expected).TypeParameter.Constraints) {
					if (!ConversionExists(argument, constraint)) {
						return false;
					}
				}
			}
			return ConversionExists(argument, expected);
		}
		#endregion
		
		#region Conversion exists
		/// <summary>
		/// Checks if an implicit conversion exists from <paramref name="from"/> to <paramref name="to"/>.
		/// </summary>
		public static bool ConversionExists(IReturnType from, IReturnType to)
		{
			// ECMA-334, § 13.1 Implicit conversions
			
			// Identity conversion:
			if (from == to) return true;
			if (from == null || to == null) return false;
			if (from.Equals(to)) {
				return true;
			}
			
			bool fromIsDefault = from.IsDefaultReturnType;
			bool toIsDefault = to.IsDefaultReturnType;
			
			if (fromIsDefault && toIsDefault) {
				// Implicit numeric conversions:
				int f = GetPrimitiveType(from);
				int t = GetPrimitiveType(to);
				if (f == SByte && (t == Short || t == Int || t == Long || t == Float || t == Double || t == Decimal))
					return true;
				if (f == Byte && (t == Short || t == UShort || t == Int || t == UInt || t == Long || t == ULong || t == Float || t == Double || t == Decimal))
					return true;
				if (f == Short && (t == Int || t == Long || t == Float || t == Double || t == Decimal))
					return true;
				if (f == UShort && (t == Int || t == UInt || t == Long || t == ULong || t == Float || t == Double || t == Decimal))
					return true;
				if (f == Int && (t == Long || t == Float || t == Double || t == Decimal))
					return true;
				if (f == UInt && (t == Long || t == ULong || t == Float || t == Double || t == Decimal))
					return true;
				if ((f == Long || f == ULong) && (t == Float || t == Double || t == Decimal))
					return true;
				if (f == Char && (t == UShort || t == Int || t == UInt || t == Long || t == ULong || t == Float || t == Double || t == Decimal))
					return true;
				if (f == Float && t == Double)
					return true;
			}
			// Implicit reference conversions:
			
			if (toIsDefault && to.FullyQualifiedName == "System.Object") {
				return true; // from any type to object
			}
			if (toIsDefault && (fromIsDefault || from.ArrayDimensions > 0)) {
				IClass c1 = from.GetUnderlyingClass();
				IClass c2 = to.GetUnderlyingClass();
				if (c1 != null && c1.IsTypeInInheritanceTree(c2)) {
					return true;
				}
			}
			if (from.ArrayDimensions > 0 && from.ArrayDimensions == to.ArrayDimensions) {
				// from array to other array type
				return ConversionExists(from.ArrayElementType, to.ArrayElementType);
			}
			return false;
		}
		#endregion
		
		#region Better conversion
		/// <summary>
		/// Gets if the conversion from <paramref name="from"/> to <paramref name="to1"/> is better than
		/// the conversion from <paramref name="from"/> to <paramref name="to2"/>.
		/// </summary>
		/// <returns>
		/// 0 = neither conversion is better<br/>
		/// 1 = from -> to1 is the better conversion<br/>
		/// 2 = from -> to2 is the better conversion.
		/// </returns>
		public static int GetBetterConversion(IReturnType from, IReturnType to1, IReturnType to2)
		{
			if (from == null) return 0;
			if (to1 == null) return 2;
			if (to2 == null) return 1;
			
			// See ECMA-334, § 14.4.2.3
			
			// If T1 and T2 are the same type, neither conversion is better.
			if (to1.Equals(to2)) {
				return 0;
			}
			// If S is T1, C1 is the better conversion.
			if (from.Equals(to1)) {
				return 1;
			}
			// If S is T2, C2 is the better conversion.
			if (from.Equals(to2)) {
				return 2;
			}
			bool canConvertFrom1To2 = ConversionExists(to1, to2);
			bool canConvertFrom2To1 = ConversionExists(to2, to1);
			// If an implicit conversion from T1 to T2 exists, and no implicit conversion
			// from T2 to T1 exists, C1 is the better conversion.
			if (canConvertFrom1To2 && !canConvertFrom2To1) {
				return 1;
			}
			// If an implicit conversion from T2 to T1 exists, and no implicit conversion
			// from T1 to T2 exists, C2 is the better conversion.
			if (canConvertFrom2To1 && !canConvertFrom1To2) {
				return 2;
			}
			if (to1.IsDefaultReturnType && to2.IsDefaultReturnType) {
				return GetBetterPrimitiveConversion(to1, to2);
			}
			// Otherwise, neither conversion is better.
			return 0;
		}
		
		const int Byte   = 1;
		const int Short  = 2;
		const int Int    = 3;
		const int Long   = 4;
		const int SByte  = 5;
		const int UShort = 6;
		const int UInt   = 7;
		const int ULong  = 8;
		const int Float  = 9;
		const int Double = 10;
		const int Char   = 11;
		const int Decimal= 12;
		
		static int GetBetterPrimitiveConversion(IReturnType to1, IReturnType to2)
		{
			int t1 = GetPrimitiveType(to1);
			int t2 = GetPrimitiveType(to2);
			if (t1 == 0 || t2 == 0) return 0; // not primitive
			if (t1 == SByte && (t2 == Byte || t2 == UShort || t2 == UInt || t2 == ULong))
				return 1;
			if (t2 == SByte && (t1 == Byte || t1 == UShort || t1 == UInt || t1 == ULong))
				return 2;
			if (t1 == Short && (t2 == UShort || t2 == UInt || t2 == ULong))
				return 1;
			if (t2 == Short && (t1 == UShort || t1 == UInt || t1 == ULong))
				return 2;
			if (t1 == Int && (t2 == UInt || t2 == ULong))
				return 1;
			if (t2 == Int && (t1 == UInt || t1 == ULong))
				return 2;
			if (t1 == Long && t2 == ULong)
				return 1;
			if (t2 == Long && t1 == ULong)
				return 2;
			return 0;
		}
		
		static int GetPrimitiveType(IReturnType t)
		{
			switch (t.FullyQualifiedName) {
					case "System.SByte": return SByte;
					case "System.Byte": return Byte;
					case "System.Int16": return Short;
					case "System.UInt16": return UShort;
					case "System.Int32": return Int;
					case "System.UInt32": return UInt;
					case "System.Int64": return Long;
					case "System.UInt64": return ULong;
					case "System.Single": return Float;
					case "System.Double": return Double;
					case "System.Char": return Char;
					case "System.Decimal": return Decimal;
					default: return 0;
			}
		}
		#endregion
		
		/// <summary>
		/// Gets the common base type of a and b.
		/// </summary>
		public static IReturnType GetCommonType(IReturnType a, IReturnType b)
		{
			if (a == null) return b;
			if (b == null) return a;
			if (ConversionExists(a, b))
				return b;
			if (ConversionExists(b, a))
				return a;
			IClass c = a.GetUnderlyingClass();
			if (c != null) {
				foreach (IClass baseClass in c.ClassInheritanceTree) {
					IReturnType baseType = baseClass.DefaultReturnType;
					if (baseClass.TypeParameters.Count > 0) {
						IReturnType[] typeArguments = new IReturnType[baseClass.TypeParameters.Count];
						for (int i = 0; i < typeArguments.Length; i++) {
							typeArguments[i] = GetTypeParameterPassedToBaseClass(a, baseClass, i);
						}
						baseType = new ConstructedReturnType(baseType, typeArguments);
					}
					if (ConversionExists(b, baseType))
						return baseType;
				}
			}
			return ReflectionReturnType.Object;
		}
		
		/// <summary>
		/// Gets the type parameter that was passed to a certain base class.
		/// For example, when <paramref name="returnType"/> is Dictionary(of string, int)
		/// this method will return KeyValuePair(of string, int)
		/// </summary>
		public static IReturnType GetTypeParameterPassedToBaseClass(IReturnType returnType, IClass baseClass, int baseClassTypeParameterIndex)
		{
			IClass c = returnType.GetUnderlyingClass();
			if (c == null) return null;
			if (baseClass.CompareTo(c) == 0) {
				if (returnType.TypeArguments == null || baseClassTypeParameterIndex >= returnType.TypeArguments.Count)
					return null;
				return returnType.TypeArguments[baseClassTypeParameterIndex];
			}
			foreach (IReturnType baseType in c.BaseTypes) {
				if (baseClass.CompareTo(baseType.GetUnderlyingClass()) == 0) {
					if (baseType.TypeArguments == null || baseClassTypeParameterIndex >= baseType.TypeArguments.Count)
						return null;
					IReturnType result = baseType.TypeArguments[baseClassTypeParameterIndex];
					if (returnType.TypeArguments != null) {
						result = ConstructedReturnType.TranslateType(result, returnType.TypeArguments, false);
					}
					return result;
				}
			}
			return null;
		}
	}
}
