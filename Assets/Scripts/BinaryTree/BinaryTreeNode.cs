using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI {
    [System.Serializable]
    public class BinaryTreeNode
    {
        public int value;


        public BinaryTreeNode left {
            get
            {
                if (_left == null)
                    _left = new BinaryTreeNode();
                return _left;
            }
        }
        public BinaryTreeNode right
        {
            get
            {
                if (_right == null)
                    _right = new BinaryTreeNode();
                return _right;
            }
        }
        
        private BinaryTreeNode _left;
        private BinaryTreeNode _right;
        private bool bInitialized = false;

        

        public void AddValue(int value)
        {
            if (!bInitialized)
            {
                this.value = value;
                bInitialized = true;
                return;
            }

            

            if (value < this.value)
            {
                left.AddValue(value);
            }
            else if (value > this.value)
            {
                right.AddValue(value);
            }
            
        }

        public void Visit()
        {
    
            if (left.bInitialized)
            {
                left.Visit();
            }
            Debug.Log(value);
            if (right.bInitialized)
                right.Visit();
        }

        public void Search(int value, int iteractionCount = 0)
        {
            iteractionCount++;
            if(value == this.value)
            {
                Debug.Log("Found value " + value + " during the iteraction " + iteractionCount);
                return;
            }
            if (value < this.value && left.bInitialized)
                left.Search(value, iteractionCount);
            else if (value > this.value && right.bInitialized)
                right.Search(value, iteractionCount);
            else
                Debug.Log("Value " + value + " during the iteraction " + iteractionCount);
        }
    }
}


