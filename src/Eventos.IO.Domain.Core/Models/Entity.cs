﻿using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eventos.IO.Domain.Core.Models
{
    public abstract class Entity<T> : AbstractValidator<T> where T : Entity<T>
    {
        public Entity()
        {
            ValidationResult = new ValidationResult();
        }

        public Guid Id { get; protected set; }

        public abstract bool EhValido();

        public ValidationResult ValidationResult { get; protected set; }

        public override bool Equals(object obj)
        {
            var compareObj = obj as Entity<T>;

            if (ReferenceEquals(this, compareObj)) return true;
            if (ReferenceEquals(null, compareObj)) return false;

            return Id.Equals(compareObj.Id);
        }

        public static bool operator ==(Entity<T> a, Entity<T> b)
        {
            if ((ReferenceEquals(a, null)) && (ReferenceEquals(b, null)))
                return true;

            if ((ReferenceEquals(a, null)) || (ReferenceEquals(b, null)))
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity<T> a, Entity<T> b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (GetType().GetHashCode() * 907) + Id.GetHashCode();
        }

        public override string ToString()
        {
            return GetType().Name + $"[Id = {Id} ]";
        }

    }
}
