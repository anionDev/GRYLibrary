﻿using System;

namespace GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments
{
    public abstract class Environment
    {
        private protected Environment() { }
        public abstract void Accept(IEnvironmentVisitor visitor);
        public abstract T Accept<T>(IEnvironmentVisitor<T> visitor);
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                return GetType().Equals(obj.GetType());
            }
        }
        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
        public override string ToString()
        {
            return GetType().Name;
        }
        public static Environment Parse(string environment)
        {
            if (environment == null)
            {
                throw new ArgumentException($"Value for parameter {nameof(environment)} is null.");
            }
            else if (nameof(Development) == environment)
            {
                return Development.Instance;
            }
            else if (nameof(QualityCheck) == environment)
            {
                return QualityCheck.Instance;
            }
            else if (nameof(Productive) == environment)
            {
                return Productive.Instance;
            }
            else
            {
                throw new System.Collections.Generic.KeyNotFoundException($"Unknown environment: {environment}");
            }
        }
    }
    public interface IEnvironmentVisitor
    {
        void Handle(Development environment);
        void Handle(Productive environment);
        void Handle(QualityCheck environment);
    }
    public interface IEnvironmentVisitor<T>
    {
        T Handle(Development environment);
        T Handle(Productive environment);
        T Handle(QualityCheck environment);
    }
}