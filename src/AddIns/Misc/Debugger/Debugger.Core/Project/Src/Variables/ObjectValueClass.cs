// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Debugger.Wrappers.CorDebug;
using Debugger.Wrappers.MetaData;

namespace Debugger
{
	public class ObjectValueClass: RemotingObjectBase
	{
		Process process;
		
		ObjectValue objectValue;
		
		Module module;
		ICorDebugClass corClass;
		TypeDefProps classProps;
		
		ICorDebugObjectValue CorObjectValue {
			get {
				return objectValue.TheValue.CorValue.As<ICorDebugObjectValue>();
			}
		}
		
		bool IsCorValueCompatible {
			get {
				return objectValue.IsCorValueCompatible;
			}
		}
		
		public Module Module {
			get {
				return module;
			}
		}
		
		public string Type { 
			get{ 
				return classProps.Name;
			} 
		}
		
		internal ICorDebugClass CorClass {
			get {
				return corClass;
			}
		}
		
		public uint ClassToken {
			get {
				return classProps.Token;
			}
		}
		
		public ObjectValueClass(ObjectValue objectValue, ICorDebugClass corClass)
		{
			this.process = objectValue.TheValue.Process;
			this.objectValue = objectValue;
			this.module = process.GetModule(corClass.Module);
			this.corClass = corClass;
			this.classProps = Module.MetaData.GetTypeDefProps(corClass.Token);
		}
		
		public VariableCollection SubVariables {
			get {
				return new VariableCollection("Base class",
				                              "{" + Type + "}",
				                              SubCollections,
				                              GetSubVariables(ObjectMember.Flags.Public, ObjectMember.Flags.PublicStatic));
			}
		}
		
		IEnumerable<VariableCollection> SubCollections {
			get {
				ObjectValueClass baseClass = BaseClass;
				VariableCollection privateStatic = new VariableCollection("Private static members",
				                                                          String.Empty,
				                                                          new VariableCollection[0],
				                                                          GetSubVariables(ObjectMember.Flags.Static, ObjectMember.Flags.PublicStatic));
				VariableCollection privateInstance = new VariableCollection("Private members",
				                                                            String.Empty,
				                                                            privateStatic.IsEmpty? new VariableCollection[0] : new VariableCollection[] {privateStatic},
					                                                        GetSubVariables(ObjectMember.Flags.None, ObjectMember.Flags.PublicStatic));
				VariableCollection publicStatic = new VariableCollection("Static members",
				                                                         String.Empty,
					                                                     new VariableCollection[0],
					                                                     GetSubVariables(ObjectMember.Flags.PublicStatic, ObjectMember.Flags.PublicStatic));
				if (baseClass != null) {
					yield return baseClass.SubVariables;
				}
				if (!privateInstance.IsEmpty) {
					yield return privateInstance;
				}
				if (!publicStatic.IsEmpty) {
					yield return publicStatic;
				}
			}
		}
		
		IEnumerable<Variable> GetSubVariables(ObjectMember.Flags requiredFlags, ObjectMember.Flags mask) {
			foreach(ObjectMember var in GetFieldVariables()) {
				if ((var.MemberFlags & mask) == requiredFlags) {
					yield return var;
				}
			}
			
			foreach(ObjectMember var in GetPropertyVariables()) {
				if ((var.MemberFlags & mask) == requiredFlags) {
					yield return var;
				}
			}
		}
		
		public ObjectValueClass BaseClass {
			get {
				ICorDebugClass superClass = GetSuperClass(process, corClass);
				if (superClass != null) {
					return new ObjectValueClass(objectValue, superClass);
				} else {
					return null;
				}
			}
		}
		
