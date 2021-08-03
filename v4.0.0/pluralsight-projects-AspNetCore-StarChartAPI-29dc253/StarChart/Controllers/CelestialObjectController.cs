using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;

        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id) {
            CelestialObject obj = _context.CelestialObjects.Find(id);
            
            if (obj == null)
            {
                return NotFound();
            }

            obj.Satellites = _context.CelestialObjects.Where(u => u.OrbitedObjectId == id).ToList();

            return Ok(obj);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name) {
            IEnumerable<CelestialObject> objects = _context.CelestialObjects.Where(u => u.Name == name).ToList();

            if (objects.Count() == 0)
            {
                return NotFound();
            }

            foreach (var obj in objects)
            {
                obj.Satellites = _context.CelestialObjects.Where(u => u.OrbitedObjectId == obj.Id).ToList();
            }
          

            return Ok(objects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<CelestialObject> objects = _context.CelestialObjects;
            
            foreach (var obj in objects)
            {
                obj.Satellites = _context.CelestialObjects.Where(u => u.OrbitedObjectId == obj.Id).ToList();
            }
            
            return Ok(objects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            celestialObject.Satellites = _context.CelestialObjects
               .Where(u => u.OrbitedObjectId == celestialObject.Id).ToList();
            _context.CelestialObjects.Add(celestialObject);
            _context.SaveChanges();

            return CreatedAtRoute("GetById", new { celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            if (id != celestialObject.Id)
            {
                return NotFound();
            }
            CelestialObject newObject = _context.CelestialObjects.Find(id);
            newObject.Name = celestialObject.Name;
            newObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            newObject.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(newObject);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            CelestialObject obj = _context.CelestialObjects.Find(id);

            if (obj == null)
            {
                return NotFound();
            }

            obj.Name = name;

            _context.CelestialObjects.Update(obj);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            IEnumerable<CelestialObject> celestialObjects = _context.CelestialObjects
                    .Where(u => u.Id == id || u.OrbitedObjectId == id).ToList();

            if (celestialObjects.Count() == 0)
            {
                return NotFound();
            }

            _context.CelestialObjects.RemoveRange(celestialObjects);
            _context.SaveChanges();

            return NoContent();
        }

    }
}
