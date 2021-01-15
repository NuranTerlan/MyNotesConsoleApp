using System;

namespace MyNotesConsoleApp.Data.Entities
{
    public interface IBaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}