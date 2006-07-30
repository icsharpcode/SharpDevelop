// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	public class ObjectValue: Value
	{
		ObjectValueClass topClass;
		
		public override string AsString {
			get {
				return "{" + Type + "}";
			}
		}
		
		public override string Type { 
			get{ 
				return topClass.Type;
			} 
		}
		
		public ObjectValueClass TopClass {
			get {
				return topClass;
			}
		}
		
		public IEnumerable<ObjectValueClass> Classes {
			get {
				ObjectValueClass currentClass = topClass;
				do {
					yield return currentClass;
					currentClass = currentClass.BaseClass;
				} while (currentClass != null);
			}
		}
		
		public ObjectValueClass GetClass(string type)
		{
			foreach(ObjectValueClass cls in Classes) {
				if (cls.Type == type) return cls;
			}
			return null;
		}
		
		internal bool IsInClassHierarchy(ICorDebugClass corClass)
		{
			foreach(ObjectValueClass cls in Classes) {
				if (cls.CorClass == corClass) return true;
			}
			return false;
		}
		
		internal ObjectValue(Variable variable):base(variable)
		{
			topClass = new ObjectValueClass(this, this.CorValue.As<ICorDebugObjectValue>().Class);
		}
		
		internal bool IsCorValueCompatible {
			get {
				ObjectValue freshValue = this.FreshValue as ObjectValue;
				return freshValue != null &&
				       topClass.Module == freshValue.TopClass.Module &&
				       topClass.ClassToken == freshValue.TopClass.ClassToken;
			}
		}
		
		protected override bool GetMayHaveSubVariables()
		{
			return true;
		}
		
		protected override VariableCollection GetSubVariables()
		{
			return topClass.SubVariables;
		}
	}
}
