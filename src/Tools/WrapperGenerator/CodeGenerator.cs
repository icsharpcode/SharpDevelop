// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Microsoft.CSharp;

namespace WrapperGenerator
{
	public class CodeGenerator
	{
		Assembly assembly;
		protected string wrapperNamespace;
		
		string comparsionCode = 
@"		~TheType()" + "\r\n" +
@"		{" + "\r\n" +
@"			object o = wrappedObject;" + "\r\n" +
@"			wrappedObject = null;" + "\r\n" +
@"			ResourceManager.ReleaseCOMObject(o, typeof(TheType));" + "\r\n" +
@"		}" + "\r\n" +
@"		" + "\r\n" +
@"		public bool Is<T>() where T: class" + "\r\n" +
@"		{" + "\r\n" +
			
//@"			try {" + "\r\n" +
//@"				CastTo<T>();" + "\r\n" +
//@"				return true;" + "\r\n" +
//@"			} catch {" + "\r\n" +
//@"				return false;" + "\r\n" +
//@"			}" + "\r\n" +
			
@"			System.Reflection.ConstructorInfo ctor = typeof(T).GetConstructors()[0];" + "\r\n" +
@"			System.Type paramType = ctor.GetParameters()[0].ParameterType;" + "\r\n" +
@"			return paramType.IsInstanceOfType(this.WrappedObject);" + "\r\n" +
			
@"		}" + "\r\n" +
@"		" + "\r\n" +
@"		public T As<T>() where T: class" + "\r\n" +
@"		{" + "\r\n" +
@"			try {" + "\r\n" +
@"				return CastTo<T>();" + "\r\n" +
@"			} catch {" + "\r\n" +
@"				return null;" + "\r\n" +
@"			}" + "\r\n" +
@"		}" + "\r\n" +
@"		" + "\r\n" +
@"		public T CastTo<T>() where T: class" + "\r\n" +
@"		{" + "\r\n" +
@"			return (T)Activator.CreateInstance(typeof(T), this.WrappedObject);" + "\r\n" +
@"		}" + "\r\n" +
@"		" + "\r\n" +
@"		public static bool operator ==(TheType o1, TheType o2)" + "\r\n" +
@"		{" + "\r\n" +
@"			return ((object)o1 == null && (object)o2 == null) ||" + "\r\n" +
@"			       ((object)o1 != null && (object)o2 != null && o1.WrappedObject == o2.WrappedObject);" + "\r\n" +
@"		}" + "\r\n" +
@"		" + "\r\n" +
@"		public static bool operator !=(TheType o1, TheType o2)" + "\r\n" +
@"		{" + "\r\n" +
@"			return !(o1 == o2);" + "\r\n" +
@"		}" + "\r\n" +
@"		" + "\r\n" +
@"		public override int GetHashCode()" + "\r\n" +
@"		{" + "\r\n" +
@"			return base.GetHashCode();" + "\r\n" +
@"		}" + "\r\n" +
@"		" + "\r\n" +
@"		public override bool Equals(object o)" + "\r\n" +
@"		{" + "\r\n" +
@"			TheType casted = o as TheType;" + "\r\n" +
@"			return (casted != null) && (casted.WrappedObject == wrappedObject);" + "\r\n" +
@"		}" + "\r\n" +
@"		";
		
		public CodeGenerator(Assembly assembly)
		{
			this.assembly = assembly;
		}
		
		protected virtual bool ShouldIncludeType(Type type)
		{
			return true;
		}
		
		protected virtual Type GetBaseClass(Type type) 
		{
			return null;
		}
		
		public IEnumerable<CodeCompileUnit> MakeCompileUnits()
		{
			foreach(Type type in assembly.GetTypes()) {
				if (!ShouldIncludeType(type)) continue;
				
				CodeCompileUnit codeCompileUnit;
				codeCompileUnit = new CodeCompileUnit();
				codeCompileUnit.UserData.Add("filename", type.Name);
				codeCompileUnit.Namespaces.Add(MakeNamespace(type));
				
				yield return codeCompileUnit;
			}
		}
		
		CodeNamespace MakeNamespace(Type type)
		{
			CodeNamespace codeNamespace = new CodeNamespace(wrapperNamespace);
			codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
			codeNamespace.Types.Add(MakeTypeDeclaration(type));
			
			return codeNamespace;
		}
		
