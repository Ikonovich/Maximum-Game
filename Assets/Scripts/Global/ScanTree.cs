using System;
using System.Collections.Generic;
using UnityEngine;
using Candlelight;

namespace MaxGame {

    /// <summary>
    /// This class implements a quadtree to allow GameItems to efficiently scan for neighbors.
    /// Resolution of the tree is determined by the Resolution parameter.
    /// The tree will stop branching when either the X or Y size is equal to or below the resolution.
    /// Utilizes the ScanNode class.
    /// </summary>


    public class ScanTree : MonoBehaviour {

        protected GameController GameController;

        protected List<GameItem> SearchResults;

        protected float Resolution = 200f;

        protected float Xsize = 2000f;

        protected float Zsize = 2000f;

        protected int Depth = 0;

        protected ScanNode TopNode;

        protected float SearchNum = 0;

        protected float UpdateCountdown = 0.05f;

        protected float UpdateTime = 0.5f;

        protected ScanNode CurrentNode;

        protected List<GameItem> ReturnedItems;

        protected void Awake() {

            GameObject controllerObject = GameObject.Find("/GameController");
            GameController = controllerObject.GetComponent<GameController>();

            GameObject terrainHolder = GameObject.Find("TerrainHolder");

            NewTree();
        }

        protected void Update() {

            if (UpdateCountdown <= 0) {

                UpdateTree();
                UpdateCountdown = UpdateTime;
            }
            else {

                UpdateCountdown -= Time.deltaTime;
            }
        }

        protected void NewTree() {

            TopNode = new ScanNode(0, 0, Xsize, Zsize);
            GenerateTree(TopNode);
            MeasureDepth();

        }

        protected void GenerateTree(ScanNode node) {

            if ((node.Xsize > Resolution) && (node.Zsize > Resolution)) {
                    
                node.CreateChildren();

                for (int i = 0; i < node.Children.Length; i++) {

                    GenerateTree(node.Children[i]);
                }
            }
            else {
                node.IsBottom = true;
            }

        }

        protected void UpdateTree() {

            UpdateCountdown = UpdateTime;

            Dictionary<int, GameItem> itemDict = GameController.GetWorldItemDict();

            int i = 0;
            foreach (KeyValuePair<int, GameItem> kvp in itemDict) {

                i += 1;
                GameItem item = kvp.Value;

                Debug.Log($"World item dict item num " + i + " with Game ID: "+ item.GameID);


                if ((item.ScanNode == null) || (ScanNode.WithinBounds(item, item.ScanNode) == false)) {

                    TopNode.AddItem(item);
                    Debug.Log($"Positioning item on tree: " + item.GameID);
                }
            }

        }

        /// <summary>
        /// Searches the tree within range of a provided point (Adjusted for the resolution of the truee), 
        /// returning a list of all registered game items within that bounds.
        /// 
        /// </summary>

        public List<GameItem> StartSearch(Vector3 position, float range) {

            SearchNum = 0;

            SearchResults = new List<GameItem>();

            Search(position, range, TopNode);
            return SearchResults;

            //return SimpleSearch(position, range);
        }


        /// <summary>
        /// This simplified search method bypasses the scan tree.
        /// </summary>

        protected List<GameItem> SimpleSearch(Vector3 position, float range) {

            Dictionary<int, GameItem> itemDict = GameController.GetWorldItemDict();
            List<GameItem> itemList = new List<GameItem>();

            foreach (KeyValuePair<int, GameItem> kvp in itemDict) {

                GameItem item = kvp.Value;


                if (item != null) {

                    if ((item.transform.position - position).magnitude < range) {

                        itemList.Add(item);

                    }
                }
            }

            return itemList;

        }


        

        protected void Search(Vector3 position, float range, ScanNode node) {

            //Debug.Log($"Search num " + SearchNum + " from position " + position.ToString());
            //Debug.Log($"Current search coords: " + node.Xorigin + " ," + node.Zorigin);
            //Debug.Log($"Current search resolution: " + node.Xsize + ", " + node.Zsize);

            SearchNum += 1;

            if (node.IsEmpty == false) {

                if (node.IsBottom == true) {
                                    
                    Debug.Log($"Bottom node reached during search");


                    for (int i = 0; i < node.ItemList.Count; i++) {
                        SearchResults.Add(node.ItemList[i]);
                        //Debug.Log($"Adding item to search results. ID: " + item.GameID + "  Position: " + item.transform.position);
                    }
                }
                else if ((ScanNode.WithinBounds(position, node) || ScanNode.WithinRange(position, range + Resolution, node))) {
                    
                    Debug.Log($"Item within bounds of node at " + node.Xorigin + " ," + node.Zorigin);
                    Debug.Log($"With resolution " + node.Xsize + ", " + node.Zsize);

                    
                    for (int i = 0; i < node.Children.Length; i++) {
                        Search(position, range, node.Children[i]);
                    }
                }  
            }

        }


