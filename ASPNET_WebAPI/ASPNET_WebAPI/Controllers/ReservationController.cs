using ASPNET_WebAPI.Data;
using ASPNET_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNET_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private CinemaDbContext _dbContext;

        public ReservationController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET function for getting reservations
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetReservations()
        {
            var currentReservations = from reservation in _dbContext.Reservations
                                      join customer in _dbContext.Users on reservation.UserId equals customer.Id
                                      join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                                      select new
                                      {
                                          id = reservation.Id,
                                          movieName = movie.Name,
                                          reservationTime = reservation.ReservationTime,
                                          customerName = customer.Name
                                      };

            return Ok(currentReservations);
        }

        [HttpGet("{Id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetReservationDetail(int id)
        {
            var selectedReservation = (from reservation in _dbContext.Reservations
                                       join customer in _dbContext.Users on reservation.UserId equals customer.Id
                                       join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                                       where reservation.Id == id
                                       select new
                                       {
                                           reservationId = reservation.Id,
                                           reservationTime = reservation.ReservationTime,
                                           customerName = customer.Name,
                                           movieName = movie.Name,
                                           customerEMail = customer.Email,
                                           qty = reservation.Qty,
                                           price = reservation.Price,
                                           phone = reservation.Phone,
                                           playingDate = movie.PlayingDate,
                                           playingTime = movie.PlayingTime
                                       }).FirstOrDefault();

            return Ok(selectedReservation);
        }

        // POST function for creating reservations
        [HttpPost]
        public IActionResult Post([FromBody] Reservation reservationObj)
        {
            reservationObj.ReservationTime = DateTime.Now;
            _dbContext.Reservations.Add(reservationObj);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        // DELETE api/<ReservationController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                var reservation = _dbContext.Reservations.Find(id);

                if (reservation == null)
                {
                    return NotFound("The movie you tried to delete was not found");
                }

                _dbContext.Reservations.Remove(reservation);
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