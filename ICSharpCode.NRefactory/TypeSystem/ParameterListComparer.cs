// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Threading;
using ICSharpCode.NRefactory.TypeSystem.Implementation;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Compares parameter lists by comparing the types of all parameters.
	/// </summary>
	/// <remarks>
	/// 'ref int' and 'out int' are considered to be equal.
	/// "Method{T}(T a)" and "Method{S}(S b)" are also considered equal.
	/// </remarks>
	public sealed class ParameterListComparer : IEqualityComparer<IList<IParameter>>
	{
		public static readonly ParameterListComparer Instance = new ParameterListComparer();
		
		// We want to consider the parameter lists "Method<T>(T a)" and "Method<S>(S b)" as equal.
		// However, the parameter types are not considered equal, as T is a different type parameter than S.
		// In order to compare the method signatures, we will normalize all method type parameters.
		sealed class NormalizeMethodTypeParameters : TypeVisitor
		{
			//ITypeParameter[] normalTypeParameters = { new DefaultTypeParameter(EntityType.Method, 0, string.Empty) };
			
			public override IType VisitTypeParameter(ITypeParameter type)
			{
				throw new NotImplementedException();
				/*if (type.OwnerType == EntityType.Method) {
					ITypeParameter[] tps = this.normalTypeParameters;
					while (type.Index >= tps.Length) {
						// We don't have a normal type parameter for this index, so we need to extend our array.
						// Because the array can be used concurrently from multiple threads, we have to use
						// Interlocked.CompareExchange.
						ITypeParameter[] newTps = new ITypeParameter[type.Index + 1];
						tps.CopyTo(newTps, 0);
						for (int i = tps.Length; i < newTps.Length; i++) {
							newTps[i] = new DefaultTypeParameter(EntityType.Method, i, string.Empty);
						}
						ITypeParameter[] oldTps = Interlocked.CompareExchange(ref normalTypeParameters, newTps, tps);
						if (oldTps == tps) {
							// exchange successful
							tps = newTps;
						} else {
							// exchange not successful
							tps = oldTps;
						}
					}
					return tps[type.Index];
				} else {
					return base.VisitTypeParameter(type);
				}*/
			}
		}
		
		readonly NormalizeMethodTypeParameters normalization = new NormalizeMethodTypeParameters();
		
		public bool Equals(IList<IParameter> x, IList<IParameter> y)
		{
			if (x == y)
				return true;
			if (x == null || y == null || x.Count != y.Count)
				return false;
			for (int i = 0; i < x.Count; i++) {
				var a = x[i];
				var b = y[i];
				if (a == null && b == null)
					continue;
				if (a == null || b == null)
					return false;
				IType aType = a.Type.AcceptVisitor(normalization);
				IType bType = b.Type.AcceptVisitor(normalization);
				
				if (!aType.Equals(bType))
					return false;
			}
			return true;
		}
		
		public int GetHashCode(IList<IParameter> obj)
		{
			int hashCode = obj.Count;
			unchecked {
				foreach (IParameter p in obj) {
					hashCode *= 27;
					IType type = p.Type.AcceptVisitor(normalization);
					hashCode += type.GetHashCode();
				}
			}
			return hashCode;
		}
	}
	
	/// <summary>
	/// Compares member signatures.
	/// </summary>
	/// <remarks>
	/// This comparer checks for equal short name, equal type parameter count, and equal parameter types (using ParameterListComparer).
	/// </remarks>
	public sealed class SignatureComparer : IEqualityComparer<IMember>
	{
		StringComparer nameComparer;
		
		public SignatureComparer(StringComparer nameComparer)
		{
			if (nameComparer == null)
				throw new ArgumentNullException("nameComparer");
			this.nameComparer = nameComparer;
		}
		
		/// <summary>
		/// Gets a signature comparer that uses an ordinal comparison for the member name.
		/// </summary>
		public static readonly SignatureComparer Ordinal = new SignatureComparer(StringComparer.Ordinal);
		
		public bool Equals(IMember x, IMember y)
		{
			if (x == y)
				return true;
			if (x == null || y == null || x.EntityType != y.EntityType || !nameComparer.Equals(x.Name, y.Name))
				return false;
			IParameterizedMember px = x as IParameterizedMember;
			IParameterizedMember py = y as IParameterizedMember;
			if (px != null && py != null) {
				IMethod mx = x as IMethod;
				IMethod my = y as IMethod;
				if (mx != null && my != null && mx.TypeParameters.Count != my.TypeParameters.Count)
					return false;
				return ParameterListComparer.Instance.Equals(px.Parameters, py.Parameters);
			} else {
				return true;
			}
		}
		
		public int GetHashCode(IMember obj)
		{
			unchecked {
				int hash = (int)obj.EntityType * 33 + nameComparer.GetHashCode(obj.Name);
				IParameterizedMember pm = obj as IParameterizedMember;
				if (pm != null) {
					hash *= 27;
					hash += ParameterListComparer.Instance.GetHashCode(pm.Parameters);
					IMethod m = pm as IMethod;
					if (m != null)
						hash += m.TypeParameters.Count;
				}
				return hash;
			}
		}
	}
}
