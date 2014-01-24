/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 1/8/2014
 * Time: 21:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.NRefactory.TypeSystem
{
	/// <summary>
	/// Helper class for dealing with System.Threading.Tasks.Task.
	/// </summary>
	public static class TaskType
	{
		/// <summary>
		/// Gets the T in Task&lt;T&gt;.
		/// Returns void for non-generic Task.
		/// Any other type is returned unmodified.
		/// </summary>
		public static IType UnpackTask(ICompilation compilation, IType type)
		{
			if (!IsTask(type))
				return type;
			if (type.TypeParameterCount == 0)
				return compilation.FindType(KnownTypeCode.Void);
			else
				return type.TypeArguments[0];
		}
		
		/// <summary>
		/// Gets whether the specified type is Task or Task&lt;T&gt;.
		/// </summary>
		public static bool IsTask(IType type)
		{
			ITypeDefinition def = type.GetDefinition();
			if (def != null) {
				if (def.KnownTypeCode == KnownTypeCode.Task)
					return true;
				if (def.KnownTypeCode == KnownTypeCode.TaskOfT)
					return type is ParameterizedType;
			}
			return false;
		}
		
		/// <summary>
		/// Creates a task type.
		/// </summary>
		public static IType Create(ICompilation compilation, IType elementType)
		{
			if (compilation == null)
				throw new ArgumentNullException("compilation");
			if (elementType == null)
				throw new ArgumentNullException("elementType");
			
			if (elementType.Kind == TypeKind.Void)
				return compilation.FindType(KnownTypeCode.Task);
			IType taskType = compilation.FindType(KnownTypeCode.TaskOfT);
			ITypeDefinition taskTypeDef = taskType.GetDefinition();
			if (taskTypeDef != null)
				return new ParameterizedType(taskTypeDef, new [] { elementType });
			else
				return taskType;
		}
	}
}
