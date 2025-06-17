using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cards : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Transform transformCard;
    public Vector3 defaultPosition;

    [Header("PlayerOrNPCSelection")]
    public GameObject arrow;
    public GameObject playerOrNPC;

    #region Privates
    private bool _isClicked;
    #endregion

    void Awake()
    {
        transformCard = this.gameObject.transform;
        defaultPosition = transformCard.position;
        arrow = GameObject.FindGameObjectWithTag("Arrow");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.gameObject.GetComponent<Animator>().enabled = false;

        arrow.transform.position = new Vector3(playerOrNPC.transform.position.x, playerOrNPC.transform.position.y + 0.9f, playerOrNPC.transform.position.z);
        arrow.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 246);

        if(!_isClicked)
        {
            transformCard.position = new Vector3(transformCard.position.x, 10, transformCard.position.z);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isClicked = true;

        Vector3 newPosition = new Vector3(defaultPosition.x, defaultPosition.y + 236.4f, defaultPosition.z);

        transformCard.position = newPosition;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeaveCard();
    }

    public void LeaveCard()
    {
        this.gameObject.GetComponent<Animator>().enabled = true;
        _isClicked = false;

        if(arrow.GetComponent<SpriteRenderer>().color == new Color32(255, 255, 255, 246))
        {
            arrow.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 0);
        }
    }
}
