using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    CardModel _cardModel;//カードデータ操作用
    CardView _cardView;//カード表示用

    public CardModel CardModel => _cardModel;

    void Awake()
    {
        _cardModel = GetComponent<CardModel>();
        _cardView = GetComponent<CardView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="cardIndex"></param>
    public void Init(int cardIndex)
    {
        _cardModel.SetCardData(cardIndex);
        _cardView.SetCardView(_cardModel);
    }
}