        /// <summary>
        /// This method iterates through the tree and stores its size in this.Depth;
        /// </summary>
        public void MeasureDepth() {

            ScanNode currentNode = TopNode;
            Depth = 1;

            while (currentNode.IsBottom != true) {

                Depth += 1;
                currentNode = currentNode.Children[0];
            }
            Debug.Log($"Measured tree depth: " + Depth);

        }

        /// <summary>
        /// This method lists all the items currently stored in the tree.
        /// </summary>

        public void GetAllItems(ScanNode node) {


            if (node.IsBottom == false) {
                foreach (ScanNode child in node.Children) {
                    
                    if (child.IsEmpty == false) {

                        GetAllItems(child);
                    }
                }
            }
            else { 

                Debug.Log($"Bottom node reached. Size: " + node.ItemList.Count);

                foreach (GameItem item in node.ItemList) {

                    Debug.Log($"Item in tree: " + item.GameID);

                }
            }
        }
    }

    /// <summary>
    /// A single node of the QuadTree implemented by the Scanner.
    /// IsEmpty is set to false to indicate that at least one child of this object at any level 
    /// contains an ItemList with a non-zero GameItem count.
    /// </summary>


    public class ScanNode {

        public bool IsBottom = false;

        public bool IsEmpty = true;

        protected ScanNode Parent;
        
        public ScanNode[] Children;

        public float Xorigin = 0f;
        
        public float Zorigin = 0f;

        public float Xsize = 0f;
        public float Zsize = 0f;

        private FloatBounds Bounds;

        public List<GameItem> ItemList;

        /// <summary>
        /// This constructor is only called to create the top level node.
        /// </summary>

        public ScanNode(float xOriginIn, float zOriginIn, float xSizeIn, float zSizeIn) {

            Xorigin = xOriginIn;
            Zorigin = zOriginIn;
            Xsize = xSizeIn;
            Zsize = zSizeIn;
            
            Bounds = new FloatBounds(new Vector3(Xorigin + (Xsize / 2), 0, Zorigin + (Zsize / 2)), new Vector3(Xsize, 0, Zsize));
        }

        /// <summary>
        /// This constructor is called by CreateChildren to create the layers of the octree.
        /// </summary>

        protected ScanNode(ScanNode parentIn, float xOriginIn, float zOriginIn, float xSizeIn, float zSizeIn) {


            Parent = parentIn;
            Xorigin = xOriginIn;
            Zorigin = zOriginIn;
            Xsize = xSizeIn;
            Zsize = zSizeIn;

            Bounds = new FloatBounds(new Vector3(Xorigin + (Xsize / 2), 0, Zorigin + (Zsize / 2)), new Vector3(Xsize, 0, Zsize));

        }

        public void CreateChildren() {

            ScanNode bottomLeft = new ScanNode(this, Xorigin, Zorigin, Xsize / 2f, Zsize / 2f);
            ScanNode bottomRight = new ScanNode(this, Xorigin + (Xsize / 2f), Zorigin, Xsize / 2f, Zsize / 2f);
            ScanNode topLeft = new ScanNode(this, Xorigin, Zorigin + (Zsize / 2f), Xsize / 2f, Zsize / 2f);
            ScanNode topRight = new ScanNode(this, Xorigin + (Xsize / 2f), Zorigin + (Zsize / 2f), Xsize / 2f, Zsize / 2f);

            Children = new ScanNode[4] {bottomLeft, bottomRight, topLeft, topRight};
        }

    
        /// <summary>
        /// If this is a bottom node, AddItem adds the item to its ItemList.
        /// Otherwise, it finds the child the item belongs in and passes it on.
        /// Returns true once it places an item, false if it fails to.
        /// </summary>

        public bool AddItem(GameItem item) {


            if (IsBottom == true) {

                IsEmpty = false;

                if (ItemList == null) {
                    
                    ItemList = new List<GameItem>();
                }

                if (item.ScanNode != null) {

                    item.ScanNode.RemoveItem(item);
                }
            
                
                item.ScanNode = this;
                ItemList.Add(item);

                Debug.Log($"Item added to tree: " + item.GameID);
                Debug.Log($"Item placed in node: " + this.ToString());
                return true;

            }
            else {

                int i = 0;
                foreach (ScanNode child in this.Children) {

                    i += 1;

                    Debug.Log($"Checking child " + i);
                    Debug.Log($"Child coords: " + child.Xorigin + " ," + child.Zorigin);
                    Debug.Log($"Child resolution: " + child.Xsize + ", " + child.Zsize);


                    if (ScanNode.WithinBounds(item, child)) {

                        Debug.Log($"Iterating down tree to place item. Current coords: " + child.Xorigin + " ," + child.Zorigin);
                        Debug.Log($"Current resolution: " + child.Xsize + ", " + child.Zsize);

                        IsEmpty = false;
                        return child.AddItem(item);

                    }

                }
            }
            Debug.Log($"Attempting to add item to tree, but failed");
            Debug.Log($"Failed item coords: " + item.transform.position.ToString());
            Debug.Log($"Failed on node with origin " + Xorigin + " ," + Zorigin + " and size " + Xsize + ", " + Zsize);
            return false;

        }

