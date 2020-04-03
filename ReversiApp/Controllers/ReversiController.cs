using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReversiApp.Areas.Identity.Data;
using ReversiApp.DAL;
using ReversiApp.Models;

namespace ReversiRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReversiController : ControllerBase
    {
        private readonly Spel _Context;
        private readonly SpelerContext spelerContext;
        private readonly IdentityContext identityContext;
        private readonly UserManager<ReversiApp.Areas.Identity.Data.Speler> userManager;
        private readonly ReversiApp.Areas.Identity.Data.Speler nuSpelend;
        private IHttpContextAccessor accessorContext;
        //private Kleur[,] tempKleur;
        private HttpContext httpContext { get { return accessorContext.HttpContext; } }

        public ReversiController(SpelerContext context, IdentityContext id, UserManager<ReversiApp.Areas.Identity.Data.Speler> um, IHttpContextAccessor accessorContext)
        {
            this.spelerContext = context;
            this.accessorContext = accessorContext;
            userManager = um;
            identityContext = id;
            nuSpelend = userManager.GetUserAsync(httpContext.User).Result;

            // Token van speler moet matchen met token van het spel
            _Context = spelerContext.Spel.FirstOrDefault(x => x.Token == nuSpelend.Token);
            //tempKleur = JsonConvert.DeserializeObject<Kleur[,]>(_Context.JsonBord);
        }

        // api/Reversi/Afgelopen GET
        [HttpGet]
        [Route("Afgelopen")]
        [Authorize]
        public bool Afgelopen()
        {
            _Context.Bord = JsonConvert.DeserializeObject<Kleur[,]>(_Context.JsonBord);

            return _Context.Afgelopen();
        }

        //Api/Reversi/DoeZet/x=4&y=2
        [HttpGet]
        [Route("DoeZet/x={rijZet}&y={kolomZet}")]
        [Authorize]
        public async Task<bool> DoeZet(int rijZet, int kolomZet)
        {
            if (_Context.AandeBeurt == nuSpelend.HuidigeKleur)
            {
                //tempKleur = _Context.Bord;
                _Context.Bord = JsonConvert.DeserializeObject<Kleur[,]>(_Context.JsonBord);

                _Context.DoeZet(rijZet, kolomZet);

                // bord updaten na een set
                var bord = _Context;
                bord.JsonBord = JsonConvert.SerializeObject(_Context.Bord);
                spelerContext.Update(bord);
                await spelerContext.SaveChangesAsync();

                return true;
            }
            return false;
        }

        //Api/Reversi/ZetMogelijk/x=4&y=2
        [HttpGet]
        [Route("ZetMogelijk/x={rijZet}&y={kolomZet}")]
        [Authorize]
        public bool ZetMogelijk(int rijZet, int kolomZet)
        {
            _Context.Bord = JsonConvert.DeserializeObject<Kleur[,]>(_Context.JsonBord);

            return _Context.ZetMogelijk(rijZet, kolomZet);
        }

        [HttpGet]
        [Route("Beurt")]
        [Authorize]
        public string Beurt()
        {

            var RootObject = new RootObject(GetAll(), _Context.OverwegendeKleur());
            return JsonConvert.SerializeObject(RootObject);
        }

        [HttpGet]
        [Route("AanDeBeurt")]
        [Authorize]
        public string AanDeBeurt()
        {
            return JsonConvert.SerializeObject(_Context.AandeBeurt);
        }

        [HttpGet]
        [Route("Spelers")]
        [Authorize]
        public string Spelers()
        {
            // returned de spelers in het huidige spel, email + kleur
            return JsonConvert.SerializeObject(identityContext.Spelers.Where(x => x.Token == _Context.Token).Select(x => new { x.Email, x.HuidigeKleur}));
        }

        public List<Plaat> GetAll()
        {
            _Context.Bord = JsonConvert.DeserializeObject<Kleur[,]>(_Context.JsonBord);
            List<Plaat> temp = new List<Plaat>();

            for (int i = 0; i < _Context.Bord.GetLength(0); i++)
            {
                for (int j = 0; j < _Context.Bord.GetLength(0); j++)
                {
                    if (_Context.Bord[i, j] == Kleur.Geen)
                    {
                        if (_Context.Bord[i, j] != _Context.tempKleur[i, j])
                        {
                            temp.Add(new Plaat { isEmpty = true, isWhite = false, isBlack = false, isAnimation = true });
                        } else
                        {
                            temp.Add(new Plaat { isEmpty = true, isWhite = false, isBlack = false, isAnimation = false });
                        }
                    }
                    if (_Context.Bord[i, j] == Kleur.Wit)
                    {
                        if (_Context.Bord[i, j] != _Context.tempKleur[i, j])
                        {
                            temp.Add(new Plaat { isEmpty = false, isWhite = true, isBlack = false, isAnimation = true });
                        } else
                        {
                            temp.Add(new Plaat { isEmpty = false, isWhite = true, isBlack = false, isAnimation = false });
                        }
                    }
                    if (_Context.Bord[i, j] == Kleur.Zwart)
                    {
                        if (_Context.Bord[i, j] != _Context.tempKleur[i, j])
                        {
                            temp.Add(new Plaat { isEmpty = false, isWhite = false, isBlack = true, isAnimation = true });
                        } else
                        {
                            temp.Add(new Plaat { isEmpty = false, isWhite = false, isBlack = true, isAnimation = false });
                        }
                    }
                }
            }

            //_Context.tempKleur = JsonConvert.DeserializeObject<Kleur[,]>(_Context.JsonBord);

            return temp;
        }

        public class Plaat
        {
            public bool isEmpty { get; set; }
            public bool isWhite { get; set; }
            public bool isBlack { get; set; }
            public bool isAnimation { get; set; }

        }

        public class RootObject
        {
            public List<Plaat> Plaats { get; set; }
            public Dictionary<string, int> Punten { get; set; }

            public RootObject(List<Plaat> temp, Dictionary<string, int> temp1)
            {
                Plaats = temp;
                Punten = temp1;
            }
        }

    }
}