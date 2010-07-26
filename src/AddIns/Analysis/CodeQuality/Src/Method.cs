using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Method : INode
    {
        /// <summary>
        /// Parameters which are used by method
        /// </summary>
        public ISet<MethodParameter> Parameters { get; private set; }

        /// <summary>
        /// Types which are used in body of method
        /// </summary>
        public ISet<Type> TypeUses { get; private set; }

        /// <summary>
        /// Methods which are called in body of method
        /// </summary>
        public ISet<Method> MethodUses { get; private set; }

        /// <summary>
        /// Fields which are accesed in body of method
        /// </summary>
        public ISet<Field> FieldUses { get; private set; }
        
        /// <summary>
        /// A name of method
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A return type of method
        /// </summary>
        public Type ReturnType { get; set; }

        /// <summary>
        /// Type which owns this method
        /// </summary>
        public Type Owner { get; set; }

        /// <summary>
        /// Whether the method is constructor or not
        /// </summary>
        public bool IsConstructor { get; set; }

        /// <summary>
        /// Whether the method is public
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Whether the method is private
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Whether the method is protected
        /// </summary>
        public bool IsProtected { get; set; }

        /// <summary>
        /// Whether the method is static
        /// </summary>
        public bool IsStatic { get; set; }

        /// <summary>
        /// Whether the method is sealed
        /// </summary>
        public bool IsSealed { get; set; }

        /// <summary>
        /// Whether the method is abstract
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Whether the method is setter
        /// </summary>
        public bool IsSetter { get; set; }

        /// <summary>
        /// Whether the method is getter
        /// </summary>
        public bool IsGetter { get; set; }

        /// <summary>
        /// Whether the method is virtual
        /// </summary>
        public bool IsVirtual { get; set; }

        /// <summary>
        /// If the return type is generic instance so all types used in generic are presented in this set.
        /// </summary>
        public ISet<Type> GenericReturnTypes { get; private set; }

        /// <summary>
        /// Whether the return type is generic instance
        /// </summary>
        public bool IsReturnTypeGenericInstance { get; set; }

        public Method()
        {
            Parameters = new HashSet<MethodParameter>();

            TypeUses = new HashSet<Type>();
            MethodUses = new HashSet<Method>();
            FieldUses = new HashSet<Field>();
            GenericReturnTypes = new HashSet<Type>();

            ReturnType = null;
            Owner = null;

            IsReturnTypeGenericInstance = false;

            Dependency = null;
        }
        
        public Relationship GetRelationship(INode node)
        {
        	Relationship relationship = new Relationship();
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
            builder.AppendLine("Method Summary");
            builder.Append(Environment.NewLine);
            builder.AppendLine(String.Format("Name: {0}", Name));
            builder.AppendLine(String.Format("Parameters: {0}", Parameters.Count));
            // more to come

            builder.Append(Environment.NewLine);

            if (IsAbstract)
                builder.AppendLine("IsAbstract");
            if (IsConstructor)
                builder.AppendLine("IsConstructor");
            if (IsGetter)
                builder.AppendLine("IsGetter");
            if (IsSetter)
                builder.AppendLine("IsSetter");
            if (IsPrivate)
                builder.AppendLine("IsPrivate");
            if (IsProtected)
                builder.AppendLine("IsProtected");
            if (IsPublic)
                builder.AppendLine("IsPublic");
            if (IsSealed)
                builder.AppendLine("IsSealed");
            if (IsStatic)
                builder.AppendLine("IsStatic");
            if (IsVirtual)
                builder.AppendLine("IsVirtual");

            return builder.ToString();
        }
        
        public BitmapSource Icon { get { return NodeIconService.GetIcon(this); } }
    }

    public class MethodParameter
    {
        /// <summary>
        /// The type of the parameter
        /// </summary>
        public Type ParameterType { get; set; }

        /// <summary>
        /// Whether the parameter is generic instance
        /// </summary>
        public bool IsGenericInstance { get; set; }

        /// <summary>
        /// Whether the parameter is in
        /// </summary>
        public bool IsIn { get; set; }

        /// <summary>
        /// Whether the parameter is out
        /// </summary>
        public bool IsOut { get; set; }

        /// <summary>
        /// Whether the parameter is optional
        /// </summary>
        public bool IsOptional { get; set; }

        /// <summary>
        /// If the parameter is generic instance so all types used in generic are presented in this set.
        /// </summary>
        public ISet<Type> GenericTypes { get; set; }

        public MethodParameter()
        {
            GenericTypes = new HashSet<Type>();
            IsGenericInstance = false;
        }
    }
}
