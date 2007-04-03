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
	/// <summary>
	/// Base class for return types that wrap around other return types.
	/// </summary>
	public abstract class ProxyReturnType : IReturnType
	{
		public abstract IReturnType BaseType {
			get;
		}
		
		public sealed override bool Equals(object obj)
		{
			if (obj == this)
				return true;
			else
				return Equals(obj as IReturnType);
		}
		
		public virtual bool Equals(IReturnType other)
		{
			IReturnType baseType = BaseType;
			bool tmp = (baseType != null && TryEnter()) ? baseType.Equals(other) : false;
			busy = false;
			return tmp;
		}
		
		public override int GetHashCode()
		{
			IReturnType baseType = BaseType;
			int tmp = (baseType != null && TryEnter()) ? baseType.GetHashCode() : 0;
			busy = false;
			return tmp;
		}
		
		// Required to prevent stack overflow on inferrence cycles
		bool busy = false;
		
		// keep this method as small as possible, it should be inlined!
		bool TryEnter()
		{
			if (busy) {
				PrintTryEnterWarning();
				return false;
			} else {
				busy = true;
				return true;
			}
		}
		
		void PrintTryEnterWarning()
		{
			LoggingService.Info("TryEnter failed on " + ToString());
		}
		
		public virtual string FullyQualifiedName {
			get {
				IReturnType baseType = BaseType;
				string tmp = (baseType != null && TryEnter()) ? baseType.FullyQualifiedName : "?";
				busy = false;
				return tmp;
			}
		}
		
		public virtual string Name {
			get {
				IReturnType baseType = BaseType;
				string tmp = (baseType != null && TryEnter()) ? baseType.Name : "?";
				busy = false;
				return tmp;
			}
		}
		
		public virtual string Namespace {
			get {
				IReturnType baseType = BaseType;
				string tmp = (baseType != null && TryEnter()) ? baseType.Namespace : "?";
				busy = false;
				return tmp;
			}
		}
		
		public virtual string DotNetName {
			get {
				IReturnType baseType = BaseType;
				string tmp = (baseType != null && TryEnter()) ? baseType.DotNetName : "?";
				busy = false;
				return tmp;
			}
		}
		
		public virtual int TypeArgumentCount {
			get {
				IReturnType baseType = BaseType;
				int tmp = (baseType != null && TryEnter()) ? baseType.TypeArgumentCount : 0;
				busy = false;
				return tmp;
			}
		}
		
		public virtual IClass GetUnderlyingClass()
		{
			IReturnType baseType = BaseType;
			IClass tmp = (baseType != null && TryEnter()) ? baseType.GetUnderlyingClass() : null;
			busy = false;
			return tmp;
		}
		
		public virtual List<IMethod> GetMethods()
		{
			IReturnType baseType = BaseType;
			List<IMethod> tmp = (baseType != null && TryEnter()) ? baseType.GetMethods() : new List<IMethod>();
			busy = false;
			return tmp;
		}
		
		public virtual List<IProperty> GetProperties()
		{
			IReturnType baseType = BaseType;
			List<IProperty> tmp = (baseType != null && TryEnter()) ? baseType.GetProperties() : new List<IProperty>();
			busy = false;
			return tmp;
		}
		
		public virtual List<IField> GetFields()
		{
			IReturnType baseType = BaseType;
			List<IField> tmp = (baseType != null && TryEnter()) ? baseType.GetFields() : new List<IField>();
			busy = false;
			return tmp;
		}
		
		public virtual List<IEvent> GetEvents()
		{
			IReturnType baseType = BaseType;
			List<IEvent> tmp = (baseType != null && TryEnter()) ? baseType.GetEvents() : new List<IEvent>();
			busy = false;
			return tmp;
		}
		
		public virtual bool IsDefaultReturnType {
			get {
				IReturnType baseType = BaseType;
				bool tmp = (baseType != null && TryEnter()) ? baseType.IsDefaultReturnType : false;
				busy = false;
				return tmp;
			}
		}
		
		public bool IsDecoratingReturnType<T>() where T : DecoratingReturnType
		{
			return CastToDecoratingReturnType<T>() != null;
		}
		
		public virtual T CastToDecoratingReturnType<T>() where T : DecoratingReturnType
		{
			IReturnType baseType = BaseType;
			T temp;
			if (baseType != null && TryEnter())
				temp = baseType.CastToDecoratingReturnType<T>();
			else
				temp = null;
			busy = false;
			return temp;
		}
		
		
		public bool IsArrayReturnType {
			get {
				return IsDecoratingReturnType<ArrayReturnType>();
			}
		}
		public ArrayReturnType CastToArrayReturnType()
		{
			return CastToDecoratingReturnType<ArrayReturnType>();
		}
		
		public bool IsGenericReturnType {
			get {
				return IsDecoratingReturnType<GenericReturnType>();
			}
		}
		public GenericReturnType CastToGenericReturnType()
		{
			return CastToDecoratingReturnType<GenericReturnType>();
		}
		
		public bool IsConstructedReturnType {
			get {
				return IsDecoratingReturnType<ConstructedReturnType>();
			}
		}
		public ConstructedReturnType CastToConstructedReturnType()
		{
			return CastToDecoratingReturnType<ConstructedReturnType>();
		}
	}
}
