using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    /**
     * Breadth First Search Algorithm
     * Breadth-first search (BFS) is an algorithm for traversing or searching tree or graph data structures.
     * It starts at the tree root (or some arbitrary node of a graph, sometimes referred to as a 'search key'[1]),
     * and explores all of the neighbor nodes at the present depth prior to moving on to the nodes at the next depth level.
     * It uses the opposite strategy as depth-first search, which instead explores the highest-depth nodes first before being
     * forced to backtrack and expand shallower nodes.
     */
    public class BreadthFirst : MonoBehaviour
    {
        [SerializeField] string jsonString = "{\"movies\":[{\"title\":\"Diner\",\"cast\":[\"Steve Guttenberg\",\"Daniel Stern\",\"Mickey Rourke\",\"Kevin Bacon\",\"Tim Daly\",\"Ellen Barkin\",\"Paul Reiser\",\"Kathryn Dowling\",\"Michael Tucker\",\"Jessica James\",\"Colette Blonigan\",\"Kelle Kipp\",\"Clement Fowler\",\"Claudia Cron\"]},{\"title\":\"Footloose\",\"cast\":[\"Kevin Bacon\",\"Lori Singer\",\"Dianne Wiest\",\"John Lithgow\",\"Sarah Jessica Parker\",\"Chris Penn\",\"Frances Lee McCain\",\"Jim Youngs\",\"John Laughlin\",\"Lynne Marta\",\"Douglas Dirkson\"]},{\"title\":\"Flatliners\",\"cast\":[\"Kiefer Sutherland\",\"Julia Roberts\",\"Kevin Bacon\",\"William Baldwin\",\"Oliver Platt\",\"Kimberly Scott\",\"Joshua Rudoy\",\"Benjamin Mouton\",\"Hope Davis\",\"Patricia Belcher\",\"Beth Grant\"]},{\"title\":\"Eat Pray Love\",\"cast\":[\"Julia Roberts\",\"Javier Bardem\",\"Billy Crudup\",\"Richard Jenkins\",\"Viola Davis\",\"James Franco\",\"Sophie Thompson\",\"Mike O 'Malley\",\"Christine Hakim\",\"Arlene Tur\",\"Hadi Subiyanto\",\"Gita Reddy\",\"Tuva Novotny\",\"Luca Argentero\",\"Rushita Singh\"]},{\"title\":\"Spotlight\",\"cast\":[\"Mark Ruffalo\",\"Michael Keaton\",\"Rachel McAdams\",\"Liev Schreiber\",\"John Slattery\",\"Brian d'Arcy James\",\"Stanley Tucci\",\"Gene Amoroso\",\"Jamey Sheridan\",\"Billy Crudup\",\"Maureen Keiller\",\"Richard Jenkins\",\"Paul Guilfoyle\",\"Len Cariou\",\"Neal Huff\",\"Michael Cyril Creighton\",\"Laurie Heineman\",\"Tim Progosh\"]}]}";
        [SerializeField] Movie[] jsonParsed;
        [SerializeField] Dictionary<string, BreadthFirstNode> nodesClassified = new Dictionary<string, BreadthFirstNode>();

        [SerializeField] string from = "Tim Progosh";
        [SerializeField] string to = "Kevin Bacon";

        [Multiline] [SerializeField] string description = 
            @"Breadth-first search (BFS) is an algorithm for traversing or searching tree or graph data structures.
            It starts at the tree root (or some arbitrary node of a graph, sometimes referred to as a 'search key'[1]),
            and explores all of the neighbor nodes at the present depth prior to moving on to the nodes at the next depth level.
            It uses the opposite strategy as depth-first search, which instead explores the highest-depth nodes first before being
            forced to backtrack and expand shallower nodes.";

        // Start is called before the first frame update
        void Start()
        {
            jsonParsed = JsonUtility.FromJson<MovieList>(jsonString).movies;

            foreach(Movie movie in jsonParsed)
            {
                BreadthFirstNode movieNode = new BreadthFirstNode(movie.title);
                foreach(string actor in movie.cast)
                {
                    if (!nodesClassified.ContainsKey(actor))
                        nodesClassified[actor] = new BreadthFirstNode(actor);
                    nodesClassified[actor].Connect(movieNode);
                }
                nodesClassified[movie.title] = movieNode;
            }
            var shortestPath = new ShortestPath(nodesClassified);
            string searchResult = shortestPath.SetStart(from).SetEnd(to).Search();
            Debug.Log(searchResult);
        }

        void DebugActors()
        {
            foreach (string node in nodesClassified.Keys)
            {
                string debug = "Node: " + node + " connections: ";

                foreach (BreadthFirstNode edge in nodesClassified[node].edges)
                    debug += edge.value + " - ";

                debug = debug.Substring(0, debug.Length - 3); //remove last dash
                Debug.Log(debug);
            }
        }
    }

    [System.Serializable]
    public class Movie
    {
        public string title;
        public string[] cast;
    }

    [System.Serializable]
    public class MovieList
    {
        public Movie[] movies;
    }


    [System.Serializable]
    public class ShortestPath
    {
        public BreadthFirstNode start {
            get
            {
                if (_start == null)
                    _start = new BreadthFirstNode("Kevin Bacon");
                return _start;
            }
            
        }
        public BreadthFirstNode end
        {
            get
            {
                if (_end == null)
                    _end = new BreadthFirstNode("Kevin Bacon");
                return _end;
            }

        }

        private BreadthFirstNode _start;
        private BreadthFirstNode _end;
        private BreadthFirstNode current;
        private Dictionary<string, BreadthFirstNode> nodes;


        public ShortestPath(Dictionary<string, BreadthFirstNode> nodes)
        {
            this.nodes = nodes;
        }
        public string Search()
        {
            Debug.Log("Searching nearest connection between " + start.value + " and " + end.value);

            string connections = start.value + " - " + end.value;

            List<BreadthFirstNode> queue = new List<BreadthFirstNode> { start };
            if(start.value == end.value)
            {
                return "Found - " + connections;
            }
            connections = "";
            int i = 0;
            while(queue.Count > 0)
            {
                i++;
                current = queue[0];
                queue.RemoveAt(0);

                connections += current.value + " - ";
                if (current.value == end.value)
                {
                    connections = connections.Substring(0, connections.Length - 3);
                    connections =  "Found at iteration #" + i + " " + connections;

                    Debug.Log("Connections: " + connections);
                    string pathStr = "";
                    List<string> path = new List<string>();

                    var currentPathValue = end;
                    while(currentPathValue != null)
                    {
                        path.Add(currentPathValue.value);
                        currentPathValue = currentPathValue.parent;
                    }
                    for(int j = path.Count - 1; j >= 0; j--)
                    {
                        pathStr += path[j];
                        if (j > 0)
                            pathStr += " --> ";
                    }

                    return pathStr;
                }

                foreach (var edge in current.edges)
                {
                    
                    if (!edge.wasSearched)
                    {
                        edge.wasSearched = true;
                        edge.parent = current;
                        queue.Add(edge);
                    }
                }
            }
            return "Not Found " + end.value;

        }
        private ShortestPath SetExtreme (string key, Extreme extreme = Extreme.Start)
        {
            if (nodes != null && nodes.ContainsKey(key))
            {
                switch (extreme)
                {
                    case Extreme.Start:
                        this._start = nodes[key];
                        start.wasSearched = true;
                        break;
                    case Extreme.End:
                        this._end = nodes[key];
                        break;
                }
            }

            return this;
        }
        public ShortestPath SetStart(string key)
        {
            return SetExtreme(key, Extreme.Start);
        }
        public ShortestPath SetEnd(string key)
        {
            return SetExtreme(key, Extreme.End);
        }
    }
    public enum Extreme
    {
        Start,
        End
    }
}