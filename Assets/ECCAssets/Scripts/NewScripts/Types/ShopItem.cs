using UnityEngine;
using System;

namespace CarGame.Types
{
    /// <summary>
    /// Объект меню
    /// </summary>
    [Serializable]
    public class ShopItem
    {
        //[Tooltip("Именования товара в магазине")]
        //public string itemName = "Car";

        [Tooltip("Объект представляюзий 3д объект или 2д значок")]
        public Transform itemIcon;

        [Tooltip("1 - открыто, 0 - закрыто")]
        public int lockState = 0;

        [Tooltip("Стоимость предмета")]
        public int price = 1000;

    }
}