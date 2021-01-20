using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyNotesConsoleApp.Data.Entities
{
    public class Tag : IBaseEntity
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } = null;

        public virtual ICollection<NoteTag> NoteTags { get; set; }
    }
}