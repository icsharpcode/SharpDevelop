// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using System.IO;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.SharpDevelop.Dom
{
	public static class ModelFactoryExtensions
	{
		/// <summary>
		/// Creates an <see cref="ICSharpCode.SharpDevelop.Dom.IAssemblyModel"/> from a file name.
		/// </summary>
		/// <param name="modelFactory">Model factory.</param>
		/// <param name="fileName">Assembly file name.</param>
		/// <returns>Created <see cref="ICSharpCode.SharpDevelop.Dom.IAssemblyModel"/>.</returns>
		public static IAssemblyModel CreateAssemblyModelFromFile(this IModelFactory modelFactory, string fileName)
		{
			var loader = new CecilLoader();
			loader.IncludeInternalMembers = true;
			loader.LazyLoad = true;
			var assembly = loader.LoadAssemblyFile(fileName);
			
			IEntityModelContext context = new AssemblyEntityModelContext(assembly);
			IAssemblyModel model = modelFactory.CreateAssemblyModel(context);
			if (model is IUpdateableAssemblyModel) {
				((IUpdateableAssemblyModel)model).Update(EmptyList<IUnresolvedTypeDefinition>.Instance, assembly.TopLevelTypeDefinitions.ToList());
				((IUpdateableAssemblyModel) model).AssemblyName = assembly.AssemblyName;
			}
			return model;
		}
		
		/// <summary>
		/// Creates an <see cref="ICSharpCode.SharpDevelop.Dom.IAssemblyModel"/> from a file name and catches
		/// errors by showing messages to user.
		/// </summary>
		/// <param name="modelFactory">Model factory.</param>
		/// <param name="fileName">Assembly file name.</param>
		/// <returns>
		/// Created <see cref="ICSharpCode.SharpDevelop.Dom.IAssemblyModel"/> or <b>null</b>,
		/// if model couldn't be created.
		/// </returns>
		public static IAssemblyModel SafelyCreateAssemblyModelFromFile(this IModelFactory modelFactory, string fileName)
		{
			try {
				return modelFactory.CreateAssemblyModelFromFile(fileName);
			} catch (BadImageFormatException) {
				SD.MessageService.ShowWarningFormatted("${res:ICSharpCode.SharpDevelop.Dom.AssemblyInvalid}", Path.GetFileName(fileName));
			} catch (FileNotFoundException) {
				SD.MessageService.ShowWarningFormatted("${res:ICSharpCode.SharpDevelop.Dom.AssemblyNotAccessible}", fileName);
			}
			
			return null;
		}
	}
}