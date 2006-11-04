// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;

namespace Debugger
{
	public class ObjectValue: ValueProxy
	{
		ObjectValueClass topClass;
		Value toStringText;
		
		public override string AsString {
			get {
				return "{" + Type + "}";
			}
		}
		
		public Value ToStringText {
			get {
				return toStringText;
			}
		}
		
		public override string Type { 
			get {
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
		
		internal ObjectValue(Value @value):base(@value)
		{
			topClass = new ObjectValueClass(this, this.CorValue.As<ICorDebugObjectValue>().Class);
			Module module = GetClass("System.Object").Module;
			ICorDebugFunction corFunction = module.GetMethod("System.Object", "ToString", 0);
			toStringText = new CallFunctionEval(this.Process,
			                                    new IExpirable[] {this.Value},
			                                    new IMutable[] {this.Value},
			                                    corFunction,
			                                    this.Value,
			                                    new Value[] {});
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
