using System;
using DBSettings;

namespace Sibintech.ShowModel
{
    public class TreeListModel : ProjTreeModel
    {
        public bool HasChildren { get; set; }
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
    }
}