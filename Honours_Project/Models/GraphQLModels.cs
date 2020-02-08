using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honours_Project.Models
{
    public class GraphQLRepositoryInfo
    {
        public GraphQLRepository Repository { get; set; }
    }

    public class GraphQLRepository
    {
        public Ref Ref { get; set; }
    }

    public class Ref
    {
        public Target Target { get; set; }
    }

    public class Target
    {
        public string ID { get; set; }

        public History History { get; set; }
    }

    public class History
    {
        public Page_Info PageInfo { get; set; }

        public List<Edge> Edges { get; set; }
    }

    public class Page_Info
    {
        public bool HasNextPage { get; set; }
    }

    public class Edge
    {
        public Node Node { get; set; }
    }

    public class Node
    {
        public string MessageHeadline { get; set; }

        public string Oid { get; set; }

        public string Message { get; set; }

        public Author Author { get; set; }

        public int ChangedFiles { get; set; }

        public int Additions { get; set; }

        public int Deletions { get; set; }
    }

    public class Author
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime Date { get; set; }
    }
}
