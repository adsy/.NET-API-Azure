using ASPNET_WebAPI.Data;
using ASPNET_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNET_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class MoviesController : ControllerBase
    {
        private CinemaDbContext _dbContext;

        public MoviesController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("[action]")]
        public IActionResult AllMovies(string sort, int? pageNumber, int? pageSize)
        {
            var _pageNumber = pageNumber ?? 1;
            var _pageSize = pageSize ?? 5;

            var movies = from movie in _dbContext.Movies
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             Duration = movie.Duration,
                             Language = movie.Language,
                             Rating = movie.Rating,
                             Genre = movie.Genre,
                             ImageUrl = movie.ImageUrl
                         };

            switch (sort)
            {
                case "asc":
                    return Ok(movies.OrderBy(x => x.Rating).Skip((_pageNumber - 1) * _pageSize).Take(_pageSize));

                case "desc":
                    return Ok(movies.OrderByDescending(m => m.Rating).Skip((_pageNumber - 1) * _pageSize).Take(_pageSize));

                default:
                    return Ok(movies.Skip((_pageNumber - 1) * _pageSize).Take(_pageSize));
            }
        }

        [HttpGet("[action]/{Id}")]
        public IActionResult MovieDetail(int Id)
        {
            var movie = _dbContext.Movies.FirstOrDefault(x => x.Id == Id);

            if (movie == null)
            {
                return NotFound();
            }
            //var updatedMovie = new
            //{
            //    Id = movie.Id,
            //    Name = movie.Name,
            //    Duration = movie.Duration,
            //    Language = movie.Language,
            //    Rating = movie.Rating,
            //    Genre = movie.Genre,
            //    ImageUrl = movie.ImageUrl
            //};

            return Ok(movie);
        }

        //api/movies/FindMovies?movieName=xxx
        [HttpGet("[action]")]
        public IActionResult FindMovies(string movieName)
        {
            var movies = from movie in _dbContext.Movies
                         where movie.Name.Contains(movieName)
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             ImageUrl = movie.ImageUrl
                         };

            return Ok(movies);
        }

        // Updated POST method to account for images being uploaded.
        // POST api/<MoviesController>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromForm] Movie movie)
        {
            // Creates a new unique name for file incase 2 files have same name - stops overwriting original file
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");

            // If image isn't null, create a filestream to the filePath created, copy image from form to wwwroot folder.
            if (movie.Image != null)
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);
                movie.Image.CopyTo(fs);
            }
            movie.ImageUrl = filePath.Remove(0, 7);
            _dbContext.Movies.Add(movie);

            _dbContext.SaveChanges();

            return Ok();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Put(int id, [FromForm] Movie movie)
        {
            var currentMovie = _dbContext.Movies.Find(id);

            if (currentMovie == null)
            {
                return NotFound("The movie you tried to update was not found");
            }

            // CODE BELOW TO UPLOAD AN IMAGE THROUGH A FORM TO wwwroot FOLDER
            // Creates a new unique name for file incase 2 files have same name - stops overwriting original file
            var guid = Guid.NewGuid();
            var filePath = Path.Combine("wwwroot", guid + ".jpg");

            // If image isn't null, create a filestream to the filePath created, copy image from form to wwwroot folder.
            if (movie.Image != null)
            {
                FileStream fs = new FileStream(filePath, FileMode.Create);
                movie.Image.CopyTo(fs);
                currentMovie.ImageUrl = filePath.Remove(0, 7);
            }

            currentMovie.Name = movie.Name;
            currentMovie.Language = movie.Language;
            currentMovie.Rating = movie.Rating;
            currentMovie.Description = movie.Description;
            currentMovie.Duration = movie.Duration;
            currentMovie.Genre = movie.Genre;
            currentMovie.PlayingDate = movie.PlayingDate;
            currentMovie.PlayingTime = movie.PlayingTime;
            currentMovie.TicketPrice = movie.TicketPrice;
            currentMovie.TrailorUrl = movie.TrailorUrl;

            _dbContext.SaveChanges();
            return Ok("Movie updated succesfully");
        }

        // DELETE api/<MoviesController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                var movie = _dbContext.Movies.Find(id);

                if (movie == null)
                {
                    return NotFound("The movie you tried to delete was not found");
                }

                _dbContext.Movies.Remove(movie);
                _dbContext.SaveChanges();
                return Ok("Record deleted succesfully");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}