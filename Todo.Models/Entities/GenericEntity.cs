using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Models
{
    public interface IEntity<T>
    {
        T Id { get; set; }
        bool IsActive { get; set; }
    }

    public class Entity : IEntity<int>
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public bool IsActive { get; set; } = true;
        [Required]
        public bool IsDelete { get; set; } = false;
    }

    public class FullAuditedEntity : Entity
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        [StringLength(100)]
        public string? CreatedBy { get; set; }
        [StringLength(100)]
        public string? UpdatedBy { get; set; }

       // [StringLength(User.UserNameLength)]
        [Column(TypeName = "VARCHAR")]
        public string? CreatedUserName { get; set; }
       /// <summary>
       /// [StringLength(User.UserNameLength)]
       /// </summary>
        [Column(TypeName = "VARCHAR")]
        public string? UpdatedUserName { get; set; }


    }
}
