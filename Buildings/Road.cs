using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Road : MonoBehaviour {
  public string Name;
  [Range(0,1)] public float Happiness;
  [Range(0,1)] public float Cleanliness;
  [Range(0,1)] public float Expensiveness;
  [Range(0,1)] public float Criminality;
  public bool Vertical;
  public int Size;
  public List<TextMeshPro> RoadNameSigns;
  public List<Building> Buildings;

  public string Hash;
  public void GenerateRoad() {
    // FIXME copy code from buildings
    // Have some parameters if we have to fill one of the unique building items

    // How many sections? (number of roads in the other axis - 1)
    // Each section should be 3 to 6 buildings
    // Place road names at each intersection (FIXME create prefab)
    // Generate all buildings and keep track of the building properties (FIXME add type to the building)
    // Add roads
    // Add foreground
    // Add background
    // Place camera in start position (do some sort of transition?)



  }


  readonly string[] roadTypes = {
    "Lane", "Road", "Street", "Strasse", "Boulevard", "Avenue", "Drive", "Alley"
  };
  readonly string[] roadNames = {
    "CPU",
    "Aenimus",
    "Apoorv",
    "Duck",
    "Eremiell",
    "GlassHalfEmpty",
    "J0nathan",
    "King Tyranis",
    "Matija",
    "Murado",
    "Only",
    "Revolution",
    "Ruger",
    "Seha",
    "Bat Milku",
    "'Kaan",
    "Taro",
    "CodeMonkey",
    "Unity",
    "Godot",
    "Unreal",
    "CryEngine",
    "FoxEngine",
    "Snowdrop",
    "Anvil",
    "Gamebryo",
    "Frostbyte",
    "IdTech",
    "Irrlich",
    "Pico-8",
    "REDEngine",
    "Ren'Py",
    "ScummVM",
    "Steve Jobs",
    "Tim Berners-Lee",
    "Bill Gates",
    "James Gosling",
    "Linus Torvalds",
    "Richard Stallman",
    "Arthur C Clark",
    "Ted Codd",
    "Steve Shirley",
    "Martha Lane Fox",
    "John von Neumann",
    "Dennis Ritchie",
    "Brian Kernighan",
    "Ada Lovelace",
    "Bjarne Stroustrup",
    "Alan Turing",
    "Steve Wozniak",
    "Charles Babbage",
    "George Boole",
    "Federico Faggin",
    "Jay Miner",
    "Enrico Fermi",
    "Nikola Tesla",
    "Johannes Kepler",
    "James Clerk Maxwell",
    "Blaise Pascal",
    "René Descartes",
    "Gottfried Wilhelm Leibniz",
    "Pierre de Fermat",
    "Bob Keener",
    "'Lviv",
    "Kharkiv",
    "Dnipro",
    "Odessa",
    "Mariupol",
  };
}


public class Building {
  public BuildingType BType;
  public Collider collider;
  public float StartTime, EndTime;
}



public enum BuildingType {
  Intersection          =  0, //  
  SmallPark             =  1, //  
  Park                  =  2, //  
  Garden                =  3, //  
  Restaurant            =  4, //  
  FoodStand             =  5, //  
  FoodTruck             =  6, //  
  Kitchen               =  7, //  
  GasStation            =  8, //  
  Bank                  =  9, //  
  FinancialServices     = 10, //  
  Store                 = 11, // 
  Pharmacy              = 12, // 
  Boutique              = 13, // 
  Tailor                = 14, // 
  Gym                   = 15, // 
  PrimarySchool         = 16, // 
  Market                = 17, // 
  SuperMarket           = 18, // 
  Butcher               = 19, // 
  Bakery                = 20, // 
  Bar                   = 21, // 
  Pub                   = 22, // 
  Motel                 = 23, // 
  Hotel                 = 24, // 
  Apartments            = 25, // 
  RentableApartments    = 26, // 
  BarberShop            = 27, // 
  Office                = 28, // 
  Library               = 29, // 

  Police                = 30, // Only one per road
  MetroStation          = 31, // Only one per road
  Dumpster              = 32, // Only on borders
  Grass                 = 33, // Only on borders
  RecyclingCenter       = 34, // Only on borders
  Jail                  = 35, // Only one in city
  SecondarySchool       = 36, // Only one in city
  HighSchool            = 37, // Only one in city
  College               = 38, // Only one in city

}




/*
 
 City generation
Number of horiz roads
Number of vert roads
Name of each road
From city name and road name and position generate the random stats
Place randomly the unique buildings
Generate each road (do we need to actually generate it? Probably only if we show it)

 
 
 
 */