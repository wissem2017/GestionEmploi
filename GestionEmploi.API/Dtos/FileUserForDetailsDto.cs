using System;

namespace GestionEmploi.API.Dtos
{
    public class FileUserForDetailsDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime  DateAdded { get; set; }
        public bool IsMain { get; set; } //si la photos initial
                
    }
}