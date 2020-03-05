﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GRYLibrary.Core.Graph
{
    public class UndirectedEdge : Edge
    {
        public IList<Vertex> ConnectedVertices { get; internal set; }
        public UndirectedEdge(IEnumerable<Vertex> connectedVertices, string name, double weight = 1) : base(name, weight)
        {
            if (connectedVertices.Count() != 2)
            {
                throw new Exception($"An {nameof(UndirectedEdge)}-object must have exactly 2 connected vertices");
            }
            this.ConnectedVertices = new List<Vertex>(connectedVertices);
        }

        public UndirectedEdge(IEnumerable<Vertex> connectedVertices) : this(connectedVertices, CalculateName(connectedVertices))
        {
        }

        private static string CalculateName(IEnumerable<Vertex> connectedVertices)
        {
            return $"({string.Join("<->", connectedVertices)})";
        }

        public override bool Connects(Vertex fromVertex, Vertex toVertex)
        {
            List<Vertex> vertices = this.ConnectedVertices.ToList();
            bool x1 = vertices[0].Equals(fromVertex);
            bool x2 = vertices[1].Equals(toVertex);
            bool x3 = vertices[1].Equals(fromVertex);
            bool x4 = vertices[0].Equals(toVertex);
            return x1 && x2 || x3 && x4;
        }

        internal DirectedEdge ToDirectedEdge()
        {
            List<Vertex> connectedVertices = ConnectedVertices.ToList();
            return new DirectedEdge(connectedVertices[0], connectedVertices[1], this.Name, this.Weight);
        }
        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }
            UndirectedEdge typedObject = (UndirectedEdge)obj;
            if (!this.ConnectedVertices.NullSafeEnumerableEquals(typedObject.ConnectedVertices))
            {
                return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override IEnumerable<Vertex> GetConnectedVertices()
        {
            return ConnectedVertices;
        }

        public override void Accept(IEdgeVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IEdgeVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
        public override IEnumerable<Vertex> GetInputs()
        {
            return new List<Vertex>(ConnectedVertices);
        }

        public override IEnumerable<Vertex> GetOutputs()
        {
            return new List<Vertex>(ConnectedVertices);
        }

        public override IEnumerable<Vertex> GetOtherConnectedVerticesVisitor(Vertex vertex)
        {
            if (this.ConnectedVertices.Contains(vertex))
            {
                List<Vertex> result = this.ConnectedVertices.ToList();
                if (!result.RemoveItemOnlyOnce(vertex))
                {
                    throw new Exception();
                }
                return result;
            }
            else
            {
                throw new Exception();
            }
        }
    }
}
