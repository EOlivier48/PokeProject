using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;
using System.IO;

public class PokeService
{

    private Dictionary<string, pokemon> pokeCache = new Dictionary<string, pokemon>();
    private Dictionary<string, string> typeCache = new Dictionary<string, string>();

    /* Gets, populates, and returns a pokemon entity based on the name parameter
     * Throws exception "Pokemon not found." If pokemon does not exist.
     */
    public pokemon GetPokemon(string name, Boolean useCache = true)
    {

        try
        {
            //let's get it from cache to avoid slamming the servers
            pokemon pokeObj = this.pokeCache[name];
            return pokeObj;
        }
        catch (KeyNotFoundException)
        {
            const string apiUrl = "http://pokeapi.co/api/v2/pokemon/";
            string jsonResponse = Get(apiUrl + name);
            pokemon pokeObj = new pokemon(name);

            dynamic responseObj = JObject.Parse(jsonResponse);
            //assuming there will only ever be two types for each pokemon max
            foreach (var t in responseObj.types)
            {
                pokeObj.SetType((int)t.slot - 1, (string)t.type.name);
            }
            //get damage relation from type object
            PopulateDamageRelationships(pokeObj);
            pokeCache.Add(name, pokeObj);
            return pokeObj;
        }
    }

    /* Poplates the pokemon object's damage relationships based on the object's types
     * If types have not been set this will not add any relationships.
     */
    private void PopulateDamageRelationships(pokemon pokeObj)
    {
        foreach (var t in pokeObj.GetTypes())
        {
            if (t != null)
            {
                string typeJson = GetTypeJson(t);
                Dictionary<string, double> typeDict = ParseTypeJsonIntoDamageDict(typeJson);
                foreach (var td in typeDict)
                {
                    pokeObj.AddDamageRelationship(td.Key, td.Value);
                }
            }
        }
        return;
    }

    /* Gets the raw json for the type requested.
     * Tries the cache first and if it doesn't exist then makes an API call to fetch it.
     */
    public string GetTypeJson(string typeName, Boolean useCache = true)
    {
        string typeJson = "";
        try
        {
            typeJson = typeCache[typeName];
        }
        catch (KeyNotFoundException)
        {
            const string apiUrl = "http://pokeapi.co/api/v2/type/";
            typeJson = Get(apiUrl + typeName);
            typeCache.Add(typeName, typeJson);
        }
        return typeJson;
    }

    /* Parses a raw type json and returns a dictionary with that type's damage relationships 
     */
    private Dictionary<string, double> ParseTypeJsonIntoDamageDict(string json)
    {
        Dictionary<string, double> typeDict = new Dictionary<string, double>();
        dynamic rawTypeObj = JObject.Parse(json);

        foreach (var dd in rawTypeObj.damage_relations.double_damage_from)
        {
            typeDict.Add((string)dd.name, 2);
        }
        foreach (var hd in rawTypeObj.damage_relations.half_damage_from)
        {
            typeDict.Add((string)hd.name, 0.5);
        }
        foreach (var nd in rawTypeObj.damage_relations.no_damage_from)
        {
            typeDict.Add((string)nd.name, 0);
        }
        return typeDict;
    }

    private string Get(string url)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        try
        {
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        catch (WebException e)
        {
            using (HttpWebResponse response = (HttpWebResponse)e.Response)
            {
                if ((int)response.StatusCode == 404)
                {
                    throw new Exception("Pokemon not found.");
                }
                else
                {
                    Console.WriteLine("Web exception encountered, errorcode: {0}", (int)response.StatusCode);
                    throw e;
                }
            }
        }
    }
}
