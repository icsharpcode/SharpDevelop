// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeClass2 : CodeClass, global::EnvDTE.CodeClass2
	{
		IClassKindUpdater classKindUpdater;
		
		public CodeClass2(IProjectContent projectContent, IClass c, IClassKindUpdater classKindUpdater)
			: base(projectContent, c)
		{
			this.classKindUpdater = classKindUpdater;
		}
		
		public CodeClass2(IProjectContent projectContent, IClass c)
			: this(projectContent, c, new ClassKindUpdater(c))
		{
		}
		
		public global::EnvDTE.CodeElements PartialClasses {
			get { return new PartialClasses(this); }
		}
		
		public static CodeClass2 CreateFromBaseType(IProjectContent projectContent, IReturnType baseType)
		{
			IClass baseTypeClass = baseType.GetUnderlyingClass();
			return new CodeClass2(projectContent, baseTypeClass);
		}
		
		public bool IsGeneric {
			get { return Class.DotNetName.Contains("`"); }
		}
		
		public global::EnvDTE.vsCMClassKind ClassKind {
			get {
				if (Class.IsPartial) {
					return global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass;
				}
				return global::EnvDTE.vsCMClassKind.vsCMClassKindMainClass;
			}
			set {
				if (value == global::EnvDTE.vsCMClassKind.vsCMClassKindPartialClass) {
					classKindUpdater.MakeClassPartial();
				} else {
					throw new NotImplementedException();
				}
			}
		}
		
		public bool IsAbstract {
			get { return Class.IsAbstract; }
		}
	}
}
