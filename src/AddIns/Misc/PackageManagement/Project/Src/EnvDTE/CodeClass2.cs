// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeClass2 : CodeClass
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
		
		public CodeElements PartialClasses {
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
		
		public vsCMClassKind ClassKind {
			get {
				if (Class.IsPartial) {
					return vsCMClassKind.vsCMClassKindPartialClass;
				}
				return vsCMClassKind.vsCMClassKindMainClass;
			}
			set {
				if (value == vsCMClassKind.vsCMClassKindPartialClass) {
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
