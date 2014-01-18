// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Interface that deals with loading and saving OpenedFile model instances.
	/// Pre-defined model providers are available in the <see cref="FileModels"/> class.
	/// Custom IFileModelProvider implementations may be used to add support for additional types of models.
	/// </summary>
	public interface IFileModelProvider<T> where T : class
	{
		/// <summary>
		/// Loads a file model from the OpenedFile.
		/// Most implementations of this method will request a lower-level model (e.g. FileModels.Binary) from the opened file
		/// and construct a higher-level representation.
		/// </summary>
		/// <exception cref="System.IO.IOException">Error loading the file.</exception>
		/// <exception cref="FormatException">Cannot construct the model because the underyling data is in an invalid format.</exception>
		T Load(OpenedFile file);
		
		/// <summary>
		/// Saves a file model.
		/// The file should be saved to a location where a subsequent Load() operation can load from.
		/// Usually, this means saving to a lower-level model (e.g. a TextDocument model is saved by creating a binary model).
		/// 
		/// After the Save() method completes successfully, the specified model may no longer be marked as dirty. For models saving to
		/// lower-level models, this is done by marking that lower-level model as dirty.
		/// In other cases, the provider may have to explicitly remove the dirty flag from the model.
		/// </summary>
		void Save(OpenedFile file, T model);
		
		/// <summary>
		/// Saves a file model to disk. This method may by-pass lower-level models (such as FileModels.Binary) and directly write to disk.
		/// Note that this method is supposed to save a copy of the file, without resetting the dirty flag (think "Save Copy As...").
		/// However, transferring the dirty flag to a lower-level model is allowed.
		/// </summary>
		/// <exception cref="System.IO.IOException">Error saving the file.</exception>
		void SaveCopyAs(OpenedFile file, T model, FileName outputFileName);
		
		/// <summary>
		/// Gets whether this provider can load from the specified other provider.
		/// It is important that CanLoadFrom() is transitive!
		/// For a file model provider that loads from a text document model, the implementation should look like this:
		/// <code>
		/// return otherProvider == FileModels.TextDocument || FileModels.TextDocument.CanLoadFrom(otherProvider);
		/// </code>
		/// </summary>
		bool CanLoadFrom<U>(IFileModelProvider<U> otherProvider) where U : class;
		
		/// <summary>
		/// Notifies the provider that a file was renamed.
		/// If the file name is already available as part of the model, the provider is responsible for updating the model.
		/// </summary>
		void NotifyRename(OpenedFile file, T model, FileName oldName, FileName newName);
		
		/// <summary>
		/// Notifies the provider that a model was marked as stale.
		/// </summary>
		void NotifyStale(OpenedFile file, T model);
		
		/// <summary>
		/// Notifies a model that it became unloaded.
		/// This can happen due to:
		/// <list type="items"></list>
		/// <item>an explicit <see cref="OpenedFile.UnloadModel"/> call</item>
		/// <item>a <see cref="OpenedFile.ReplaceModel"/> call replacing this model with another</item>
		/// <item>a <see cref="OpenedFile.GetModel"/> call replacing a stale model with another</item>
		/// <item>the OpenedFile getting disposed</item>
		/// </summary>
		void NotifyUnloaded(OpenedFile file, T model);
	}
}
