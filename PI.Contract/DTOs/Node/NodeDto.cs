using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Contract.DTOs.Node
{
   public class NodeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public List<NodeDto> Children { get; set; }
        //  public int ManagerCount { get; set; }
        public List<NodeDto> Manager { get; set; }
        public List<NodeDto> Costcenter { get; set; }
        public List<NodeDto> Supervisor { get; set; }
        public bool IsActive { get; set; }
    }
}
