// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace ICSharpCode.AvalonEdit.Highlighting
{
	/// <summary>
	/// Manages a list of syntax highlighting definitions.
	/// </summary>
	public class HighlightingManager : IHighlightingDefinitionReferenceResolver
	{
		sealed class RegisteredHighlighting
		{
			public IHighlightingDefinition Definition;
			public Func<IHighlightingDefinition> LazyLoadingFunction;
		}
		
		Dictionary<string, RegisteredHighlighting> highlightingsByName = new Dictionary<string, RegisteredHighlighting>();
		Dictionary<string, RegisteredHighlighting> highlightingsByExtension = new Dictionary<string, RegisteredHighlighting>(StringComparer.OrdinalIgnoreCase);
		
		/// <summary>
		/// Gets a highlighting definition by name.
		/// Returns null if the definition is not found.
		/// </summary>
		public IHighlightingDefinition GetDefinition(string name)
		{
			RegisteredHighlighting rh;
			if (highlightingsByName.TryGetValue(name, out rh))
				return GetDefinition(rh);
			else
				return null;
		}
		
		/// <summary>
		/// Gets the names of the registered highlightings.
		/// </summary>
		public IEnumerable<string> HighlightingNames {
			get { return highlightingsByName.Keys; }
		}
		
		/// <summary>
		/// Gets a highlighting definition by extension.
		/// Returns null if the definition is not found.
		/// </summary>
		public IHighlightingDefinition GetDefinitionByExtension(string extension)
		{
			RegisteredHighlighting rh;
			if (highlightingsByExtension.TryGetValue(extension, out rh))
				return GetDefinition(rh);
			else
				return null;
		}
		
		static IHighlightingDefinition GetDefinition(RegisteredHighlighting rh)
		{
			if (rh != null) {
				var func = rh.LazyLoadingFunction;
				if (func != null) {
					// prevent endless recursion when there are cyclic references between syntax definitions
					rh.LazyLoadingFunction = null;
					rh.Definition = func();
				}
				return rh.Definition;
			} else {
				return null;
			}
		}
		
		/// <summary>
		/// Registers a highlighting definition.
		/// </summary>
		/// <param name="name">The name to register the definition with.</param>
		/// <param name="extensions">The file extensions to register the definition for.</param>
		/// <param name="highlighting">The highlighting definition.</param>
		public void RegisterHighlighting(string name, string[] extensions, IHighlightingDefinition highlighting)
		{
			if (highlighting == null)
				throw new ArgumentNullException("highlighting");
			RegisterHighlighting(name, extensions, new RegisteredHighlighting { Definition = highlighting });
		}
		
		/// <summary>
		/// Registers a highlighting definition.
		/// </summary>
		/// <param name="name">The name to register the definition with.</param>
		/// <param name="extensions">The file extensions to register the definition for.</param>
		/// <param name="lazyLoadedHighlighting">A function that loads the highlighting definition.</param>
		public void RegisterHighlighting(string name, string[] extensions, Func<IHighlightingDefinition> lazyLoadedHighlighting)
		{
			if (lazyLoadedHighlighting == null)
				throw new ArgumentNullException("lazyLoadedHighlighting");
			RegisterHighlighting(name, extensions, new RegisteredHighlighting { LazyLoadingFunction = lazyLoadedHighlighting });
		}
		
		void RegisterHighlighting(string name, string[] extensions, RegisteredHighlighting rh)
		{
			if (name != null) {
				highlightingsByName[name] = rh;
			}
			if (extensions != null) {
				foreach (string ext in extensions) {
					highlightingsByExtension[ext] = rh;
				}
			}
		}
		
		/// <summary>
		/// Gets the default HighlightingManager instance.
		/// The default HighlightingManager comes with built-in highlightings.
		/// </summary>
		public static HighlightingManager Instance {
			get {
				return DefaultHighlightingManager.Instance;
			}
		}
		
		internal sealed class DefaultHighlightingManager : HighlightingManager
		{
			public new static readonly DefaultHighlightingManager Instance = new DefaultHighlightingManager();
			
			public DefaultHighlightingManager()
			{
				Resources.RegisterBuiltInHighlightings(this);
			}
			
			// Registering a built-in highlighting
			internal void RegisterHighlighting(string name, string[] extensions, string resourceName)
			{
				try {
					#if DEBUG
					// don't use lazy-loading in debug builds, show errors immediately
					Xshd.XshdSyntaxDefinition xshd;
					using (Stream s = Resources.OpenStream(resourceName)) {
						using (XmlTextReader reader = new XmlTextReader(s)) {
							xshd = Xshd.HighlightingLoader.LoadXshd(reader, false);
						}
					}
					Debug.Assert(name == xshd.Name);
					if (extensions != null)
						Debug.Assert(System.Linq.Enumerable.SequenceEqual(extensions, xshd.Extensions));
					else
						Debug.Assert(xshd.Extensions.Count == 0);
					
					// round-trip xshd:
					using (XmlTextWriter writer = new XmlTextWriter("c:\\temp\\" + resourceName, System.Text.Encoding.UTF8)) {
						writer.Formatting = Formatting.Indented;
						new Xshd.SaveXshdVisitor(writer).WriteDefinition(xshd);
					}
					using (FileStream fs = File.Create("c:\\temp\\" + resourceName + ".bin")) {
						new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(fs, xshd);
					}
					using (FileStream fs = File.Create("c:\\temp\\" + resourceName + ".compiled")) {
						new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(fs, Xshd.HighlightingLoader.Load(xshd, this));
					}
					
					RegisterHighlighting(name, extensions, Xshd.HighlightingLoader.Load(xshd, this));
					#else
					RegisterHighlighting(name, extensions, LoadHighlighting(resourceName));
					#endif
				} catch (HighlightingDefinitionInvalidException ex) {
					throw new InvalidOperationException("The built-in highlighting '" + name + "' is invalid.", ex);
				}
			}
			
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
			                                                 Justification = "LoadHighlighting is used only in release builds")]
			Func<IHighlightingDefinition> LoadHighlighting(string resourceName)
			{
				Func<IHighlightingDefinition> func = delegate {
					Xshd.XshdSyntaxDefinition xshd;
					using (Stream s = Resources.OpenStream(resourceName)) {
						using (XmlTextReader reader = new XmlTextReader(s)) {
							// in release builds, skip validating the built-in highlightings
							xshd = Xshd.HighlightingLoader.LoadXshd(reader, true);
						}
					}
					return Xshd.HighlightingLoader.Load(xshd, this);
				};
				return func;
			}
		}
	}
}
