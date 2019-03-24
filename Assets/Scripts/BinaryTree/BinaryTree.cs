using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    /**
     * A Binary Tree is used to traverse the shortest iteration loop possible through a range of sortable values
     * In computer science, a binary tree is a tree data structure in which each node has at most two children,
     * which are referred to as the left child and the right child. A recursive definition using just set theory
     * notions is that a (non-empty) binary tree is a tuple (L, S, R), where L and R are binary trees or the empty
     * set and S is a singleton set.[1] Some authors allow the binary tree to be the empty set as well.[2]
     */
    public class BinaryTree : MonoBehaviour
    {
        public List<int> values;

        public BinaryTreeNode rootNode;

        [SerializeField] bool doVisit = true;
        [SerializeField] bool doSearch = false;
        [SerializeField] bool randomGenerateTree = false;
        [SerializeField] int nodesRandomGenerated = 1000;
        [SerializeField] int search = 0;

        [Multiline]
        [SerializeField]
        string description =
            @"/**
A Binary Tree is used to traverse the shortest iteration loop possible through a range of sortable values
In computer science, a binary tree is a tree data structure in which each node has at most two children,
which are referred to as the left child and the right child. A recursive definition using just set theory
notions is that a (non-empty) binary tree is a tuple (L, S, R), where L and R are binary trees or the empty
set and S is a singleton set.[1] Some authors allow the binary tree to be the empty set as well.[2]";

        // Start is called before the first frame update
        void Start()
        {
            rootNode = new BinaryTreeNode();
            if (randomGenerateTree)
                RandomGenerateTree();
            foreach(int v in values)
            {
                rootNode.AddValue(v);
            }

            if (doSearch)
                rootNode.Search(search);
            if (doVisit)
                rootNode.Visit();
        }

        void RandomGenerateTree()
        {
            for (int i = 0; i < nodesRandomGenerated; i++)
                values.Add(Random.Range(0, 999));
        }
    }

}