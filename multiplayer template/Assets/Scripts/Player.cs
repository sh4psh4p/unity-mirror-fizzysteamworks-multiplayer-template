using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class Player : NetworkBehaviour
{
    public TMP_Text NameText;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    public override void OnStartLocalPlayer()
    {
        NameText.gameObject.SetActive(false);
    }

    void Update()
    {
        NameText.gameObject.transform.LookAt(2 * NameText.transform.position - Camera.main.transform.position);
    }

    void OnNameChanged(string _Old, string _New)
    {
        NameText.text = playerName;
    }

    [Command]
    public void CmdSetupPlayer(string _name)
    {
        playerName = _name;
    }
}
