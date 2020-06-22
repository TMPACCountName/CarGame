using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CarGame.Types;

namespace CarGame
{
    /// <summary>
    /// Этот скрипт отображает предметы, которые можно разблокировать за игровые деньги. У каждого предмета есть значок 3D, который вращается, 
    /// показывая цену предмета, если он все еще заблокирован. Заблокированные предметы также затемнены.
    /// </summary>
    public class Shop : MonoBehaviour
    {
        
        internal GameController gameController;

        //Оставшееся бабло в магазине
        internal int moneyLeft = 0;

        [Tooltip("Текстовый объект общей суммы денег")]
        public Transform moneyText;

        [Tooltip("Текстовый объект отображающий цену товара")]
        public Text priceText;

        [Tooltip("Знак будет рядом с деньгами")]
        public string moneySign = "$";

        [Tooltip("Текстовое поле с название денег")]
        public string moneyPlayerprefs = "Money";

        //Текущий элемент на котором мы находимся
        internal int currentItem = 0;

        [Tooltip("Куда писать выбранную вещь")]
        public string currentPlayerprefs = "CurrentCar";

        [Tooltip("Список предметов, их цена, состояние разблокировки. 1 - разблокировано и не может быть использовано 1 заблочено")]
        public ShopItem[] items;

        // Итем окно 2д или 3д
        internal Transform currentIcon;

        [Tooltip("Эффект покупки итема")]
        public Transform unlockEffect;

        [Tooltip("Кнопки навигации")]
        public Button buttonNextItem;
        public Button buttonPrevItem;
        public Button buttonSelectItem;

        [Tooltip("Кнопкни клавы или геймпада для навигации")]
        public string changeItemButton = "Horizontal";
        public string selectItemButton = "Submit";
        internal bool buttonPressed = false;

        [Tooltip("Камера включается при открытии магазина")]
        public GameObject shopCamera;

        [Tooltip("Положение элемента в 3хмерном пространстве")]
        public Vector3 itemIconPosition;

        [Tooltip("Скорость вращения выбранного элемента")]
        public float itemSpinSpeed = 100;
        internal float itemRotation = 0;

        //Характеристики машины
        internal float speedMax = 0;
        internal float rotateSpeedMax = 0;
        internal float healthMax = 0;
        internal float damageMax = 0;

        internal int index;

        public void OnEnable()
        {
            if (shopCamera) shopCamera.SetActive(true);
            //Получаем количество денег шо у нас есть
            moneyLeft = PlayerPrefs.GetInt(moneyPlayerprefs, moneyLeft);

            //Обновляем текст денег
            moneyText.GetComponent<Text>().text = moneyLeft.ToString() + moneySign;

            // Получаем данные характеристик
            if (speedMax == 0 && rotateSpeedMax == 0 && healthMax == 0 && damageMax == 0)
            {
                for (index = 0; index < items.Length; index++)
                {
                    if (speedMax < items[index].itemIcon.GetComponent<Car>().speed) speedMax = items[index].itemIcon.GetComponent<Car>().speed;
                    if (rotateSpeedMax < items[index].itemIcon.GetComponent<Car>().rotateSpeed) rotateSpeedMax = items[index].itemIcon.GetComponent<Car>().rotateSpeed;
                    if (healthMax < items[index].itemIcon.GetComponent<Car>().health) healthMax = items[index].itemIcon.GetComponent<Car>().health;
                    if (damageMax < items[index].itemIcon.GetComponent<Car>().damage) damageMax = items[index].itemIcon.GetComponent<Car>().damage;
                }
            }

            //Номер выбранной машинки
            currentItem = PlayerPrefs.GetInt(currentPlayerprefs, currentItem);

            // Установка первого итема
            ChangeItem(0);
        }

        public void OnDisable()
        {

            if (shopCamera) shopCamera.SetActive(false);
            // Удалить значок предыдущего элемента
            if (currentIcon)
            {
                // Ресет поворота итема
                itemRotation = currentIcon.eulerAngles.y;

                Destroy(currentIcon.gameObject);
            }
        }

        void Start()
        {
            //PlayerPrefs.DeleteAll();

            // Если в магазине нет товаров то его не используем
            if (items.Length <= 0) return;

            if (gameController == null) gameController = GameObject.FindObjectOfType<GameController>();

            // Слушаем кнопку и берем обновляем итем вперед
            buttonNextItem.onClick.AddListener(delegate { ChangeItem(1); });

            // Слушаем кнопку и обновляем итем назад
            buttonPrevItem.onClick.AddListener(delegate { ChangeItem(-1); });

            //Слушаем кнопку и выбираем магину
            buttonSelectItem.onClick.AddListener(delegate { StartCoroutine("SelectItem"); });
        }

        void Update()
        {
            // Нет итемов, нет дела
            if (items.Length <= 0) return;

            // Вращать текущий итем
            if (currentIcon) currentIcon.Rotate(Vector3.up * itemSpinSpeed * Time.deltaTime, Space.World);

            // Контроль кнопок
            if (buttonPressed == false)
            {
                if (Input.GetAxisRaw(changeItemButton) > 0) buttonNextItem.onClick.Invoke();// ChangeItem(1);
                if (Input.GetAxisRaw(changeItemButton) < 0) buttonPrevItem.onClick.Invoke();// ChangeItem(-1);
                if (Input.GetButtonDown(selectItemButton)) buttonSelectItem.onClick.Invoke();// SelectItem();

                if (Input.GetAxisRaw(changeItemButton) != 0 || Input.GetButton(selectItemButton)) buttonPressed = true;

            }
            else if (Input.GetAxisRaw(changeItemButton) == 0 || Input.GetButtonUp(selectItemButton)) buttonPressed = false;
        }

