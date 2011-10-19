// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using ICSharpCode.CodeQualityAnalysis.Controls;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Namespace : IDependency, INode
    {
        private DependencyGraph _graphCache;

        /// <summary>
        /// Types within namespace
        /// </summary>
        public ISet<Type> Types { get; private set; }

        /// <summary>
        /// Name of namespace
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Module where is namespace located
        /// </summary>
        public Module Module { get; set; }

        public Namespace()
        {
            Types = new HashSet<Type>();

            Dependency = this;
        }

        public DependencyGraph BuildDependencyGraph()
        {
            if (_graphCache != null)
                return _graphCache;

            var g = new DependencyGraph();

            foreach (var type in Types)
            {
                g.AddVertex(new DependencyVertex(type));
            }

            foreach (var type in Types)
            {
                var types = type.GetUses();

                foreach (var dependType in types)
                {
                    if (dependType != type && dependType.Namespace == type.Namespace)
                        g.AddEdge(new DependencyEdge(new DependencyVertex(type), new DependencyVertex(dependType)));
                }
            }

            _graphCache = g;
            return g;
        }
        
        public IEnumerable<Type> GetAllTypes()
        {
        	foreach (var type in Types) {
        		yield return type;
        		foreach (var usedType in type.GetAllTypes()) {
        			yield return usedType;
        		}
        	}
        }
		
		public IEnumerable<Method> GetAllMethods()
		{
			foreach (var type in Types) {
        		foreach (var method in type.GetAllMethods()) {
        			yield return method;
        		}
        	}
		}
		
		public IEnumerable<Field> GetAllFields()
		{
			foreach (var type in Types) {
        		foreach (var field in type.GetAllFields()) {
        			yield return field;
        		}
        	}
		}
        
        public Relationship GetRelationship(INode node)
        {
        	Relationship relationship = new Relationship();
        	
        	if (node == this) {
        		relationship.Relationships.Add(RelationshipType.Same);
        		return relationship;
        	}
        	
        	if (node is Namespace) {
        		Namespace ns = (Namespace)node;
        		
        		foreach (var type in this.GetAllTypes()) {
        			if (type != null && type.Namespace == ns) {
        				relationship.AddRelationship(RelationshipType.UseThis);
        			}
        		}
        	}
        	
        	if (node is Type) {
        		Type type = (Type)node;
        		
        		foreach (var usedType in this.GetAllTypes()) {
        			if (type == usedType) {
        				relationship.AddRelationship(RelationshipType.UseThis);
        			}
        		}
        	}
        	
        	if (node is Method) {
        		Method method = (Method)node;
        		
        		foreach (var usedMethod in this.GetAllMethods()) {
        			if (method == usedMethod) {
        				relationship.AddRelationship(RelationshipType.UseThis);
        			}
        		}
        	}
        	
        	if (node is Field) {
        		Field field = (Field)node;
        		
        		foreach (var usedField in this.GetAllFields()) {
        			if (field == usedField) {
        				relationship.AddRelationship(RelationshipType.UseThis);
        			}
        		}
        	}
        	
        	return relationship;
        }

        public override string ToString()
        {
            return Name;
        }

        public IDependency Dependency { get; set; }

        public string GetInfo()
        {
            var builder = new StringBuilder();
            builder.AppendLine("Namespace Summary");
            builder.Append(Environment.NewLine);
            builder.AppendLine(String.Format("Name: {0}", Name));
            builder.AppendLine(String.Format("Methods: {0}", Types.Sum(type => type.Methods.Count)));
            builder.AppendLine(String.Format("Fields: {0}", Types.Sum(type => type.Fields.Count)));
            builder.AppendLine(String.Format("Types: {0}", Types.Count));
            // more to come
            
            return builder.ToString();
        }
        
        public BitmapSource Icon { get { return NodeIconService.GetIcon(this); } }
    }
}
