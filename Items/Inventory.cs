using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
  public TextMeshProUGUI Kgs;
  public TextMeshProUGUI Nums;
  public TextMeshProUGUI Credits;
  public Image InvImage;
  public Sprite[] InvImages;
  InventoryType invType;
  public Dictionary<Item.ItemType, int> items = new();
  public int credits = 0;
  int count = 0;
  int weight = 0; // 100 is 1Kg

  public void SetType(InventoryType t) {
    invType = t;
    InvImage.sprite = InvImages[(int)t];

    // FIXME in case we cannot keep the items do something
  }

  // Returns true in case the item cannot be picked (no more available space)
  public bool PutItem(Item item) {
    if (item.IsMoney()) {
      credits += item.GetCreditsValue();
      Credits.text = $"{Credits.text[0]}{(credits/100f):F2}";
      return false;
    }

    int maxNum = GetMaxNum();
    int maxWeight = GetMaxWeight();
    if (count >= maxNum) return true;

    if (items.ContainsKey(item.itemType)) items[item.itemType]++;
    else items[item.itemType] = 1;

    count++;
    weight += (int)(item.GetItemWeight() * 100);

    Kgs.text = $"{weight / 100f:F2}<size=18>/{maxWeight} Kg</size>";
    Nums.text = $"{count}<size=18>/{maxWeight} Kg</size>";

    InvImage.color = (weight > maxWeight * 100) ? Color.red : Color.white;
    return false;
  }

  private int GetMaxWeight() {
    switch (invType) {
      case InventoryType.Hands: return 2;
      case InventoryType.Pockets: return 2;
      case InventoryType.Shopper: return 3;
      case InventoryType.Bag: return 5;
      case InventoryType.Backpack: return 15;
      case InventoryType.Suitcase: return 25;
      default: return 0;
    }
  }
  private int GetMaxNum() {
    switch (invType) {
      case InventoryType.Hands: return 2;
      case InventoryType.Pockets: return 4;
      case InventoryType.Shopper: return 15;
      case InventoryType.Bag: return 30;
      case InventoryType.Backpack: return 50;
      case InventoryType.Suitcase: return 50;
      default: return 0;
    }
  }

  public enum InventoryType {
    // Max items    Max weight
    Hands=0,    // 2            2kg       
    Pockets=1,  // 4            2kg       
    Shopper=2,  // 15           3kg       
    Bag=3,      // 30           5kg       
    Backpack=4, // 50           15kg      
    Suitcase=5  // 50           25kg      
  }

}

