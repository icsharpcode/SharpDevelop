// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

using NoGoop.Util;

namespace NoGoop.ObjBrowser
{
	// Handles the support for casts
	//
	// Casts are allowed either temporarily for a given member
	// within a given object, or to be remembered for the type of
	// the member.
	internal class CastInfo
	{
		// Dynamic information for each cast
		protected String                _memberSig;
		protected String                _declaringType;
		protected Type                  _castType;
		protected bool                  _perm;
		
		// The object that does the cast, since c# does not support
		// dynamic casting, we make an object that does this
		protected Object                _castingObject;
		
		// The method of the object that does the cast.  This takes
		// an object as a parameter and returns an object
		protected MethodInfo            _castingMethod;
		
		internal String MemberSig {
			get {
				return _memberSig;
			}
		}
		
		internal String DeclaringType {
			get {
				return _declaringType;
			}
		}
		
		internal Type CastType {
			get {
				return _castType;
			}
		}
		
		internal bool Perm {
			get {
				return _perm;
			}
		}
		
		internal String Key {
			get {
				return _declaringType + "/" + _memberSig;
			}
		}
		
		protected void Init(Type castType)
		{
			_castType = castType;
			CreateCasterObject();
		}
		
		internal CastInfo(Type castType)
		{
			Init(castType);
		}
		
		internal CastInfo(String memberSig,
						 String declaringType,
						 Type castType,
						 bool perm)
		{
			_memberSig = memberSig;
			_declaringType = declaringType;
			_perm = perm;
			Init(castType);
		}
		
		protected const String          CASTS = "casts"; 
		
		// Used to make sure the class names are unique
		protected static int            _classNumber;
		
		// K(CastInfo.Key) V(CastInfo)
		protected static Hashtable      _casts = new Hashtable();
		
		static CastInfo()
		{
			SavedCastInfoCollection savedCasts = ComponentInspectorProperties.SavedCasts;
			for (int i = savedCasts.Count - 1; i >= 0; --i) {
				SavedCastInfo savedCastInfo = savedCasts[i];
				try {
					Type castType = ReflectionHelper.GetType(savedCastInfo.CastTypeName);
					if (castType == null) {
						throw new Exception ("Cast type: " + savedCastInfo.CastTypeName + " not found");
					}
					CastInfo ci = new CastInfo(savedCastInfo.MemberSignature,
											  savedCastInfo.TypeName,
											  castType,
											  true);
					_casts.Add(ci.Key, ci);
				} catch (Exception ex) {
					TraceUtil.WriteLineWarning(typeof(CastInfo),
						"Warning: Invalid cast information in "
						+ "registry for: " 
						+ savedCastInfo.TypeName + "/" + savedCastInfo.MemberSignature 
						+ " - deleting  Exception: " + ex);
					savedCasts.Remove(savedCastInfo);
				}
			}
		}
		
		protected static String FormMemberKey(MemberInfo mi)
		{
			return mi.DeclaringType.FullName + "/" + mi.ToString();
		}
		
		// Does the actual cast returning the result object
		internal Object DoCast(Object obj)
		{
			Object retObj = null;
			try {
				retObj = _castingMethod.Invoke(_castingObject, new Object[] { obj });
			} catch (Exception ex) {
				Exception showException = ex;
				// Remove the useless wrapper exception
				if (showException is TargetInvocationException)
					showException = ex.InnerException;
				throw showException;
			}
			TraceUtil.WriteLineInfo(this,
									"Docast - to: " 
									+ _castType
									+ " obj: " + obj
									+ " result: " + retObj
									+ " resType: " + retObj.GetType());
			return retObj;
		}
		
		// Creates a dymamic class to perform the cast
		protected void CreateCasterObject()
		{
			TypeBuilder tb;
			lock (GetType()){
				tb = AssemblySupport.ModBuilder.DefineType("CasterClass" + _classNumber++,
							  TypeAttributes.Class | TypeAttributes.Public);
				tb.DefineDefaultConstructor(MethodAttributes.Public);
			}
			Type[] parameterTypes = new Type[1];
			parameterTypes[0] = typeof(Object);
			// The event handler method, Calls the "LoggerBase"
			// method to do the actual logging
			MethodBuilder mb = tb.DefineMethod("DoCast", MethodAttributes.Public,
				typeof(Object), parameterTypes);
			ILGenerator gen = mb.GetILGenerator();
			System.Reflection.Emit.Label l = gen.DefineLabel();
			gen.DeclareLocal(_castType);
			// Load object to cast
			gen.Emit(OpCodes.Ldarg_1);
			// Do the cast
			gen.Emit(OpCodes.Castclass, _castType);
			// Save result of cast
			gen.Emit(OpCodes.Stloc_0);
			// Branch?
			gen.Emit(OpCodes.Br_S, l);
			gen.MarkLabel(l);
			// Get result of cast
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Ret);
			Type type = tb.CreateType();
			// Only method is "DoCast", do this to
			// avoid using the name so that we don't have a problem with the
			// obfuscator
			_castingMethod = type.GetMethods(BindingFlags.Public
						  | BindingFlags.Instance
						  | BindingFlags.DeclaredOnly)[0];
			_castingObject = Activator.CreateInstance(type);
		}
		
		// Adds cast information, if the memberInfo is null, this 
		// cast is not rememebered at all
		internal static CastInfo AddCast(MemberInfo memberInfo,
										Type castType,
										bool perm,
										Object currentObj)
		{
			CastInfo ci;
			if (memberInfo == null) {
				ci = new CastInfo(castType);
			} else {
				ci = new CastInfo(memberInfo.ToString(),
								 memberInfo.DeclaringType.FullName,
								 castType,
								 perm);
			}
			// Optionally make sure the cast will work for a 
			// specified object
			if (currentObj != null)
				ci.DoCast(currentObj);
			// We don't try to remember if if there is no memberInfo
			if (memberInfo == null)
				return ci;
			lock (_casts) {
				if (_casts[ci.Key] == null) {
					_casts.Add(ci.Key, ci);
				} else {
					// Get rid of the permanent key if it was there
					if (!perm) {
						ComponentInspectorProperties.SavedCasts.Remove(ci.DeclaringType, ci.MemberSig);
					}
				}
			}
			if (perm) {
				ComponentInspectorProperties.SavedCasts.Remove(memberInfo.DeclaringType.FullName, memberInfo.ToString());
				SavedCastInfo savedCast = new SavedCastInfo(memberInfo.DeclaringType.FullName, memberInfo.ToString(), castType.FullName);
				ComponentInspectorProperties.SavedCasts.Add(savedCast);
			}
			return ci;
		}
		
		// Returns any cast information for the specified member
		internal static CastInfo GetCastInfo(MemberInfo memberInfo)
		{
			CastInfo ci;
			lock (_casts) {
				ci = (CastInfo)_casts[FormMemberKey(memberInfo)];
			}
			return ci;
		}
		
		// Remove any cast information
		internal static void RemoveCast(CastInfo castInfo)
		{
			// Not a remembered cast
			if (castInfo._memberSig == null)
				return;
			lock (_casts) {
				_casts.Remove(castInfo.Key);
				if (castInfo.Perm) {
					ComponentInspectorProperties.SavedCasts.Remove(castInfo.DeclaringType, castInfo.MemberSig);
				}
			}
		}
	}
}
