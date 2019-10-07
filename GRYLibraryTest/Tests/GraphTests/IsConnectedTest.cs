﻿using GRYLibrary.Miscellaneous.GraphOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GRYLibraryTest.Tests.GraphTests
{
    [TestClass]
    public class IsConnectedTest
    {
        [TestMethod]
        public void TestGraphWithOneVertex()
        {
            Graph graph = new UndirectedGraph();
            try
            {
                graph.IsConnected();
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
                //test passed
            }
            Vertex v1 = new Vertex("v1");
            graph.AddVertex(v1);
            Assert.IsTrue(graph.IsConnected());
            Vertex v2 = new Vertex("v2");
            graph.AddVertex(v2);
            Assert.IsFalse(graph.IsConnected());
            graph.AddEdge(new Edge(v1, v2, "e1"));
            Assert.IsTrue(graph.IsConnected());
            graph.AddEdge(new Edge(v2, v1, "e2"));
            Assert.IsTrue(graph.IsConnected());
        }
    }
}