        public bool RemoveItem(GameItem item) {

            if (IsBottom == true) {

                Debug.Log($"Attempting to remove item from tree");
                ItemList.Remove(item);

                if (ItemList.Count == 0) {

                    IsEmpty = true;
                    Parent.ChildEmptied();
                    return true;
                }
            }
            else {

                if (ScanNode.WithinBounds(item, this)) {
                    return this.RemoveItem(item);
                }

            }

            Debug.Log($"Attempted to remove item from tree, but failed");
            return false;

        }

        /// <summary>
        /// This method is called by any child node on its parent when the child becomes non empty.
        /// If the parent was set to empty, it propagates up the hierarchy. 
        /// </summary>
        public void ChildFilled() {

            if (IsEmpty == true) {

                IsEmpty = false;
                Debug.Log($"Node set to not empty");

                if (Parent != null) {

                    Parent.ChildFilled();
                }
            }

            
        }

        /// <summary>
        /// This method is called by any child node on its parent when the child becomes empty.
        /// If the parent's remaining children are also empty, it propagates up the chain. 
        /// </summary>

        public void ChildEmptied() {

            bool allEmpty = true;

            foreach (ScanNode child in Children) {

                if (child.IsEmpty == false) {

                    allEmpty = false;
                }
            }

            if (allEmpty == true) {

                
                IsEmpty = true;
                Parent.ChildEmptied();
            }
        }

        public override string ToString() {

            return ("Origin: (" + Xorigin + ", " + Zorigin + ")" + "\nSize: (" + Xsize + ", " + Zsize + ")" + "\nIsBottom: " + IsBottom + "\nIsEmpty: " + IsEmpty);
            
        }

        
        /// <summary>
        /// Checks to see if a particular item is within the bounds of a particular node.
        /// </summary>
        /// <param item="The item whose position should be checked"></param>
        /// <param node="The node whose bounds should be checked."></param>

        public static bool WithinBounds(GameItem item, ScanNode node) {

            Vector3 xz = new Vector3(item.transform.position.x, 0f, item.transform.position.z);

            return node.Bounds.Contains(xz);


            // float xCoord = item.transform.position.x;
            // float zCoord = item.transform.position.z;
            
            // if ((xCoord >= node.Xorigin) && (xCoord <= (node.Xorigin + node.Xsize))) {

            //     if ((zCoord >= node.Zorigin) && (zCoord <= (node.Zorigin + node.Zsize))) {

            //         return true;
            //     }
            // }
            // return false;
        }

        /// <summary>
        /// Checks to see if a particular xz coordinate is within the bounds of a particular node.
        /// </summary>
        /// <param coord="The xz coord whose position should be checked"></param>
        /// <param node="The node whose bounds should be checked."></param>

        public static bool WithinBounds(Vector3 position, ScanNode node) {

            Vector3 xz = new Vector3(position.x, 0f, position.z);

            return node.Bounds.Contains(xz);
        }

        /// <summary>
        /// Checks to see if any part of a particular node is within the range provided.
        /// </summary>

        public static bool WithinRange(Vector3 position, float range, ScanNode node) {

            FloatBounds bounds = new FloatBounds(position, new Vector3(range, 1, range));

            return bounds.Contains(new Vector3(node.Xorigin, 0f, node.Zorigin));


        }

    }

    /// <summary>
    /// This struct functions as a 2 dimensional bounds in the
    /// XZ plane, stored as integers for maximum efficiency of
    /// bounds checking.
    /// It can take either 3D or 2D int vectors during instantiation,
    /// but ignores the Y axis of a 3D vector. 
    /// It has Contains methods for both 2D and 3D int vectors.
    /// </summary>

    struct FloatBounds {

        private float Xmin;
        private float Xmax;
        private float Zmin;
        private float Zmax;

        public FloatBounds(Vector3 center, Vector3 size) {

            Xmin = center.x - size.x;
            Xmax = center.x + size.x;

            Zmin = center.z - size.z;
            Zmax = center.z + size.z;

        }

        public FloatBounds(Vector2 center, Vector2 size) {

            Xmin = center.x - size.x;
            Xmax = center.x + size.x;

            Zmin = center.y - size.y;
            Zmax = center.y + size.y;

        }

        public bool Contains(Vector3 position) {

            float xPos = position.x;
            float zPos = position.z;

            return ((xPos > Xmin) && (xPos < Xmax) && (zPos > Zmin) && (zPos < Zmax));


        }

        public bool Contains(Vector2 position) {

            float xPos = position.x;
            float zPos = position.y;

            return ((xPos > Xmin) && (xPos < Xmax) && (zPos > Zmin) && (zPos < Zmax));

        }

        

        

    }
}