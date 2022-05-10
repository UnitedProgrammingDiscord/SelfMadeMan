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
  Intersection,               //  
  SmallPark,                  //  
  Park,                       //  
  Garden,                     //  
  Restaurant,                 //  
  FoodStand,                  //  
  FoodTruck,                  //  
  Kitchen,                    //  
  GasStation,                 //  
  Bank,                       //  
  FinancialServices,          //  
  Store,                      // 
  Pharmacy,                   // 
  Boutique,                   // 
  Tailor,                     // 
  Gym,                        // 
  PrimarySchool,              // 
  Market,                     // 
  SuperMarket,                // 
  Butcher,                    // 
  Bakery,                     // 
  Bar,                        // 
  Pub,                        // 
  Motel,                      // 
  Hotel,                      // 
  Apartments,                 // 
  RentableApartments,         // 
  BarberShop,                 // 
  Office,                     // 
  Library,                    // 

  Police,                     // Only one per road
  MetroStation,               // Only one per road
  Dumpster,                   // Only on borders
  Grass,                      // Only on borders
  RecyclingCenter,            // Only on borders
  Jail,                       // Only one in city
  SecondarySchool,            // Only one in city
  HighSchool,                 // Only one in city
  College,                    // Only one in city

}