using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BattlePlayerPrefabPool : MonoBehaviour, IPunPrefabPool
{
    [SerializeField]
    List<GameObject> battlePrefabList;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.PrefabPool = this;
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        foreach (GameObject battlePrefab in battlePrefabList)
        {
            if (battlePrefab.name == prefabId)
            {
                // 生成されたネットワークオブジェクトは非アクティブ状態で返す必要がある
                // （その後、PhotonNetworkの内部で正しく初期化されてから自動的にアクティブ状態に戻される）
                GameObject go = Instantiate(battlePrefab, position, rotation);
                go.SetActive(false);
                return go;
            }
        }

        return null;
    }

    public void Destroy(GameObject gameObject)
    {
        Destroy(gameObject);
    }
}
