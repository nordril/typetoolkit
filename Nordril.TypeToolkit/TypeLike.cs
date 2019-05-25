using System;
using System.Collections.Generic;
using System.Text;

namespace Nordril.TypeToolkit
{
    /*
    public abstract class TypeLike
    {
    }

    public class ConcreteType : TypeLike
    {
        public Type ClrOpenType { get; set; }
        public IList<TypeVariable> RemainingArguments { get; set; }
        public IDictionary<string, TypeLike> AppliedArguments { get; set; }
        public bool IsGeneric => RemainingArguments.Count + AppliedArguments.Count > 0;
    }

    public class TypeVariable : TypeLike
    {
        public string Name { get; set; }
        public ISet<TypeConstraint> Constraints { get; set; }
        public Variance Variance { get; set; }
    }

    public abstract class TypeConstraint
    {
    }

    public class HasDefaultConstructor : TypeConstraint
    {
    }

    public class ImplementsInterface : TypeConstraint
    {
        public Type Interface { get; set; }
    }

    public class HasBaseClass : TypeConstraint
    {
        public Type BaseClass { get; set; }
    }

    public class IsClass : TypeConstraint
    {
    }

    public class IsStruct : TypeConstraint
    {
    }

    public class IsEnum : TypeConstraint
    {
    }

    public enum Variance
    {
        Invariant,
        Covariant,
        Contravariant
    }
    */
}
