// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// Description of FormsArtefactFactories.
	/// </summary>
	public static class FormsArtefactFactories
	{
		public const string FORMS_ARTEFACT_FACTORIES_PATH = "/SharpServerTools/SharpDbTools/FormsArtefactFactory";
		public static Dictionary<string, FormsArtefactFactory> factories = new Dictionary<string, FormsArtefactFactory>();
		
		static FormsArtefactFactories()
		{
			AddInTreeNode node = 
				AddInTree.GetTreeNode(FORMS_ARTEFACT_FACTORIES_PATH);
			List<Codon> codons = node.Codons;
			foreach (Codon codon in codons) {
				// create an instance of the relevant FormsArtefactFactory indexed by invariant name
				string invariant = codon.Id;
				FormsArtefactFactory factory = (FormsArtefactFactory)node.BuildChildItem(invariant, null, null);
				factories.Add(invariant, factory);
			}
		}
		
		public static FormsArtefactFactory GetFactory(string invariantName)
		{
			LoggingService.Debug("Looking for FormsArtefactFactory for: " + invariantName);
			
			// to test this base it on hardcoded strings for the type of the factory
			
			// TODO: drive this from the AddIn tree
			
			FormsArtefactFactory factory = null;
			factories.TryGetValue(invariantName, out factory);
			if (factory == null) {
				throw new ArgumentException("No FormsArtefactFactory found for InvariantName: "
				                            + invariantName);
			}
			return factory;
		}
	}
}
