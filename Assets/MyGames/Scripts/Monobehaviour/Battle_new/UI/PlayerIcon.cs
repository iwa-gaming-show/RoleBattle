using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerIcon : MonoBehaviour
{
    PhotonView _photonView;

    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }
}
