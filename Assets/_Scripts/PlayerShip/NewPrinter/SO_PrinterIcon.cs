using System.Collections.Generic;
using UnityEngine;

namespace Printer.Model
{
    [CreateAssetMenu(fileName = "PrinterCategory", menuName = "ScriptableObject/PlayerShip/3DPrinter/PrinterCategory")]
    public class SO_PrinterIcon : ScriptableObject
    {
        public int ID => GetInstanceID();

        [field: SerializeField]
        public string ItemName { get; set; }

        [field: SerializeField]
        [field: TextArea]
        public string ItemDescription { get; set; }
        
        [field: SerializeField]
        public Sprite ItemImage {  get; set; }

        [field: SerializeField]

        public ShipPrinterItemType itemType;

        [field: SerializeField]
        public List<SO_PrinterIcon> SubCategories { get; set; }


        // If Icon is a craftable, List of crafting ingredients
    }
}
