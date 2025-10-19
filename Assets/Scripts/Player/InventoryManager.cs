using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core.Singleton;

public class InventoryManager : MonoBehaviour, IObserver
{
    public List<Image> itemsImages = new();
    public Image currentMask;
    public List<Sprite> itemsSprites = new();
    public Image alexImage;
    private Animator _animator;
    private int _selectedSlot;
    [SerializeField] private Color _defaultSlotColor;
    private GameObject _currentItemImage;
    private GameObject _player;
    private int _lastItemSpriteIndex;
    private int _itemsAdded;

    #region Masks Images
    [SerializeField] private Sprite _alexNoMask;
    [SerializeField] private List<Sprite> _alexMasks;
    #endregion

    void Awake()
    {
        if (PlayerPrefs.GetInt("itemsNumber") == 0) _itemsAdded = 0;
        
        if (_player == null) { _player = GameObject.FindGameObjectWithTag("Player"); }
        BasicSettings();
    }

    void OnEnable()
    {
        BasicSettings();
    }

    private void BasicSettings()
    {
        ColorUtility.TryParseHtmlString("#EF776F", out _defaultSlotColor);
        _selectedSlot = 1;

        if (currentMask.sprite == null) { alexImage.sprite = _alexNoMask; }
        else
        {
            foreach (Sprite itemSprite in itemsSprites)
            {
                if (currentMask.sprite == itemSprite)
                {
                    alexImage.sprite = _alexMasks[itemsSprites.IndexOf(itemSprite)];
                }
            }
        }

        int i = 0;
        foreach (Image image in itemsImages)
        {
            if (image.gameObject.transform.parent != null && i != 0)
            {
                _animator = image.gameObject.transform.parent.gameObject.GetComponent<Animator>();
                _animator.SetBool("DEACTIVATE", true);
                //_animator.enabled = false;
            }

            i++;
        }

        if(PlayerPrefs.GetInt("itemsNumber") > 0)
        {
            //Debug.Log("Tem Item");
            for(int iterator = 0; iterator < PlayerPrefs.GetInt("itemsNumber"); iterator++)
            {
                if(!itemsSprites[PlayerPrefs.GetInt("item" + iterator)].name.Equals(PlayerPrefs.GetString("currentItem")))
                {
                    bool alreadyInInventory = false;

                    foreach (Image image in itemsImages)
                    {
                        if (image.gameObject.activeSelf && image.sprite == itemsSprites[PlayerPrefs.GetInt("item" + iterator)])
                        {
                            alreadyInInventory = true;
                            break;
                        }
                    }

                    if (!alreadyInInventory)
                    {
                        itemsImages[iterator].gameObject.SetActive(true);
                        itemsImages[iterator].sprite = itemsSprites[PlayerPrefs.GetInt("item" + iterator)];
                    }
                }else
                {
                    currentMask.gameObject.SetActive(true);
                    PlayerPrefs.SetString("isMasked", "true");
                    currentMask.sprite = itemsSprites[PlayerPrefs.GetInt("item" + iterator)];
                }
            }
        }
    }

    void Update()
    {
        Navigate();
        Select();
    }

    private void SelectNewSlot(int selectedSlot, int deactivated, int activated)
    {
        _selectedSlot = selectedSlot;
        itemsImages[deactivated].gameObject.transform.parent.gameObject.GetComponent<Animator>().SetBool("DEACTIVATE", true);
        itemsImages[activated].gameObject.transform.parent.gameObject.GetComponent<Animator>().enabled = true;
        itemsImages[activated].gameObject.transform.parent.gameObject.GetComponent<Animator>().SetBool("DEACTIVATE", false);
    }

