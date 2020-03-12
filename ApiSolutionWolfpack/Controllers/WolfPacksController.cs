using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiSolutionWolfpack.Models;
using Newtonsoft.Json;

namespace ApiSolutionWolfpack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WolfPacksController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public WolfPacksController(DatabaseContext context)
        {
            _context = context;
        }

        // Get a list of pack with wolves in them
        // GET: api/WolfPacks
        [HttpGet]
        public async Task<ActionResult<String>> GetWolfPacks()
        {
            var packs = await _context.Packs.ToListAsync();
            Dictionary<String, List<Wolf>> wolfpacks = new Dictionary<String, List<Wolf>>();

            foreach (Pack pack in packs)
            {

                List<Wolf> wolfList = new List<Wolf>();
                List<long> wolfIdList = (from s in _context.WolfPacks
                                         where s.PackName == pack.Name
                                         select s.WolfId).ToList();
                //if there are no wolves left in the pack, delete it
                if (wolfIdList.Count() > 0)
                {
                    foreach (long wolfId in wolfIdList)
                    {
                        var wolf = await _context.Wolves.FindAsync(wolfId);
                        wolfList.Add(wolf);
                    }
                    wolfpacks.Add(pack.Name, wolfList);
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    throw;
                }

            }
            string json = JsonConvert.SerializeObject(wolfpacks, Formatting.Indented);
            return json;
        }

        // Get a pack
        // GET: api/WolfPacks/packname
        [HttpGet("{packname}")]
        public async Task<ActionResult<String>> GetWolfPack(string packName)
        {
            var pack = await _context.Packs.FindAsync(packName);
            List<Wolf> wolfList = new List<Wolf>();
            List<long> wolfIdList = (from s in _context.WolfPacks
                                     where s.PackName == packName
                                     select s.WolfId).ToList();
            if (wolfIdList == null || pack == null)
            {
                return NotFound("Pack " + packName + " not found");
            }
            foreach (long wolfId in wolfIdList)
            {
                var wolf = await _context.Wolves.FindAsync(wolfId);
                wolfList.Add(wolf);
            }

            string json = JsonConvert.SerializeObject(wolfList, Formatting.Indented);
            return json;
        }

        // Delete a pack 
        // DELETE: api/WolfPacks/packname
        [HttpDelete("{packname}")]
        public async Task<ActionResult<WolfPack>> DeleteWolfPack(string packName)
        {
            var wolfPack = _context.WolfPacks.Where(x => x.PackName == packName);

            if (wolfPack == null)
            {
                return NotFound("Pack " + packName +" not found");
            }
            var pack = _context.Packs.Where(x => x.Name == packName);

            _context.Packs.RemoveRange(
                _context.Packs.Where(x => x.Name == packName));

            _context.WolfPacks.RemoveRange(
                _context.WolfPacks.Where(x => x.PackName == packName));
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok("Successfully deleted pack " + packName);
        }

        // Get information for wolf from a pack
        // GET: api/WolfPacks/packname/wolfid
        [HttpGet("{packname}/{wolfid}")]
        public async Task<ActionResult<String>> GetWolfInPack(string packName, long wolfId)
        {
            List<long> wolfL = (from s in _context.WolfPacks
                                 where s.PackName == packName && s.WolfId == wolfId
                                 select s.WolfId).ToList();

            if (!(wolfL.Count() > 0))
            {
                return NotFound("Wolf with id " + wolfId + " was not found in pack "+packName);
            }

            var wolf = await _context.Wolves.FindAsync(wolfId);

            if (wolf == null)
            {
                return NotFound("Wolf with id " + wolfId + " was not found");
            }

            string json = JsonConvert.SerializeObject(wolf, Formatting.Indented);
            return json;

        }
        // Delete a wolf in a pack (if last wolf then pack is deleted as well)
        // DELETE: api/WolfPacks/packname/wolfid
        [HttpDelete("{packname}/{wolfid}")]
        public async Task<ActionResult<WolfPack>> DeleteWolfInPack(string packName, long wolfId)
        {
            var pack = await _context.Packs.FindAsync(packName);
            var wolf = await _context.Wolves.FindAsync(wolfId);
            if (pack == null || wolf == null)
            {
                return NotFound("pack or wolf does not exist");
            }
            
            //remove from wolfpacks the relation between the wolf and the pack
             _context.WolfPacks.RemoveRange(
                _context.WolfPacks.Where(x => x.PackName == packName)
                .Where(y => y.WolfId == wolfId));
            
            //save
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            List<long> wolfIdList = (from s in _context.WolfPacks
                                     where s.PackName == pack.Name
                                     select s.WolfId).ToList();
            
            //if there are no wolves left in the pack then delete the pack
            if(!(wolfIdList.Count() > 0))
            {
                _context.Packs.Remove(pack);
                _context.WolfPacks.RemoveRange(
                    _context.WolfPacks.Where(x => x.PackName == packName));
            }
            
            //save
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok("Successfully deleted wolf with id " + wolfId + " in pack " + packName);
        }

        // Add a wolf to a pack (creates the pack if did not exist before)
        // POST: api/WolfPacks
        [HttpPost]
        public async Task<ActionResult<WolfPack>> PostWolfPack(WolfPack wolfPack)
        {
            var wolf = await _context.Wolves.FindAsync(wolfPack.WolfId);
            var pack = await _context.Packs.FindAsync(wolfPack.PackName);
            List<long> wolfId = (from s in _context.WolfPacks
                          where s.PackName == wolfPack.PackName && s.WolfId == wolfPack.WolfId
                          select s.WolfId).ToList();

            if (wolf == null)  
            {
                return NotFound("Wolf with id " + wolfPack.WolfId + " not found");
            }
            if (pack == null)
            {
                _context.Packs.Add(new Pack
                {
                    Name = wolfPack.PackName
                });
            }
            if (wolfId.Count() > 0)
            {
                return Ok("Wolf with id "+ wolfPack.WolfId + " is already added to pack "+wolfPack.PackName);
            }

            _context.WolfPacks.Add(wolfPack);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                throw;
            }

            return Ok("Successfully added wolf with id " + wolf.WolfId + " in pack " + wolfPack.PackName);
        }
    }
}
