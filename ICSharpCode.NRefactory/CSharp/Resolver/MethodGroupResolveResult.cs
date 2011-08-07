// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Represents a group of methods.
	/// </summary>
	public class MethodGroupResolveResult : ResolveResult
	{
		readonly ReadOnlyCollection<IMethod> methods;
		readonly ReadOnlyCollection<IType> typeArguments;
		readonly IType targetType;
		readonly string methodName;
		
		public MethodGroupResolveResult(IType targetType, string methodName, IList<IMethod> methods, IList<IType> typeArguments) : base(SharedTypes.UnknownType)
		{
			if (targetType == null)
				throw new ArgumentNullException("targetType");
			if (methods == null)
				throw new ArgumentNullException("methods");
			this.targetType = targetType;
			this.methodName = methodName;
			this.methods = new ReadOnlyCollection<IMethod>(methods);
			this.typeArguments = typeArguments != null ? new ReadOnlyCollection<IType>(typeArguments) : EmptyList<IType>.Instance;
		}
		
		/// <summary>
		/// Gets the type of the reference to the target object.
		/// </summary>
		public IType TargetType {
			get { return targetType; }
		}
		
		/// <summary>
		/// Gets the name of the methods in this group.
		/// </summary>
		public string MethodName {
			get { return methodName; }
		}
		
		/// <summary>
		/// Gets the methods that were found.
		/// This list does not include extension methods.
		/// </summary>
		public ReadOnlyCollection<IMethod> Methods {
			get { return methods; }
		}
		
		/// <summary>
		/// Gets the type arguments that were explicitly provided.
		/// </summary>
		public ReadOnlyCollection<IType> TypeArguments {
			get { return typeArguments; }
		}
		
		/// <summary>
		/// List of extension methods, used to avoid re-calculating it in ResolveInvocation() when it was already
		/// calculated by ResolveMemberAccess().
		/// </summary>
		internal List<List<IMethod>> extensionMethods;
		
		// Resolver+UsingScope are used to fetch extension methods on demand
		internal CSharpResolver resolver;
		internal UsingScope usingScope;
		
		/// <summary>
		/// Gets all candidate extension methods.
		/// </summary>
		public List<List<IMethod>> GetExtensionMethods()
		{
			if (resolver != null) {
				Debug.Assert(extensionMethods == null);
				UsingScope oldUsingScope = resolver.UsingScope;
				try {
					resolver.UsingScope = usingScope;
					extensionMethods = resolver.GetExtensionMethods(targetType, methodName, typeArguments.Count);
				} finally {
					resolver.UsingScope = oldUsingScope;
					resolver = null;
					usingScope = null;
				}
			}
			return extensionMethods;
		}
		
		public override string ToString()
		{
			return string.Format("[{0} with {1} method(s)]", GetType().Name, methods.Count);
		}
		
		public OverloadResolution PerformOverloadResolution(ITypeResolveContext context, ResolveResult[] arguments, string[] argumentNames = null, bool allowExtensionMethods = true, bool allowExpandingParams = true)
		{
			var typeArgumentArray = this.TypeArguments.ToArray();
			OverloadResolution or = new OverloadResolution(context, arguments, argumentNames, typeArgumentArray);
			or.AllowExpandingParams = allowExpandingParams;
			foreach (IMethod method in this.Methods) {
				// TODO: grouping by class definition?
				or.AddCandidate(method);
			}
			if (allowExtensionMethods && !or.FoundApplicableCandidate) {
				// No applicable match found, so let's try extension methods.
				
				var extensionMethods = this.GetExtensionMethods();
				
				if (extensionMethods.Count > 0) {
					ResolveResult[] extArguments = new ResolveResult[arguments.Length + 1];
					extArguments[0] = new ResolveResult(this.TargetType);
					arguments.CopyTo(extArguments, 1);
					string[] extArgumentNames = null;
					if (argumentNames != null) {
						extArgumentNames = new string[argumentNames.Length + 1];
						argumentNames.CopyTo(extArgumentNames, 1);
					}
					var extOr = new OverloadResolution(context, extArguments, extArgumentNames, typeArgumentArray);
					extOr.AllowExpandingParams = allowExpandingParams;
					
					foreach (var g in extensionMethods) {
						foreach (var m in g) {
							extOr.AddCandidate(m);
						}
						if (extOr.FoundApplicableCandidate)
							break;
					}
					// For the lack of a better comparison function (the one within OverloadResolution
					// cannot be used as it depends on the argument set):
					if (extOr.FoundApplicableCandidate || or.BestCandidate == null) {
						// Consider an extension method result better than the normal result only
						// if it's applicable; or if there is no normal result.
						or = extOr;
					}
				}
			}
			return or;
		}
	}
}
