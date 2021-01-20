using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyNotesConsoleApp.Data.Entities
{
    public class Note : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } = null;

        public virtual ICollection<NoteTag> NoteTags { get; set; }
    }
}