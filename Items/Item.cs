using UnityEngine;

public class Item : MonoBehaviour {
  public ItemType itemType;

  public bool IsMoney() {
    switch (itemType) {
      case ItemType.Coin5: return true;
      case ItemType.Coins10: return true;
      case ItemType.Coins25: return true;
      case ItemType.Coins50: return true;
      case ItemType.Banknotes1: return true;
      case ItemType.Banknotes5: return true;
      case ItemType.Banknotes10: return true;
      case ItemType.Banknotes20: return true;
    }
    return false;
  }

  public int GetCreditsValue() {
    switch (itemType) {
      case ItemType.Coin5: return 5;
      case ItemType.Coins10: return 10;
      case ItemType.Coins25: return 25;
      case ItemType.Coins50: return 50;
      case ItemType.Banknotes1: return 100;
      case ItemType.Banknotes5: return 500;
      case ItemType.Banknotes10: return 1000;
      case ItemType.Banknotes20: return 2000;
    }
    return 0;
  }

  public float GetItemWeight() {
    return GetItemWeight(itemType);
  }

  public static float GetItemWeight(ItemType it) {
    switch (it) {
      case ItemType.Coin5: return 0;
      case ItemType.Coins10: return 0;
      case ItemType.Coins25: return 0;
      case ItemType.Coins50: return 0;
      case ItemType.Banknotes1: return 0;
      case ItemType.Banknotes5: return 0;
      case ItemType.Banknotes10: return 0;
      case ItemType.Banknotes20: return 0;
      case ItemType.GlassBottle: return .5f;
      case ItemType.GlassBottleWine: return .75f;
      case ItemType.PlasticBottle: return .1f;
      case ItemType.PlasticBottleWater: return 1f;
      case ItemType.PlasticBottleJuice: return 1f;
      case ItemType.Can: return .1f;
      case ItemType.CanJuice: return .3f;
      case ItemType.CanBeer: return .3f;
      case ItemType.CanSoda: return .3f;
      case ItemType.CanEnergyDrink: return .3f;
      case ItemType.Paper: return .1f;
      case ItemType.Bones: return .2f;
      case ItemType.Snack: return .15f;
      case ItemType.SnackSugar: return .15f;
      case ItemType.SnackProtein: return .15f;
      case ItemType.SnackChocolate: return .15f;
      case ItemType.Soap: return .1f;
      case ItemType.Razor: return .1f;
      case ItemType.Broom: return 1f;
      case ItemType.Book: return .5f;
      case ItemType.Shopper: return 0f;
      case ItemType.Bag: return 0f;
      case ItemType.Backpack: return 0f;
      case ItemType.Suitcase: return 0f;
      default: return 0;
    }
  }
  public enum ItemType {
    Coin5,
    Coins10,
    Coins25,
    Coins50,
    Banknotes1,
    Banknotes5,
    Banknotes10,
    Banknotes20,
    GlassBottle,
    GlassBottleWine,
    PlasticBottle,
    PlasticBottleWater,
    PlasticBottleJuice,
    Can,
    CanJuice,
    CanBeer,
    CanSoda,
    CanEnergyDrink,
    Paper,
    Bones,
    Snack,
    SnackSugar,
    SnackProtein,
    SnackChocolate,
    Soap,
    Razor,
    Broom,
    Book,
    Shopper,
    Bag,
    Backpack,
    Suitcase,
  }
}