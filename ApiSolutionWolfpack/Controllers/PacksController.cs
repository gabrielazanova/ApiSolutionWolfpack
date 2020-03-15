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
    public class PacksController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public PacksController(DatabaseContext context)
        {
            _context = context;
        }

        // Get all packs
        // GET: api/Packs
        [HttpGet]
        public async Task<ActionResult<String>> GetPacks()
        {
            string json = JsonConvert.SerializeObject(
                await _context.Packs.ToListAsync(), Formatting.Indented);
            return json;
        }

        // Get a pack
        // GET: api/Packs/5
        [HttpGet("{packname}")]
        public async Task<ActionResult<String>> GetPack(String packName)
        {
            var pack = await _context.Packs.FindAsync(packName);

            if (pack == null)
            {
                return NotFound("Pack "+packName+" not found");
            }

            string json = JsonConvert.SerializeObject(pack, Formatting.Indented);
            return json;
        }

        // Delete a pack
        // DELETE: api/Packs/packname
        [HttpDelete("{packname}")]
        public async Task<ActionResult<String>> DeletePack(String packName)
        {
            var pack = await _context.Packs.FindAsync(packName);
            if (pack == null)
            {
                return NotFound();
            }

            //remove from packs
            _context.Packs.Remove(pack);

            //remove every relation with the wolves from the many-to-many relationship
            _context.WolfPacks.RemoveRange(
                _context.WolfPacks.Where(x => x.PackName == pack.Name));
           
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            string json = JsonConvert.SerializeObject(pack, Formatting.Indented);
            return json;
        }
    }
}