        /// <summary>
        /// Смена объекта и проверка на залоченные
        /// </summary>
        /// <param name="changeValue"></param>
        public void ChangeItem(int changeValue)
        {
            // нет итемов уходим
            if (items.Length <= 0) return;

            // Смена индекса объекта
            currentItem += changeValue;

            // Проверка на выход за границы массива
            if (currentItem > items.Length - 1) currentItem = 0;
            else if (currentItem < 0) currentItem = items.Length - 1;

            //Удалить значек прошлого элемента
            if (currentIcon)
            {
                // Переустановить ртейт
                itemRotation = currentIcon.eulerAngles.y;

                Destroy(currentIcon.gameObject);
            }

            // Показать текущий итем
            if (items[currentItem].itemIcon)
            {
                // Создать итем
                currentIcon = Instantiate(items[currentItem].itemIcon.transform, itemIconPosition, Quaternion.identity) as Transform;

                // Поворот элемента
                currentIcon.eulerAngles = Vector3.up * itemRotation;

                // Анимация предмета если она есть
                if (currentIcon.GetComponent<Animation>()) currentIcon.GetComponent<Animation>().Play();

                // Установка характеристик в дату если они есть
                if (currentIcon.GetComponent<Car>())
                {
                    // Заполнить окна
                    if (transform.Find("Base/Stats/Speed/Full")) transform.Find("Base/Stats/Speed/Full").GetComponent<Image>().fillAmount = currentIcon.GetComponent<Car>().speed / speedMax;
                    if (transform.Find("Base/Stats/RotateSpeed/Full")) transform.Find("Base/Stats/RotateSpeed/Full").GetComponent<Image>().fillAmount = currentIcon.GetComponent<Car>().rotateSpeed / rotateSpeedMax;
                    if (transform.Find("Base/Stats/Health/Full")) transform.Find("Base/Stats/Health/Full").GetComponent<Image>().fillAmount = currentIcon.GetComponent<Car>().health / healthMax;
                    if (transform.Find("Base/Stats/Damage/Full")) transform.Find("Base/Stats/Damage/Full").GetComponent<Image>().fillAmount = currentIcon.GetComponent<Car>().damage / damageMax;

                    // Значения текста внутри значков
                    if (transform.Find("Base/Stats/Speed/Text")) transform.Find("Base/Stats/Speed/Text").GetComponent<Text>().text = currentIcon.GetComponent<Car>().speed.ToString();
                    if (transform.Find("Base/Stats/RotateSpeed/Text")) transform.Find("Base/Stats/RotateSpeed/Text").GetComponent<Text>().text = currentIcon.GetComponent<Car>().rotateSpeed.ToString();
                    if (transform.Find("Base/Stats/Health/Text")) transform.Find("Base/Stats/Health/Text").GetComponent<Text>().text = currentIcon.GetComponent<Car>().health.ToString();
                    if (transform.Find("Base/Stats/Damage/Text")) transform.Find("Base/Stats/Damage/Text").GetComponent<Text>().text = currentIcon.GetComponent<Car>().damage.ToString();
                }
            }

            // состояние блокировки текущего элемента
            items[currentItem].lockState = PlayerPrefs.GetInt(currentIcon.name, items[currentItem].lockState);

            // Если итем анлокед то пишем слово GO
            if (items[currentItem].lockState == 1)
            {
                if (priceText) priceText.text = "GO!";

                // Сохранить выбор
                PlayerPrefs.SetInt(currentPlayerprefs, currentItem);
            }
            else // затемнить предмет если он закрыт
            {
                // Визуализация основного боди машинки
                MeshRenderer meshRenderer = currentIcon.transform.Find("Base/Chasis").GetComponent<MeshRenderer>();

                // Затемнение
                for (index = 0; index < meshRenderer.materials.Length; index++)
                {
                    meshRenderer.materials[index].SetColor("_Color", Color.black);
                    meshRenderer.materials[index].SetColor("_EmissionColor", Color.black);
                }

                // Показать цену товара
                if (priceText) priceText.text = items[currentItem].price.ToString() + moneySign.ToString();
            }
        }

        /// <summary>
        /// Выбор итема, если заблочен и можно то купить если разблочен то играть
        /// </summary>
        public void SelectItem()
        {
            // Проверка на лоок
            if (items[currentItem].lockState == 1)
            {
                // Установка предмета как предмета игрока
                gameController.playerObject = items[currentItem].itemIcon.GetComponent<Car>();

                // Начать игру
                gameController.StartGame();
                // Сломать значок в магазе
                Destroy(currentIcon.gameObject);
                // Деактивация магазина
                gameObject.SetActive(false);
            }
            else if (moneyLeft >= items[currentItem].price)
            {
                // Открыть итем
                items[currentItem].lockState = 1;

                // Установить инфмацию о  блокировке объекта
                PlayerPrefs.SetInt(currentIcon.name, items[currentItem].lockState);

                // Забрать деньги за итем
                moneyLeft -= items[currentItem].price;

                // Сохранить что осталось
                PlayerPrefs.SetInt(moneyPlayerprefs, moneyLeft);

                //Обновить текстове поле денег
                moneyText.GetComponent<Text>().text = moneyLeft.ToString() + moneySign;

                // Создать эффект разблокировки
                if (unlockEffect) Instantiate(unlockEffect, currentIcon.position, currentIcon.rotation);

                // Установить итем
                ChangeItem(0);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;

            Gizmos.DrawCube(itemIconPosition, new Vector3(1, 0.1f, 1));
        }

        public void SetUIMoney()
        {
            moneyText.GetComponent<Text>().text = PlayerPrefs.GetInt(moneyPlayerprefs).ToString() + moneySign;
        }
    }
}