    private void Navigate()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (_selectedSlot == 1) { SelectNewSlot(2, 0, 1); }
            else if (_selectedSlot == 3) { SelectNewSlot(4, 2, 3); }
            else if (_selectedSlot == 5) { SelectNewSlot(6, 4, 5); }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (_selectedSlot == 2) { SelectNewSlot(1, 1, 0); }
            else if (_selectedSlot == 4) { SelectNewSlot(3, 3, 2); }
            else if (_selectedSlot == 6) { SelectNewSlot(5, 5, 4); }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_selectedSlot == 1) { SelectNewSlot(3, 0, 2); }
            else if (_selectedSlot == 2) { SelectNewSlot(4, 1, 3); }
            else if (_selectedSlot == 3) { SelectNewSlot(5, 2, 4); }
            else if (_selectedSlot == 4) { SelectNewSlot(6, 3, 5); }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_selectedSlot == 3) { SelectNewSlot(1, 2, 0); }
            else if (_selectedSlot == 4) { SelectNewSlot(2, 3, 1); }
            else if (_selectedSlot == 5) { SelectNewSlot(3, 4, 2); }
            else if (_selectedSlot == 6) { SelectNewSlot(4, 5, 3); }
        }
    }

    private void Select()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _currentItemImage = itemsImages[_selectedSlot - 1].gameObject.transform.parent?.Find("ItemImage").gameObject;

            if (_currentItemImage.activeSelf)
            {
                PlayerPrefs.SetString("currentItem", itemsImages[_selectedSlot - 1].sprite.name);
                if (!currentMask.gameObject.activeSelf)
                {
                    currentMask.gameObject.SetActive(true);
                    currentMask.sprite = _currentItemImage.GetComponent<Image>().sprite;

                    _currentItemImage.SetActive(false);
                }
                else
                {
                    (_currentItemImage.GetComponent<Image>().sprite, currentMask.sprite) = (currentMask.sprite, _currentItemImage.GetComponent<Image>().sprite);
                }

                for (int i = 0; i < itemsSprites.Count; i++)
                {
                    if (currentMask.sprite == itemsSprites[i])
                    {
                        alexImage.sprite = _alexMasks[i];
                    }
                }

                if (_player == null) { _player = GameObject.FindGameObjectWithTag("Player"); }
                _player?.GetComponent<HumanPlayer>().SetIsMasked(true);
                PlayerPrefs.SetString("isMasked", "true");
            }
            else
            {
                if (currentMask.gameObject.activeSelf)
                {
                    if (itemsSprites.Contains(currentMask.sprite))
                    {
                        _currentItemImage.GetComponent<Image>().sprite = currentMask.sprite;
                        if (!_currentItemImage.activeSelf) _currentItemImage.SetActive(true);

                        alexImage.sprite = _alexNoMask;

                        if (_player == null) { _player = GameObject.FindGameObjectWithTag("Player"); }
                        _player?.GetComponent<HumanPlayer>().SetIsMasked(false);
                    }

                    currentMask.gameObject.SetActive(false);
                    PlayerPrefs.SetString("isMasked", "false");
                    PlayerPrefs.SetString("currentItem", "");
                }
            }
        }
    }

    private void CheckItems()
    {
        foreach (Image image in itemsImages)
        {
            if (!image.gameObject.activeSelf)
            {
                image.gameObject.SetActive(true);
                image.sprite = itemsSprites[_lastItemSpriteIndex];

                PlayerPrefs.SetInt("item" + _itemsAdded, _lastItemSpriteIndex);
                _itemsAdded++;
                PlayerPrefs.SetInt("itemsNumber", _itemsAdded);
                //Debug.Log(PlayerPrefs.GetInt("itemsNumber"));
                break;
            }
        }
    }

    public void OnNotify(EventsEnum evt)
    {
        if (evt == EventsEnum.NewItem)
        {
            CheckItems();
        }
    }

    public void LastItemRecieved(Sprite itemSprite)
    {
        if (itemsSprites.Contains(itemSprite))
        {
            _lastItemSpriteIndex = itemsSprites.IndexOf(itemSprite);
        }
    }

    public void SetItem(Sprite itemSprite)
    {
        _currentItemImage = this.gameObject.transform.Find("Slots").Find("Slot").Find("ItemImage").gameObject;
        _currentItemImage.SetActive(true);
        _currentItemImage.GetComponent<Image>().sprite = itemSprite;

        if (_currentItemImage.activeSelf)
        {
            PlayerPrefs.SetString("currentItem", itemSprite.name);
            if (!currentMask.gameObject.activeSelf)
            {
                currentMask.gameObject.SetActive(true);
                currentMask.sprite = _currentItemImage.GetComponent<Image>().sprite;

                _currentItemImage.SetActive(false);
            }

            for (int i = 0; i < itemsSprites.Count; i++)
            {
                if (currentMask.sprite == itemsSprites[i])
                {
                    alexImage.sprite = _alexMasks[i];
                }
            }

            if (_player == null) { _player = GameObject.FindGameObjectWithTag("Player"); }
            _player?.GetComponent<HumanPlayer>().SetIsMasked(true);
            PlayerPrefs.SetString("isMasked", "true");
        }
    }

    public void SetAlexImage(int index)
    {
        alexImage.sprite = _alexMasks[index];
    }
}
