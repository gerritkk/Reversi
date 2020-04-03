using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ReversiApp.Areas.Identity.Data;
using ReversiApp.DAL;
using ReversiApp.Models;

namespace ReversiApp.Controllers
{
    public class SpelController : Controller
    {
        private readonly SpelerContext _context;
        //private readonly IdentityContext identityContext;
        private readonly Speler nuSpelend;
        private readonly UserManager<Speler> userManager;
        private readonly IdentityContext identityContext;
        private IHttpContextAccessor accessorContext;
        private HttpContext httpContext { get { return accessorContext.HttpContext; } }

        public SpelController(SpelerContext context, IdentityContext id, UserManager<Speler> um, IHttpContextAccessor accessorContext)
        {
            _context = context;
            userManager = um;
            this.accessorContext = accessorContext;
            identityContext = id;
            //identityContext = id;
            nuSpelend = userManager.GetUserAsync(httpContext.User).Result;
        }

        // GET: Spel
        [Authorize]
        public async Task<IActionResult> Index()
        {
            List<Spel> alle_spellen = await _context.Spel.ToListAsync();
            List<Spel> solo_spellen = new List<Spel>();

            foreach (var item in alle_spellen)
            {
                var twee_dezelfden = identityContext.Spelers.Where(m => m.Token == item.Token).Select(m => m.Token).Count();

                // en spellen toevoegen waaraan je al meedoet
                if (nuSpelend.Token == item.Token)
                {
                    return RedirectToAction(nameof(Spelen), new { id = item.ID});
                }

                // alle spellen met maar 1 speler toevoegen aan nieuwe list
                if (twee_dezelfden <= 1)
                {
                    solo_spellen.Add(item);
                }
            }
            return View(solo_spellen);
        }

        // GET: Spel/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spel = await _context.Spel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (spel == null)
            {
                return NotFound();
            }

            return View(spel);
        }

        // GET: Spel/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Spelen(int? id)
        {
            // meegegeven id en er is iemand ingelogd
            if (id == null || nuSpelend == null)
            {
                return NotFound();
            }

            var spel = await _context.Spel.FirstOrDefaultAsync(m => m.ID == id);
            if (spel == null)
            {
                return NotFound();
            }

            var aantal_mensen_in_game = identityContext.Spelers.Where(m => m.Token == spel.Token).Select(m => m.Token).Count();

            if (aantal_mensen_in_game >= 2 && nuSpelend.Token != spel.Token)
            {
                return NotFound();
            }

            var speler_temp = nuSpelend;

            if (aantal_mensen_in_game == 1 && nuSpelend.Token == spel.Token)
            {
                // maker van spel 'rejoint'
                ViewData["enigeSpeler"] = true;
                speler_temp.HuidigeKleur = Kleur.Wit;
            } else
            {
                // tweede speler joint
                ViewData["enigeSpeler"] = false;

                Speler temp = await identityContext.Spelers.FirstOrDefaultAsync(m => m.Token == spel.Token && m.Email != speler_temp.Email);

                if (temp.HuidigeKleur == Kleur.Wit)
                {
                    speler_temp.HuidigeKleur = Kleur.Zwart;
                } else
                {
                    speler_temp.HuidigeKleur = Kleur.Wit;
                }

                //speler_temp.HuidigeKleur = Kleur.Zwart;
            }

            // token van spel wordt gegeven aan de speler
            // speler die meedoet aan een spel krijgt kleur zwart
            speler_temp.Token = spel.Token;
            identityContext.Update(speler_temp);
            await identityContext.SaveChangesAsync();

            spel.Bord = JsonConvert.DeserializeObject<Kleur[,]>(spel.JsonBord);
            return View(spel);
        }

        // POST: Spel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ID,Omschrijving,Token,AandeBeurt")] Spel spel)
        {
            // valid en er is iemand ingelogd
            if (ModelState.IsValid && nuSpelend != null)
            {
                var spel_temp = spel;
                spel_temp.JsonBord = JsonConvert.SerializeObject(spel.Bord);

                _context.Add(spel_temp);
               await _context.SaveChangesAsync();

                // token van spel wordt gegeven aan de speler
                // maker van het spel krijgt kleur wit
                var speler_temp = nuSpelend;
                speler_temp.Token = spel_temp.Token;
                speler_temp.HuidigeKleur = Kleur.Wit;
                identityContext.Update(speler_temp);
                await identityContext.SaveChangesAsync();

                ViewData["enigeSpeler"] = true;
                ViewData["speler1"] = nuSpelend.Email;
                ViewData["speler2"] = "afwachtend...";
                return RedirectToAction(nameof(Index));
            }
            return View(spel);
        }

        // GET: Spel/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spel = await _context.Spel.FindAsync(id);
            if (spel == null)
            {
                return NotFound();
            }
            return View(spel);
        }

        // POST: Spel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Omschrijving,Token,JsonBord,AandeBeurt")] Spel spel)
        {
            if (id != spel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(spel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpelExists(spel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(spel);
        }

        // GET: Spel/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var spel = await _context.Spel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (spel == null)
            {
                return NotFound();
            }

            return View(spel);
        }

        // POST: Spel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var spel = await _context.Spel.FindAsync(id);
            _context.Spel.Remove(spel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpelExists(int id)
        {
            return _context.Spel.Any(e => e.ID == id);
        }
    }
}
