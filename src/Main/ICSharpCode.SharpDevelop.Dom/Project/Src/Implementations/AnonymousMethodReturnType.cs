// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// The return type of anonymous method expressions or lambda expressions.
	/// </summary>
	public sealed class AnonymousMethodReturnType : ProxyReturnType
	{
		IReturnType returnType;
		IList<IParameter> parameters = new List<IParameter>();
		ICompilationUnit cu;
		
		public AnonymousMethodReturnType(ICompilationUnit cu)
		{
			this.cu = cu;
		}
		
		/// <summary>
		/// Return type of the anonymous method. Can be null if inferred from context.
		/// </summary>
		public IReturnType MethodReturnType {
			get {
				return returnType;
			}
			set {
				returnType = value;
			}
		}
		
		/// <summary>
		/// Gets the list of method parameters.
		/// </summary>
		public IList<IParameter> MethodParameters {
			get {
				return parameters;
			}
			set {
				if (value == null) throw new ArgumentNullException("value");
				parameters = value;
			}
		}
		
		public override bool IsDefaultReturnType {
			get {
				return false;
			}
		}
		
		volatile DefaultClass cachedClass;
		
		public override IClass GetUnderlyingClass()
		{
			if (cachedClass != null) return cachedClass;
			DefaultClass c = new DefaultClass(cu, ClassType.Delegate, ModifierEnum.None, DomRegion.Empty, null);
			c.BaseTypes.Add(cu.ProjectContent.SystemTypes.Delegate);
			AddDefaultDelegateMethod(c, returnType ?? cu.ProjectContent.SystemTypes.Object, parameters);
			cachedClass = c;
			return c;
		}
		
		internal static void AddDefaultDelegateMethod(DefaultClass c, IReturnType returnType, IList<IParameter> parameters)
		{
			ModifierEnum modifiers = ModifierEnum.Public | ModifierEnum.Synthetic;
			DefaultMethod invokeMethod = new DefaultMethod("Invoke", returnType, modifiers, c.Region, DomRegion.Empty, c);
			foreach (IParameter par in parameters) {
				invokeMethod.Parameters.Add(par);
			}
			c.Methods.Add(invokeMethod);
			invokeMethod = new DefaultMethod("BeginInvoke", c.ProjectContent.SystemTypes.IAsyncResult, modifiers, c.Region, DomRegion.Empty, c);
			foreach (IParameter par in parameters) {
				invokeMethod.Parameters.Add(par);
			}
			invokeMethod.Parameters.Add(new DefaultParameter("callback", c.ProjectContent.SystemTypes.AsyncCallback, DomRegion.Empty));
			invokeMethod.Parameters.Add(new DefaultParameter("object", c.ProjectContent.SystemTypes.Object, DomRegion.Empty));
			c.Methods.Add(invokeMethod);
			invokeMethod = new DefaultMethod("EndInvoke", returnType, modifiers, c.Region, DomRegion.Empty, c);
			invokeMethod.Parameters.Add(new DefaultParameter("result", c.ProjectContent.SystemTypes.IAsyncResult, DomRegion.Empty));
			c.Methods.Add(invokeMethod);
		}
		
		public override IReturnType BaseType {
			get {
				return GetUnderlyingClass().DefaultReturnType;
			}
		}
		
		public override string Name {
			get {
				return "delegate";
			}
		}
		
		public override string FullyQualifiedName {
			get {
				StringBuilder b = new StringBuilder("delegate(");
				bool first = true;
				foreach (IParameter p in parameters) {
					if (first) first = false; else b.Append(", ");
					b.Append(p.Name);
					if (p.ReturnType != null) {
						b.Append(":");
						b.Append(p.ReturnType.Name);
					}
				}
				b.Append(")");
				if (returnType != null) {
					b.Append(":");
					b.Append(returnType.Name);
				}
				return b.ToString();
			}
		}
		
		public override string Namespace {
			get {
				return "";
			}
		}
		
		public override string DotNetName {
			get {
				return Name;
			}
		}
	}
}
