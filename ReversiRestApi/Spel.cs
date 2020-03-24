using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReversiRestApi
{
    public class Spel : ISpel
    {
        public int ID { get; set; }
        public string Omschrijving { get; set; }
        public string Token { get; set; }
        public ICollection<Speler> Spelers { get; set; }
        public Kleur[,] Bord { get; set; }
        public Kleur AandeBeurt { get; set; }
        public enum Kleur { Geen, Wit, Zwart };
        public int[][] richting = new int[8][];
        public int xTwee, yTwee, laatsteRichting;

        public Spel()
        {
            // initialisatie van het bord
            Bord = new Kleur[8, 8];
            Bord[3, 3] = Kleur.Wit;
            Bord[3, 4] = Kleur.Zwart;
            Bord[4, 3] = Kleur.Zwart;
            Bord[4, 4] = Kleur.Wit;

            richting[0] = new int[2] { -1, -1 };
            richting[1] = new int[2] { 0, -1 };
            richting[2] = new int[2] { 1, -1 };
            richting[3] = new int[2] { -1, 0 };
            richting[4] = new int[2] { 1, 0 };
            richting[5] = new int[2] { -1, 1 };
            richting[6] = new int[2] { 0, 1 };
            richting[7] = new int[2] { 1, 1 };
        }

        public bool Afgelopen()
        {
            //for (int a = 0; a < 2; a++)
            //{
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 7; j++)
                {
                    if (Bord[i, j] == Kleur.Geen)
                    {
                        if (ZetMogelijk(i, j))
                        {
                            return false;
                        }
                    }
                }
            }
            //}
            return true;
        }

        public bool DoeZet(int rijZet, int kolomZet)
        {
            if (ZetMogelijk(rijZet, kolomZet))
            {

                int tempX = rijZet, tempY = kolomZet;

                while (ZetIsBinnenBord(tempX, tempY))
                {

                    Bord[tempX, tempY] = AandeBeurt;
                    tempX = tempX + richting[laatsteRichting][0];
                    tempY = tempY + richting[laatsteRichting][1];
                }

                if (AandeBeurt == Kleur.Wit)
                {
                    AandeBeurt = Kleur.Zwart;
                }
                else
                {
                    AandeBeurt = Kleur.Wit;
                }

                return true;
            }
            return false;
        }

        public Kleur OverwegendeKleur()
        {
            int wit = 0, zwart = 0, geen = 0;

            for (int i = 0; i < Bord.GetLength(0); i++)
            {
                for (int j = 0; j < Bord.GetLength(1); j++)
                {
                    if (Bord[i, j] == Kleur.Geen)
                    {
                        geen++;
                    }
                    else if (Bord[i, j] == Kleur.Wit)
                    {
                        wit++;
                    }
                    else if (Bord[i, j] == Kleur.Zwart)
                    {
                        zwart++;
                    }
                }
            }

            if (wit > zwart) return Kleur.Wit;
            if (zwart > wit) return Kleur.Zwart;
            return Kleur.Geen;
        }

        public bool Pas()
        {
            bool mogelijk = false;

            // Pas wanneer de huidige speler geen stenen van de andere speler kan pakken
            // Beurt gaat naar de andere speler
            for (int i = 0; i < Bord.GetLength(0); i++)
            {
                for (int j = 0; j < Bord.GetLength(1); j++)
                {
                    if (ZetIsBinnenBord(i, j))
                    {
                        if (ZetMogelijk(i, j))
                        {
                            mogelijk = true;
                        }
                    }
                }
            }

            // Zet is mogelijk dus geen pas
            if (mogelijk)
            {
                if (AandeBeurt == Kleur.Wit)
                {
                    AandeBeurt = Kleur.Zwart;
                }
                else
                {
                    AandeBeurt = Kleur.Wit;
                }
                return true;
            }

            return false;
        }

        public bool ZetMogelijk(int rijZet, int kolomZet)
        {

            int x = rijZet, y = kolomZet;

            // Zet is binnen het bord
            if (ZetIsBinnenBord(x, y) && TenminsteEenSoortgenootOmMijHeen(x, y))
            {
                // er zijn 8 richtingen rondom het blokje
                for (int i = 0; i < 8; i++)
                {
                    int tempX = x + richting[i][0], tempY = y + richting[i][1];

                    // coords binnen het bord
                    if (ZetIsBinnenBord(tempX, tempY))
                    {
                        // Geldige richting om op te gaan
                        if (Bord[tempX, tempY] != AandeBeurt && Bord[tempX, tempY] != Kleur.Geen)
                        {

                            while (ZetIsBinnenBord(tempX, tempY))
                            {
                                if (Bord[tempX, tempY] == Kleur.Geen || Bord[tempX, tempY] == AandeBeurt)
                                {
                                    xTwee = tempX;
                                    yTwee = tempY;
                                    laatsteRichting = i;
                                    return true;
                                }
                                tempX = tempX + richting[i][0];
                                tempY = tempY + richting[i][1];
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool ZetIsBinnenBord(int x, int y)
        {
            // Zet is binnen het bord
            return (x >= 0 && x <= 7) && (y >= 0 && y <= 7);
        }

        public bool TenminsteEenSoortgenootOmMijHeen(int x, int y)
        {
            // Deze methode kijkt of er tenminste 1 soortgenoot om je heen zit
            // Dus dat de steen niet random ergens neergezet wordt
            for (int i = 0; i < 8; i++)
            {
                int tempX = x + (richting[i][0] * 2), tempY = y + (richting[i][1] * 2);

                // coords binnen het bord
                if (ZetIsBinnenBord(tempX, tempY))
                {
                    while (ZetIsBinnenBord(tempX, tempY))
                    {
                        if (Bord[tempX, tempY] == AandeBeurt)
                        {
                            return true;
                        }
                        tempX = tempX + richting[i][0];
                        tempY = tempY + richting[i][1];
                    }
                }
            }
            return false;
        }

        //public bool MeerDanTweeInEenRij(int x, int y)
        //{
        //    int minimaalTwee = 1;
        //    int temp = minimaalTwee;
        //    // Deze methode kijkt of er tenminste 1 soortgenoot om je heen zit
        //    // Dus dat de steen niet random ergens neergezet wordt
        //    for (int i = 0; i < 8; i++)
        //    {
        //        int tempX = x + richting[i][0], tempY = y + richting[i][1];

        //        // coords binnen het bord
        //        if (ZetIsBinnenBord(tempX, tempY))
        //        {
        //            while (ZetIsBinnenBord(tempX, tempY))
        //            {
        //                if (Bord[tempX, tempY] == AandeBeurt && (tempX != 3 && tempY != 3) && (tempX != 4 && tempY != 4) && (tempX != 3 && tempY != 4) && (tempX != 4 && tempY != 3))
        //                {
        //                    minimaalTwee++;
        //                    tempX = tempX + richting[i][0];
        //                    tempY = tempY + richting[i][1];
        //                } else
        //                {
        //                    break;
        //                }
        //            }

        //            if (minimaalTwee > temp)
        //            {
        //                temp = minimaalTwee;
        //            } else
        //            {
        //                minimaalTwee = 1;
        //            }
        //        }
        //    }

        //    if (temp == 2)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
    }
}
