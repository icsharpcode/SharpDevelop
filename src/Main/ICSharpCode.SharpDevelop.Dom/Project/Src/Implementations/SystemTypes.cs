// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class SystemTypes
	{
		public readonly IReturnType Void = VoidReturnType.Instance;
		public readonly IReturnType Object;
		public readonly IReturnType Delegate;
		public readonly IReturnType ValueType;
		public readonly IReturnType Enum;
		
		public readonly IReturnType Boolean;
		public readonly IReturnType Int32;
		public readonly IReturnType String;
		
		public readonly IReturnType Array;
		public readonly IReturnType Attribute;
		public readonly IReturnType Type;
		
		public readonly IReturnType Exception;
		public readonly IReturnType AsyncCallback;
		public readonly IReturnType IAsyncResult;
		public readonly IReturnType IDisposable;
		
		IProjectContent pc;
		
		public SystemTypes(IProjectContent pc)
		{
			this.pc = pc;
			Object    = CreateFromName("System.Object");
			Delegate  = CreateFromName("System.Delegate");
			ValueType = CreateFromName("System.ValueType");
			Enum      = CreateFromName("System.Enum");
			
			Boolean = CreateFromName("System.Boolean");
			Int32   = CreateFromName("System.Int32");
			String  = CreateFromName("System.String");
			
			Array     = CreateFromName("System.Array");
			Attribute = CreateFromName("System.Attribute");
			Type      = CreateFromName("System.Type");
			
			Exception     = CreateFromName("System.Exception");
			AsyncCallback = CreateFromName("System.AsyncCallback");
			IAsyncResult  = CreateFromName("System.IAsyncResult");
			IAsyncResult  = CreateFromName("System.IDisposable");
		}
		
		IReturnType CreateFromName(string name)
		{
			IClass c = pc.GetClass(name, 0);
			if (c != null) {
				return c.DefaultReturnType;
			} else {
				LoggingService.Warn("SystemTypes.CreateFromName could not find " + name);
				return VoidReturnType.Instance;
			}
		}
		
		/// <summary>
		/// Creates the return type for a primitive system type.
		/// </summary>
		public IReturnType CreatePrimitive(Type type)
		{
			if (type.HasElementType || type.ContainsGenericParameters) {
				throw new ArgumentException("Only primitive types are supported.");
			}
			return CreateFromName(type.FullName);
		}
	}
	
	internal sealed class VoidClass : DefaultClass
	{
		internal static readonly string VoidName = typeof(void).FullName;
		public static readonly VoidClass Instance = new VoidClass();
		
		private VoidClass()
			: base(DefaultCompilationUnit.DummyCompilationUnit, VoidName)
		{
		}
		
		protected override IReturnType CreateDefaultReturnType()
		{
			return VoidReturnType.Instance;
		}
	}
	
	public sealed class VoidReturnType : AbstractReturnType
	{
		public static readonly VoidReturnType Instance = new VoidReturnType();
		
		private VoidReturnType()
		{
			FullyQualifiedName = VoidClass.VoidName;
		}
		
		public override IClass GetUnderlyingClass()
		{
			return VoidClass.Instance;
		}
		
		public override List<IMethod> GetMethods()
		{
			return new List<IMethod>();
		}
		
		public override List<IProperty> GetProperties()
		{
			return new List<IProperty>();
		}
		
		public override List<IField> GetFields()
		{
			return new List<IField>();
		}
		
		public override List<IEvent> GetEvents()
		{
			return new List<IEvent>();
		}
	}
}