		public IEnumerable<ObjectMember> GetFieldVariables()
		{
			foreach(FieldProps f in Module.MetaData.EnumFields(ClassToken)) {
				FieldProps field = f; // One per scope/delegate
				if (field.IsStatic && field.IsLiteral) continue; // Skip field
				yield return new ObjectMember(
					field.Name,
					(field.IsStatic ? ObjectMember.Flags.Static : ObjectMember.Flags.None) |
					(field.IsPublic ? ObjectMember.Flags.Public : ObjectMember.Flags.None),
					new Value(
						process,
						new IExpirable[] {this.objectValue.TheValue},
						new IMutable[] {this.objectValue.TheValue},
						delegate { return GetCorValueOfField(field); }
					)
				);
			}
		}
		
		ICorDebugValue GetCorValueOfField(FieldProps field)
		{
			if (!IsCorValueCompatible) throw new CannotGetValueException("Object type changed");

			// Current frame is used to resolve context specific static values (eg. ThreadStatic)
			ICorDebugFrame curFrame = null;
			if (process.IsPaused && process.SelectedThread != null && process.SelectedThread.LastFunction != null && process.SelectedThread.LastFunction.CorILFrame != null) {
				curFrame = process.SelectedThread.LastFunction.CorILFrame.CastTo<ICorDebugFrame>();
			}
			
			try {
				if (field.IsStatic) {
					return corClass.GetStaticFieldValue(field.Token, curFrame);
				} else {
					return CorObjectValue.GetFieldValue(corClass, field.Token);
				}
			} catch {
				throw new CannotGetValueException("Can not get value of field");
			}
		}
		
		IEnumerable<MethodProps> Methods {
			get {
				return this.Module.MetaData.EnumMethods(this.ClassToken);
			}
		}
		
		public IEnumerable<ObjectMember> GetPropertyVariables()
		{
			foreach(MethodProps m in Methods) {
				MethodProps method = m; // One per scope/delegate
				if (method.HasSpecialName && method.Name.StartsWith("get_") && method.Name != "get_Item") {
					ObjectMember.Flags flags = (method.IsStatic ? ObjectMember.Flags.Static : ObjectMember.Flags.None) |
					                           (method.IsPublic ? ObjectMember.Flags.Public : ObjectMember.Flags.None);
					yield return new ObjectMember(
						method.Name.Remove(0, 4),
						flags,
						new CallFunctionEval(
							process,
							new IExpirable[] {this.objectValue.TheValue},
							new IMutable[] {process.DebugeeState},
							Module.CorModule.GetFunctionFromToken(method.Token),
							method.IsStatic ? null : this.objectValue.TheValue, // this
							new Value[] {}
						)
					); // args
				}
			}
		}
		
		protected static ICorDebugClass GetSuperClass(Process process, ICorDebugClass currClass)
		{
			Module currModule = process.GetModule(currClass.Module);
			uint superToken = currModule.MetaData.GetTypeDefProps(currClass.Token).SuperClassToken;
			
			// It has no base class
			if ((superToken & 0x00FFFFFF) == 0x00000000) return null;
			
			// TypeDef - Localy defined
			if ((superToken & 0xFF000000) == 0x02000000) {
				return currModule.CorModule.GetClassFromToken(superToken);
			}
			
			// TypeSpec - generic class whith 'which'
			if ((superToken & 0xFF000000) == 0x1B000000) {
				// Walkaround - fake 'object' type
				string fullTypeName = "System.Object";
				
				foreach (Module superModule in process.Modules) {
					try	{
						uint token = superModule.MetaData.FindTypeDefByName(fullTypeName, 0).Token;
						return superModule.CorModule.GetClassFromToken(token);
					} catch {
						continue;
					}
				}
			}
			
			// TypeRef - Referencing to external assembly
			if ((superToken & 0xFF000000) == 0x01000000) {
				string fullTypeName = currModule.MetaData.GetTypeRefProps(superToken).Name;
				
				foreach (Module superModule in process.Modules) {
					// TODO: Does not work for nested
					// TODO: preservesig
					try	{
						uint token = superModule.MetaData.FindTypeDefByName(fullTypeName, 0).Token;
						return superModule.CorModule.GetClassFromToken(token);
					} catch {
						continue;
					}
				}
			}
			
			throw new DebuggerException("Superclass not found");
		}
	}
}
