using System;
using System.ComponentModel.DataAnnotations;
namespace DBSettings
{
    public class ProjTreeModel
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Childrens { get; set; }
        public DateTime CreatedDate  { get; set; }
        public DateTime RedactedDate  { get; set; }
    }
}