		CodeTypeDeclaration MakeTypeDeclaration(Type type)
		{
			if (type.IsEnum) {
				return MakeEnumDeclaration(type);
			} else {
				return MakeClassDeclaration(type);
			}
		}
		
		CodeTypeDeclaration MakeEnumDeclaration(Type type)
		{
			CodeTypeDeclaration enumDeclaration = new CodeTypeDeclaration();
			enumDeclaration.Attributes = MemberAttributes.Private;
			enumDeclaration.Name = type.Name;
			enumDeclaration.IsEnum = true;
			enumDeclaration.BaseTypes.Add(Enum.GetUnderlyingType(type));
			
			if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0) {
				enumDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration("Flags"));
			}
			
			foreach(string name in Enum.GetNames(type)) {
				CodeMemberField field = new CodeMemberField();
				field.Name = name;
				object val = Convert.ChangeType(Enum.Parse(type, name), Enum.GetUnderlyingType(type));
				field.InitExpression = new CodePrimitiveExpression(val);
				enumDeclaration.Members.Add(field);
			}
			
			return enumDeclaration;
		}
		
		CodeTypeDeclaration MakeClassDeclaration(Type type)
		{
			Type baseType = GetBaseClass(type);
			
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration();
			codeTypeDeclaration.Attributes = MemberAttributes.Private;
			codeTypeDeclaration.IsPartial = true;
			codeTypeDeclaration.Name = type.Name;
			if (baseType != null) {
				codeTypeDeclaration.BaseTypes.Add(baseType.Name);
			}
			
			codeTypeDeclaration.Members.Add(MakeWrappedObjectField(type));
			codeTypeDeclaration.Members.Add(MakeWrappedObjectProperty(type));
			codeTypeDeclaration.Members.Add(MakeConstructor(type));
			//codeTypeDeclaration.Members.Add(MakeCanWrapMethod(type));
			codeTypeDeclaration.Members.Add(MakeWrapMethod(type));
			codeTypeDeclaration.Members.Add(new CodeSnippetTypeMember(comparsionCode.Replace("TheType", type.Name)));
			codeTypeDeclaration.Members.AddRange(MakeMembers(type));
			
			return codeTypeDeclaration;
		}
		
		CodeMemberField MakeWrappedObjectField(Type type)
		{
			CodeMemberField codeWrappedObjectField = new CodeMemberField();
			codeWrappedObjectField.Attributes = MemberAttributes.Private;
			codeWrappedObjectField.Type = new CodeTypeReference(type.FullName);
			codeWrappedObjectField.Name = "wrappedObject";
			
			return codeWrappedObjectField;
		}
		
		CodeExpression ExpressionForWrappedObjectField {
			get {
				return new CodeFieldReferenceExpression(
							new CodeThisReferenceExpression(),
							"wrappedObject");
			}
		}
		
		CodeMemberProperty MakeWrappedObjectProperty(Type type)
		{
			CodeMemberProperty codeWrappedObjectProperty = new CodeMemberProperty();
			codeWrappedObjectProperty.Attributes = MemberAttributes.Assembly | MemberAttributes.Final;
			codeWrappedObjectProperty.Type = new CodeTypeReference(type.FullName);
			codeWrappedObjectProperty.Name = "WrappedObject";
			codeWrappedObjectProperty.HasGet = true;
			codeWrappedObjectProperty.HasSet = false;
			
			CodeMethodReturnStatement codeGetter = new CodeMethodReturnStatement(ExpressionForWrappedObjectField);
			codeWrappedObjectProperty.GetStatements.Add(codeGetter);
			
			return codeWrappedObjectProperty;
		}
		
		CodeExpression ExpressionForWrappedObjectProperty {
			get {
				return new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "WrappedObject");
			}
		}
		
		CodeConstructor MakeConstructor(Type type)
		{
			Type baseType = GetBaseClass(type);
			
			CodeConstructor codeConstructor = new CodeConstructor();
			codeConstructor.Attributes = MemberAttributes.Public;
			codeConstructor.Parameters.Add(new CodeParameterDeclarationExpression(type, "wrappedObject"));
			
			if (baseType != null) {
				codeConstructor.BaseConstructorArgs.Add(
					new CodeCastExpression(
						GetBaseClass(type).FullName,
						new CodeArgumentReferenceExpression("wrappedObject")));
			}
			
			codeConstructor.Statements.Add(
				new CodeAssignStatement(
					ExpressionForWrappedObjectField,
					new CodeArgumentReferenceExpression("wrappedObject")));
			
			codeConstructor.Statements.Add(
				new CodeMethodInvokeExpression(
					new CodeMethodReferenceExpression(
						new CodeTypeReferenceExpression("ResourceManager"),
						"TrackCOMObject"),
					new CodeArgumentReferenceExpression("wrappedObject"),
					new CodeTypeOfExpression(type.Name)));
			
			return codeConstructor;
		}
		
		/*
		CodeMemberMethod MakeCanWrapMethod(Type wrappedType)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Attributes = MemberAttributes.Static | MemberAttributes.Public;
			method.ReturnType = new CodeTypeReference(typeof(bool));
			method.Name = "CanWrap";
			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object), "objectToWrap"));
			
			method.Statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("objectToWrap is " + wrappedType.FullName)));
			
			return method;
		}
		*/
		
		CodeMemberMethod MakeWrapMethod(Type wrappedType)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Attributes = MemberAttributes.Static | MemberAttributes.Public;
			method.ReturnType = new CodeTypeReference(wrappedType.Name);
			method.Name = "Wrap";
			method.Parameters.Add(new CodeParameterDeclarationExpression(wrappedType, "objectToWrap"));
			
			/*
			foreach(Type type in wrappedType.Assembly.GetTypes()) {
				if (ShouldIncludeType(type) && GetBaseClass(type).Name == wrappedType.Name) {
					method.Statements.Add(
						new CodeConditionStatement(
							new CodeMethodInvokeExpression(
								new CodeMethodReferenceExpression(
									new CodeTypeReferenceExpression(type.Name),
									"CanWrap"),
								new CodeArgumentReferenceExpression("objectToWrap")),
							new CodeMethodReturnStatement(
								new CodeMethodInvokeExpression(
									new CodeMethodReferenceExpression(
										new CodeTypeReferenceExpression(type.Name),
										"Wrap"),
									new CodeCastExpression(
										namespaceToWrap + "." + type.Name,
										new CodeArgumentReferenceExpression("objectToWrap"))))));
					                                                                               
				}
			}
			*/
			
			method.Statements.Add(
				// if
				new CodeConditionStatement(
					// objectToWrap != null
					new CodeBinaryOperatorExpression(
						new CodeArgumentReferenceExpression("objectToWrap"),
						CodeBinaryOperatorType.IdentityInequality,
						new CodePrimitiveExpression(null)),
					// return new TheType(objectToWrap);
					new CodeStatement[] {
						new CodeMethodReturnStatement(
							new CodeObjectCreateExpression(
								wrappedType.Name,
								new CodeArgumentReferenceExpression("objectToWrap")))},
					// else return null;
					new CodeStatement[] {
						new CodeMethodReturnStatement(
							new CodePrimitiveExpression(null))}));
			
			return method;
		}
		
		CodeTypeMember[] MakeMembers(Type type)
		{
			List<CodeTypeMember> codeTypeMembers = new List<CodeTypeMember>();
			
			foreach(MethodInfo method in type.GetMethods()) {
				if (method.DeclaringType == type) {
					codeTypeMembers.Add(MakeMember(method));
				}
			}
			
			return codeTypeMembers.ToArray();
		}
		
		CodeExpression Unwrap(Type rawType, CodeExpression codeExpression)
		{
			if (rawType.IsEnum) {
				return new CodeCastExpression(
					new CodeTypeReference(rawType),
					codeExpression);
			} else {
				return new CodePropertyReferenceExpression(codeExpression, "WrappedObject");
			}
		}
		
		CodeExpression Wrap(Type rawType, CodeExpression codeExpression)
		{
			if (rawType.IsEnum) {
				return new CodeCastExpression(
					new CodeTypeReference(rawType.Name),
					codeExpression);
			} else {
				return new CodeMethodInvokeExpression(
				           new CodeMethodReferenceExpression(
				               new CodeTypeReferenceExpression(rawType.Name),
				               "Wrap"),
				           codeExpression);
			}
		}
		
		class MethodContext
		{
			public CodeTypeMember CodeMemberMethod = new CodeMemberMethod();
			CodeGenerator generator;
			MethodInfo method;
			
			public string Name;
			public bool IsReturnTypeWrapped;
			public Type RawReturnType;
			public string WrappedReturnType;
			
			public List<CodeParameterDeclarationExpression> WrapperParams = new List<CodeParameterDeclarationExpression>();
			public List<CodeStatement> DoBeforeInvoke = new List<CodeStatement>();
			CodeStatement doInvoke;
			public List<CodeExpression> BaseMethodInvokeParams = new List<CodeExpression>();
			public List<CodeStatement> DoAfterInvoke = new List<CodeStatement>();
			
			public MethodContext(CodeGenerator generator, MethodInfo method)
			{
				this.generator = generator;
				this.method = method;
				Name = method.Name;
				RawReturnType = method.ReturnType;
				IsReturnTypeWrapped = generator.ShouldIncludeType(method.ReturnType);
				if (IsReturnTypeWrapped) {
					WrappedReturnType = method.ReturnType.Name;
				} else {
					WrappedReturnType = method.ReturnType.FullName;
				}
			}
			
			void AddBaseInvokeCode()
			{
				CodeExpression baseInvoke = 
					new CodeMethodInvokeExpression(
						new CodeMethodReferenceExpression(
							generator.ExpressionForWrappedObjectProperty,
							method.Name),
						BaseMethodInvokeParams.ToArray());
				
				if (IsReturnTypeWrapped) {
					baseInvoke = generator.Wrap(RawReturnType, baseInvoke);
				}
				
				if (RawReturnType != typeof(void)) {
					if (DoAfterInvoke.Count == 0) {
						doInvoke = new CodeMethodReturnStatement(baseInvoke);
					} else {
						DoBeforeInvoke.Insert(0,
							new CodeVariableDeclarationStatement(WrappedReturnType, "returnValue"));
						doInvoke =
							new CodeAssignStatement(
								new CodeVariableReferenceExpression("returnValue"),
								baseInvoke);
						DoAfterInvoke.Add(
							new CodeMethodReturnStatement(
								new CodeVariableReferenceExpression("returnValue")));
					}
				} else {
					doInvoke = new CodeExpressionStatement(baseInvoke);
				}
			}
			
			public CodeTypeMember Emit()
			{
				AddBaseInvokeCode();
				
				List<CodeStatement> body = new List<CodeStatement>();
				body.AddRange(DoBeforeInvoke);
				body.Add(doInvoke);
				body.AddRange(DoAfterInvoke);
				
				CodeMemberMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final;
				CodeMemberMethod.Name = Name;
				
				if (CodeMemberMethod is CodeMemberMethod) {
					((CodeMemberMethod)CodeMemberMethod).ReturnType = new CodeTypeReference(WrappedReturnType);
					((CodeMemberMethod)CodeMemberMethod).Parameters.AddRange(WrapperParams.ToArray());
					((CodeMemberMethod)CodeMemberMethod).Statements.AddRange(body.ToArray());
				} else {
					((CodeMemberProperty)CodeMemberMethod).Type = new CodeTypeReference(WrappedReturnType);
					((CodeMemberProperty)CodeMemberMethod).HasGet = true;
					((CodeMemberProperty)CodeMemberMethod).GetStatements.AddRange(body.ToArray());
				}
				
				return CodeMemberMethod;
			}
		}
		
		class ParamContext
		{
			public string Name;
			public bool IsWrapped;
			public string WrappedType;
			public Type RawType;
			public CodeTypeReference TypeRef;
			public FieldDirection Direction;
			public CodeParameterDeclarationExpression ArgDeclaration;
			public CodeArgumentReferenceExpression ArgRefExpression;
			public CodeExpression UnwrappedArgExpression;
			public CodeExpression UnwrappedDirectionalArgExpression;
			
			public ParamContext(CodeGenerator generator, ParameterInfo par)
			{
				Name = par.Name;
				if (par.ParameterType.IsByRef) {
					RawType = par.ParameterType.GetElementType();
				} else {
					RawType = par.ParameterType;
				}
				
				IsWrapped = generator.ShouldIncludeType(RawType);
				WrappedType = IsWrapped ? RawType.Name : RawType.FullName;
				TypeRef = new CodeTypeReference(WrappedType);
				if (!par.ParameterType.IsByRef) {
					Direction = FieldDirection.In;
				} else if (par.IsOut) {
					Direction = FieldDirection.Out;
				} else {
					Direction = FieldDirection.Ref;
				}
				
				ArgDeclaration = new CodeParameterDeclarationExpression();
				ArgDeclaration.Type = TypeRef;
				ArgDeclaration.Name = par.Name;
				ArgDeclaration.Direction = Direction;
				
				ArgRefExpression = new CodeArgumentReferenceExpression(par.Name);
				UnwrappedArgExpression = IsWrapped?generator.Unwrap(RawType, ArgRefExpression):ArgRefExpression;
				UnwrappedDirectionalArgExpression = new CodeDirectionExpression(Direction, UnwrappedArgExpression);
			}
		}
		
		CodeTypeMember MakeMember(MethodInfo method)
		{
			MethodContext methodContext = new MethodContext(this, method);
			
			ParameterInfo[] pars = method.GetParameters();
			for(int i = 0; i < pars.Length; i++) {
				ParamContext parContext = new ParamContext(this, pars[i]);
				
				if (parContext.IsWrapped) {
					if (parContext.Direction == FieldDirection.In) {
						if (pars[i].ParameterType.IsArray) {
							UnwrapArrayArgument(methodContext, parContext);
						} else {
							UnwrapArgument(methodContext, parContext);
						}
					} else {
						UnwrapRefArgument(methodContext, parContext);
					}
				} else {
					PassArgument(methodContext, parContext);
				}
				
				// If last parameter is 'out' and method returns void
				if (i == pars.Length - 1 &&
				    parContext.Direction == FieldDirection.Out &&
				    methodContext.RawReturnType == typeof(void)) {
					
					// Placeholder for the parameter
					methodContext.DoBeforeInvoke.Insert(0,
						new CodeVariableDeclarationStatement(parContext.WrappedType, parContext.Name));
					// Remove the parameter
					methodContext.WrapperParams.RemoveAt(methodContext.WrapperParams.Count - 1);
					
					methodContext.WrappedReturnType = parContext.WrappedType;
					methodContext.DoAfterInvoke.Add(
						new CodeMethodReturnStatement(
							new CodeVariableReferenceExpression(parContext.Name)));
				}
			}
			
			if (methodContext.WrapperParams.Count == 0) {
				if (methodContext.Name.StartsWith("Is")) {
					methodContext.CodeMemberMethod = new CodeMemberProperty();
				}
				if (methodContext.Name.StartsWith("Get")) {
					methodContext.CodeMemberMethod = new CodeMemberProperty();
					methodContext.Name = methodContext.Name.Remove(0, 3);
				}
			}
			
			return methodContext.Emit();
		}
		
		void PassArgument(MethodContext methodContext, ParamContext parContext)
		{
			methodContext.WrapperParams.Add(parContext.ArgDeclaration);
			methodContext.BaseMethodInvokeParams.Add(parContext.UnwrappedDirectionalArgExpression);
		}
		
		void UnwrapArgument(MethodContext methodContext, ParamContext parContext)
		{
			methodContext.WrapperParams.Add(parContext.ArgDeclaration);
			methodContext.BaseMethodInvokeParams.Add(parContext.UnwrappedDirectionalArgExpression);
		}
		
		void UnwrapRefArgument(MethodContext methodContext, ParamContext parContext)
		{
			methodContext.WrapperParams.Add(parContext.ArgDeclaration);
			
			CodeVariableDeclarationStatement tmpVariableDeclaration = new CodeVariableDeclarationStatement(parContext.RawType, ((parContext.Direction==FieldDirection.Ref)?"ref":"out") + "_" + parContext.Name);
			if (parContext.Direction == FieldDirection.Ref) {
				tmpVariableDeclaration.InitExpression = parContext.UnwrappedArgExpression;
			}
			CodeExpression tmpVariableExpression = new CodeVariableReferenceExpression(tmpVariableDeclaration.Name);
			CodeExpression tmpVariableDirectionalExpression = new CodeDirectionExpression(parContext.Direction, tmpVariableExpression);
			methodContext.DoBeforeInvoke.Add(tmpVariableDeclaration);
			
			methodContext.BaseMethodInvokeParams.Add(tmpVariableDirectionalExpression);
			
			CodeAssignStatement assignExpression = new CodeAssignStatement(parContext.ArgRefExpression, Wrap(parContext.RawType, tmpVariableExpression));
			methodContext.DoAfterInvoke.Add(assignExpression);
		}
		
		void UnwrapArrayArgument(MethodContext methodContext, ParamContext parContext)
		{
			//OUTPUT:
			//		
			//		public void void__array(Test[] arg)
			//		{
			//			Debugger.Interop.CorDebug.Test[] array_arg = new Debugger.Interop.CorDebug.Test[arg.Length];
			//			for (int i = 0; (i < arg.Length); i = (i + 1))
			//			{
			//				if ((arg[i] != null))
			//				{
			//					array_arg[i] = arg[i].WrappedObject;
			//				}
			//			}
			//			this.WrappedObject.void__array(array_arg);
			//			for (int i = 0; (i < arg.Length); i = (i + 1))
			//			{
			//				if ((array_arg[i] != null))
			//				{
			//					arg[i] = Test.Wrap(array_arg[i]);
			//				} else
			//				{
			//					arg[i] = null;
			//				}
			//			}
			//		}
			
			methodContext.WrapperParams.Add(parContext.ArgDeclaration);
			
			string rawArrayName = "array_" + parContext.Name;
			
			CodeExpression arg_Length =
				// arg.Length
				new CodePropertyReferenceExpression(parContext.ArgRefExpression, "Length");
			CodeExpression i =
				// i
				new CodeVariableReferenceExpression("i");
			CodeStatement loopInit =
				// int i = 0
				new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(0));
			CodeExpression loopCondition =
				// (i < arg.Length)
				new CodeBinaryOperatorExpression(
					i,
					CodeBinaryOperatorType.LessThan,
					arg_Length);
			CodeStatement loopIteration =
				// i = (i + 1)
				new CodeAssignStatement(
					i,
					new CodeBinaryOperatorExpression(
						i,
						CodeBinaryOperatorType.Add,
						new CodePrimitiveExpression(1)));
			CodeExpression arg_i =
				// arg[i]
				new CodeIndexerExpression(
					parContext.ArgRefExpression,
					i);
			CodeVariableReferenceExpression array_arg =
				// array_arg
				new CodeVariableReferenceExpression(rawArrayName);
			CodeExpression array_arg_i =
				// array_arg[i]
				new CodeIndexerExpression(
					array_arg,
					i);
			
			// Debugger.Interop.CorDebug.Test[] array_arg = new Debugger.Interop.CorDebug.Test[arg.Length];
			methodContext.DoBeforeInvoke.Add(
				new CodeVariableDeclarationStatement(
					new CodeTypeReference(parContext.RawType),
					array_arg.VariableName,
					new CodeArrayCreateExpression(parContext.RawType, arg_Length)));
			
			methodContext.DoBeforeInvoke.Add(
				new CodeIterationStatement(
					loopInit,
					loopCondition,
					loopIteration,
					// if
					new CodeConditionStatement(
						// (arg[i] != null)
						new CodeBinaryOperatorExpression(
							arg_i,
							CodeBinaryOperatorType.IdentityInequality,
							new CodePrimitiveExpression(null)),
						// array_arg[i] = arg[i].WrappedObject;
						new CodeAssignStatement(
							array_arg_i,
							Unwrap(parContext.RawType, arg_i)))));
			
			methodContext.DoAfterInvoke.Add(
				new CodeIterationStatement(
					loopInit,
					loopCondition,
					loopIteration,
					// if
					new CodeConditionStatement(
						// (array_arg[i] != null)
						new CodeBinaryOperatorExpression(
							array_arg_i,
							CodeBinaryOperatorType.IdentityInequality,
							new CodePrimitiveExpression(null)),
						//
						new CodeStatement [] {
							// arg[i] = Test.Wrap(array_arg[i]);
							new CodeAssignStatement(
								arg_i,
								Wrap(
									parContext.RawType.Assembly.GetType(parContext.RawType.FullName.Replace("[]","")),
									array_arg_i))},
						// else
						new CodeStatement [] {
							// arg[i] = null;
							new CodeAssignStatement(
								arg_i,
								new CodePrimitiveExpression(null))})));
			
			methodContext.BaseMethodInvokeParams.Add(array_arg);
		}
	}
}
