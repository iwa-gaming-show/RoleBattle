using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MultiPlayerIcon : MonoBehaviour
{
    PhotonView _photonView;

    void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }
}
