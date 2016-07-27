using PI.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Data.Entity
{
    public class Node 
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public List<Node> Children { get; set; }
      //  public int ManagerCount { get; set; }
        public List<Node> Manager { get; set; }
        public List<Node> Costcenter { get; set; }
        public List<Node> Supervisor { get; set; }
        public bool IsActive { get; set; }

    }
}
