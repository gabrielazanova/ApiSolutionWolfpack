using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiSolutionWolfpack.Models;
using Newtonsoft.Json;
using System.Drawing;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiSolutionWolfpack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WolvesController : ControllerBase
    {
        private readonly DatabaseContext _context;
        
        public WolvesController(DatabaseContext context)
        {
            _context = context;
        }

        // Get basic information of all wolves
        // GET: api/Wolves
        [HttpGet]
        public async Task<ActionResult<String>> GetWolves()
        {
            var wolves = await _context.Wolves.ToListAsync();
            var wolfList = (from wolf in _context.Wolves
                                     select new
                                     {
                                         wolfid = wolf.WolfId,
                                         name = wolf.Name,
                                         gender = wolf.Gender,
                                         birthdate = wolf.Birthdate
                                     }).ToList();

            string json = JsonConvert.SerializeObject(wolfList, Formatting.Indented);
            return json;
        }

        // Get basic information of a wolf
        // GET: api/Wolves/5
        [HttpGet("{id}")]
        public async Task<ActionResult<String>> GetWolf(long id)
        {
            var wolf = await _context.Wolves.FindAsync(id);
            if (wolf == null)
            {
                return NotFound("Wolf with id " + id +" was not found");
            }

            string json = JsonConvert.SerializeObject(wolf, Formatting.Indented);
            return json;
        }

        // Get location of a wolf by id
        // GET: api/Wolves/id/location
        [HttpGet("{id}/location")]
        public async Task<ActionResult<String>> GetWolfLoc(long id)
        {
            var wolf = await _context.Wolves.FindAsync(id);
            if (wolf == null)
            {
                return NotFound("Wolf with id " + id + " was not found");
            }

            var wolfLocation = (from w in _context.Wolves
                                where w.WolfId == id
                                select new
                                {
                                    location = w.Location
                                });

            if (wolfLocation == null)
            {
                return NotFound("Location for wolf with id " + id + " was not found");
            }
            string json = JsonConvert.SerializeObject(wolfLocation, Formatting.Indented);
            return json;
        }

        // Update wolf
        // PUT: api/Wolves/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWolf(long id, Wolf wolf)
        {
            if (id != wolf.WolfId)
            {
                return BadRequest();
            }

            _context.Entry(wolf).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return Ok("Updated wolf with id "+id);
        }

        // Update location of a wolf
        // PATCH: api/Wolves/5/location
        [HttpPatch ("{id}/location")]
        public async Task<IActionResult> PutPack(long id, [FromBody] PointF newLocation)
        {
            var wolf = await _context.Wolves.FindAsync(id);

            if (wolf == null)
            {
                return NotFound();
            }

            wolf.Location = newLocation;
            _context.Entry(wolf).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok("Successfully set location of wolf with id "+ id +" to "+ newLocation);
        }

        // Add a wolf
        // POST: api/Wolves
        [HttpPost]
        public async Task<ActionResult<Wolf>> PostWolf(Wolf wolf)
        {
            _context.Wolves.Add(wolf);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok("Successfully added wolf with id " + wolf.WolfId);
        }

        // Delete a wolf
        // DELETE: api/Wolves/wolfid
        [HttpDelete("{id}")]
        public async Task<ActionResult<Wolf>> DeleteWolf(long id)
        {
            var wolf = await _context.Wolves.FindAsync(id);
            if (wolf == null)
            {
                return NotFound("Wolf with id " + id + " was not found");
            }
            //remove the wolf from wolves
            _context.Wolves.Remove(wolf);
            List<String> packList = (from pack in _context.WolfPacks
                                     where pack.WolfId == id
                                     select pack.PackName).ToList();
            //delete every wolf pack combination that includes the wolf
            _context.WolfPacks.RemoveRange(
                _context.WolfPacks.Where(x => x.WolfId == id));

            //save
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            //remove every pack associated with the wolf if empty
            foreach (String pack in packList)
            {
                List<long> packs = (from s in _context.WolfPacks
                                         where s.PackName == pack
                                         select s.WolfId).ToList();
                //if there are no wolves left in the pack, delete it
                if (!(packs.Count() > 0))
                {
                    _context.Packs.RemoveRange(
                        _context.Packs.Where(x => x.Name == pack));
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return Ok("Successfully deleted wolf with id " + id);
        }
    }
}
