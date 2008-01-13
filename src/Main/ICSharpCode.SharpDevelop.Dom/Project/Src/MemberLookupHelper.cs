// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Class with methods to help finding the correct overload for a member.
	/// </summary>
	/// <remarks>
	/// This class does member lookup as specified by the C# spec (ECMA-334, § 14.3).
	/// Other languages might need custom lookup methods.
	/// </remarks>
	public static class MemberLookupHelper
	{
		static List<IMember> GetAllMembers(IReturnType rt)
		{
			List<IMember> members = new List<IMember>();
			if (rt != null) {
				rt.GetMethods().ForEach(members.Add);
				rt.GetProperties().ForEach(members.Add);
				rt.GetFields().ForEach(members.Add);
				rt.GetEvents().ForEach(members.Add);
			}
			return members;
		}
		
		public static List<IList<IMember>> LookupMember(
			IReturnType type, string name, IClass callingClass,
			LanguageProperties language, bool isInvocation)
		{
			if (language == null)
				throw new ArgumentNullException("language");
			
			bool isClassInInheritanceTree = false;
			IClass underlyingClass = type.GetUnderlyingClass();
			if (underlyingClass != null)
				isClassInInheritanceTree = underlyingClass.IsTypeInInheritanceTree(callingClass);
			
			IEnumerable<IMember> members;
			if (language == LanguageProperties.VBNet && language.NameComparer.Equals(name, "New")) {
				members = GetAllMembers(type).OfType<IMethod>().Where(m => m.IsConstructor).Select(m=>(IMember)m);
			} else {
				members = GetAllMembers(type).Where(m => language.NameComparer.Equals(m.Name, name));
			}
			
			return LookupMember(members, callingClass, isClassInInheritanceTree, isInvocation);
		}
		
		class SignatureComparer : IEqualityComparer<IMethod>
		{
			public bool Equals(IMethod x, IMethod y)
			{
				if (GetHashCode(x) != GetHashCode(y))
					return false;
				var paramsX = x.Parameters;
				var paramsY = y.Parameters;
				if (paramsX.Count != paramsY.Count)
					return false;
				if (x.TypeParameters.Count != y.TypeParameters.Count)
					return false;
				for (int i = 0; i < paramsX.Count; i++) {
					IParameter px = paramsX[i];
					IParameter py = paramsY[i];
					if ((px.IsOut || px.IsRef) != (py.IsOut || py.IsRef))
						return false;
					if (!object.Equals(px.ReturnType, py.ReturnType))
						return false;
				}
				return true;
			}
			
			Dictionary<IMethod, int> cachedHashes = new Dictionary<IMethod, int>();
			
			public int GetHashCode(IMethod obj)
			{
				int hashCode;
				if (cachedHashes.TryGetValue(obj, out hashCode))
					return hashCode;
				hashCode = obj.TypeParameters.Count;
				unchecked {
					foreach (IParameter p in obj.Parameters) {
						hashCode *= 1000000579;
						if (p.IsOut || p.IsRef)
							hashCode += 1;
						hashCode += p.ReturnType.GetHashCode();
					}
				}
				cachedHashes[obj] = hashCode;
				return hashCode;
			}
		}
		
		sealed class InheritanceLevelComparer : IComparer<IClass>
		{
			public readonly static InheritanceLevelComparer Instance = new InheritanceLevelComparer();
			
			public int Compare(IClass x, IClass y)
			{
				if (x == y)
					return 0;
				if (x.IsTypeInInheritanceTree(y))
					return 1;
				else
					return -1;
			}
		}
		
		public static List<IList<IMember>> LookupMember(
			IEnumerable<IMember> possibleMembers, IClass callingClass,
			bool isClassInInheritanceTree, bool isInvocation)
		{
//			Console.WriteLine("Possible members:");
//			foreach (IMember m in possibleMembers) {
//				Console.WriteLine("  " + m.DotNetName);
//			}
			
			IEnumerable<IMember> accessibleMembers = possibleMembers.Where(member => member.IsAccessible(callingClass, isClassInInheritanceTree));
			if (isInvocation) {
				accessibleMembers = accessibleMembers.Where(IsInvocable);
			}
			
			// base most member => most derived member
			//Dictionary<IMember, IMember> overrideDict = new Dictionary<IMember, IMember>();
			
			Dictionary<IMethod, IMethod> overrideMethodDict = new Dictionary<IMethod, IMethod>(new SignatureComparer());
			IMember nonMethodOverride = null;
			
			List<IList<IMember>> allResults = new List<IList<IMember>>();
			List<IMember> results = new List<IMember>();
			foreach (var group in accessibleMembers
			         .GroupBy((IMember m) => m.DeclaringType.GetCompoundClass())
			         .OrderByDescending(g => g.Key, InheritanceLevelComparer.Instance))
			{
				//Console.WriteLine("Member group " + group.Key);
				foreach (IMember m in group) {
					//Console.WriteLine("  " + m.DotNetName);
					if (m.IsOverride) {
						IMethod method = m as IMethod;
						if (method != null)
							overrideMethodDict[method] = method;
						else
							nonMethodOverride = m;
					} else {
						IMethod method = m as IMethod;
						if (method != null && overrideMethodDict.TryGetValue(method, out method))
							results.Add(method);
						else
							results.Add(m);
					}
				}
				if (results.Count > 0) {
					allResults.Add(results);
					results = new List<IMember>();
				}
			}
			return allResults;
		}
		
		static bool IsInvocable(IMember member)
		{
			if (member is IMethod || member is IEvent)
				return true;
			IProperty p = member as IProperty;
			if (p != null && p.Parameters.Count > 0)
				return true;
			IClass c = member.ReturnType.GetUnderlyingClass();
			return c != null && c.ClassType == ClassType.Delegate;
		}
		
		/// <summary>
		/// Gets all accessible members, including indexers and constructors.
		/// </summary>
		public static List<IMember> GetAccessibleMembers(IReturnType rt, IClass callingClass, LanguageProperties language)
		{
			if (language == null)
				throw new ArgumentNullException("language");
			
			bool isClassInInheritanceTree = false;
			IClass underlyingClass = rt.GetUnderlyingClass();
			if (underlyingClass != null)
				isClassInInheritanceTree = underlyingClass.IsTypeInInheritanceTree(callingClass);
			
			List<IMember> result = new List<IMember>();
			foreach (var g in GetAllMembers(rt).GroupBy(m => m.Name, language.NameComparer)) {
				foreach (var group in LookupMember(g, callingClass, isClassInInheritanceTree, false)) {
					result.AddRange(group);
				}
			}
			return result;
		}
		
		#region FindOverload
		/// <summary>
		/// Finds the correct overload according to the C# specification.
		/// </summary>
		/// <param name="methods">List with the methods to check.<br/>
		/// <b>Generic methods in the input type are replaced by methods with have the types substituted!</b>
		/// </param>
		/// <param name="arguments">The types of the arguments passed to the method.</param>
		/// <param name="resultIsAcceptable">Out parameter. Will be true if the resulting method
		/// is an acceptable match, false if the resulting method is just a guess and will lead
		/// to a compile error.</param>
		/// <returns>The method that will be called.</returns>
		public static IMethod FindOverload(IList<IMethod> methods, IReturnType[] arguments, out bool resultIsAcceptable)
		{
			resultIsAcceptable = false;
			if (methods.Count == 0)
				return null;
			int[] ranking = RankOverloads(methods, arguments, false, out resultIsAcceptable);
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
		/// <param name="list">List with the methods to check.<br/>
		/// <b>Generic methods in the input type are replaced by methods with have the types substituted!</b>
		/// </param>
		/// <param name="arguments">The types of the arguments passed to the method.
		/// A null return type means any argument type is allowed.</param>
		/// <param name="allowAdditionalArguments">Specifies whether the method can have
		/// more parameters than specified here. Useful for method insight scenarios.</param>
		/// <param name="acceptableMatch">Out parameter that is true when the best ranked
		/// method is acceptable for a method call (no invalid casts)</param>
		/// <returns>Integer array. Each value in the array </returns>
		public static int[] RankOverloads(IList<IMethod> list,
		                                  IReturnType[] arguments,
		                                  bool allowAdditionalArguments,
		                                  out bool acceptableMatch)
		{
			acceptableMatch = false;
			if (list.Count == 0) return new int[] {};
			
			IReturnType[][] inferredTypeParameters;
			// See ECMA-334, § 14.3
			
			// We longer pass the explicit type arguments to RankOverloads, this is now done when
			// the method group is constructed.
			
			// Note that when there are no type parameters, methods having type parameters
			// are not removed, since the type inference process might be able to infer the
			// type arguments.
			
			List<IMethodOrProperty> l2 = new List<IMethodOrProperty>();
			foreach (IMethod m in list) l2.Add(m);
			int[] ranking = RankOverloads(l2, arguments, allowAdditionalArguments, out acceptableMatch, out inferredTypeParameters);
			ApplyInferredTypeParameters(list, inferredTypeParameters);
			return ranking;
		}
		
		static void ApplyInferredTypeParameters(IList<IMethod> list, IReturnType[][] inferredTypeParameters)
		{
			if (inferredTypeParameters == null)
				return;
			for (int i = 0; i < list.Count; i++) {
				IReturnType[] inferred = inferredTypeParameters[i];
				if (inferred != null && inferred.Length > 0) {
					IMethod m = (IMethod)list[i].CreateSpecializedMember();
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
								// this is not really correct, we would need to compare the member with other members
								// but this works as we're interested in the best overload only
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
								if (parameter.ReturnType.IsArrayReturnType) {
									paramsType = parameter.ReturnType.CastToArrayReturnType().ArrayElementType;
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
			if (r.IsGenericReturnType && !(s.IsGenericReturnType))
				return 2;
			if (s.IsGenericReturnType && !(r.IsGenericReturnType))
				return 1;
			if (r.IsArrayReturnType && s.IsArrayReturnType)
				return GetMoreSpecific(r.CastToArrayReturnType().ArrayElementType, s.CastToArrayReturnType().ArrayElementType);
			if (r.IsConstructedReturnType && s.IsConstructedReturnType)
				return GetMoreSpecific(r.CastToConstructedReturnType().TypeArguments, s.CastToConstructedReturnType().TypeArguments);
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
					if (parameters[i].IsParams && parameters[i].ReturnType.IsArrayReturnType) {
						ArrayReturnType art = parameters[i].ReturnType.CastToArrayReturnType();
						if (art.ArrayDimensions == 1) {
							InferTypeArgument(art.ArrayElementType, arguments[i], result);
						}
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
		
		/// <summary>
		/// Infers type arguments specified by passing expectedArgument as parameter where passedArgument
		/// was expected. The resulting type arguments are written to outputArray.
		/// Returns false when expectedArgument and passedArgument are incompatible, otherwise true
		/// is returned (true is used both for successful inferring and other kind of errors).
		/// </summary>
		/// <remarks>
		/// The C# spec (§ 25.6.4) has a bug: it says that type inference works if the passedArgument is IEnumerable{T}
		/// and the expectedArgument is an array; passedArgument and expectedArgument must be swapped here.
		/// </remarks>
		public static bool InferTypeArgument(IReturnType expectedArgument, IReturnType passedArgument, IReturnType[] outputArray)
		{
			if (expectedArgument == null) return true;
			if (passedArgument == null || passedArgument == NullReturnType.Instance) return true;
			
			if (passedArgument.IsArrayReturnType) {
				IReturnType passedArrayElementType = passedArgument.CastToArrayReturnType().ArrayElementType;
				if (expectedArgument.IsArrayReturnType && expectedArgument.CastToArrayReturnType().ArrayDimensions == passedArgument.CastToArrayReturnType().ArrayDimensions) {
					return InferTypeArgument(expectedArgument.CastToArrayReturnType().ArrayElementType, passedArrayElementType, outputArray);
				} else if (expectedArgument.IsConstructedReturnType) {
					switch (expectedArgument.FullyQualifiedName) {
						case "System.Collections.Generic.IList":
						case "System.Collections.Generic.ICollection":
						case "System.Collections.Generic.IEnumerable":
							return InferTypeArgument(expectedArgument.CastToConstructedReturnType().TypeArguments[0], passedArrayElementType, outputArray);
					}
				}
				// If P is an array type, and A is not an array type of the same rank,
				// or an instantiation of IList<>, ICollection<>, or IEnumerable<>, then
				// type inference fails for the generic method.
				return false;
			}
			if (expectedArgument.IsGenericReturnType) {
				GenericReturnType methodTP = expectedArgument.CastToGenericReturnType();
				if (methodTP.TypeParameter.Method != null) {
					if (methodTP.TypeParameter.Index < outputArray.Length) {
						outputArray[methodTP.TypeParameter.Index] = passedArgument;
					}
					return true;
				}
			}
			if (expectedArgument.IsConstructedReturnType) {
				// The spec for this case is quite complex.
				// For our purposes, we can simplify enourmously:
				if (!passedArgument.IsConstructedReturnType) return false;
				
				IList<IReturnType> expectedTA = expectedArgument.CastToConstructedReturnType().TypeArguments;
				IList<IReturnType> passedTA   = passedArgument.CastToConstructedReturnType().TypeArguments;
				
				int count = Math.Min(expectedTA.Count, passedTA.Count);
				for (int i = 0; i < count; i++) {
					InferTypeArgument(expectedTA[i], passedTA[i], outputArray);
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
				if (IsApplicable(arguments[i], parameters[i])) {
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
				if (IsApplicable(arguments[lastParameter], parameters[lastParameter])) {
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
			if (rt == null || !rt.IsArrayReturnType) {
				return false;
			}
			for (int i = lastParameter; i < arguments.Length; i++) {
				if (IsApplicable(arguments[i], rt.CastToArrayReturnType().ArrayElementType)) {
					score++;
				} else {
					ok = false;
				}
			}
			return ok;
		}
		
		static bool IsApplicable(IReturnType argument, IParameter expected)
		{
			bool parameterIsRefOrOut = expected.IsRef || expected.IsOut;
			bool argumentIsRefOrOut = argument != null && argument.IsDecoratingReturnType<ReferenceReturnType>();
			if (parameterIsRefOrOut != argumentIsRefOrOut)
				return false;
			if (parameterIsRefOrOut) {
				return object.Equals(argument, expected.ReturnType);
			} else {
				return IsApplicable(argument, expected.ReturnType);
			}
		}
		
		public static bool IsApplicable(IReturnType argument, IReturnType expected)
		{
			if (argument == null || argument == NullReturnType.Instance)
				return true; // "null" can be passed for any argument
			return ConversionExistsInternal(argument, expected, true);
		}
		#endregion
		
		#region Conversion exists
		/// <summary>
		/// Checks if an implicit conversion exists from <paramref name="from"/> to <paramref name="to"/>.
		/// </summary>
		public static bool ConversionExists(IReturnType from, IReturnType to)
		{
			return ConversionExistsInternal(from, to, false);
		}
		
		static bool ConversionExistsInternal(IReturnType from, IReturnType to, bool allowGenericTarget)
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
			
			if ((toIsDefault || to.IsConstructedReturnType || to.IsGenericReturnType)
			    && (fromIsDefault || from.IsArrayReturnType || from.IsConstructedReturnType))
			{
				foreach (IReturnType baseTypeOfFrom in GetTypeInheritanceTree(from)) {
					if (IsConstructedConversionToGenericReturnType(baseTypeOfFrom, to, allowGenericTarget))
						return true;
				}
			}
			
			if (from.IsArrayReturnType && to.IsArrayReturnType) {
				ArrayReturnType fromArt = from.CastToArrayReturnType();
				ArrayReturnType toArt   = to.CastToArrayReturnType();
				// from array to other array type
				if (fromArt.ArrayDimensions == toArt.ArrayDimensions) {
					return ConversionExistsInternal(fromArt.ArrayElementType, toArt.ArrayElementType, allowGenericTarget);
				}
			}
			
			if (from.IsDecoratingReturnType<AnonymousMethodReturnType>() && (toIsDefault || to.IsConstructedReturnType)) {
				IList<IParameter> methodParameters = from.CastToDecoratingReturnType<AnonymousMethodReturnType>().MethodParameters;
				IClass toClass = to.GetUnderlyingClass();
				if (toClass != null && toClass.ClassType == ClassType.Delegate) {
					if (methodParameters == null) {
						return true;
					} else {
						foreach (IMethod m in toClass.Methods) {
							if (m.Name == "Invoke") {
								return m.Parameters.Count == methodParameters.Count;
							}
						}
						return true;
					}
				}
			}
			
			return false;
		}
		
		static bool IsConstructedConversionToGenericReturnType(IReturnType from, IReturnType to, bool allowGenericTarget)
		{
			// null could be passed when type arguments could not be resolved/inferred
			if (from == null && to == null)
				return true;
			if (from == null || to == null)
				return false;
			
			if (from.Equals(to))
				return true;
			
			if (!allowGenericTarget)
				return false;
			
			if (to.IsGenericReturnType) {
				foreach (IReturnType constraintType in to.CastToGenericReturnType().TypeParameter.Constraints) {
					if (!ConversionExistsInternal(from, constraintType, allowGenericTarget)) {
						return false;
					}
				}
				return true;
			}
			
			// for conversions like from IEnumerable<string> to IEnumerable<T>, where T is a GenericReturnType
			ConstructedReturnType cFrom = from.CastToConstructedReturnType();
			ConstructedReturnType cTo   = to.CastToConstructedReturnType();
			if (cFrom != null && cTo != null) {
				if (cFrom.FullyQualifiedName == cTo.FullyQualifiedName && cFrom.TypeArguments.Count == cTo.TypeArguments.Count) {
					for (int i = 0; i < cFrom.TypeArguments.Count; i++) {
						if (!IsConstructedConversionToGenericReturnType(cFrom.TypeArguments[i], cTo.TypeArguments[i], allowGenericTarget))
							return false;
					}
					return true;
				}
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
		
		#region GetCommonType
		/// <summary>
		/// Gets the common base type of a and b.
		/// </summary>
		public static IReturnType GetCommonType(IProjectContent projectContent, IReturnType a, IReturnType b)
		{
			if (projectContent == null)
				throw new ArgumentNullException("projectContent");
			if (a == null) return b;
			if (b == null) return a;
			if (ConversionExists(a, b))
				return b;
			//if (ConversionExists(b, a)) - not required because the first baseTypeOfA is a
			//	return a;
			foreach (IReturnType baseTypeOfA in GetTypeInheritanceTree(a)) {
				if (ConversionExists(b, baseTypeOfA))
					return baseTypeOfA;
			}
			return projectContent.SystemTypes.Object;
		}
		#endregion
		
		#region GetTypeParameterPassedToBaseClass / GetTypeInheritanceTree
		/// <summary>
		/// Gets the type parameter that was passed to a certain base class.
		/// For example, when <paramref name="returnType"/> is Dictionary(of string, int)
		/// this method will return KeyValuePair(of string, int)
		/// </summary>
		public static IReturnType GetTypeParameterPassedToBaseClass(IReturnType parentType, IClass baseClass, int baseClassTypeParameterIndex)
		{
			foreach (IReturnType rt in GetTypeInheritanceTree(parentType)) {
				ConstructedReturnType crt = rt.CastToConstructedReturnType();
				if (crt != null && baseClass.CompareTo(rt.GetUnderlyingClass()) == 0) {
					if (baseClassTypeParameterIndex < crt.TypeArguments.Count) {
						return crt.TypeArguments[baseClassTypeParameterIndex];
					}
				}
			}
			return null;
		}
		
		/// <summary>
		/// Translates typeToTranslate using the type arguments from parentType;
		/// </summary>
		static IReturnType TranslateIfRequired(IReturnType parentType, IReturnType typeToTranslate)
		{
			if (typeToTranslate == null)
				return null;
			ConstructedReturnType parentConstructedType = parentType.CastToConstructedReturnType();
			if (parentConstructedType != null) {
				return ConstructedReturnType.TranslateType(typeToTranslate, parentConstructedType.TypeArguments, false);
			} else {
				return typeToTranslate;
			}
		}
		
		/// <summary>
		/// Gets all types the specified type inherits from (all classes and interfaces).
		/// Unlike the class inheritance tree, this method takes care of type arguments and calculates the type
		/// arguments that are passed to base classes.
		/// </summary>
		public static IEnumerable<IReturnType> GetTypeInheritanceTree(IReturnType typeToListInheritanceTreeFor)
		{
			if (typeToListInheritanceTreeFor == null)
				throw new ArgumentNullException("typeToListInheritanceTreeFor");
			
			IClass classToListInheritanceTreeFor = typeToListInheritanceTreeFor.GetUnderlyingClass();
			if (classToListInheritanceTreeFor == null)
				return new IReturnType[] { typeToListInheritanceTreeFor };
			
			if (typeToListInheritanceTreeFor.IsArrayReturnType) {
				IReturnType elementType = typeToListInheritanceTreeFor.CastToArrayReturnType().ArrayElementType;
				List<IReturnType> resultList = new List<IReturnType>();
				resultList.Add(typeToListInheritanceTreeFor);
				resultList.AddRange(GetTypeInheritanceTree(
					new ConstructedReturnType(
						classToListInheritanceTreeFor.ProjectContent.GetClass("System.Collections.Generic.IList", 1).DefaultReturnType,
						new IReturnType[] { elementType }
					)
				));
				resultList.Add(classToListInheritanceTreeFor.ProjectContent.GetClass("System.Collections.IList", 0).DefaultReturnType);
				resultList.Add(classToListInheritanceTreeFor.ProjectContent.GetClass("System.Collections.ICollection", 0).DefaultReturnType);
				// non-generic IEnumerable is already added by generic IEnumerable
				return resultList;
			}
			
			List<IReturnType> visitedList = new List<IReturnType>();
			Queue<IReturnType> typesToVisit = new Queue<IReturnType>();
			bool enqueuedLastBaseType = false;
			
			IReturnType currentType = typeToListInheritanceTreeFor;
			IClass currentClass = classToListInheritanceTreeFor;
			IReturnType nextType;
			do {
				if (currentClass != null) {
					if (!visitedList.Contains(currentType)) {
						visitedList.Add(currentType);
						foreach (IReturnType type in currentClass.BaseTypes) {
							typesToVisit.Enqueue(TranslateIfRequired(currentType, type));
						}
					}
				}
				if (typesToVisit.Count > 0) {
					nextType = typesToVisit.Dequeue();
				} else {
					nextType = enqueuedLastBaseType ? null : DefaultClass.GetBaseTypeByClassType(classToListInheritanceTreeFor);
					enqueuedLastBaseType = true;
				}
				if (nextType != null) {
					currentType = nextType;
					currentClass = nextType.GetUnderlyingClass();
				}
			} while (nextType != null);
			return visitedList;
		}
		#endregion
		
		#region IsSimilarMember / FindBaseMember
		/// <summary>
		/// Gets if member1 is the same as member2 or if member1 overrides member2.
		/// </summary>
		public static bool IsSimilarMember(IMember member1, IMember member2)
		{
			member1 = GetGenericMember(member1);
			member2 = GetGenericMember(member2);
			do {
				if (IsSimilarMemberInternal(member1, member2))
					return true;
			} while ((member1 = FindBaseMember(member1)) != null);
			return false;
		}
		
		/// <summary>
		/// Gets the generic member from a specialized member.
		/// Specialized members are the result of overload resolution with type substitution.
		/// </summary>
		static IMember GetGenericMember(IMember member)
		{
			// e.g. member = string[] ToArray<string>(IEnumerable<string> input)
			// result = T[] ToArray<T>(IEnumerable<T> input)
			if (member != null) {
				while (member.GenericMember != null)
					member = member.GenericMember;
			}
			return member;
		}
		
		static bool IsSimilarMemberInternal(IMember member1, IMember member2)
		{
			if (member1 == member2)
				return true;
			if (member1 == null || member2 == null)
				return false;
			if (member1.FullyQualifiedName != member2.FullyQualifiedName)
				return false;
			if (member1.IsStatic != member2.IsStatic)
				return false;
			IMethodOrProperty m1 = member1 as IMethodOrProperty;
			IMethodOrProperty m2 = member2 as IMethodOrProperty;
			if (m1 != null || m2 != null) {
				if (m1 != null && m2 != null) {
					if (DiffUtility.Compare(m1.Parameters, m2.Parameters) != 0)
						return false;
					if (m1 is IMethod && m2 is IMethod) {
						if ((m1 as IMethod).TypeParameters.Count != (m2 as IMethod).TypeParameters.Count)
							return false;
					}
				} else {
					return false;
				}
			}
			return true;
		}
		
		public static IMember FindSimilarMember(IClass type, IMember member)
		{
			member = GetGenericMember(member);
			if (member is IMethod) {
				IMethod parentMethod = (IMethod)member;
				foreach (IMethod m in type.Methods) {
					if (string.Equals(parentMethod.Name, m.Name, StringComparison.InvariantCultureIgnoreCase)) {
						if (m.IsStatic == parentMethod.IsStatic) {
							if (DiffUtility.Compare(parentMethod.Parameters, m.Parameters) == 0) {
								return m;
							}
						}
					}
				}
			} else if (member is IProperty) {
				IProperty parentMethod = (IProperty)member;
				foreach (IProperty m in type.Properties) {
					if (string.Equals(parentMethod.Name, m.Name, StringComparison.InvariantCultureIgnoreCase)) {
						if (m.IsStatic == parentMethod.IsStatic) {
							if (DiffUtility.Compare(parentMethod.Parameters, m.Parameters) == 0) {
								return m;
							}
						}
					}
				}
			}
			return null;
		}
		
		public static IMember FindBaseMember(IMember member)
		{
			if (member == null) return null;
			if (member is IMethod && (member as IMethod).IsConstructor) return null;
			IClass parentClass = member.DeclaringType;
			IClass baseClass = parentClass.BaseClass;
			if (baseClass == null) return null;
			
			foreach (IClass childClass in baseClass.ClassInheritanceTree) {
				IMember m = FindSimilarMember(childClass, member);
				if (m != null)
					return m;
			}
			return null;
		}
		#endregion
	}
}
