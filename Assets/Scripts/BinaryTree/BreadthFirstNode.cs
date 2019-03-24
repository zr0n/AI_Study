using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [System.Serializable]
    public class BreadthFirstNode
    {
        public bool wasSearched;
        public List<BreadthFirstNode> edges {
            get
            {
                if (_edges == null)
                    _edges = new List<BreadthFirstNode>();
                return _edges;
            }
        }

        private List<BreadthFirstNode> _edges;
        public string value = "";
        public BreadthFirstNode parent;

        public BreadthFirstNode(string value)
        {
            this.value = value;
        }

        public void Connect(BreadthFirstNode edge)
        {
            edges.Add(edge);
            edge.edges.Add(this);
        }
    }
}