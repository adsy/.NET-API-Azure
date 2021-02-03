using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNET_WebAPI.Models
{
    public class Movie
    {
        //public int id { get; set; }

        //[Required(ErrorMessage = "Name cannot be null or empty.")]
        //public string Name { get; set; }

        //[Required(ErrorMessage = "Language cannot be null or empty.")]
        //public string Language { get; set; }

        //[Required(ErrorMessage = "Rating cannot be null or empty.")]
        //public double Rating { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string Duration { get; set; }
        public DateTime PlayingDate { get; set; }
        public DateTime PlayingTime { get; set; }
        public int TicketPrice { get; set; }
        public int Rating { get; set; }
        public string Genre { get; set; }
        public string TrailorUrl { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<Reservation> Reservations { get; set; }

        // SQL doesnt support IFormFile - images also not stored in DB as that is
        // bad practise.
        // Store images in wwwroot folder - store path file name in DB.
        // Call GET method - provides image path to file in wwwroot
        [NotMapped]
        public IFormFile Image { get; set; }

        //[Required(ErrorMessage = "Image cannot be null or empty.")]
        //public string ImageUrl { get; set; }
    }
}