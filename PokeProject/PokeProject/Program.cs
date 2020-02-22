using System;
using System.Collections.Generic;

namespace PokeProject
{
    class Program
    {

        static void Main(string[] args)
        {
            PokeService pokeService = new PokeService();
            bool quit = false;
            while (quit != true)
            {
                string pokeName = "";
                Console.WriteLine("Type a pokemon name or type quit to quit.");
                pokeName = Console.ReadLine();
                pokeName = pokeName.Trim().ToLower().Replace(".","").Replace(" ","-");

                if (pokeName != "quit")
                {
                    if(pokeName == "")
                    {
                        continue;
                    }

                    try
                    {
                        pokemon pokeObj = pokeService.GetPokemon(pokeName, true);
                        NiceOutput(pokeObj);
                    }
                    catch (Exception e) when (e.Message == "Pokemon not found.")
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }

                }
                else
                {
                    quit = true;
                }
            }
        }

        static void NiceOutput(pokemon pokeObj)
        {
            List<string> quad_damage_from   = new List<string>();
            List<string> doub_damage_from   = new List<string>();
            List<string> half_damage_from   = new List<string>();
            List<string> quar_damage_from   = new List<string>();
            List<string> no_damage_from     = new List<string>();

            foreach( var dr in pokeObj.GetDamageRelationships())
            {
                switch(dr.Value)
                {
                    case 0:
                        no_damage_from.Add(dr.Key);
                        break;
                    case 0.25:
                        quar_damage_from.Add(dr.Key);
                        break;
                    case 0.50:
                        half_damage_from.Add(dr.Key);
                        break;
                    case 2:
                        doub_damage_from.Add(dr.Key);
                        break;
                    case 4:
                        quad_damage_from.Add(dr.Key);
                        break;
                }
            }

            Console.Write("Pokemon " + pokeObj.GetName() + ". ");
            string[] types = pokeObj.GetTypes();
            Console.Write("Type: " + types[0]);
            if (types[1] != null)
            {
                Console.Write("/" + types[1]);
            }
            Console.Write(Environment.NewLine);
            OutputDamageRatio(no_damage_from, "NO");
            OutputDamageRatio(quar_damage_from, "0.25x");
            OutputDamageRatio(half_damage_from, "0.5x");
            OutputDamageRatio(doub_damage_from, "2x");
            OutputDamageRatio(quad_damage_from, "4x");
            Console.WriteLine("  Recieves normal damage from all other types.");

        }

        static void OutputDamageRatio(List<string> list, string qualifier)
        {
            if (list.Count > 0)
            {
                Console.WriteLine("  Recieves " + qualifier + " damage from:");
                Console.Write("    ");
                list.Sort();
                string last = list[list.Count - 1];
                foreach (string t in list)
                {
                    if (t == last)
                    {
                        Console.Write(t + Environment.NewLine);
                    }
                    else
                    {
                        Console.Write(t + ", ");
                    }
                }
            }
        }
    }
}
